using Newtonsoft.Json;
using SignPro_Api_Demo_Desktop.Controller;
using SignPro_Api_Demo_Desktop.Models;
using SignPro_Api_Demo_Desktop.Models.Api;
using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;


namespace SignPro_Api_Demo_Desktop
{
    /// <summary>
    /// Interaction logic for NewCustomer.xaml
    /// </summary>
    public partial class NewCustomer : Page
    {

        private string originalFile = "Hospital_Acrofields.pdf";
        private FileModel _file = null;
        private ConfigurationModel _configuration = null;
        private ApiParameterModel api;
        private Registration registration = new Registration();
        PdfDebenuController pdfController = new PdfDebenuController();
        public NewCustomer(ApiParameterModel api)
        {
            this.api = api;
            InitializeComponent();
            btnNewCustomer.Visibility = Visibility.Visible;
            chkAnnotation.IsChecked = api.Configuration.ShowAnnotate;
            chkSignature.IsChecked = api.Configuration.ShowManualSignature;
            chkTextTags.IsChecked = api.Configuration.ProcessTextTags;
            LblDate.Content = DateTime.Now.ToString("dd/MM/yyyy");  // This parameter value is used for API, do not change the format
//            LblDate.Content = DateTime.Now.ToString(@"d", CultureInfo.CurrentCulture);
            LblCity.Content = ConfigurationManager.AppSettings["LabelCity"];

            Label_NewCustomerRegistration.Content = Properties.Resources.IDC_LABEL_NEWCUSTOMERREGISTRATION;
            Label_FirstName.Content = Properties.Resources.IDC_LABEL_FIRSTNAME;
            Label_SurName.Content = Properties.Resources.IDC_LABEL_SURNAME;
            Label_Street.Content = Properties.Resources.IDC_LABEL_STREET;
            Label_City.Content = Properties.Resources.IDC_LABEL_CITY;
            Label_Country.Content = Properties.Resources.IDC_LABEL_COUNTRY;
            Label_Phone.Content = Properties.Resources.IDC_LABEL_PHONE;
            chkAnnotation.Content = Properties.Resources.IDC_CB_ANNOTATION;
            chkSignature.Content = Properties.Resources.IDC_CB_SIGNATURE;
            chkTextTags.Content = Properties.Resources.IDC_CB_TEXTTAG;
            chkUploadResult.Content = Properties.Resources.IDC_CB_UPDATERESULT;
            btnNewCustomer.Content = Properties.Resources.IDC_BTN_NEWCUSTOMER;
            LblCity.Content = Properties.Resources.IDC_LABEL_LBLCITY;
            btnShowRegistration.Content = Properties.Resources.IDC_BTN_SHOWREGISTRATION;
        }

