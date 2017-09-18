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
            List<CompanyVM> items = new List<CompanyVM>();
            items.Add(new CompanyVM() { CompanyName = "OPGK Rzeszów", WorkersCount = 42 });
            items.Add(new CompanyVM() { CompanyName = "Assecco Poland", WorkersCount = 139 });
            items.Add(new CompanyVM() { CompanyName = "PGS Software", WorkersCount = 97 });
            CompanyList.ItemsSource = items;


            List<EventVM> eventItems = new List<EventVM>();
            eventItems.Add(new EventVM() { CompanyName = "OPGK Rzeszów", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wejscie", WorkerName = "Tomasz Krupa", CardID = "1011"});
            eventItems.Add(new EventVM() { CompanyName = "PGS Software", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wyjscie", WorkerName = "Robert Nowak", CardID = "1021" });
            eventItems.Add(new EventVM() { CompanyName = "PGS Software", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wyjscie", WorkerName = "Jan Kowalski", CardID = "1054" });
            eventItems.Add(new EventVM() { CompanyName = "PGS Software", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wyjscie", WorkerName = "Paweł Solny", CardID = "1045" });
            eventItems.Add(new EventVM() { CompanyName = "Assecco Poland", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wejscie", WorkerName = "Aleksandra Nowicka", CardID = "1099" });
            eventItems.Add(new EventVM() { CompanyName = "OPGK Rzeszów", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wejscie", WorkerName = "Marta Krupa", CardID = "1109" });
            
            EventsList.ItemsSource = eventItems;




        }
    }
}
