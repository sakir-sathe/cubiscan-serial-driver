using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace CubiscanSerialDriver
{
    /// <summary>
    /// Reference UI that opens a serial (COM) port and exchanges Cubiscan
    /// dimension/weight commands.
    ///
    /// The serial logic here is intentionally self-contained so it can be lifted
    /// into a headless library, a Windows service, or a local HTTP/WebSocket
    /// bridge that a web app calls — keeping the device-facing code on the machine
    /// physically connected to the Cubiscan, with no third-party subscription.
    ///
    /// Protocol note: Cubiscan devices frame commands as STX + command + ETX,
    /// followed by CR/LF. The exact command character and the response layout are
    /// model/firmware specific; verify the constants below against your unit.
    /// </summary>
    public partial class MainForm : Form
    {
        // Expected number of bytes in a complete response for each request type.
        // These are model specific — adjust them if your device differs.
        private const int WeightResponseLength = 16;
        private const int DimensionResponseLength = 62;

        // Command frames: STX(0x02) + command char + ETX(0x03) + CR(0x0D) + LF(0x0A).
        // 0x4D = 'M' requests dimensions; 0x73 = 's' requests weight.
        private static readonly byte[] DimensionCommand = { 0x02, 0x4D, 0x03, 0x0D, 0x0A };
        private static readonly byte[] WeightCommand = { 0x02, 0x73, 0x03, 0x0D, 0x0A };

        // Accumulates incoming bytes because a response can arrive in several chunks.
        private readonly List<byte> _receiveBuffer = new List<byte>();

        // Fires if a request gets no complete response, so the UI never hangs.
        private readonly Timer _responseTimer;

        private SerialPort _serialPort;
        private MeasurementRequest _pendingRequest;

        public MainForm()
        {
            InitializeComponent();
            _responseTimer = new Timer { Interval = 5000 };
            _responseTimer.Tick += ResponseTimer_Tick;
            FormClosing += MainForm_FormClosing;
            outputTextBox.ReadOnly = true;
        }

        /// <summary>Populates the setting dropdowns with sensible defaults and lists ports.</summary>
        private void MainForm_Load(object sender, EventArgs e)
        {
            outputTextBox.Clear();
            SetConnectionState(false);
            baudRateComboBox.Items.AddRange(new object[] { 9600, 300, 600, 1200, 2400, 14400, 19200, 38400, 57600, 115200 });
            dataBitsComboBox.Items.AddRange(new object[] { 8, 7 });
            handshakeComboBox.Items.AddRange(new object[] { "None", "XOnXOff", "RequestToSend", "RequestToSendXOnXOff" });
            baudRateComboBox.SelectedItem = 9600;
            dataBitsComboBox.SelectedItem = 8;
            handshakeComboBox.SelectedItem = "None";
            RefreshPorts();
        }

        private void RefreshPortsButton_Click(object sender, EventArgs e)
        {
            RefreshPorts();
        }

        /// <summary>Re-reads the available COM ports, preserving the current selection when possible.</summary>
        private void RefreshPorts()
        {
            string selectedPort = portComboBox.Text;
            string[] ports = SerialPort.GetPortNames();
            Array.Sort(ports, StringComparer.OrdinalIgnoreCase);

            portComboBox.Items.Clear();
            portComboBox.Items.AddRange(ports);

            if (Array.Exists(ports, port => string.Equals(port, selectedPort, StringComparison.OrdinalIgnoreCase)))
            {
                portComboBox.SelectedItem = Array.Find(ports, port => string.Equals(port, selectedPort, StringComparison.OrdinalIgnoreCase));
            }
            else if (ports.Length > 0)
            {
                portComboBox.SelectedIndex = 0;
            }
            else
            {
                // No serial ports exist until a device or USB-serial adapter is attached.
                portComboBox.Text = string.Empty;
                AppendOutput("No serial ports found. Connect the device and refresh the port list.");
            }
        }

        /// <summary>Toggles the connection: opens the selected port, or closes an open one.</summary>
        private void ConnectButton_Click(object sender, EventArgs e)
        {
            if (_serialPort != null && _serialPort.IsOpen)
            {
                CloseSerialPort();
                AppendOutput("Disconnected.");
                return;
            }

            if (string.IsNullOrWhiteSpace(portComboBox.Text) ||
                !int.TryParse(baudRateComboBox.Text, out int baudRate) ||
                !int.TryParse(dataBitsComboBox.Text, out int dataBits) ||
                !Enum.TryParse(handshakeComboBox.Text, out Handshake handshake))
            {
                AppendOutput("Select a port, baud rate, data bits, and handshake before connecting.");
                return;
            }

            SerialPort port = new SerialPort
            {
                PortName = portComboBox.Text,
                BaudRate = baudRate,
                DataBits = dataBits,
                Parity = Parity.None,
                StopBits = StopBits.One,
                Handshake = handshake,
                Encoding = Encoding.ASCII,
                ReadTimeout = 1000,
                WriteTimeout = 1000
            };

            try
            {
                port.DataReceived += SerialPortOnDataReceived;
                port.Open();
                _serialPort = port;
                _receiveBuffer.Clear();
                SetConnectionState(true);
                AppendOutput("Connected to " + port.PortName + " at " + baudRate + " baud.");
            }
            catch (Exception exception)
            {
                // Opening can fail if the port is missing, in use, or access is denied.
                port.DataReceived -= SerialPortOnDataReceived;
                port.Dispose();
                AppendOutput("Could not connect: " + exception.Message);
            }
        }

        /// <summary>
        /// Fires on the serial thread whenever bytes arrive. Reads them and marshals
        /// processing back to the UI thread.
        /// </summary>
        private void SerialPortOnDataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort port = sender as SerialPort;
            if (port == null || !port.IsOpen)
            {
                return;
            }

            try
            {
                int count = port.BytesToRead;
                if (count == 0)
                {
                    return;
                }

                byte[] data = new byte[count];
                int bytesRead = port.Read(data, 0, count);
                if (bytesRead == 0)
                {
                    return;
                }

                if (bytesRead != data.Length)
                {
                    Array.Resize(ref data, bytesRead);
                }

                PostToUi(() => ProcessIncomingData(port, data));
            }
            catch (Exception exception)
            {
                PostToUi(() => AppendOutput("Serial read failed: " + exception.Message));
            }
        }

        /// <summary>
        /// Buffers bytes until a full response is available, then dispatches it to the
        /// matching parser. Runs on the UI thread.
        /// </summary>
        private void ProcessIncomingData(SerialPort port, byte[] data)
        {
            // Ignore data from a port we already closed or replaced.
            if (_serialPort != port || !port.IsOpen)
            {
                return;
            }

            if (_pendingRequest == MeasurementRequest.None)
            {
                AppendOutput("Received data without an active request: " + BitConverter.ToString(data));
                return;
            }

            _receiveBuffer.AddRange(data);
            int expectedLength = _pendingRequest == MeasurementRequest.Weight
                ? WeightResponseLength
                : DimensionResponseLength;

            // Wait for the rest of the frame if the device sent only part of it.
            if (_receiveBuffer.Count < expectedLength)
            {
                return;
            }

            byte[] response = _receiveBuffer.GetRange(0, expectedLength).ToArray();
            _receiveBuffer.RemoveRange(0, expectedLength);
            MeasurementRequest request = _pendingRequest;
            _pendingRequest = MeasurementRequest.None;
            _responseTimer.Stop();

            if (request == MeasurementRequest.Weight)
            {
                DisplayWeight(response);
            }
            else
            {
                DisplayDimensions(response);
            }

            // Surface any trailing bytes so an unexpected format is easy to diagnose.
            if (_receiveBuffer.Count > 0)
            {
                AppendOutput("Additional response bytes: " + BitConverter.ToString(_receiveBuffer.ToArray()));
                _receiveBuffer.Clear();
            }
        }

        /// <summary>Extracts the weight value, which follows the 'A' marker byte in the frame.</summary>
        private void DisplayWeight(byte[] response)
        {
            int markerIndex = Array.IndexOf(response, (byte)'A');
            int valueIndex = markerIndex + 1;
            if (markerIndex < 0 || valueIndex + 10 > response.Length)
            {
                AppendOutput("The weight response did not match the expected format.");
                return;
            }

            string weight = Encoding.ASCII.GetString(response, valueIndex, 10).Trim('\0', '\r', '\n', ' ', '\t');
            AppendOutput("Weight = " + weight);
        }

        /// <summary>Extracts the L/W/H values, which follow the ',' marker byte in the frame.</summary>
        private void DisplayDimensions(byte[] response)
        {
            int markerIndex = Array.IndexOf(response, (byte)',');
            int valueIndex = markerIndex + 1;
            if (markerIndex < 0 || valueIndex + 32 > response.Length)
            {
                AppendOutput("The dimensions response did not match the expected format.");
                return;
            }

            string dimensions = Encoding.ASCII.GetString(response, valueIndex, 32).Trim('\0', '\r', '\n', ' ', '\t');
            AppendOutput("Length, Width, Height = " + dimensions);
        }

        private void DimensionsButton_Click(object sender, EventArgs e)
        {
            SendCommand(MeasurementRequest.Dimensions, DimensionCommand);
        }

        private void WeightButton_Click(object sender, EventArgs e)
        {
            SendCommand(MeasurementRequest.Weight, WeightCommand);
        }

        /// <summary>Writes a command frame and arms the response timeout.</summary>
        private void SendCommand(MeasurementRequest request, byte[] command)
        {
            if (_serialPort == null || !_serialPort.IsOpen)
            {
                AppendOutput("Connect to a serial port before requesting a measurement.");
                return;
            }

            // Only one outstanding request at a time keeps response matching simple.
            if (_pendingRequest != MeasurementRequest.None)
            {
                AppendOutput("A measurement request is already waiting for a response.");
                return;
            }

            _receiveBuffer.Clear();
            _pendingRequest = request;
            try
            {
                _serialPort.Write(command, 0, command.Length);
                _responseTimer.Start();
                AppendOutput(request == MeasurementRequest.Weight
                    ? "Weight request sent. Waiting for response..."
                    : "Dimensions request sent. Waiting for response...");
            }
            catch (Exception exception)
            {
                _pendingRequest = MeasurementRequest.None;
                _responseTimer.Stop();
                AppendOutput("Could not send the request: " + exception.Message);
            }
        }

        /// <summary>Clears a stuck request when no complete response arrives in time.</summary>
        private void ResponseTimer_Tick(object sender, EventArgs e)
        {
            _responseTimer.Stop();
            _pendingRequest = MeasurementRequest.None;
            _receiveBuffer.Clear();
            AppendOutput("No complete response received within 5 seconds.");
        }

        /// <summary>Enables/disables controls so the UI reflects the connection state.</summary>
        private void SetConnectionState(bool connected)
        {
            portComboBox.Enabled = !connected;
            baudRateComboBox.Enabled = !connected;
            dataBitsComboBox.Enabled = !connected;
            handshakeComboBox.Enabled = !connected;
            refreshPortsButton.Enabled = !connected;
            connectButton.Text = connected ? "Disconnect" : "Connect";
            dimensionsButton.Enabled = connected;
            weightButton.Enabled = connected;
        }

        /// <summary>Closes and disposes the port, detaching handlers and resetting state.</summary>
        private void CloseSerialPort()
        {
            _responseTimer.Stop();
            _pendingRequest = MeasurementRequest.None;
            _receiveBuffer.Clear();

            if (_serialPort != null)
            {
                SerialPort port = _serialPort;
                _serialPort = null;
                port.DataReceived -= SerialPortOnDataReceived;
                if (port.IsOpen)
                {
                    port.Close();
                }
                port.Dispose();
            }

            SetConnectionState(false);
        }

        private void AppendOutput(string message)
        {
            outputTextBox.AppendText(message + Environment.NewLine);
        }

        /// <summary>Marshals an action onto the UI thread, ignoring it if the form is gone.</summary>
        private void PostToUi(Action action)
        {
            if (IsDisposed || !IsHandleCreated)
            {
                return;
            }

            try
            {
                BeginInvoke(action);
            }
            catch (InvalidOperationException)
            {
                // The form handle was destroyed between the checks and the call.
            }
        }

        /// <summary>Ensures the port is released when the window closes.</summary>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseSerialPort();
            _responseTimer.Dispose();
        }

        /// <summary>Identifies which measurement a pending response belongs to.</summary>
        private enum MeasurementRequest
        {
            None,
            Dimensions,
            Weight
        }
    }
}