        private void BtnShowRegistration_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Customer_Registry(registration));
        }
        
        private void BtnNewCustomer_Click(object sender, RoutedEventArgs e)
        {
            btnShowRegistration.Visibility = Visibility.Visible;
            btnNewCustomer.Visibility = Visibility.Hidden;

            registration = GetRegistration();
            string timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmssfff", CultureInfo.InvariantCulture);
            string fileName = txtSurName.Text + "_" + txtFirstName.Text  + ".pdf";
            if (UpdatePdf(registration, fileName))
            {
                //string apiData = File.ReadAllText(@".\config\Default.json");
                //ApiParameterModel api = JsonConvert.DeserializeObject<ApiParameterModel>(apiData);

                _file = new FileModel();
                InputModel _input = new InputModel();
                _input.FileSystem = Directory.GetCurrentDirectory() + "\\PdfTransit\\" + fileName;

                OutputModel api_output = new OutputModel();
                api_output.FileSystem = Directory.GetCurrentDirectory() + "\\SignedDocs\\" + fileName;

                AuthenticationModel _authentication = new AuthenticationModel();
                _authentication.HttpUser = null;
                _authentication.HttpPassword = null;
                _authentication.PdfUserPassword = null;

                _file.Input = _input;
                _file.Output = api_output;
                _file.Authentication = _authentication;

                SetConfiguration(api);

                //SetConfiguration();
                //api.Configuration = _configuration;
                api.File = _file;
                // Setup the initial data to send to sign pro PDF API from data in the "form"
                //var api = new ApiParameterModel
                //{
                //    Configuration = _configuration,
                //    File = _file,
                //};

                // Use this to ignore a few errors that might occur due to the simplicity of this demo?
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore
                };
                api.Signatures[0].Signer = registration.FirstName + " " + registration.Surname;
                api.DateFields[0].Value = LblDate.Content.ToString();
                var _api_settings = JsonConvert.SerializeObject(api, Newtonsoft.Json.Formatting.Indented);

                // encode the JSON and send it off to sign pro PDF API
                var encoded_json = Convert.ToBase64String(Encoding.UTF8.GetBytes(_api_settings));

                string cmd_Api = "signpro:" + encoded_json;

                string startPrg;

                if (Environment.Is64BitOperatingSystem)
                {
                    startPrg = "C:\\Program Files (x86)\\Wacom sign pro PDF\\Sign Pro PDF.exe";
                    //Process.Start("C:\\Program Files (x86)\\Wacom sign pro PDF\\Sign Pro PDF.exe", " -api \"" + cmd_Api + "\"");
                }
                else
                {
                    startPrg = "C:\\Program Files\\Wacom sign pro PDF\\Sign Pro PDF.exe";
                    //Process.Start("C:\\Program Files\\Wacom sign pro PDF\\Sign Pro PDF.exe", " -api \"" + cmd_Api + "\"");
                }

                string strIdMonitor = ConfigurationManager.AppSettings["idMonitor"];
                int idMonitor = Convert.ToInt32(strIdMonitor);

                ExeForm exeForm = new ExeForm();
                exeForm.CallExeOnMonitor(startPrg, " -api \"" + cmd_Api + "\"", idMonitor);
            }
            else
            {
//                MessageBox.Show("Pdf Not Update, please check the data inserted!");
                MessageBox.Show(Properties.Resources.IDC_MSG_PDFNOTUPDATE);
            }
        }


        private void SetConfiguration(ApiParameterModel api)
        {
            if (chkAnnotation.IsChecked ?? false)
            {
                api.Configuration.ShowAnnotate = true;
            }
            else
            {
                api.Configuration.ShowAnnotate = false;
            }
            if (chkSignature.IsChecked ?? false)
            {
                api.Configuration.ShowManualSignature = true;
            }
            else
            {
                api.Configuration.ShowManualSignature = false;
            }
            if (chkTextTags.IsChecked ?? false)
            {
                api.Configuration.ProcessTextTags = true;
            }
            else
            {
                api.Configuration.ProcessTextTags = false;
            }

        }

        private Registration GetRegistration()
        {
            return new Registration
            {
                FirstName = txtFirstName.Text,
                Surname = txtSurName.Text,
                Street = txtStreet.Text,
                City = txtCity.Text,
                Country = txtCountry.Text,
                Phone = txtPhone.Text,
            };
        }

        private bool UpdatePdf(Registration registration,string PdfOut)
        {
            // for MUI 
            string[] localeFolders = Directory.GetDirectories(Directory.GetCurrentDirectory() + "\\Pdf", "*", SearchOption.TopDirectoryOnly);
            string threadCulture = Thread.CurrentThread.CurrentCulture.ToString();
            string localeFolderName = "en-US"; // set default locale
            int count = 0;
            foreach (string locale in localeFolders)
            {
                if (locale.Substring(locale.LastIndexOf("\\") + 1) == threadCulture)
                {
                    localeFolderName = threadCulture;
                    break;
                }
                count++;
            }
            string fileIn = Directory.GetCurrentDirectory() + string.Format("\\Pdf\\{0}\\", localeFolderName) +originalFile;
            string fileOut = Directory.GetCurrentDirectory() + "\\PdfTransit\\" + PdfOut;
            //string lang = ConfigurationManager.AppSettings["InputData"];
            //if ("Japanese".Equals(ConfigurationManager.AppSettings["InputData"]))
            if (IsHandleUnicode())
            {
                return pdfController.UpdateCustomizationJapCin(fileIn, fileOut, registration);
            }
            else
            {
                return pdfController.UpdateCustomization(fileIn, fileOut, registration);
            }
        }

        private bool IsHandleUnicode()
        {
            bool result = false;
            string[] unicodeLocales = { "ja-JP", "ko-KR", "zh-TW", "zh-CN" }; // add if any other locales

            string threadCulture = Thread.CurrentThread.CurrentCulture.ToString();

            int count = 0;
            foreach (string locale in unicodeLocales)
            {
                if (locale == threadCulture)
                    break;

                count++;
            }
            if (count < unicodeLocales.Length)
                result = true;

            return result;
        }
    }

}
