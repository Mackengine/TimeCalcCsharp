using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TimeReader.Test.Models;

namespace TimeReader.Test.Helpers
{
    public class Handler
    {
        private CarData sharedData;
        public void Run()
        {
            sharedData = new CarData()
            {
                data = new List<CarTime>(),
                run = true,
                serialMessages = new List<String>(),
                usb = new SerialPort(),
                current = String.Empty
            };

            sharedData.usb.PortName = "COM3";
            sharedData.usb.ReadTimeout = 500;
            sharedData.usb.WriteTimeout = 100000;
            sharedData.usb.BaudRate = 1200;//need for the mode.
            sharedData.usb.Parity = Parity.None;
            sharedData.usb.StopBits = StopBits.One;
            sharedData.usb.DataBits = 8;
            sharedData.usb.Handshake = Handshake.None;
            sharedData.usb.RtsEnable = true;
            sharedData.usb.Open();
            sharedData.usb.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
            //2 threads to run in the backgroun and read data
            //Thread readConsoleThread = new Thread(ReadConsole);
            //Thread readSerialThread = new Thread(ReadSerial);

            //start em up, let's GO BOIIIIII
            //readConsoleThread.Start(sharedData);
            //readSerialThread.Start(sharedData);

            //so this is the "main" thread which will handle the data that's
            //inputted in the other threads
            while (sharedData.run)
            {
                if (sharedData.serialMessages.Any())
                {
                    foreach (String message in sharedData.serialMessages)
                    {
                        //quit condition
                        if (message.ToLower() == "quit")
                        {
                            Finalize(sharedData);
                            sharedData.run = false;
                            continue;
                        }

                        //testing
                        Console.WriteLine(message);
                    }
                }
            }

            //clean up
            sharedData.usb.Close();
            sharedData.usb.Dispose();
            //readConsoleThread.Abort();
            //readSerialThread.Abort();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            sharedData.current += indata;
            //Console.WriteLine("Data Received:");
            CarTime newest;
            decimal d;
            if (sharedData.current.Trim().Contains("(M)"))
            {
                if (!decimal.TryParse(CleanRawUsb(sharedData.current), out d))
                {
                    Console.WriteLine($"Skipped: {sharedData.current}");
                    sharedData.current = String.Empty;
                    return;
                }

                newest = sharedData.data.FirstOrDefault(dd => dd.Stop == null);
                if (newest != null)
                {
                    newest.Stop = d.ToString();
                }
                else
                {
                    Console.WriteLine($"Found a time with no match: {d}");
                }
                //reset
                sharedData.current = String.Empty;
            }
            //Console.Write(indata);
        }

        private String CleanRawUsb(String input)
        {
            return input.Replace("\u000e", String.Empty)
                .Replace("\u000f", String.Empty)
                .Replace("\r", String.Empty)
                .Replace("(M)", String.Empty)
                .Replace(" ", String.Empty);
        }

        //grab data, dump to csv or excel or whatever
        private void Finalize(CarData sharedData)
        {

        }

        private void ReadConsole(object data)
        {
            String number;
            String time;
            while (((CarData)data).run)
            {
                Console.WriteLine("Car Number:");
                number = Console.ReadLine();
                Console.WriteLine("Time:");
                time = Console.ReadLine();
                ((CarData)data).data.Add(new CarTime()
                {
                    Number = number,
                    Start = time, 
                    Stop = null
                });
            }
        }

        private void ReadSerial(object data)
        {
            CarData d = ((CarData)data);
            CarTime newest;
            String message;
            while (d.run)
            {
                try
                {
                    message = d.usb.ReadLine();
                    newest = d.data.FirstOrDefault(dd => dd.Stop == null);
                    if (newest != null)
                    {
                        newest.Stop = message;
                    }
                    else
                    {
                        Console.WriteLine($"Found a time with no match: {message}");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message != "The operation has timed out.")
                    {
                        Console.WriteLine(ex.Message);
                    }
                    continue;
                    //nothing to do, just means nothing on the serial
                }
            }
        }
    }
}
