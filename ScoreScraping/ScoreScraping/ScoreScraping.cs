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

namespace ScoreScraping
{
    class Hole
    {
        public string url;
        public string row;
        public ArrayList yardList;

        public Hole()
        {
            url = string.Empty;
            ArrayList yardList = new ArrayList();
        }
    }

    class ScoreScraping
    {
        public static CookieContainer cookieContainer;
        public ArrayList holeList;

        public ScoreScraping()
        {
            cookieContainer = new CookieContainer();
            holeList = new ArrayList();
        }

        /// <summary>
        /// 引数urlにアクセスした際に取得できるHTMLを返します。
        /// </summary>
        /// <param name="url">URL(アドレス)</param>
        /// <returns>取得したHTML</returns>
        public string GetHtml(string url)
        {
            // html取得文字列
            string html = "";

            try
            {
                // 指定されたURLに対してのRequestを作成します。
                var req = (HttpWebRequest)WebRequest.Create(url);

                // 指定したURLに対してReqestを投げてResponseを取得します。
                using (var res = (HttpWebResponse)req.GetResponse())
                using (var resSt = res.GetResponseStream())
                // 取得した文字列をUTF8でエンコードします。
                using (var sr = new StreamReader(resSt, Encoding.UTF8))
                //                using (var sr = new StreamReader(resSt, Encoding.Unicode))
                {
                    // HTMLを取得する。
                    html = sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetHtml: {0}", ex.Message));
            }
            return html;
        }

        /// <summary>
        /// 正規化表現を使用してHTMLからタイトルを取得します。
        /// </summary>
        /// <param name="html">HTML文字列</param>
        /// <returns>HTML文字列から取得したタイトル</returns>
        public string GetTitle(string html)
        {
            string s = string.Empty;

            try
            {
                // 正規化表現
                // 大文字小文字区別なし       : RegexOptions.IgnoreCase
                // 「.」を改行にも適応する設定: RegexOptions.Singleline
                var reg = new Regex(@"<title>(?<title>.*?)</title>",
                             RegexOptions.IgnoreCase | RegexOptions.Singleline);

                // html文字列内から条件にマッチしたデータを抜き取ります。
                var m = reg.Match(html);

                // 条件にマッチした文字列内からKey("title部分")にマッチした値を抜き取ります。
                s = m.Groups["title"].Value;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("GetTitle: {0}", ex.Message));
            }

            return s;
        }


        public async Task<CookieContainer> LoginAsync(string Url, string Id, string Password)
        {
            CookieContainer cc;
            using (var handler = new HttpClientHandler())
            {
                using (var client = new HttpClient(handler))
                {
                    //ログイン用のPOSTデータ生成
                    var content = new FormUrlEncodedContent(new Dictionary<string, string>
                    {
                        {"qLoginName", Id },
                        {"qPasswd", Password },
                        {"qAutoLogin" , "1" },
                        {"mm_sid","RFJlqsCACK7_" },
                        {"mm_rurl","https://www.golfdigest.co.jp/" },
                        {"qActionMode","login"},
                        {"guid","ON" },
                        {"mm_wsf","0"},
                        {"qUrl","https%3A%2F%2Fwww.golfdigest.co.jp%2F" }
                        //,
//                        {"skip","1"}
                    });

                    //ログイン
                    await client.PostAsync(Url, content);

                    //クッキー保存
                    cc = handler.CookieContainer;
                }
            }
            CookieCollection cookies = cc.GetCookies(new Uri(Url));

            foreach (Cookie c in cookies)
            {
                Console.WriteLine("クッキー名:" + c.Name.ToString());
                Console.WriteLine("クッキーを使うサイトのドメイン名:" + c.Domain.ToString());
                Console.WriteLine("クッキー発行日時:" + c.TimeStamp.ToString() + Environment.NewLine);
            }

            Console.WriteLine("ログイン処理完了！");

            return cc;
        }


        //  ---- 他の方法 ----

        //        static Encoding encoder = Encoding.GetEncoding("EUC-JP");
        static Encoding encoder = Encoding.GetEncoding("utf-8");

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
