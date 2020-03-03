using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignPro_Api_Demo_Desktop.Models
{
    public class User
    {
        public string FileName { get; set; }
        public bool PDFParseTextTags { get; set; }
        public bool PDFAllowManualSignatures { get; set; }
        public bool PDFAllowAnnotation { get; set; }
        public string CustomerID { get; set; }
        public string CustomerSurname { get; set; }
        public string CustomerFirstName { get; set; }

    }
}