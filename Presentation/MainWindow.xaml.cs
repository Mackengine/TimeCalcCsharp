using Presentation.Helpes;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private CarData sharedData;
        private UsbRunner usb;
        private ObservableCollection<ComboBoxItem> availableUsbPorts = new ObservableCollection<ComboBoxItem>();
        //private ComboBoxItem selectedUsbPort = null;
        private ExcelAuditor auditor = new ExcelAuditor();
        public MainWindow()
        {
            InitializeComponent();

            //test data
            sharedData = new CarData() { data = new List<CarTimeRow>(), current = String.Empty };
            this.times.ItemsSource = sharedData.data;

            //Since everything will need small tweaks, rather than reference the other
            //projects I'm going to commit a sin and copy/paste
            //String[] ports = System.IO.Ports.SerialPort.GetPortNames();
            //SetAvailableUsbPorts(GetAvailableUsbPorts().ToArray());

            usbChooser.Click += ChooseUsb;
            rowAdder.Click += AddRow;

            SetTimer();
        }

        private IEnumerable<String> GetAvailableUsbPorts()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort");
            //ManagementObjectCollection usbs = ;
            List<String> result = new List<String>();
            foreach (ManagementObject usb in searcher.Get())
            {
                result.Add(usb["Name"].ToString());
            }

            return result;
        }

        //private void SetAvailableUsbPorts(String[] allUsbPorts)
        //{
        //    if (allUsbPorts.Length == 0)
        //    {
        //        allUsbPorts = new string[1] { "No Usb Connected" };
        //    }
        //    foreach (String p in allUsbPorts)
        //    {
        //        if (selectedUsbPort == null)
        //        {
        //            selectedUsbPort = new ComboBoxItem() { Content = p, IsSelected = true };
        //            availableUsbPorts.Add(selectedUsbPort);
        //        }
        //        else
        //        {
        //            availableUsbPorts.Add(new ComboBoxItem() { Content = p });
        //        }
        //    }

        //    usbPorts.ItemsSource = availableUsbPorts;
        //    usbPorts.Items.Refresh();
        //}

        public void SetTimer()
        {
            DispatcherTimer dispatcher = new DispatcherTimer();
            dispatcher.Tick += new EventHandler(RebindData);
            dispatcher.Interval = new TimeSpan(0, 0, 0, 1, 0);
            dispatcher.Start();
        }

        public void RebindData(object sender, EventArgs e)
        {
            try
            {
                this.times.Items.Refresh();
                //String[] ports = System.IO.Ports.SerialPort.GetPortNames();
                //if (ports.Length > 0)
                //{
                //    this.SetAvailableUsbPorts(ports);
                //}
            }
            catch
            {
                //do nothing, you can't do that if it's being edited.
            }
        }

        private void SerialDataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            string indata = sp.ReadExisting();
            sharedData.current += indata;
            //Console.WriteLine("Data Received:");
            CarTimeRow newest;
            decimal d;
            if (sharedData.current.Trim().Contains("(S)") || sharedData.current.Contains("(M)"))
            {
                if (!decimal.TryParse(CleanRawUsb(sharedData.current), out d))
                {
                    Console.WriteLine($"Skipped: {sharedData.current}");
                    sharedData.current = String.Empty;
                    return;
                }

                newest = sharedData.data.FirstOrDefault(dd => String.IsNullOrEmpty(dd.StartTime));
                if (newest != null)
                {
                    newest.StartTime = d.ToString();
                    //CalculateElapsedTime(newest, d);
                }
                else
                {
                    //Console.WriteLine($"Found a time with no match: {d}");
                    sharedData.data.Add(new CarTimeRow() 
                    { 
                        CarNumber = String.Empty, 
                        StartTime = d.ToString(), 
                        EndTime = String.Empty, 
                        CalculatedTime = String.Empty, 
                        Status = "Started" 
                    });
                }
                //reset
                sharedData.current = String.Empty;
            }
            //Console.Write(indata);
        }

        private void CalculateElapsedTime(CarTimeRow newest, decimal finish)
        {
            decimal start;
            if (!Decimal.TryParse(newest.StartTime, out start))
            {
                return;
            }
            //TODO: If it rolls over, compensate for that
            //newest.CalculatedTime = (finish - start).ToString();

            //audit
            auditor.WriteToDisk(sharedData.data);
        }

        private String CleanRawUsb(String input)
        {
            Regex regex = new Regex("[^0-9.]");
            String result = regex.Replace(input, String.Empty);
            return result;
        }

        private void AddRow(object sender, RoutedEventArgs e)
        {
            sharedData.data.Add(new CarTimeRow() { CarNumber = String.Empty, StartTime = String.Empty, EndTime = String.Empty, CalculatedTime = String.Empty, Status = "New" });
            this.times.Items.Refresh();
        }

        private void ChooseUsb(object sender, RoutedEventArgs e)
        {
            String value = ((ComboBoxItem)usbPorts.SelectedValue).Content.ToString();
            if (value == "No Usb Connected")
            {
                timerStatus.Text = "No Usb Connected, Please install a Usb device.";
                return;
            }
            usb = new UsbRunner(value);
            bool usbWorking = usb.Setup(new SerialDataReceivedEventHandler(SerialDataReceivedHandler));
            if (!usbWorking)
            {
                timerStatus.Text = "COM port failed to open. Make sure timer is connected.";
            }
            else
            {
                timerStatus.Text = "COM port connected."
            }
        }
    }
}
