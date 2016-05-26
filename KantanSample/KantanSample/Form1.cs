using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WindowsFormsApplication1
{
    public partial class Form1 : Form
    {
        private bool bSentMail = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.timer1.Start();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            bSentMail = false;
            this.timer1.Stop();
        }
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            GetTemp();
            this.tbDate.Text = DateTime.Now.ToString();
        }

        private void GetTemp()
        {
            USBMeter um = new USBMeter();
            if (um.IsUSBMeter())
            {
                um.GetData();
                this.tbTemp.Text = um.Temp.ToString();
                this.tbHumid.Text = um.Humid.ToString();

                if (um.Temp > 32 && bSentMail==false)
                {
                    SendMail(um.Temp.ToString());
                }
            }
        }

        private void SendMail(string sTemp)
        {
            string sSMTPServer = "smtp.gmail.com";
            int iSMTPServerPort = 587;
            string sFrom = "test.sdljapan@gmail.com";
            string sTo = tbSendTo.Text.ToString();
            string sSubject = "テスト：ラボの温度が危険な状態です！";
            string sBody = "これはテストメールです。by tbojo 現在室温が " + sTemp + " 度を超えました。";
            SmtpClient sc = new SmtpClient(sSMTPServer, iSMTPServerPort);
            sc.EnableSsl = true;
            sc.Credentials = new NetworkCredential(sFrom, "sdllab.2");
            MailMessage msg = new MailMessage(sFrom, sTo, sSubject, sBody);
            sc.Send(msg);
            bSentMail = true;
        }


        /// ----------------------------------------------------------------------
        /// <summary>
        /// USBMeter.dll呼び出しクラス
        /// </summary>
        /// ----------------------------------------------------------------------
        public class USBMeter
        {
            /// ------------------------------------------------------------------
            /// <summary>
            /// モジュールの検索
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            /// ------------------------------------------------------------------
            [DllImport("USBMeter.dll", CharSet = CharSet.Ansi)]
//            public static extern String FindUSB(ref long index);
            public static extern IntPtr FindUSB(ref long index);

            /// ------------------------------------------------------------------
            /// <summary>
            /// 温度・湿度の取得
            /// </summary>
            /// <param name="dev"></param>
            /// <param name="temp"></param>
            /// <param name="humid"></param>
            /// <returns></returns>
            /// ------------------------------------------------------------------
            [DllImport("USBMeter.dll", CharSet = CharSet.Ansi)]
            public static extern long GetTempHumid(String dev, ref double temp, ref double humid);

            /// ------------------------------------------------------------------
            /// <summary>
            /// ヒーターの制御
            /// </summary>
            /// <param name="dev"></param>
            /// <param name="val"></param>
            /// <returns></returns>
            /// ------------------------------------------------------------------
            [DllImport("USBMeter.dll", CharSet = CharSet.Ansi)]
            public static extern long SetHeater(String dev, int val);

            /// ------------------------------------------------------------------
            /// <summary>
            /// LEDの制御
            /// </summary>
            /// <param name="dev"></param>
            /// <param name="port"></param>
            /// <param name="val"></param>
            /// <returns></returns>
            /// ------------------------------------------------------------------
            [DllImport("USBMeter.dll", CharSet = CharSet.Ansi)]
            public static extern long ControlIO(String dev, int port, int val);

            /// ------------------------------------------------------------------
            /// <summary>
            /// ファームウェアバージョンの取得
            /// </summary>
            /// <param name="dev"></param>
            /// <returns></returns>
            /// ------------------------------------------------------------------
            [DllImport("USBMeter.dll", CharSet = CharSet.Ansi)]
            public static extern string GetVers(String dev);

            /// ------------------------------------------------------------------
            /// <summary>
            /// 温度・湿度の取得(2)
            /// </summary>
            /// <param name="dev"></param>
            /// <param name="temp"></param>
            /// <param name="humid"></param>
            /// <returns></returns>
            /// ------------------------------------------------------------------
            [DllImport("USBMeter.dll", CharSet = CharSet.Ansi)]
            public static extern long GetTempHumidTrue(String dev, ref double temp, ref double humid);

            /// <summary>
            /// デバイス名
            /// </summary>
//            public String USBDEV = "";
            public IntPtr iRet = new IntPtr();
            public string USBDEV = "";

            /// <summary>
            /// FindUSBで取得される数値
            /// </summary>
            public long USBNUM = 0;

            /// <summary>
            /// パラメータ
            /// </summary>
            public struct USBMETER_DATA
            {
                /// <summary>
                /// 温度
                /// </summary>
                public double temp;

                /// <summary>
                /// 湿度
                /// </summary>
                public double humid;
            }
            public USBMETER_DATA Data;

            public USBMeter()
            {
                Data.temp = Data.humid = 0;

                iRet = new IntPtr();
                USBDEV = "";
                USBNUM = 0;
                try
                {
                    iRet = FindUSB(ref USBNUM);
                }
                catch
                {
                    USBNUM = 0;
//                    USBDEV = "";
                }
            }

            /// ------------------------------------------------------------------
            /// <summary>
            /// USBMeterが有効かどうかのチェック
            /// </summary>
            /// <returns></returns>
            /// ------------------------------------------------------------------
            public bool IsUSBMeter()
            {
                return USBNUM > 0 && USBDEV != "";
            }

            /// ------------------------------------------------------------------
            /// <summary>
            /// 値を取得する
            /// </summary>
            /// ------------------------------------------------------------------
            public void GetData()
            {
                GetTempHumid(USBDEV, ref Data.temp, ref Data.humid);
            }

            /// ------------------------------------------------------------------
            /// <summary>
            /// 値を取得する(2)
            /// </summary>
            /// ------------------------------------------------------------------
            public void GetData2()
            {
                GetTempHumidTrue(USBDEV, ref Data.temp, ref Data.humid);
            }

            /// ------------------------------------------------------------------
            /// <summary>
            /// 温度を取得
            /// </summary>
            /// ------------------------------------------------------------------
            public double Temp
            {
                get
                {
                    return Data.temp;
                }
            }

            /// ------------------------------------------------------------------
            /// <summary>
            /// 湿度を取得
            /// </summary>
            /// ------------------------------------------------------------------
            public double Humid
            {
                get
                {
                    return Data.humid;
                }
            }

            /// ------------------------------------------------------------------
            /// <summary>
            /// ヒーターON/OFF
            /// </summary>
            /// <param name="b">ONならtrue</param>
            /// <returns></returns>
            /// ------------------------------------------------------------------
            public void SetHeater(bool b)
            {
                SetHeater(USBDEV, b ? 1 : 0);
            }

            /// ------------------------------------------------------------------
            /// <summary>
            /// LEDのON/OFF
            /// </summary>
            /// <param name="port">LEDポート 0-1</param>
            /// <param name="b">ONならtrue</param>
            /// <returns></returns>
            /// ------------------------------------------------------------------
            public void SetLED(int port, bool b)
            {
                ControlIO(USBDEV, port, b ? 1 : 0);
            }

            /// ------------------------------------------------------------------
            /// <summary>
            /// ファームウェアバージョン
            /// </summary>
            /// <returns></returns>
            /// ------------------------------------------------------------------
            public string GetVersion()
            {
                return GetVers(USBDEV);
            }
        }


    }
}
