using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TimeReader
{
    public class Handler
    {
        private bool _continue;
        private SerialPort _serialPort;
        private SerialPort _serialPortOutput;
        private List<string> deviceTimeStamps = new List<string>();
        private List<double> doubleTimeStamps = new List<double>();
        private List<string> userTimeStamps = new List<string>();
        private List<string> outputTimeStamps = new List<string>();
        public async Task<bool> Run()
        {
            string name;
            string message;
            StringComparer stringComparer = StringComparer.OrdinalIgnoreCase;

            //TODO convert to async so that userInputReader can be running simultaneously
            Thread readThread = new Thread(Read);
            Thread outputThread = new Thread(Read);

            // Create a new SerialPort object with default settings.
            _serialPort = new SerialPort();
            _serialPortOutput = new SerialPort();

            // Allow the user to set the appropriate properties.
            Console.WriteLine("Please set the values for the input device");
            _serialPort.PortName = SetPortName(_serialPort.PortName);
            _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
            _serialPort.Parity = SetPortParity(_serialPort.Parity);
            _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
            _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
            _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

            Console.WriteLine("Please set the values for the output port");
            _serialPortOutput.PortName = SetPortName(_serialPortOutput.PortName);
            _serialPortOutput.BaudRate = SetPortBaudRate(_serialPortOutput.BaudRate);
            _serialPortOutput.Parity = SetPortParity(_serialPortOutput.Parity);
            _serialPortOutput.DataBits = SetPortDataBits(_serialPortOutput.DataBits);
            _serialPortOutput.StopBits = SetPortStopBits(_serialPortOutput.StopBits);
            _serialPortOutput.Handshake = SetPortHandshake(_serialPortOutput.Handshake);

            // Set the read/write timeouts
            _serialPort.ReadTimeout = 500;
            _serialPort.WriteTimeout = 500;
            _serialPortOutput.ReadTimeout = 500;
            _serialPortOutput.WriteTimeout = 500;

            _serialPort.Open();
            _continue = true;
            readThread.Start(deviceTimeStamps);

            _serialPortOutput.Open();
            outputThread.Start(outputTimeStamps);

            

            Console.Write("Name: ");
            name = Console.ReadLine();

            Console.WriteLine("Type QUIT to exit");

            while (_continue)
            {
                if(deviceTimeStamps.Any())
                {
                    continue;
                }
                //message = Console.ReadLine();

                //if (stringComparer.Equals("quit", message))
                //{
                //    _continue = false;
                //}
                //else
                //{
                //    _serialPort.WriteLine(
                //        String.Format("<{0}>: {1}", name, message));
                //}
            }

            readThread.Join();
            _serialPort.Close();

     

            return true;
        }

        //TODO convert to async method, change so that it can either be passed deviceTimeStamps by reference or return deviceTimeStamps
        //This will probably end up being something like private Task<IEnumerable<String>> Read() and return the timestamps directly
        private void Read(object serialTimes)
        {
            //List<double> deviceTimeStamps = new List<double>();
            while (_continue)
            {
                try
                {
                    var counter = 0;
                    double doubleMessage;
                    var message = _serialPort.ReadLine();
                    //Console.WriteLine(message);
                    message = Reverse(message);
                    doubleMessage = Convert.ToDouble(message);
                    ((List<string>)serialTimes).Add(message);
                    Console.WriteLine(deviceTimeStamps[counter]);
                    counter++;
                }
                catch (TimeoutException) { }
            }

        }
          
        // Display Port values and prompt user to enter a port.
        private string SetPortName(string defaultPortName)
        {
            string portName;

            Console.WriteLine("Available Ports:");
            foreach (string s in SerialPort.GetPortNames())
            {
                Console.WriteLine("   {0}", s);
            }
            
            Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
            portName = Console.ReadLine();

            if (portName == "" || !(portName.ToLower()).StartsWith("com"))
            {
                portName = defaultPortName;
            }
            return portName;
        }


        // Display BaudRate values and prompt user to enter a value.
        private int SetPortBaudRate(int defaultPortBaudRate)
        {
            string baudRate;

            Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
            baudRate = Console.ReadLine();

            if (baudRate == "")
            {
                baudRate = defaultPortBaudRate.ToString();
            }

            return int.Parse(baudRate);
        }

        // Display PortParity values and prompt user to enter a value.
        private Parity SetPortParity(Parity defaultPortParity)
        {
            string parity;

            Console.WriteLine("Available Parity options:");
            foreach (string s in Enum.GetNames(typeof(Parity)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
            parity = Console.ReadLine();

            if (parity == "")
            {
                parity = defaultPortParity.ToString();
            }

            return (Parity)Enum.Parse(typeof(Parity), parity, true);
        }

        // Display DataBits values and prompt user to enter a value.
        private int SetPortDataBits(int defaultPortDataBits)
        {
            string dataBits;

            Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
            dataBits = Console.ReadLine();

            if (dataBits == "")
            {
                dataBits = defaultPortDataBits.ToString();
            }

            return int.Parse(dataBits.ToUpperInvariant());
        }

        // Display StopBits values and prompt user to enter a value.
        private StopBits SetPortStopBits(StopBits defaultPortStopBits)
        {
            string stopBits;

            Console.WriteLine("Available StopBits options:");
            foreach (string s in Enum.GetNames(typeof(StopBits)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter StopBits value (None is not supported and \n" +
             "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
            stopBits = Console.ReadLine();

            if (stopBits == "")
            {
                stopBits = defaultPortStopBits.ToString();
            }

            return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
        }

        private Handshake SetPortHandshake(Handshake defaultPortHandshake)
        {
            string handshake;

            Console.WriteLine("Available Handshake options:");
            foreach (string s in Enum.GetNames(typeof(Handshake)))
            {
                Console.WriteLine("   {0}", s);
            }

            Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
            handshake = Console.ReadLine();

            if (handshake == "")
            {
                handshake = defaultPortHandshake.ToString();
            }

            return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
        }

        //reads user input to a list to be returned for passing into a parsing and then a calculation function 
        private async Task<List<string>> userInputReader(string[] args)
        {
            List<string> userTimeInput = new List<string>();
            int i = 0;
            while (userTimeInput[i] != "quit")
            {
                string readMessage;
                readMessage = Console.ReadLine();
                userTimeInput.Add(readMessage);
                i++;
            }
            for (int j = 0; j < i; j++)
            {
                Console.WriteLine(userTimeInput[j]);
            }
            return userTimeInput;
        }

        private double timeMath(double userTime, double deviceTime)
        {
            double timeDifference;
            if (deviceTime - userTime < 0)
            {
                timeDifference = deviceTime - userTime + 1000;
            }
            else
            {
                timeDifference = deviceTime - userTime;
            }

            return timeDifference;
        }
        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }
    }
}
