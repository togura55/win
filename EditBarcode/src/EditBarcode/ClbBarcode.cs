using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using System.Xml;

namespace EditBarcode
{
    class ClbBarcode
    {

        public string clbConfigRootPath;
        public string clbVersion;
        string clbConfigFile;

        public class SettingNode
        {
            public string Name { get; set; }        // name
            public string SerializeAs { get; set; } // SerializeAs
            public string Value { get; set; }       // value

            public SettingNode()
            {
                Name = ""; SerializeAs = ""; Value = "";
            }
        }
        public List<SettingNode> SettingNodes { get; set; }

        public string[] BarcodeNames { get; }
        public string[] BarcodeValues { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ClbBarcode()
        {
            SettingNodes = new List<SettingNode>();

            BarcodeNames = new string[3] { "Region", "Department", "Team" };
            BarcodeValues = new string[3] { "", "", "" };

            clbConfigRootPath = @"Wacom\\WacomGSS.CLBCreate.exe_Url_rkoruptfxinvzvkwdaklbscqah1q0dd0";
            clbVersion = string.Empty;
            clbConfigFile = @"user.config";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns>true/false</returns>
        private bool SetBarcodeValues(string f)
        {
            if (f.Length != 6)
                return false;

            BarcodeValues[0] = f.Substring(0, 2);
            BarcodeValues[1] = f.Substring(2, 2);
            BarcodeValues[2] = f.Substring(4, 2);

            return true;
        }

        /// <summary>
        /// Set the split digit to the BarcodeValue list
        /// </summary>
        /// <param name="f">strings of 6-digit number</param>
        /// <param name="value">Value of increment/decrement upper 6 digit of CLB barcode</param>
        /// <returns>true/false</returns>
        public bool SetUpperBarcode(ref string f, string value = "0")
        {
            try
            {
                if (f != string.Empty && value == "0")
                {
                    // validate the barcode value
                    // 6-digit?
                    if (f.Length != 6)
                    {
                        // error
                        f = String.Format("Error: Invalid parameter {0}", f);
                        return false;
                    }
                    // number?
                    if (!long.TryParse(f, out long l))
                    {
                        // error
                        f = String.Format("Error: Invalid parameter {0}", f);
                        return false;
                    }

                    // split barcode and set
                    SetBarcodeValues(f);
                }

                // Do inc/dec barcode digit
                if (value != "0" && int.TryParse(value, out int integ))
                {
                    int.TryParse(BarcodeValues[0] + BarcodeValues[1] + BarcodeValues[2], out int i);
                    i += integ;
                    SetBarcodeValues(i.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns>true/false</returns>
        private bool GetUserConfigPath(ref string f)
        {
            char separator = '.';
            Version maxVer = new Version(@"0.0.0.0");

            string dir = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\" + clbConfigRootPath;
            if (!Directory.Exists(dir))
            {
                f = String.Format("Unable to find a directory: {0}", dir);
                return false;
            }

            try
            {
                if (clbVersion != string.Empty)  // retrieve folder name of largiest number 
                {
                    foreach (string s in Directory.EnumerateDirectories(dir))
                    {
                        string v = s.Substring(dir.Length + 1);
                        if ((v.Length - v.Replace(separator.ToString(), "").Length)  // Separator Count
                            == 3)   // assuming the format of e.g. 1.0.2.10
                        {
                            Version v1 = new Version(v);
                            switch (v1.CompareTo(maxVer))
                            {
                                case 0:
                                    break;
                                case 1:
                                    maxVer = v1;
                                    break;
                                case -1:
                                    break;
                            }
                        }
                    }
                    clbVersion = maxVer.ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            f = dir + "\\" + clbVersion + "\\" + clbConfigFile;  // make a full path
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns>true/false</returns>
        public bool SetUserConfigXml(ref string f)
        {
            string filename = string.Empty;
            if (!GetUserConfigPath(ref filename))
            {
                f = filename;  // return an error message
                return false;
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                doc.Load(filename);
                XmlNodeList nodeList = doc.SelectNodes(@"//WacomGSS.CLBCreate.Properties.Settings");
                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i].HasChildNodes == true)
                    {
                        for (int j = 0; j < nodeList[i].ChildNodes.Count; j++) // loop of "setting"
                        {
                            int index = Array.IndexOf(BarcodeNames, nodeList[i].ChildNodes[j].Attributes["name"].Value);
                            if ((index >= 0) && (nodeList[i].ChildNodes[j].HasChildNodes == true))
                            {
                                for (int k = 0; k < nodeList[i].ChildNodes[j].ChildNodes.Count; k++)
                                {
                                    nodeList[i].ChildNodes[j].ChildNodes[k].InnerText = BarcodeValues[index];
                                }
                            }
                        }
                    }
                }

                File.Copy(filename, filename + ".bak", true);  // file backup
                doc.Save(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="f"></param>
        /// <returns>true/false</returns>
        public bool GetUserConfigXml(ref string f)
        {
            string filename = string.Empty;
            if (!GetUserConfigPath(ref filename))
            {
                f = filename; // return an error message
                return false;
            }

            XmlDocument doc = new XmlDocument();
            try
            {
                // Parsing nodes in user config Xml
                doc.Load(filename);
                XmlNodeList nodeList = doc.SelectNodes(@"//WacomGSS.CLBCreate.Properties.Settings");
                for (int i = 0; i < nodeList.Count; i++)
                {
                    if (nodeList[i].HasChildNodes == true)
                    {
                        for (int j = 0; j < nodeList[i].ChildNodes.Count; j++) // loop of "setting"
                        {
                            SettingNode item = new SettingNode() { Name = "", SerializeAs = "", Value = "" };
                            item.Name = nodeList[i].ChildNodes[j].Attributes["name"].Value;

                            if (nodeList[i].ChildNodes[j].HasChildNodes == true)
                            {
                                for (int k = 0; k < nodeList[i].ChildNodes[j].ChildNodes.Count; k++)
                                {
                                    item.Value = nodeList[i].ChildNodes[j].ChildNodes[k].InnerText;
                                    SettingNodes.Add(item);
                                }
                            }

                        }
                    }
                }

                // Store the barcode digit to the local
                for (int i = 0; i < BarcodeValues.Length; i++)
                {
                    string t1 = string.Empty;
                    for (int j = 0; j < SettingNodes.Count; j++)
                    {
                        if (BarcodeNames[i] == SettingNodes[j].Name)
                        {
                            BarcodeValues[i] = SettingNodes[j].Value;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                f = ex.Message;
                return false;
            }

            return true;
        }
    }
}
