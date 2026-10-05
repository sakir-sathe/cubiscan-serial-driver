namespace CubiscanSerialDriver
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.refreshPortsButton = new System.Windows.Forms.Button();
            this.portComboBox = new System.Windows.Forms.ComboBox();
            this.portLabel = new System.Windows.Forms.Label();
            this.baudRateLabel = new System.Windows.Forms.Label();
            this.baudRateComboBox = new System.Windows.Forms.ComboBox();
            this.baudRateHintLabel = new System.Windows.Forms.Label();
            this.dataBitsLabel = new System.Windows.Forms.Label();
            this.dataBitsComboBox = new System.Windows.Forms.ComboBox();
            this.dataBitsHintLabel = new System.Windows.Forms.Label();
            this.handshakeLabel = new System.Windows.Forms.Label();
            this.handshakeComboBox = new System.Windows.Forms.ComboBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.outputTextBox = new System.Windows.Forms.TextBox();
            this.dimensionsButton = new System.Windows.Forms.Button();
            this.weightButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // refreshPortsButton
            // 
            this.refreshPortsButton.Location = new System.Drawing.Point(284, 31);
            this.refreshPortsButton.Name = "refreshPortsButton";
            this.refreshPortsButton.Size = new System.Drawing.Size(102, 24);
            this.refreshPortsButton.TabIndex = 0;
            this.refreshPortsButton.Text = "Get All Ports";
            this.refreshPortsButton.UseVisualStyleBackColor = true;
            this.refreshPortsButton.Click += new System.EventHandler(this.RefreshPortsButton_Click);
            // 
            // portComboBox
            // 
            this.portComboBox.FormattingEnabled = true;
            this.portComboBox.Location = new System.Drawing.Point(132, 31);
            this.portComboBox.Name = "portComboBox";
            this.portComboBox.Size = new System.Drawing.Size(121, 24);
            this.portComboBox.TabIndex = 2;
            // 
            // portLabel
            // 
            this.portLabel.AutoSize = true;
            this.portLabel.Location = new System.Drawing.Point(12, 31);
            this.portLabel.Name = "portLabel";
            this.portLabel.Size = new System.Drawing.Size(114, 17);
            this.portLabel.TabIndex = 3;
            this.portLabel.Text = "Available Ports : ";
            // 
            // baudRateLabel
            // 
            this.baudRateLabel.AutoSize = true;
            this.baudRateLabel.Location = new System.Drawing.Point(12, 73);
            this.baudRateLabel.Name = "baudRateLabel";
            this.baudRateLabel.Size = new System.Drawing.Size(79, 17);
            this.baudRateLabel.TabIndex = 4;
            this.baudRateLabel.Text = "BaudRate :";
            // 
            // baudRateComboBox
            // 
            this.baudRateComboBox.FormattingEnabled = true;
            this.baudRateComboBox.Location = new System.Drawing.Point(132, 73);
            this.baudRateComboBox.Name = "baudRateComboBox";
            this.baudRateComboBox.Size = new System.Drawing.Size(121, 24);
            this.baudRateComboBox.TabIndex = 5;
            // 
            // baudRateHintLabel
            // 
            this.baudRateHintLabel.AutoSize = true;
            this.baudRateHintLabel.Location = new System.Drawing.Point(284, 79);
            this.baudRateHintLabel.Name = "baudRateHintLabel";
            this.baudRateHintLabel.Size = new System.Drawing.Size(158, 17);
            this.baudRateHintLabel.TabIndex = 6;
            this.baudRateHintLabel.Text = "(9600 is recommended)";
            // 
            // dataBitsLabel
            // 
            this.dataBitsLabel.AutoSize = true;
            this.dataBitsLabel.Location = new System.Drawing.Point(12, 109);
            this.dataBitsLabel.Name = "dataBitsLabel";
            this.dataBitsLabel.Size = new System.Drawing.Size(73, 17);
            this.dataBitsLabel.TabIndex = 7;
            this.dataBitsLabel.Text = "Data Bits :";
            // 
            // dataBitsComboBox
            // 
            this.dataBitsComboBox.FormattingEnabled = true;
            this.dataBitsComboBox.Location = new System.Drawing.Point(132, 106);
            this.dataBitsComboBox.Name = "dataBitsComboBox";
            this.dataBitsComboBox.Size = new System.Drawing.Size(121, 24);
            this.dataBitsComboBox.TabIndex = 8;
            // 
            // dataBitsHintLabel
            // 
            this.dataBitsHintLabel.AutoSize = true;
            this.dataBitsHintLabel.Location = new System.Drawing.Point(284, 113);
            this.dataBitsHintLabel.Name = "dataBitsHintLabel";
            this.dataBitsHintLabel.Size = new System.Drawing.Size(153, 17);
            this.dataBitsHintLabel.TabIndex = 9;
            this.dataBitsHintLabel.Text = "(8 bit is recommended)";
            // 
            // handshakeLabel
            // 
            this.handshakeLabel.AutoSize = true;
            this.handshakeLabel.Location = new System.Drawing.Point(12, 145);
            this.handshakeLabel.Name = "handshakeLabel";
            this.handshakeLabel.Size = new System.Drawing.Size(88, 17);
            this.handshakeLabel.TabIndex = 10;
            this.handshakeLabel.Text = "Handshake :";
            // 
            // handshakeComboBox
            // 
            this.handshakeComboBox.FormattingEnabled = true;
            this.handshakeComboBox.Location = new System.Drawing.Point(132, 145);
            this.handshakeComboBox.Name = "handshakeComboBox";
            this.handshakeComboBox.Size = new System.Drawing.Size(121, 24);
            this.handshakeComboBox.TabIndex = 11;
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(15, 209);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(95, 36);
            this.connectButton.TabIndex = 12;
            this.connectButton.Text = "Connect";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.ConnectButton_Click);
            // 
            // outputTextBox
            // 
            this.outputTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.outputTextBox.Location = new System.Drawing.Point(7, 284);
            this.outputTextBox.Multiline = true;
            this.outputTextBox.Name = "outputTextBox";
            this.outputTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.outputTextBox.Size = new System.Drawing.Size(436, 104);
            this.outputTextBox.TabIndex = 13;
            // 
            // dimensionsButton
            // 
            this.dimensionsButton.Location = new System.Drawing.Point(132, 209);
            this.dimensionsButton.Name = "dimensionsButton";
            this.dimensionsButton.Size = new System.Drawing.Size(110, 36);
            this.dimensionsButton.TabIndex = 14;
            this.dimensionsButton.Text = "Get Dim";
            this.dimensionsButton.UseVisualStyleBackColor = true;
            this.dimensionsButton.Click += new System.EventHandler(this.DimensionsButton_Click);
            // 
            // weightButton
            // 
            this.weightButton.Location = new System.Drawing.Point(284, 209);
            this.weightButton.Name = "weightButton";
            this.weightButton.Size = new System.Drawing.Size(102, 35);
            this.weightButton.TabIndex = 15;
            this.weightButton.Text = "Get Weight";
            this.weightButton.UseVisualStyleBackColor = true;
            this.weightButton.Click += new System.EventHandler(this.WeightButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(460, 400);
            this.Controls.Add(this.weightButton);
            this.Controls.Add(this.dimensionsButton);
            this.Controls.Add(this.outputTextBox);
            this.Controls.Add(this.connectButton);
            this.Controls.Add(this.handshakeComboBox);
            this.Controls.Add(this.handshakeLabel);
            this.Controls.Add(this.dataBitsHintLabel);
            this.Controls.Add(this.dataBitsComboBox);
            this.Controls.Add(this.dataBitsLabel);
            this.Controls.Add(this.baudRateHintLabel);
            this.Controls.Add(this.baudRateComboBox);
            this.Controls.Add(this.baudRateLabel);
            this.Controls.Add(this.portLabel);
            this.Controls.Add(this.portComboBox);
            this.Controls.Add(this.refreshPortsButton);
            this.Name = "MainForm";
            this.Text = "Cubiscan Serial Driver";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button refreshPortsButton;
        private System.Windows.Forms.ComboBox portComboBox;
        private System.Windows.Forms.Label portLabel;
        private System.Windows.Forms.Label baudRateLabel;
        private System.Windows.Forms.ComboBox baudRateComboBox;
        private System.Windows.Forms.Label baudRateHintLabel;
        private System.Windows.Forms.Label dataBitsLabel;
        private System.Windows.Forms.ComboBox dataBitsComboBox;
        private System.Windows.Forms.Label dataBitsHintLabel;
        private System.Windows.Forms.Label handshakeLabel;
        private System.Windows.Forms.ComboBox handshakeComboBox;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.TextBox outputTextBox;
        private System.Windows.Forms.Button dimensionsButton;
        private System.Windows.Forms.Button weightButton;
    }
}
