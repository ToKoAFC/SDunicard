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

namespace RSunicard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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

            eventItems.Add(new EventVM() { CompanyName = "OPGK Rzeszów", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wejscie", WorkerName = "Tomasz Krupa", CardID = "1011" });
            eventItems.Add(new EventVM() { CompanyName = "PGS Software", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wyjscie", WorkerName = "Robert Nowak", CardID = "1021" });
            eventItems.Add(new EventVM() { CompanyName = "PGS Software", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wyjscie", WorkerName = "Jan Kowalski", CardID = "1054" });
            eventItems.Add(new EventVM() { CompanyName = "PGS Software", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wyjscie", WorkerName = "Paweł Solny", CardID = "1045" });
            eventItems.Add(new EventVM() { CompanyName = "Assecco Poland", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wejscie", WorkerName = "Aleksandra Nowicka", CardID = "1099" });
            eventItems.Add(new EventVM() { CompanyName = "OPGK Rzeszów", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wejscie", WorkerName = "Marta Krupa", CardID = "1109" });

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
        
    }
}
