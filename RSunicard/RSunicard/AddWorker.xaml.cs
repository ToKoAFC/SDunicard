using RSunicard.Logic;
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
using System.Windows.Shapes;

namespace RSunicard
{
    /// <summary>
    /// Interaction logic for AddWorker.xaml
    /// </summary>
    public partial class AddWorker : Window
    {
        public AddWorker()
        {
            InitializeComponent();
            companySelectList.ItemsSource = Service.GetCompanySelectList();
        }

        private void AddNewWorkerlick(object sender, RoutedEventArgs e)
        {
            var company = companySelectList.SelectedItem as CompanyVM;
            Service.AddNewWorker(company.CompanyName, newWorkerName.Text);
            this.Close();
        }
    }
}
