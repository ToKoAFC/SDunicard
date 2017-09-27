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
using System.Threading;

namespace RSunicard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SerialPort sp;

        public MainWindow()
        {
            InitializeComponent();
            ConnectToCOM();
            Dashboard_Show();
        }

        private void ConnectToCOM()
        {
            try
            {
                sp = new SerialPort(COMinput.Text, 9600, Parity.None, 8, StopBits.One);
                sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                sp.Open();
                notificationGrid.Visibility = Visibility.Collapsed;
                connectButton.Visibility = Visibility.Collapsed;
                notificationLabel.Visibility = Visibility.Collapsed;
                notificationOKbutton.Visibility = Visibility.Collapsed;
                ShowNotification(InfoTypeEnum.Success, $"Połączono z {COMinput.Text}");
            }
            catch (Exception ex)
            {
                notificationGrid.Visibility = Visibility.Visible;
                connectButton.Visibility = Visibility.Visible;
                notificationLabel.Visibility = Visibility.Visible;
                notificationOKbutton.Visibility = Visibility.Collapsed;
                notificationGrid.Background = Brushes.LightPink;
                notificationLabel.Content = "Nie można połączyć się z portem szeregowym";
            }
        }


        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            var input = sp.ReadExisting();
            if (input.Length != 8)
            {
                Dispatcher.BeginInvoke((Action)(() => ShowNotification(InfoTypeEnum.Error, "Niepoprawe dane odebrane od czytnika!")));
                sp.Write("Brak karty ERROR\r\n");
                return;
            }
            if (Service.CheckCardIdIfExist(input))
            {
                var result = Service.AddEventToWorker(input);
                if (result.CardIdExisted)
                {
                    Dispatcher.BeginInvoke((Action)(() => ReloadContent()));
                    sp.Write($"{result.WorkerName} {result.EventType}\r\n");
                }
                return;
            }
            Dispatcher.BeginInvoke((Action)(() => SetNewWorkerCardId(input)));
        }

        private void SetNewWorkerCardId(string cardId)
        {
            ShowNotification(InfoTypeEnum.Info, "Wykryto nową karte, dodaj pracownika!");
            StateTab.Background = DashboardTab.Background = RaportsTab.Background = SettingsTab.Background = new SolidColorBrush(Color.FromRgb(189, 189, 189));
            ManageTab.Background = new SolidColorBrush(Color.FromRgb(117, 117, 117));


            StateContent.Visibility = DashboardContent.Visibility = RaportContent.Visibility = SettingsContent.Visibility = Visibility.Collapsed;
            ManageContent.Visibility = Visibility.Visible;
            ManagecardIDinput.Text = cardId;

        }

        private void ReloadContent()
        {
            LoadDashboardContent();
            LoadStateContent();
        }

        private void ConnectToCOMPortClick(object sender, RoutedEventArgs e)
        {
            ConnectToCOM();
        }

        //NOTIFICATION
        private void ShowNotification(InfoTypeEnum type, string message)
        {
            notificationGrid.Visibility = Visibility.Visible;
            connectButton.Visibility = Visibility.Collapsed;
            COMinput.Visibility = Visibility.Collapsed;
            COMlabel.Visibility = Visibility.Collapsed;
            notificationLabel.Visibility = Visibility.Visible;
            notificationOKbutton.Visibility = Visibility.Visible;
            switch (type)
            {
                case InfoTypeEnum.Error:
                    notificationGrid.Background = Brushes.LightPink;
                    break;
                case InfoTypeEnum.Info:
                    notificationGrid.Background = Brushes.LightGoldenrodYellow;
                    break;
                case InfoTypeEnum.Success:
                    notificationGrid.Background = Brushes.LightGreen;
                    break;
            }
            notificationLabel.Content = message;
        }
        private void DiscardNotification(object sender, RoutedEventArgs e)
        {
            notificationGrid.Visibility = Visibility.Collapsed;
            connectButton.Visibility = Visibility.Collapsed;
            notificationLabel.Visibility = Visibility.Collapsed;
            notificationOKbutton.Visibility = Visibility.Collapsed;
        }

        //TABS SWITCHING SERVICE
        private void DashboardClick(object sender, RoutedEventArgs e)
        {
            Dashboard_Show();
        }
        private void Dashboard_Show()
        {
            StateTab.Background = ManageTab.Background = RaportsTab.Background = SettingsTab.Background = new SolidColorBrush(Color.FromRgb(189, 189, 189));
            DashboardTab.Background = new SolidColorBrush(Color.FromRgb(117, 117, 117));

            StateContent.Visibility = ManageContent.Visibility = RaportContent.Visibility = SettingsContent.Visibility = Visibility.Collapsed;
            DashboardContent.Visibility = Visibility.Visible;
            LoadDashboardContent();
        }

        private void StateClick(object sender, RoutedEventArgs e)
        {
            State_Show();
        }
        private void State_Show()
        {
            RaportsTab.Background = ManageTab.Background = DashboardTab.Background = SettingsTab.Background = new SolidColorBrush(Color.FromRgb(189, 189, 189));
            StateTab.Background = new SolidColorBrush(Color.FromRgb(117, 117, 117));

            DashboardContent.Visibility = ManageContent.Visibility = RaportContent.Visibility = SettingsContent.Visibility = Visibility.Collapsed;
            StateContent.Visibility = Visibility.Visible;
            LoadStateContent();
        }

        private void ManageClick(object sender, RoutedEventArgs e)
        {
            Manage_Show();
        }
        private void Manage_Show()
        {
            StateTab.Background = DashboardTab.Background = RaportsTab.Background = SettingsTab.Background = new SolidColorBrush(Color.FromRgb(189, 189, 189));
            ManageTab.Background = new SolidColorBrush(Color.FromRgb(117, 117, 117));


            StateContent.Visibility = DashboardContent.Visibility = RaportContent.Visibility = SettingsContent.Visibility = Visibility.Collapsed;
            ManageContent.Visibility = Visibility.Visible;
            LoadManageContent();

        }

        private void RaportsClick(object sender, RoutedEventArgs e)
        {
            Raports_Show();
        }
        private void Raports_Show()
        {
            StateTab.Background = ManageTab.Background = DashboardTab.Background = SettingsTab.Background = new SolidColorBrush(Color.FromRgb(189, 189, 189));
            RaportsTab.Background = new SolidColorBrush(Color.FromRgb(117, 117, 117));

            StateContent.Visibility = ManageContent.Visibility = DashboardContent.Visibility = SettingsContent.Visibility = Visibility.Collapsed;
            RaportContent.Visibility = Visibility.Visible;
        }

        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            Settings_Show();
        }
        private void Settings_Show()
        {
            StateTab.Background = ManageTab.Background = DashboardTab.Background = RaportsTab.Background = new SolidColorBrush(Color.FromRgb(189, 189, 189));
            SettingsTab.Background = new SolidColorBrush(Color.FromRgb(117, 117, 117));

            StateContent.Visibility = ManageContent.Visibility = RaportContent.Visibility = DashboardContent.Visibility = Visibility.Collapsed;
            SettingsContent.Visibility = Visibility.Visible;

        }


        //LOAD CONTENTS
        // DASHBOARD
        private void LoadDashboardContent()
        {
            var dashboardItems = Service.GetTodaysEvents();
            DashboardTable.ItemsSource = dashboardItems;
        }

        //STATE
        private void LoadStateContent()
        {
            var companyList = Service.GetCompanyList();
            StateCompanyList.ItemsSource = companyList;
        }
        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var item = sender as ListViewItem;
            var company = item.Content as CompanyVM;
            if (item == null || company == null)
            {
                return;
            }
            var workers = Service.GetPresentWorkers(company.CompanyName);
            StateWorkersList.ItemsSource = workers;
        }

        //MENAGE
        private void LoadManageContent()
        {
            ManagecompanySelectList.ItemsSource = Service.GetCompanySelectList();
        }
        private void AddNewCompanyClick(object sender, RoutedEventArgs e)
        {
            Service.AddNewCompany(newCompanyName.Text);
            LoadManageContent();
            ShowNotification(InfoTypeEnum.Success, $"Dodano nową firmę o nazwie: {newCompanyName.Text}");
            newCompanyName.Text = string.Empty;
        }

        private void AddNewWorkerlick(object sender, RoutedEventArgs e)
        {
            var company = ManagecompanySelectList.SelectedItem as CompanyVM;
            Service.AddNewWorker(company.CompanyName, newWorkerName.Text, ManagecardIDinput.Text);
            ShowNotification(InfoTypeEnum.Success, $"{newWorkerName.Text} został dodany do firmy {company.CompanyName}");
            newWorkerName.Text = string.Empty;
            ManagecardIDinput.Text = string.Empty;
        }
    }
}
