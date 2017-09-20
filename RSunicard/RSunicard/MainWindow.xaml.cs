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
            List<CompanyVM> items = new List<CompanyVM>();
            items.Add(new CompanyVM() { CompanyName = "OPGK Rzeszów", WorkersCount = 42 });
            items.Add(new CompanyVM() { CompanyName = "Assecco Poland", WorkersCount = 139 });
            items.Add(new CompanyVM() { CompanyName = "PGS Software", WorkersCount = 97 });
            CompanyList.ItemsSource = items;


            List<EventVM> eventItems = new List<EventVM>();
            eventItems.Add(new EventVM() { CompanyName = "OPGK Rzeszów", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wejscie", WorkerName = "Tomasz Krupa", CardID = "1011" });
            eventItems.Add(new EventVM() { CompanyName = "PGS Software", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wyjscie", WorkerName = "Robert Nowak", CardID = "1021" });
            eventItems.Add(new EventVM() { CompanyName = "PGS Software", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wyjscie", WorkerName = "Jan Kowalski", CardID = "1054" });
            eventItems.Add(new EventVM() { CompanyName = "PGS Software", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wyjscie", WorkerName = "Paweł Solny", CardID = "1045" });
            eventItems.Add(new EventVM() { CompanyName = "Assecco Poland", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wejscie", WorkerName = "Aleksandra Nowicka", CardID = "1099" });
            eventItems.Add(new EventVM() { CompanyName = "OPGK Rzeszów", EventDate = DateTime.Now.ToString("dd MM yyyy h:mm:ss"), EventType = "Wejscie", WorkerName = "Marta Krupa", CardID = "1109" });

            EventsList.ItemsSource = eventItems;


            var data = new DBModel()
            {
                Companies = new List<DBCompany>()
                {
                    new DBCompany
                    {
                    CompanyId = 1,
                    CompanyName = "OPGK Rzeszów",
                    Workers = new List<DBWorker>()
                    {
                        new DBWorker
                        {
                            CardID = 1120,
                            FirstName = "Tomek",
                            SurName = "Krupa"
                        },
                        new DBWorker
                        {
                            CardID = 1410,
                            FirstName = "Paweł",
                            SurName = "Lsa"
                        },
                        new DBWorker
                        {
                            CardID = 1112,
                            FirstName = "Kamil",
                            SurName = "Osa"
                        },
                    }
                },
                new DBCompany
                {
                    CompanyId = 1,
                    CompanyName = "Assecoo Polandw",
                    Workers = new List<DBWorker>()
                    {
                        new DBWorker
                        {
                            CardID = 11620,
                            FirstName = "Todasdamek",
                            SurName = "Krudaspa"
                        },
                        new DBWorker
                        {
                            CardID = 1660,
                            FirstName = "Pdasaweł",
                            SurName = "dsaLsa"
                        },
                        new DBWorker
                        {
                            CardID = 16612,
                            FirstName = "aaa",
                            SurName = "Osdasa"
                        },
                    }
                },
                }
            };

            var db = Service.GetDBModel();
            db.Companies.Add(new DBCompany()
            {
                CompanyId = 3,
                CompanyName = "Nowa firma",
                Workers = new List<DBWorker>()
                {
                    new DBWorker
                    {
                        CardID = 43,
                        FirstName = "toemm",
                        SurName = "fda"
                    }
                }
            });
            Service.SaveDatabase(db);
        }
    }
}
