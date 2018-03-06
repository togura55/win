using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.Live;


namespace Wmetahelper
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
//        private readonly LiveConnectSession session;

        public MainWindow()
        {
            InitializeComponent();
        }

        private LiveAuthClient liveAuthClient;
        private LiveConnectClient liveConnectClient;
        private readonly string clientId = "tsuyoshi.ogura@wacom.com";
        string[] scopes = new string[] { "wl.signin", "wl.skydrive" };


        private void Pbtn_LiveConnect_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                this.liveAuthClient = new LiveAuthClient(clientId);
                webBrowser.Navigate(this.liveAuthClient.GetLoginUrl(scopes));

                //LiveConnectClient liveClient = new LiveConnectClient(this.session);
                //LiveOperationResult operationResult =
                //    await liveClient.GetAsync("folder.8c8ce076ca27823f.8C8CE076CA27823F!126");
                //dynamic result = operationResult.Result;
                //infoTextBlock.Text = "Folder name: " + result.name + ", ID: " + result.id;
            }
            catch (LiveConnectException exception)
            {
                infoTextBlock.Text = "Error getting folder info: " + exception.Message;
            }
        }

        private async void webBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            if (this.webBrowser.Source.AbsoluteUri.StartsWith("https://wacomaad-my.sharepoint.com/personal/tsuyoshi_ogura_wacom_com/_layouts/15/onedrive.aspx?id=%2Fpersonal%2Ftsuyoshi_ogura_wacom_com%2FDocuments%2Fshare%2FCLB-Create"))
            {
                //認証後のurlからcodeパラメーターを取得
                string authenticationCode = this.webBrowser.Source
                                            .Query.TrimStart('?').Split('&')
                                            .Where(x => x.IndexOf("code=") == 0)
                                            .Single()
                                            .Substring(5);

                LiveConnectSession session = await this.liveAuthClient.ExchangeAuthCodeAsync(authenticationCode);
                this.liveConnectClient = new LiveConnectClient(session);
                LiveOperationResult meRs = await this.liveConnectClient.GetAsync("me");
                dynamic meData = meRs.Result;

                MessageBox.Show("Name: " + meData.name + "ID: " + meData.id);
            }
        }

        // read file property
        //private async void btnReadFile_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        LiveConnectClient liveClient = new LiveConnectClient(this.session);
        //        LiveOperationResult operationResult =
        //            await liveClient.GetAsync("file.8c8ce076ca27823f.8C8CE076CA27823F!129");
        //        dynamic result = operationResult.Result;
        //        this.infoTextBlock.Text = string.Join(" ", "File name:", result.name, "ID:", result.id);
        //    }
        //    catch (LiveConnectException exception)
        //    {
        //        this.infoTextBlock.Text = "Error getting file info: " + exception.Message;
        //    }
        //}

        // file download
        //private System.Threading.CancellationTokenSource ctsDownload;

        //private async void btnDownloadFile_Click(object sender, RoutedEventArgs e)
        //{
        //    try
        //    {
        //        var picker = new Windows.Storage.Pickers.FileSavePicker();
        //        picker.SuggestedFileName = "MyDownloadedPicutre.jpg";
        //        picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.Downloads;
        //        picker.FileTypeChoices.Add("Picture", new List<string>(new string[] { ".jpg" }));
        //        StorageFile file = await picker.PickSaveFileAsync();
        //        if (file != null)
        //        {
        //            this.progressBar.Value = 0;
        //            var progressHandler = new Progress<LiveOperationProgress>(
        //                (progress) => { this.progressBar.Value = progress.ProgressPercentage; });
        //            this.ctsDownload = new System.Threading.CancellationTokenSource();
        //            LiveConnectClient liveClient = new LiveConnectClient(this.session);
        //            await liveClient.BackgroundDownloadAsync("file.8c8ce076ca27823f.8C8CE076CA27823F!161", file,
        //                this.ctsDownload.Token, progressHandler);
        //            this.infoTextBlock.Text = "Download completed.";
        //        }
        //    }
        //    catch (System.Threading.Tasks.TaskCanceledException)
        //    {
        //        this.infoTextBlock.Text = "Download cancelled.";
        //    }
        //    catch (LiveConnectException exception)
        //    {
        //        this.infoTextBlock.Text = "Error getting file contents: " + exception.Message;
        //    }
        //}

        //private void btnCancelDownload_Click(object sender, RoutedEventArgs e)
        //{
        //    if (this.ctsDownload != null)
        //    {
        //        this.ctsDownload.Cancel();
        //    }
        //}
    }
}
