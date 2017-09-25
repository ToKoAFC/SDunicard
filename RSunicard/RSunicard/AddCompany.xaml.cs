using RSunicard.Logic;
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
    /// Interaction logic for AddCompany.xaml
    /// </summary>
    public partial class AddCompany : Window
    {
        public AddCompany()
        {
            InitializeComponent();
        }
        private void AddNewCompanyClick(object sender, RoutedEventArgs e)
        {
            Service.AddNewCompany(newCompanyName.Text);
            this.Close();
        }
    }
}
