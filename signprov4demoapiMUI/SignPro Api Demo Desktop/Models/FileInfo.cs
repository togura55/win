using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace SignPro_Api_Demo_Desktop.Models
{
    [XmlRoot("TestFiles")]
    public class TestFiles
    {
        [XmlElement("PDFFile")]
        public List<PDFFile> PDFFiles { get; set; }
    }
    public class PDFFile
    {
        [XmlElement("FileName")]
        public string FileName { get; set; }
        [XmlElement("DisplayName")]
        public string DisplayName { get; set; }
    }
}