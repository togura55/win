using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Collections;
using System.Xml.Serialization;
using System.Security.Cryptography;

namespace ScoreScraping
{


    public class Hole
    {
        public string url;
        public string row;
        public List<string> yardList;

        public Hole()
        {
            url = string.Empty;
            List<string> yardList = new List<string>();
        }
    }

    public class TargetWebsite
    {
        //
        // https://qiita.com/kz-rv04/items/62a56bd4cd149e36ca70
        //
        private const string AES_IV = @"pf69DL6GrWFyZcM0";
        private const string AES_Key = @"9Fix4L4HB4PKeKW0";

        public string name;
        public string loginUrl;
        public string id;
        [XmlElement("pwd")]public string Pwd   // encripted
        {
            get { return pPwd; }
            set
            {
                pPwd = value;
                pPassword = Decrypt(value, AES_IV, AES_Key);
            }
        }
        [XmlIgnore] public string Password     // plain text
        {
            get { return pPassword; }
            set
            {
                pPassword = value;
                pPwd = Encrypt(value, AES_IV, AES_Key);
            }
        }

        [XmlIgnore] private string pPassword;
        [XmlIgnore] private string pPwd;

        /// <summary>
        /// 対称鍵暗号を使って文字列を暗号化する
        /// </summary>
        /// <param name="text">暗号化する文字列</param>
        /// <param name="iv">対称アルゴリズムの初期ベクター</param>
        /// <param name="key">対称アルゴリズムの共有鍵</param>
        /// <returns>暗号化された文字列</returns>
        private static string Encrypt(string text, string iv, string key)
        {
            using (RijndaelManaged rijndael = new RijndaelManaged())
            {
                rijndael.BlockSize = 128;
                rijndael.KeySize = 128;
                rijndael.Mode = CipherMode.CBC;
                rijndael.Padding = PaddingMode.PKCS7;

                rijndael.IV = Encoding.UTF8.GetBytes(iv);
                rijndael.Key = Encoding.UTF8.GetBytes(key);

                ICryptoTransform encryptor = rijndael.CreateEncryptor(rijndael.Key, rijndael.IV);

                byte[] encrypted;
                using (MemoryStream mStream = new MemoryStream())
                {
                    using (CryptoStream ctStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(ctStream))
                        {
                            sw.Write(text);
                        }
                        encrypted = mStream.ToArray();
                    }
                }
                return (System.Convert.ToBase64String(encrypted));
            }
        }

