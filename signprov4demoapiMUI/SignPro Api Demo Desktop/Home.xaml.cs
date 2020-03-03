using Newtonsoft.Json;
using SignPro_Api_Demo_Desktop.Models.Api;
using System;
using System.Collections.Generic;
using System.IO;
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

namespace SignPro_Api_Demo_Desktop
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        private object MainWindows;
        ApiParameterModel api;
        public Home()
        {
            InitializeComponent();
            string apiData = File.ReadAllText(@".\config\Default.json");
            api = JsonConvert.DeserializeObject<ApiParameterModel>(apiData);

//            Properties.Resources.IDC_MSG_PDFNOTUPDATE

            Label_Home.Content = Properties.Resources.IDC_LABEL_HOME;
            Label_Dashboard.Content = Properties.Resources.IDC_LABEL_DASHBOARD;
            TextBlock_NewCustomerUpper.Text = Properties.Resources.IDC_LABEL_NEWCUSTOMERUPPER;
            TextBlock_NewCustomerLower.Text = Properties.Resources.IDC_LABEL_NEWCUSTOMERLOWER;
            TextBlock_NewsFrom.Text = Properties.Resources.IDC_TEXTBLOCK_NEWFORM;
            TextBlock_YourCustomer.Text = Properties.Resources.IDC_TEXTBLOCK_YOURCUSTOMER;
            TextBlock_RegisterNew.Text = Properties.Resources.IDC_TEXTBLOCK_REGISTERNEW;
            btnHome.Content = Properties.Resources.IDC_BTN_HOME;
            btnDashboard.Content = Properties.Resources.IDC_BTN_DASHBOARD;
            btnCustomer.Content = Properties.Resources.IDC_BTN_CUSTOMER;
        }
        private void BtnHome_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDashboard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCustomer_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new NewCustomer(api));

        }
    }
}
