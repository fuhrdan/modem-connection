//*****************************************************************************
//** Modem Connection                                                        **
//** Simple program to detect and use a modem.  24.4k baud modem hardcoded   **
//** but can be changed depending on the speed (2400, 4800, 9600, 12.2k, etc **
//*****************************************************************************


using System;
using System.IO.Ports;
using System.Threading;

class ModemConnection
{
    static void Main(string[] args)
    {
        int baudRate = 24400; // 24.4k modem speed
        string phoneNumber = "1234567890"; // Replace with the phone number to dial

        try
        {
            string[] ports = SerialPort.GetPortNames();
            if (ports.Length == 0)
            {
                Console.WriteLine("No COM ports found.");
                return;
            }

            foreach (string portName in ports)
            {
                try
                {
                    using (SerialPort serialPort = new SerialPort(portName, baudRate))
                    {
                        // Set up serial port parameters
                        serialPort.Parity = Parity.None;
                        serialPort.StopBits = StopBits.One;
                        serialPort.DataBits = 8;
                        serialPort.Handshake = Handshake.None;
                        serialPort.ReadTimeout = 5000;
                        serialPort.WriteTimeout = 5000;

                        // Open the serial port
                        serialPort.Open();
                        Console.WriteLine($"Trying {portName}...");

                        // Send AT command to initialize the modem
                        serialPort.WriteLine("AT\r");
                        Thread.Sleep(1000); // Wait for the modem to respond

                        // Read and display modem response
                        string response = serialPort.ReadExisting();
                        if (response.Contains("OK"))
                        {
                            Console.WriteLine($"Modem detected on {portName}. Response: {response}");

                            // Send AT command to dial the phone number
                            serialPort.WriteLine($"ATDT{phoneNumber}\r");
                            Thread.Sleep(10000); // Wait for connection to be established

                            // Read and display connection response
                            response = serialPort.ReadExisting();
                            Console.WriteLine("Dialing response: " + response);

                            // Close the serial port
                            serialPort.Close();
                            Console.WriteLine("Serial port closed.");
                            break; // Exit the loop since we've found the modem
                        }
                        else
                        {
                            Console.WriteLine($"No modem detected on {portName}. Response: {response}");
                            serialPort.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error on {portName}: " + ex.Message);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
