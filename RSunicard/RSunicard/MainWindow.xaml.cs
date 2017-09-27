using RSunicard.Models;
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
using System.Web.Script.Serialization;
using RSunicard.Database.Models;
using Newtonsoft.Json;
using RSunicard.Logic;
using System.IO.Ports;

namespace RSunicard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SerialPort sp = new SerialPort("COM3", 9600, Parity.None, 8, StopBits.One);

        public MainWindow()
        {
            InitializeComponent();
            ConnectToCOM();
        }

        private void ConnectToCOM()
        {
            try
            {
                sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                sp.Open();
                warningGrid.Visibility = Visibility.Collapsed;
                connectButton.Visibility = Visibility.Collapsed;
                warningLabel.Visibility = Visibility.Collapsed;
            }
            catch
            {
                warningGrid.Visibility = Visibility.Visible;
                connectButton.Visibility = Visibility.Visible;
                warningLabel.Visibility = Visibility.Visible;
                warningGrid.Background = Brushes.LightPink;
                warningLabel.Content = "Nie można połączyć się z portem szeregowym";
            }
        }
        

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var input = sp.ReadExisting();
            var result = Service.ReceiveSerialPortSignal(input);
            if (result)
            {

            }
        }


        private void DashboardClick(object sender, RoutedEventArgs e)
        {
            RaportsTab.Opacity = 0.6;
            RaportsTab.Background = Brushes.LightGray;
            ManageTab.Opacity = 0.6;
            ManageTab.Background = Brushes.LightGray;
            DashboardTab.Opacity = 1;
            DashboardTab.Background = Brushes.Gray;
            DashboardContent.Visibility = Visibility.Visible;
            ManageContent.Visibility = Visibility.Collapsed;
            RaportContent.Visibility = Visibility.Collapsed;
            var dbModel = Service.GetDBModel();

            //Company section
            var companyItems = new List<CompanyVM>();
            if (dbModel.Companies != null)
            {
                companyItems = dbModel.Companies.Select(x => new CompanyVM
                {
                    CompanyName = x.CompanyName,
                    WorkersCount = x.Workers.Count
                }).ToList();
            }
            CompanyList.ItemsSource = companyItems;
        }

        private void ManageClick(object sender, RoutedEventArgs e)
        {
            RaportsTab.Opacity = 0.6;
            RaportsTab.Background = Brushes.LightGray;
            DashboardTab.Opacity = 0.6;
            DashboardTab.Background = Brushes.LightGray;
            ManageTab.Opacity = 1;
            ManageTab.Background = Brushes.Gray;
            DashboardContent.Visibility = Visibility.Collapsed;
            ManageContent.Visibility = Visibility.Visible;
            RaportContent.Visibility = Visibility.Collapsed;
            //companySelectList.ItemsSource = Service.GetCompanySelectList();
        }

        private void RaportsClick(object sender, RoutedEventArgs e)
        {
            DashboardTab.Opacity = 0.6;
            DashboardTab.Background = Brushes.LightGray;
            ManageTab.Opacity = 0.6;
            ManageTab.Background = Brushes.LightGray;
            RaportsTab.Opacity = 1;
            RaportsTab.Background = Brushes.Gray;
            DashboardContent.Visibility = Visibility.Collapsed;
            ManageContent.Visibility = Visibility.Collapsed;
            RaportContent.Visibility = Visibility.Visible;
        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var eventItems = new List<EventVM>();
            var item = sender as ListViewItem;
            var company = item.Content as CompanyVM;
            if (item == null || company == null)
            {
                return;
            }
            eventItems = Service.GetEvents(company.CompanyName);
            EventsList.ItemsSource = eventItems;
        }

        private void AddNewCompanyClick(object sender, RoutedEventArgs e)
        {
            AddCompany winAddCompany = new AddCompany();
            winAddCompany.Show();
        }

        private void AddNewWorkerlick(object sender, RoutedEventArgs e)
        {
            AddWorker winAddNewWorker = new AddWorker();
            winAddNewWorker.Show();
        }

        private void ConnectToCOMPortClick(object sender, RoutedEventArgs e)
        {
            ConnectToCOM();
        }
    }
}
