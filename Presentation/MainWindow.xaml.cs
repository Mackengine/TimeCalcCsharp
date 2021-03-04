using Presentation.Helpes;
using Presentation.Models;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
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
        public MainWindow()
        {
            InitializeComponent();

            //test data
            sharedData = new CarData() { data = new List<CarTimeRow>(), current = String.Empty };
            sharedData.data.Add(new CarTimeRow() { CarNumber = "7", StartTime = "3", EndTime = "4", CalculatedTime = "5", Status = "Recalculating ..." });
            sharedData.data.Add(new CarTimeRow() { CarNumber = "118", StartTime = "0", EndTime = "1", CalculatedTime = "2", Status = "Success" });
            //this.DataContext = data;
            this.times.ItemsSource = sharedData.data;
            //this.times.DataContext = data;
            //Since everything will need small tweaks, rather than reference the other
            //projects I'm going to commit a sin and copy/paste
            usb = new UsbRunner("COM3");
            usb.Setup(new SerialDataReceivedEventHandler(SerialDataReceivedHandler));

            rowAdder.Click += AddRow;

            SetTimer();
        }

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

                newest = sharedData.data.FirstOrDefault(dd => String.IsNullOrEmpty(dd.EndTime));
                if (newest != null)
                {
                    newest.EndTime = d.ToString();
                    CalculateElapsedTime(newest, d);
                }
                else
                {
                    Console.WriteLine($"Found a time with no match: {d}");
                    sharedData.data.Add(new CarTimeRow() { CarNumber = String.Empty, StartTime = String.Empty, EndTime = d.ToString(), CalculatedTime = String.Empty, Status = "Unknown" });
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
            newest.CalculatedTime = (finish - start).ToString();
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
    }
}
