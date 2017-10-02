using RSunicard.Logic;
using RSunicard.Logic.Extensions;
using RSunicard.Models;
using System;
using System.IO.Ports;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RSunicard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public SerialPort sp = new SerialPort();

        public MainWindow()
        {
            InitializeComponent();
            ConnectToCOM();
            Dashboard_Show();
        }
        private void CheckSerialPortConnection()
        {
            SerialPortsList.ItemsSource = SerialPort.GetPortNames();
            if (sp.IsOpen) return;
            connentionBar.Visibility = Visibility.Visible;
        }

        private void ConnectToCOM()
        {
            if (sp.IsOpen)
            {
                connentionBar.Visibility = Visibility.Visible;
                return;
            }
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
            Dispatcher.BeginInvoke((Action)(() => ReloadContent()));
            Dispatcher.BeginInvoke((Action)(() => SetNewWorkerCardId(input)));
        }

        private void SetNewWorkerCardId(string cardId)
        {
            ShowNotification(InfoTypeEnum.Info, "Wykryto nową karte, dodaj pracownika!");
            Manage_Show();
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
                    notificatiaonBar.Background = Brushes.LightGray;
                    break;
                case InfoTypeEnum.Success:
                    notificatiaonBar.Background = Brushes.LightGreen;
                    break;
            }
            NotificationLabel.Content = message;
        }
        private void DiscardNotification(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
            notificatiaonBar.Visibility = Visibility.Collapsed;
        }

        #region Tab switching section
        private void DashboardClick(object sender, RoutedEventArgs e)
        {
            Dashboard_Show();
        }
        private void Dashboard_Show()
        {
            StateTab.Background = ManageTab.Background = Brushes.White;
            DashboardTab.Background = new SolidColorBrush(Color.FromRgb(190, 230, 253));

            StateContent.Visibility = ManageContent.Visibility = Visibility.Collapsed;
            DashboardContent.Visibility = Visibility.Visible;
            LoadDashboardContent();
        }
        private void LoadDashboardContent()
        {
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
            var dashboardItems = Service.GetTodaysEvents();
            DashboardTable.ItemsSource = dashboardItems;
        }

        private void StateClick(object sender, RoutedEventArgs e)
        {
            State_Show();
        }
        private void State_Show()
        {
            ManageTab.Background = DashboardTab.Background = Brushes.White;
            StateTab.Background = new SolidColorBrush(Color.FromRgb(190, 230, 253));

            DashboardContent.Visibility = ManageContent.Visibility = Visibility.Collapsed;
            StateContent.Visibility = Visibility.Visible;
            LoadStateContent();
        }
        private void LoadStateContent()
        {
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
            var companyList = Service.GetCompanyList();
            StateCompanyList.ItemsSource = companyList;
        }

        private void ManageClick(object sender, RoutedEventArgs e)
        {
            Manage_Show();
        }
        private void Manage_Show()
        {
            StateTab.Background = DashboardTab.Background = Brushes.White;
            ManageTab.Background = new SolidColorBrush(Color.FromRgb(190, 230, 253));

            StateContent.Visibility = DashboardContent.Visibility = Visibility.Collapsed;
            ManageContent.Visibility = Visibility.Visible;
            LoadManageContent();
        }
        private void LoadManageContent()
        {
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
            var items = Service.GetCompanySelectList();
            ManagecompanySelectList.ItemsSource = items;
            var raportdays = Service.GetRaportsAvailableDays();
            RaportDaysList.ItemsSource = raportdays;
            RaportCompanyDayList.ItemsSource = raportdays;
            RaportCompanyList.ItemsSource = items;
            ManagecompanyDeleteSelectList.ItemsSource = items;
            ManageworkersDeleteSelectList.ItemsSource = Service.GetWorkersSelectList();
        }
        #endregion
        //STATE
        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
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
        private void AddNewCompanyClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
            Service.AddNewCompany(newCompanyName.Text.RemoveDiacritics());
            LoadManageContent();
            ShowNotification(InfoTypeEnum.Success, $"Dodano nową firmę o nazwie: {newCompanyName.Text}");
            newCompanyName.Text = string.Empty;
        }

        private void AddNewWorkerlick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
            var company = ManagecompanySelectList.SelectedItem as CompanyVM;
            if (company == null) return;
            Service.AddNewWorker(company.CompanyName.RemoveDiacritics(), newWorkerName.Text.RemoveDiacritics(), ManagecardIDinput.Text);
            LoadManageContent();
            ShowNotification(InfoTypeEnum.Success, $"{newWorkerName.Text} został dodany do firmy {company.CompanyName}");
            newWorkerName.Text = string.Empty;
            ManagecardIDinput.Text = string.Empty;
        }

        private void DeleteCompanyClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
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
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
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
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
            var raport = RaportDaysList.SelectedItem as RaportVM;
            if (raport == null)
            {
                return;
            }
            Service.GenerateRaport(raport);
            ShowNotification(InfoTypeEnum.Success, $"Utworzono raport z dnia {DateTime.Now.ToShortDateString()}.");
        }
        private void DailyCompanyRaportClick(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => CheckSerialPortConnection()));
            var raport = RaportCompanyDayList.SelectedItem as RaportVM;
            var company = RaportCompanyList.SelectedItem as CompanyVM;
            if (raport == null || company == null)
            {
                return;
            }
            Service.GenerateRaportForCompany(raport, company.CompanyName);
            ShowNotification(InfoTypeEnum.Success, $"Utworzono raport z dnia {DateTime.Now.ToShortDateString()}, dla firmy{company.CompanyName}.");
        }
    }
}
