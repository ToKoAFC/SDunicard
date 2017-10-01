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
using System.Globalization;

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
            SerialPortsList.ItemsSource = SerialPort.GetPortNames();
            Dashboard_Show();
        }

        private void ConnectToCOM()
        {
            try
            {
                sp = new SerialPort(SerialPortsList.SelectedItem as string, 9600, Parity.None, 8, StopBits.One);
                sp.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);
                sp.Open();
                connentionBar.Visibility = Visibility.Collapsed;
                ShowNotification(InfoTypeEnum.Success, $"Połączono z {SerialPortsList.SelectedItem as string}");
            }
            catch (Exception ex)
            {
                connentionBar.Visibility = Visibility.Visible;
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
                    sp.Write($"{result.WorkerName.RemoveDiacritics()} {result.EventType}\r\n");
                }
                return;
            }
            Dispatcher.BeginInvoke((Action)(() => SetNewWorkerCardId(input)));
        }

        private void SetNewWorkerCardId(string cardId)
        {
            ShowNotification(InfoTypeEnum.Info, "Wykryto nową karte, dodaj pracownika!");
            StateTab.Background = DashboardTab.Background = RaportsTab.Background = new SolidColorBrush(Color.FromRgb(189, 189, 189));
            ManageTab.Background = new SolidColorBrush(Color.FromRgb(117, 117, 117));


            StateContent.Visibility = DashboardContent.Visibility = RaportContent.Visibility =  Visibility.Collapsed;
            ManageContent.Visibility = Visibility.Visible;
            ManagecardIDinput.Text = cardId;

        }

        private void ReloadContent()
        {
            LoadDashboardContent();
            LoadStateContent();
            LoadManageContent();
        }

        private void ConnectToCOMPortClick(object sender, RoutedEventArgs e)
        {
            ConnectToCOM();
        }

        //NOTIFICATION
        private void ShowNotification(InfoTypeEnum type, string message)
        {
            notificatiaonBar.Visibility = Visibility.Visible;
            switch (type)
            {
                case InfoTypeEnum.Error:
                    notificatiaonBar.Background = Brushes.LightPink;
                    break;
                case InfoTypeEnum.Info:
                    notificatiaonBar.Background = Brushes.LightGoldenrodYellow;
                    break;
                case InfoTypeEnum.Success:
                    notificatiaonBar.Background = Brushes.LightGreen;
                    break;
            }
            NotificationLabel.Content = message;
        }
        private void DiscardNotification(object sender, RoutedEventArgs e)
        {
            notificatiaonBar.Visibility = Visibility.Collapsed;
        }

        //TABS SWITCHING SERVICE
        private void DashboardClick(object sender, RoutedEventArgs e)
        {
            Dashboard_Show();
        }
        private void Dashboard_Show()
        {
            StateTab.Background = ManageTab.Background = RaportsTab.Background = Brushes.White;
            DashboardTab.Background = new SolidColorBrush(Color.FromRgb(190, 230, 253));

            StateContent.Visibility = ManageContent.Visibility = RaportContent.Visibility = Visibility.Collapsed;
            DashboardContent.Visibility = Visibility.Visible;
            LoadDashboardContent();
        }

        private void StateClick(object sender, RoutedEventArgs e)
        {
            State_Show();
        }
        private void State_Show()
        {
            RaportsTab.Background = ManageTab.Background = DashboardTab.Background = Brushes.White;
            StateTab.Background = new SolidColorBrush(Color.FromRgb(190, 230, 253));

            DashboardContent.Visibility = ManageContent.Visibility = RaportContent.Visibility = Visibility.Collapsed;
            StateContent.Visibility = Visibility.Visible;
            LoadStateContent();
        }

        private void ManageClick(object sender, RoutedEventArgs e)
        {
            Manage_Show();
        }
        private void Manage_Show()
        {
            StateTab.Background = DashboardTab.Background = RaportsTab.Background = Brushes.White;
            ManageTab.Background = new SolidColorBrush(Color.FromRgb(190, 230, 253));


            StateContent.Visibility = DashboardContent.Visibility = RaportContent.Visibility = Visibility.Collapsed;
            ManageContent.Visibility = Visibility.Visible;
            LoadManageContent();

        }

        private void RaportsClick(object sender, RoutedEventArgs e)
        {
            Raports_Show();
        }
        private void Raports_Show()
        {
            StateTab.Background = ManageTab.Background = DashboardTab.Background = Brushes.White;
            RaportsTab.Background = new SolidColorBrush(Color.FromRgb(190, 230, 253));

            StateContent.Visibility = ManageContent.Visibility = DashboardContent.Visibility = Visibility.Collapsed;
            RaportContent.Visibility = Visibility.Visible;
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
            var items = Service.GetCompanySelectList();
            ManagecompanySelectList.ItemsSource = items;
            ManagecompanyDeleteSelectList.ItemsSource = items;
            ManageworkersDeleteSelectList.ItemsSource = Service.GetWorkersSelectList();
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
            if (company == null) return;
            Service.AddNewWorker(company.CompanyName, newWorkerName.Text, ManagecardIDinput.Text);
            LoadManageContent();
            ShowNotification(InfoTypeEnum.Success, $"{newWorkerName.Text} został dodany do firmy {company.CompanyName}");
            newWorkerName.Text = string.Empty;
            ManagecardIDinput.Text = string.Empty;
        }

        private void DeleteCompanyClick(object sender, RoutedEventArgs e)
        {
            var company = ManagecompanyDeleteSelectList.SelectedItem as CompanyVM;
            if (company == null) return;
            Service.DeleteCompany(company.CompanyName);
            LoadManageContent();
            ShowNotification(InfoTypeEnum.Success, $"Usunięto firmę: {company.CompanyName}");
            newWorkerName.Text = string.Empty;
            ManagecardIDinput.Text = string.Empty;
        }

        private void DeleteWorkerClick(object sender, RoutedEventArgs e)
        {
            var worker = ManageworkersDeleteSelectList.SelectedItem as WorkerVM;
            if (worker == null) return;
            Service.DeleteWorker(worker.CardID);
            LoadManageContent();
            ShowNotification(InfoTypeEnum.Success, $"Usunięto pracownika: {worker.Name}");
            newWorkerName.Text = string.Empty;
            ManagecardIDinput.Text = string.Empty;
        }

        //RAPORTS 

        private void DailyRaportClick(object sender, RoutedEventArgs e)
        {
            //todo Serivce.daily...
            ShowNotification(InfoTypeEnum.Success, $"Utworzono backup bazy danych. Data :{DateTime.Now.ToShortDateString()}");
        }
        private void BackupDatabaseClick(object sender, RoutedEventArgs e)
        {
            //todo Serivce.backup...
            ShowNotification(InfoTypeEnum.Success, $"Utworzono backup bazy danych. Data :{DateTime.Now.ToShortDateString()}");
        }
        private void CurrentRaportClick(object sender, RoutedEventArgs e)
        {
            //todo Serivce.current...
            ShowNotification(InfoTypeEnum.Success, $"Utworzono backup bazy danych. Data :{DateTime.Now.ToShortDateString()}");
        }

        // SETTINGS

        string[] ports = SerialPort.GetPortNames();
    }

    public static class StringExtension
    {
        public static string RemoveDiacritics(this string text)
        {
            return Encoding.UTF8.GetString(Encoding.GetEncoding("ISO-8859-8").GetBytes(text));
        }
    }
}
