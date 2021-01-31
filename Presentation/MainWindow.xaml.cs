using Presentation.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

namespace Presentation
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<CarTimeRow> data = new List<CarTimeRow>();
        public MainWindow()
        {
            InitializeComponent();
            
            //test data
            data.Add(new CarTimeRow() { CarNumber = "7", StartTime = "3", EndTime = "4", CalculatedTime = "5", Status = "Recalculating ..." });
            data.Add(new CarTimeRow() { CarNumber = "118", StartTime = "0", EndTime = "1", CalculatedTime = "2", Status = "Success" });
            //this.DataContext = data;
            this.times.ItemsSource = data;
            //this.times.DataContext = data;
        }

        private void AddRow(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
