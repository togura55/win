using SignPro_Api_Demo_Desktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for NewCustomer.xaml
    /// </summary>
    public partial class Customer_Registry : Page
    {


        public Customer_Registry(Registration registration)
        {
            InitializeComponent();
            txtFirstName1.Text = registration.FirstName;
            txtSurname1.Text = registration.Surname;
            txtStreet1.Text = registration.Street;
            txtCity1.Text = registration.City;
            txtCountry1.Text = registration.Country;
            txtPhone1.Text = registration.Phone;
            ListPdfSigned();

            Label_CustomerRegistration.Content = Properties.Resources.IDC_LABEL_CUSTOMERREGISTRATION;
            Label_FirstName.Content = Properties.Resources.IDC_LABEL_FIRSTNAME;
            Label_Surname.Content = Properties.Resources.IDC_LABEL_SURNAME;
            Label_Street.Content = Properties.Resources.IDC_LABEL_STREET;
            Label_City.Content = Properties.Resources.IDC_LABEL_CITY;
            Label_Country.Content = Properties.Resources.IDC_LABEL_COUNTRY;
            Label_Phone.Content = Properties.Resources.IDC_LABEL_PHONE;
            DGTC_Files.Header = Properties.Resources.IDC_TEXT_FileName;

        }

        private void DeletePdf(object sender, RoutedEventArgs e)
        {
            GridPdf gridPdf = (sender as Button).DataContext as GridPdf;
            
            if (gridPdf != null)
            {
                string fileName = String.Concat(Directory.GetCurrentDirectory(), "\\SignedDocs\\", gridPdf.FileName);
                File.Delete(fileName);
            }
            ListPdfSigned();
        }

        private void ShowPdf(object sender, RoutedEventArgs e)
        {
            GridPdf gridPdf = (sender as Button).DataContext as GridPdf;
            
            if (gridPdf != null)
            {
                string fileName = String.Concat(Directory.GetCurrentDirectory(), "\\SignedDocs\\", gridPdf.FileName);
                System.Diagnostics.Process.Start(fileName);
            }
        }

        private void ListPdfSigned()
        {
            ObservableCollection<GridPdf> models = new ObservableCollection<GridPdf>();

            try
            {
                string dir = Directory.GetCurrentDirectory() + "\\SignedDocs\\";
                foreach (string f in Directory.GetFiles(dir))
                {
                    models.Add(new GridPdf() { FileName = System.IO.Path.GetFileName(f) });
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            DataGridPdf.ItemsSource = models;
        }



    }
}
