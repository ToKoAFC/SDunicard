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
    /// Interaction logic for CompanyDetails.xaml
    /// </summary>
    public partial class CompanyDetails : Window
    {
        public CompanyDetails(string companyName)
        {
            InitializeComponent();
            var company = Service.GetCompanyDetails(companyName);
            WorkersList.ItemsSource = company.Workers;
            companyNameLabel.Content = company.CompanyName;
        }      
    }
}