        /// <summary>
        /// 対称鍵暗号を使って暗号文を復号する
        /// </summary>
        /// <param name="cipher">暗号化された文字列</param>
        /// <param name="iv">対称アルゴリズムの初期ベクター</param>
        /// <param name="key">対称アルゴリズムの共有鍵</param>
        /// <returns>復号された文字列</returns>
        private static string Decrypt(string cipher, string iv, string key)
        {
            string plain = string.Empty;

            try
            {
                using (RijndaelManaged rijndael = new RijndaelManaged())
                {
                    rijndael.BlockSize = 128;
                    rijndael.KeySize = 128;
                    rijndael.Mode = CipherMode.CBC;
                    rijndael.Padding = PaddingMode.PKCS7;

                    rijndael.IV = Encoding.UTF8.GetBytes(iv);
                    rijndael.Key = Encoding.UTF8.GetBytes(key);

                    ICryptoTransform decryptor = rijndael.CreateDecryptor(rijndael.Key, rijndael.IV);


                    using (MemoryStream mStream = new MemoryStream(System.Convert.FromBase64String(cipher)))
                    {
                        using (CryptoStream ctStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader sr = new StreamReader(ctStream))
                            {
                                plain = sr.ReadLine();
                            }
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Decript:{0}", ex.Message));
            }

            return plain;

        }
    }

    public class ScoreScraping
    {
        [XmlIgnore] public static CookieContainer cookieContainer;
        [XmlIgnore] public List<Hole> holeList;
        [XmlIgnore] public int siteIndex;
        public string website;
        public List<TargetWebsite> TargetWebsites { get; set; } // required type of accessor

        public ScoreScraping()
        {
            cookieContainer = new CookieContainer();
            holeList = new List<Hole>();
            siteIndex = 0;
            website = string.Empty;
            TargetWebsites = new List<TargetWebsite>();
        }

        static readonly Encoding encoder = Encoding.GetEncoding("utf-8");

        static string HttpGet(string url)
        {
            string result = string.Empty;

            try
            {
                // リクエストの作成
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = cookieContainer;
                req.Timeout = 2000;
                req.ContentType = "text/html";

                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                if (res.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    ////受信したCookieのコレクションを取得する
                    //CookieCollection cookies = req.CookieContainer.GetCookies(req.RequestUri);
                    ////Cookie名と値を列挙する
                    //foreach (Cookie cook in cookies)
                    //    Console.WriteLine("{0}={1}", cook.Name, cook.Value);
                    ////取得したCookieを保存しておく
                    //cookieContainer.Add(cookies);

                    // レスポンスの読み取り
                    Stream resStream = res.GetResponseStream();
                    StreamReader sr = new StreamReader(resStream, encoder);
                    result = sr.ReadToEnd();
                    sr.Close();
                    resStream.Close();
                }
            }

            catch (WebException ex)
            {
                using (var response = ex.Response as HttpWebResponse)
                {
                    Console.WriteLine(response.ResponseUri);
                    Console.WriteLine("{0}:{1}", response.StatusCode, response.StatusDescription);

                    //if (response.StatusDescription == "Internal Server Error")
                    //{
                    //    return result;
                    //}
                    if (response.ContentType.IndexOf("text") >= 0)
                    {
                        using (var reader = new StreamReader(response.GetResponseStream()))
                        {
                            string tmp = reader.ReadToEnd();
                            Console.WriteLine(reader.ReadToEnd());
                        }
                    }

                    throw new Exception(string.Format("HttpGet-WebException:{0},{1},{2},{3}",
                        ex.Status, ex.Message, response.StatusCode, response.StatusDescription));
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("HttpGet-Exception:{0}", ex.Message));
            }
            return result;
        }

        static string HttpPost(string url, Hashtable vals)
        {
            string param = "";
            foreach (string k in vals.Keys)
            {
                param += String.Format("{0}={1}&", k, vals[k]);
            }
            byte[] data = Encoding.ASCII.GetBytes(param);

            // リクエストの作成
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = data.Length;
            req.CookieContainer = cookieContainer;

            // ポスト・データの書き込み
            Stream reqStream = req.GetRequestStream();
            reqStream.Write(data, 0, data.Length);
            reqStream.Close();

            WebResponse res = req.GetResponse();

            // cookieの取得と保存 -> 既にccに入って戻ってくるから要らない
            //            CookieCollection cookies = req.CookieContainer.GetCookies(req.RequestUri);
            //            cc.Add(cookies);

            // レスポンスの読み取り
            Stream resStream = res.GetResponseStream();
            StreamReader sr = new StreamReader(resStream, encoder);
            string result = sr.ReadToEnd();
            sr.Close();
            resStream.Close();

            return result;
        }

        public string ReadLog(string url)
        {
            // ページへのアクセス
            return HttpGet(url);
        }

        public string Login(string login, string id, string password, string serviceName)
        {
            // ログイン・ページへのアクセス
            Hashtable vals = new Hashtable();

            switch (serviceName)
            {
                case "ShotNavi":
                    vals["username"] = id;
                    vals["password"] = password;
                    vals["url"] = "/mypage/index.php";
                    vals["_ga"] = "GA1.2.1769414683.1586567554";
                    vals["__gads"] = "ID=eec254810b4a0827:T=1586567555:S=ALNI_MadGl_56WV4avO0HaQzJY0cG4M69g";
                    vals["__utmz"] = "131891661.1586567554.1.1.utmcsr=(direct)|utmccn=(direct)|utmcmd=(none)";
                    vals["PS"] = "sjiko7a1uvai7ocp89csi4o583";
                    vals["_gid"] = "GA1.2.1127279129.1589158744";
                    vals["__utma"] = "131891661.1769414683.1586567554.1588834499.1589158744.4";
                    vals["__utmc"] = "131891661";
                    vals["__utmt"] = "1";
                    vals["__utmb"] = "131891661.1.10.1589158744";
                    vals["__utmli"] = "tab-area";

                    break;

                case "GDO":
                    vals["qLoginName"] = id;
                    vals["qPasswd"] = password;
                    vals["qAutoLogin"] = "1";
                    vals["mm_sid"] = "RFJlqsCACK7_";
                    vals["mm_rurl"] = "https://www.golfdigest.co.jp/";
                    vals["qActionMode"] = "login";
                    vals["guid"] = "ON";
                    vals["mm_wsf"] = "0";
                    vals["qUrl"] = "https%3A%2F%2Fwww.golfdigest.co.jp%2F";
                    break;

                default:
                    break;
            }

            return HttpPost(login, vals);
        }



    }
}
