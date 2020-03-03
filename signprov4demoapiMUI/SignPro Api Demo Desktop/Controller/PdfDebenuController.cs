using DebenuPDFLibraryDLL1614;
using log4net;
using SignPro_Api_Demo_Desktop.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignPro_Api_Demo_Desktop.Controller
{
    public class PdfDebenuController
    {
        private const string DebenuLic = "jo88a6bd9xc3xw7199bf61b9y";
        private const string DebenuPath = @"Lib\DebenuPDFLibraryDLL1614.dll";
        private PDFLibrary DPL = new PDFLibrary(DebenuPath);
        private static ILog s_Log = LogManager.GetLogger("PdfDebenuController");
        private bool Loaded = false;

        private const int FIELD_TYPE_NONE = 0;
        private const int FIELD_TYPE_TEXT = 1;
        private const int FIELD_TYPE_PUSHBUTTON = 2;
        private const int FIELD_TYPE_CHECKBOX = 3;
        private const int FIELD_TYPE_RADIOBUTTON = 4;
        private const int FIELD_TYPE_LIST = 5;
        private const int FIELD_TYPE_SIGNATURE = 6;
        private const int FIELD_TYPE_PARENT = 7;

        public bool UpdateCustomization(string fileName, string fileOut, Registration registration)
        {

            DPL.LoadFromFile(fileName, "");


            DPL.SetFormFieldValueByTitle("Name", registration.FirstName);
            DPL.SetFormFieldValueByTitle("Surname", registration.Surname);
            DPL.SetFormFieldValueByTitle("Street", registration.Street);
            DPL.SetFormFieldValueByTitle("City", registration.City);
            DPL.SetFormFieldValueByTitle("Country", registration.Country);
            DPL.SetFormFieldValueByTitle("Phone", registration.Phone);
            DPL.SetFormFieldValueByTitle("Email", registration.Email);

            if (!Flatten()) return false;

            return SavePdf(fileOut);

        }

        public bool UpdateCustomizationJapCin(string fileName, string fileOut, Registration registration)
        {
            PdfDebenuController.s_Log.Debug("Update value :" + fileName);
            DPL.LoadFromFile(fileName, "");
            int FieldCountAcroForms = DPL.FormFieldCount();
            int TotalPages = DPL.PageCount();
            // Loop through each page
            for (int p = 1; p <= TotalPages; p++)
            {
                // Select page number
                DPL.SelectPage(p);
                // Loop through each form field on the selected page
                for (int i = 1; i <= FieldCountAcroForms; i++)
                {

                    // Determine form field type
                    if (DPL.GetFormFieldType(i) == FIELD_TYPE_TEXT)
                    {
                        switch (DPL.GetFormFieldTitle(i))
                        {
                            case "Name":
                                DPL.SetFormFieldValue(i, registration.FirstName);
                                break;
                            case "Surname":
                                DPL.SetFormFieldValue(i, registration.Surname);
                                break;
                            case "Street":
                                DPL.SetFormFieldValue(i, registration.Street);
                                break;
                            case "City":
                                DPL.SetFormFieldValue(i, registration.City);
                                break;
                            case "Country":
                                DPL.SetFormFieldValue(i, registration.Country);
                                break;
                            case "Phone":
                                DPL.SetFormFieldValue(i, registration.Phone);
                                break;
                            case "Email":
                                DPL.SetFormFieldValue(i, registration.Email);
                                break;
                            default:
                                Console.WriteLine("");
                                break;
                        }
                        if (!string.IsNullOrEmpty(DPL.GetFormFieldValue(i)))
                        {
                            // Text Fields with preset Values may need us to embed a
                            // compatible Font (Cyrllic, JP characters etc.)
                            // Embed the subset Font we need for our Value in the Document
                            var fontID = DPL.AddTrueTypeSubsettedFont("Code2000", DPL.GetFormFieldValue(i), 0);
                            // Add the subsetted Font as a FormFieldFont
                            var formFontID = this.DPL.AddFormFont(fontID);
                            this.DPL.SetFormFieldFont(i, formFontID);
                            this.DPL.SetFormFieldValueEx(i, DPL.GetFormFieldValue(i), 1); //Unicode
                            this.DPL.SetNeedAppearances(1);
                            this.DPL.UpdateAppearanceStream(i);
                        }
                    }
                }
            }

            if (!Flatten()) return false;

            return SavePdf(fileOut);

        }

        public PdfDebenuController()
        {

            PdfDebenuController.s_Log.Debug("Start PdfController constructor");
            try
            {
                if (DPL.LibraryLoaded())
                {
                    if (DPL.UnlockKey(DebenuLic) == 0 || DPL.Unlocked() == 0)
                    {
                        PdfDebenuController.s_Log.Error("License unlock failed for Debenu PDF library. Licence key may need updating.");
                    }
                    else
                    {
                        Loaded = true;
                        PdfDebenuController.s_Log.Debug("License loaded correctly");

                    }
                }
                else
                {
                    PdfDebenuController.s_Log.Error("Could not locate the Debenu PDF Library DLL, please check the path.");
                }
            }
            catch (Exception ex2)
            {
                PdfDebenuController.s_Log.Error("Failed to initialise sign pro MainModel.", ex2);
            }
            PdfDebenuController.s_Log.Debug("End PdfController constructor");
        }


        private bool Flatten()
        {
            bool status = true;
            PdfDebenuController.s_Log.Debug("Flattened");
            int TotFields = DPL.FormFieldCount();
            PdfDebenuController.s_Log.Debug("Tot fields :" + TotFields);
            for (int i = TotFields; i > 0; i--)
            {
                string name = DPL.GetFormFieldTitle(i); // formfield name
                string type = GetType(DPL.GetFormFieldType(i));

                if (!GetStatus(DPL.FlattenFormField(i)))
                {
                    status = false;
                    StringBuilder sb = new StringBuilder("Field ");
                    sb.Append(name);
                    sb.Append(" - ");
                    sb.Append(type);
                    sb.Append(" - Not Flattened");
                    PdfDebenuController.s_Log.Error(sb.ToString());
                }
                else
                {
                    StringBuilder sb = new StringBuilder("Field ");
                    sb.Append(name);
                    sb.Append(" - ");
                    sb.Append(type);
                    sb.Append(" - Flattened");
                    PdfDebenuController.s_Log.Debug(sb.ToString());
                }
            }
            PdfDebenuController.s_Log.Debug("Flattened End");
            return status;
        }

        private bool GetStatus(int id)
        {
            if (id == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GetType(int idType)
        {
            string strType = null;
            switch (idType)
            {
                case 0:
                    strType = "None";
                    break;
                case 1:
                    strType = "Text";
                    break;
                case 2:
                    strType = "PushButton";
                    break;
                case 3:
                    strType = "CheckBox";
                    break;
                case 4:
                    strType = "RadioButton";
                    break;
                case 5:
                    strType = "List";
                    break;
                case 6:
                    strType = "Signature";
                    break;
                case 7:
                    strType = "Parent";
                    break;
                default:
                    strType = "None";
                    break;
            }
            return strType;
        }

        private bool SavePdf(string pdfFileName)
        {
            PdfDebenuController.s_Log.Debug("SavePdf :" + pdfFileName);
            int errStatus = DPL.SaveToFile(pdfFileName);
            if (errStatus == 1)
            {
                PdfDebenuController.s_Log.Info(pdfFileName + "Saved");
                return true;
            }
            else
            {
                PdfDebenuController.s_Log.Error("SavePdf Error:" + errStatus);
                return false;
            }
        }

    }
}
