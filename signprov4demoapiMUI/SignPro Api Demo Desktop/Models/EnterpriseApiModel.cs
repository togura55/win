using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignPro_Api_Demo_Desktop.Models
{
    public class APIModel
    {
        public File file { get; set; }
        public Configuration configuration { get; set; }
        public Signature[] signatures { get; set; }
        public Initials[] initials { get; set; }
    }

    public class File
    {
        public Input input { get; set; }
        public Output output { get; set; }
        public Authentication authentication { get; set; }
    }

    public class Input
    {
        public string filesystem { get; set; }
        public string http_get { get; set; }
    }

    public class Output
    {
        public string filesystem { get; set; }
        public string http_post { get; set; }
    }

    public class Authentication
    {
        public string pdf_user_password { get; set; }
        public string http_user { get; set; }
        public string http_password { get; set; }
    }

    public class Configuration
    {
        public string api_key { get; set; }
        public bool show_annotate { get; set; }
        public bool show_manual_signature { get; set; }
        public string error_handler_url { get; set; }
        public int required_signatures { get; set; }
        public bool process_text_tags { get; set; }
    }

    public class Signature
    {
        public string name { get; set; }
        public string signer { get; set; }
        public string reason { get; set; }
        public string type { get; set; }
        public bool biometric { get; set; }
        public bool required { get; set; }
        public Certificate certificate { get; set; }
        public Location location { get; set; }
    }

    public class Initials
    {
        public string name { get; set; }
        public string signer { get; set; }
        public string reason { get; set; }
        public bool required { get; set; }
        public Certificate certificate { get; set; }
        public Location location { get; set; }
    }

    public class Certificate
    {
        public string location { get; set; }
        public string path { get; set; }
        public string serial { get; set; }
        public string password { get; set; }
    }

    public class Location
    {
        public int Page { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int W { get; set; }
        public int H { get; set; }
    }
}