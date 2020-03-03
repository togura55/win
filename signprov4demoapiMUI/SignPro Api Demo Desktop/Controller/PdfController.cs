using iTextSharp.text.pdf;
using SignPro_Api_Demo_Desktop.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignPro_Api_Demo_Desktop.Controller
{
    public class PdfController
    {
        public static bool UpdateCustomization(string fileName,string fileOut,Registration registration)
        {
            try
            {

                // read the template file
                PdfReader reader = new PdfReader(fileName);

                // instantiate PDFStamper object
                // The Output file will be created from template file and edited by the PDFStamper
                PdfStamper stamper = new PdfStamper(reader, new FileStream(
                            fileOut, FileMode.Create));


                // Object to deal with the Output file's textfields
                AcroFields fields = stamper.AcroFields;

                // set form fields("Field Name in PDF", "String to be placed in this PDF text field");
                //  fields.SetField("Id", (string) dtRow["Id"]);

                fields.SetField("Name", registration.FirstName);
                fields.SetField("Surname", registration.Surname);
                fields.SetField("Street", registration.Street);
                fields.SetField("City", registration.City);
                fields.SetField("Country", registration.Country);
                fields.SetField("Phone", registration.Phone);
                fields.SetField("Email", registration.Email);

                // form flattening rids the form of editable text fields so
                // the final output can't be edited
                stamper.FormFlattening = true;
                // closing the stamper
                stamper.Close();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
