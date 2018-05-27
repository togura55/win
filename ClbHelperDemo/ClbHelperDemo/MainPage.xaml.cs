using Microsoft.OneDrive.Sdk; // required Microsoft.OneDrive.Sdk v1.x, instead of 2.x
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Windows;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.Storage;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace ClbHelperDemo
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // Folder path
        private string folderPath = "CLB Paper";

        // The StoredInkFile object
        StoredInkFile storedInkFile;

        // OneDriveController
        FileStorgeController.OneDrive oneDrive;

        // Polling period in seconds
        double period = 30;

        string refreshToken;
        string dirId = null;
        private DispatcherTimer timer;
        private int count;
        private bool timer_flag = false;

        public MainPage()
        {
            this.InitializeComponent();

            textBlock_Response.Text = "App is started.";

            ReadLocalSettings();

            storedInkFile = new StoredInkFile();
            oneDrive = new FileStorgeController.OneDrive();

            App.Current.Suspending += OnSuspending;
            App.Current.Resuming += OnResuming;

            Pbtn_GetDriveId.Visibility = Visibility.Collapsed;
            Pbtn_GetFileList.Content = "List Files";
            Pbtn_GetFileList.Visibility = Visibility.Collapsed;
            textBlock_FolderName.Text = "Folder Name";
            textBlock_FolderName.Visibility = Visibility.Collapsed;
            textBox_FolderName.Text = folderPath;
            textBox_FolderName.Visibility = Visibility.Collapsed;
            textBlock_Period.Text = "Period (s)";
            textBlock_Period.Visibility = Visibility.Collapsed;
            textBox_Period.Text = period.ToString();
            textBox_Period.Visibility = Visibility.Collapsed;
            listBox_FileList.Visibility = Visibility.Collapsed;
        }

        //Resumingイベントのイベントハンドラ
        private void OnResuming(object sender, object e)
        {
            //ここにデータ復元の処理を書く
        }

        //Suspendingイベントのイベントハンドラ
        private void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            //ここにデータ保存の処理を書く
            UpdateLocalSettings();
        }

        private void UpdateLocalSettings()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            string token = oneDrive.GetRefreshToken();
            if (token != string.Empty)
                localSettings.Values["RefreshToken"] = token;
        }

        private void ReadLocalSettings()
        {
            ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
            object val = localSettings.Values["RefreshToken"];
            if (val != null && val is string)
            {
                this.refreshToken = (string)val;
                textBlock_Response.Text = (string)val;
            }
            else
            {
                textBlock_Response.Text = "No settings";
            }
        }

        private void Pbtn_Login_Click(object sender, RoutedEventArgs e)
        {
            textBlock_Response.Text = "Login button is pressed.";

            folderPath = textBox_FolderName.Text;

            if (oneDrive.OneDriveClientAuth == null)
            {
                // Show in text box that we are connected.
                textBlock_Response.Text = "We are now connected.";

                oneDrive.LoginByWebAuthentication();
                //               LoginByAuthProvider();
                //               LoginBySilentlyAuth();

                // We are either just autheticated and connected or we already connected, 
                // either way we need the drive button now.
                Pbtn_GetDriveId.Visibility = Visibility.Visible;
                Pbtn_GetFileList.Visibility = Visibility.Visible;
                textBox_FolderName.Visibility = Visibility.Visible;
                textBlock_FolderName.Visibility = Visibility.Visible;
                textBox_FolderName.Visibility = Visibility.Visible;
                textBlock_Period.Visibility = Visibility.Visible;
                textBox_Period.Visibility = Visibility.Visible;

                listBox_FileList.Visibility = Visibility.Visible;
            }
            else
            {
                textBlock_Response.Text = "No OneDriveClient objects.";
            }
        }

        private async void Pbtn_GetDriveId_Click(object sender, RoutedEventArgs e)
        {
            if (oneDrive.OneDriveClientAuth != null && oneDrive.OneDriveClientAuth.IsAuthenticated == true)
            {
                var drive = await oneDrive.OneDriveClientAuth.Drive.Request().GetAsync();
                textBlock_Response.Text = drive.Id.ToString();
            }
            else
            {
                textBlock_Response.Text = "We should never get here...";
            }

        }

        private async void Pbtn_GetFileList_Click(object sender, RoutedEventArgs e)
        {
            //           this.CurrentDirectoryObjects.Clear();

            if (timer_flag)
                StopPollingProc();
            else
            {
                dirId = await oneDrive.GetFolderId(folderPath);

                if (dirId != null)
                {
                    count = 0;
                    await RepeatProc();   // Do the 1st time

                    // ToDo: detect non-number strings

                    double value = Convert.ToDouble(textBox_Period.Text);
                    StartPollingProc(value);  // start timer in second after 2nd time
                }
                else
                {
                    textBlock_Response.Text = "Unable to find a path: " + folderPath;
                }
            }

        }

        private void StopPollingProc()
        {
            timer.Stop();
            this.Pbtn_GetFileList.Content = "List Files";
            timer_flag = timer_flag ? false : true;
        }

        private void StartPollingProc(double value)
        {
            timer = new DispatcherTimer { };

            // タイマーイベントの間隔を指定。
            // ここではvalue秒おきに実行する
            timer.Interval = TimeSpan.FromSeconds(value);

            // value秒おきに実行するイベントを指定
            timer.Tick += _timer_Tick;

            // タイマーイベントを開始する
            timer.Start();

            this.Pbtn_GetFileList.Content = "Stop";
            timer_flag = timer_flag ? false : true;
        }

        private async void _timer_Tick(object sender, object e)
        {
            await RepeatProc();
        }

        private async Task RepeatProc()
        {
            count++;

            textBlock_Response.Text = "Polling Count = " + count.ToString();

            await oneDrive.GetFileList(storedInkFile, this.dirId);
            if (storedInkFile.count > 0)
            {
                listBox_FileList.ClearValue(ListBox.ItemsSourceProperty); // clear once
                listBox_FileList.Items.Clear();

                foreach (var obj in storedInkFile.properties)
                {
                    listBox_FileList.Items.Add(obj.Name.ToString());

                    // for debug
                    listBox_LogMessages.Items.Add(
                        count.ToString() + ": " +
                        folderPath + ": " +
                        "File: " + obj.Name.ToString());
                }
            }
        }
    }
}
