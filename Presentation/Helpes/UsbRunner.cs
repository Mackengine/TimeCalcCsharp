using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Helpes
{
    public class UsbRunner
    {
        private SerialPort usb;
        public UsbRunner(String port)
        {
            usb = new SerialPort();
            
            //set default parameters.
            usb.PortName = port;
            usb.ReadTimeout = 500;
            usb.WriteTimeout = 100000;
            usb.BaudRate = 1200;//need for the mode.
            usb.Parity = Parity.None;
            usb.StopBits = StopBits.One;
            usb.DataBits = 8;
            usb.Handshake = Handshake.None;
            usb.RtsEnable = true;
        }

        public bool Setup(SerialDataReceivedEventHandler theHandler)
        {
            try
            {
                usb.DataReceived += theHandler;
                usb.Open();
            }
            catch
            {
                return false;
            }
            

            return true;
        }

    }
}
