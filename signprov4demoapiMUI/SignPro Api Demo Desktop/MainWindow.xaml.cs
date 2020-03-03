using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SignPro_Api_Demo_Desktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Label_Consultant.Content = Properties.Resources.IDC_LABEL_CONSULTANT;
            Label_CompanyName.Content = Properties.Resources.IDC_LABEL_COMPANYNAME;

            this.Title = string.Format("{0} {1}",
                Application.ResourceAssembly.GetName().Name,
                Application.ResourceAssembly.GetName().Version.ToString());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            FrmMainContainer.NavigationService.Navigate(new Home());
        }
        private void Logo_MouseDown(object sender, MouseEventArgs e)
        {
            FrmMainContainer.NavigationService.Navigate(new Home());
        }

    }
}
