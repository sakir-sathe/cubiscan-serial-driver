# Cubiscan Serial Driver

A minimal, open-source Windows Forms reference application that connects to a
[Cubiscan](https://cubiscan.com/) dimensioning/weighing device over a serial
(COM) port and requests **dimensions** and **weight**. It is intended as a
starting point for developers who need to integrate a Cubiscan into their own
software without a third‑party device subscription.

> Status: reference / proof of concept. The serial command bytes and expected
> response formats must be verified against your specific Cubiscan model and
> firmware before production use.

## Features

- Enumerate available serial ports and pick one.
- Configure baud rate, data bits, and handshake (defaults: 9600 / 8 / None).
- Connect / disconnect to the device over RS‑232 / USB virtual COM.
- Send the **Get Dimensions** and **Get Weight** commands.
- Non‑blocking, buffered read of the response with a 5‑second timeout.

## Requirements

- Windows
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- A Cubiscan device exposed as a serial/COM port (RS‑232 adapter or USB)

## Build and run

```powershell
dotnet build
dotnet run
```

## How to use

1. Click **Get All Ports** and select the device's COM port.
2. Confirm baud rate, data bits, and handshake (9600 / 8 / None are typical).
3. Click **Connect**.
4. Click **Get Dim** or **Get Weight** to request a measurement.

Results and status messages appear in the output box.

## Reusing the serial logic

The serial communication lives in [`MainForm.cs`](MainForm.cs). The command
bytes and response parsing are isolated so they can be lifted into a headless
library, a background service, or a local HTTP/WebSocket bridge that a web app
can call — keeping the device-facing code on the machine physically connected
to the Cubiscan.

The protocol constants to review for your model:

| Request    | Command bytes (hex)        | Expected response length |
| ---------- | -------------------------- | ------------------------ |
| Dimensions | `02 4D 03 0D 0A`           | 62 bytes                 |
| Weight     | `02 73 03 0D 0A`           | 16 bytes                 |

## Using this prototype with a web app

Once this desktop app talks to your Cubiscan successfully, you have a *known‑good*
command set and response format. A browser cannot open a COM port directly, so the
pattern is to keep this serial code on the machine physically connected to the
device and expose it over the network for your web app to call.

Recommended path:

1. **Confirm it works here first.** Run this app, connect, and verify **Get Dim**
   and **Get Weight** return correct values. This proves the port settings, command
   bytes, and response parsing for your specific model.
2. **Move the serial logic into a small local service.** Lift the serial code from
   [`MainForm.cs`](MainForm.cs) (open port, send command frame, buffer and parse the
   response) into a headless .NET project — for example an ASP.NET Core Minimal API
   or a Windows Service — running on the PC wired to the Cubiscan.
3. **Expose a tiny local API.** Have that service own the COM port and offer simple
   endpoints such as `GET /weight` and `GET /dimensions` that return JSON, e.g.
   `{ "length": 10.0, "width": 8.0, "height": 4.0, "weight": 2.35 }`. A WebSocket
   can be used instead if you want the browser to receive live measurements.
4. **Call it from the web app.** The frontend (or your backend) sends an HTTP/WebSocket
   request to that local service and displays the returned measurement. No browser
   serial access and no third‑party device subscription are required.

Notes:

- Keep **one owner of the serial port**. Run a single instance of the service so the
  port is not opened by two processes at once.
- If the web app runs on a different machine than the device, the local service must
  be reachable on the network (host/port + firewall), and you should add basic auth
  and HTTPS.
- The command bytes and response layout above are what the web service should reuse —
  they are the whole reason to validate them with this prototype first.

## License

Released under the [MIT License](LICENSE).
