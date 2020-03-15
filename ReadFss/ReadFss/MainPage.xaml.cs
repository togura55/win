using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// 空白ページの項目テンプレートについては、https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x411 を参照してください

namespace ReadFss
{
    /// <summary>
    /// それ自体で使用できる空白ページまたはフレーム内に移動できる空白ページ。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        string error_message = string.Empty;
        FssStream fs = null;

        public MainPage()
        {
            this.InitializeComponent();
            ResetUI();

            byte[] byteArray = { 0, 1 , 2, 3};
            ushort data = BitConverter.ToUInt16(byteArray, 0);

            int i = 0;
            data = (ushort)(byteArray[i] << 8 | byteArray[i+1]);

        }

        private void Ptn_ReadFile_Click(object sender, RoutedEventArgs e)
        {
            // Open read file dialog
            // Read data stream
            string s = string.Empty;
            ResetUI();
            ImportInkData(s);
        }

        private void Pbtn_Copy_Click(object sender, RoutedEventArgs e)
        {
            string dataString = string.Empty;
            foreach (String item in ListBox_Messages.Items)
            {
                dataString += item + System.Environment.NewLine;
            }

            DataPackage dataPackage = new DataPackage();
            // copy 
            dataPackage.RequestedOperation = DataPackageOperation.Copy;
            dataPackage.SetText(dataString);
            Clipboard.SetContent(dataPackage);

        }

        private void Pbtn_Clear_Click(object sender, RoutedEventArgs e)
        {
            ResetUI();
        }

        private void ResetUI()
        {
            ListBox_Messages.Items.Clear();

            Ptn_ReadFile.Content = "Read File";
            textBlock.Text = "";
            Pbtn_Clear.Content = "Clear";
            Pbtn_Copy.Content = "Copy";
        }

        // ---- 表示部 ----------
        private void ShowFssData()
        {
            try
            {
                string buff = string.Empty;

                buff = string.Format("Stream data size: {0}", fs.size);
                ListBox_Messages.Items.Add(buff);

                string first_label_text = System.Text.Encoding.ASCII.GetString(fs.first_label); //ASCII encoding
                buff = string.Format("1st label({0}): {1}, 0x{2:X},0x{3:X} ", fs.first_label.Length, first_label_text, fs.first_label[0], fs.first_label[1]);
                ListBox_Messages.Items.Add(buff);
                buff = string.Format("1st data({0}): {1},{2}({3}), 0x{1:X},0x{2:X} ",
                    fs.first_data.Length, fs.first_data[0], fs.first_data[1], BitConverter.ToUInt16(fs.first_data, 0));
                ListBox_Messages.Items.Add(buff);
 
                SetStringToListBox("2nd label({0}): ", "{0},", fs.second_label);
                SetStringToListBox("2nd data({0}): ", "{0},", fs.second_data);
                SetStringToListBox("unknown 1({0}): ", "{0:X2},", fs.unknown_1);
                SetStringToListBox("ExtraData_Key({0}): ", "{0:X2},", fs.extraData_Key); // strings, indefinite length
                SetStringToListBox("ExtraData_Sep({0}): ", "{0:X2},", fs.extraData_Sep); // separater
                SetStringToListBox("ExtraData_Value({0}): ", "{0:X2},", fs.extraData_Value);  // strings, indefinite length
                SetStringToListBox("CaptureWho_Sep({0}): ", "{0:X2},", fs.captureWho_Sep); // separater
                SetStringToListBox("CaptureWho({0}): ", "{0:X2},", fs.captureWho);// strings,  indefinite length
                SetStringToListBox("CaptureWhy_Sep({0}): ", "{0:X2},", fs.captureWhy_Sep); // separater
                SetStringToListBox("CaptureWhy({0}): ", "{0:X2},", fs.captureWhy);// strings,  indefinite length
                SetStringToListBox("unknown 2({0}): ", "{0:X2},", fs.unknown_2); // data, indefinite length
                SetStringToListBox("stroke packets({0}): ", "{0:X2},", fs.stroke_packets); // data, indefinite length
                SetStringToListBox("unknown 3({0}): ", "{0:X2},", fs.unknown_3);
                SetStringToListBox("unknown 4({0}): ", "{0:X2},", fs.unknown_4);
                SetStringToListBox("unknown 5({0}): ", "{0:X2},", fs.unknown_5); // strings and data, indefinite length, EOF

                buff = string.Format("StrokePart({0}): ", fs.strokeList.Count);
                foreach (int data in fs.strokeList)
                    buff += string.Format("{0},", data);
                ListBox_Messages.Items.Add(buff);
            }
            catch (Exception ex)
            {
                ListBox_Messages.Items.Add(string.Format("Exception: {0}", ex.Message));
            }
        }

        private void SetStringToListBox(string stringFormat, string dataFormat, byte[] byteArray)
        {
            string buff = string.Format(stringFormat, byteArray.Length);
            foreach (int data in byteArray)
                buff += string.Format(dataFormat, data);
            ListBox_Messages.Items.Add(buff);
        }

        private async void ImportInkData(string s)
        {
            try
            {
                fs = new FssStream();

                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".fss");

                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    // Application now has read/write access to the picked file
                    this.textBlock.Text = "Picked fss: " + file.Name;

                    var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
                    fs.byteStream = new byte[buffer.Length];

                    using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                    {
                        dataReader.ReadBytes(fs.byteStream);
                        fs.size = fs.byteStream.Length;
                        // Analyze it
                        fs.Decode();

                        ShowFssData();

                        //if (fs.Decode() < 0)
                        //{
                        //    Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(error_message);
                        //    await md.ShowAsync();
                        //}
                        //else
                        //{
                        //    ShowFssData();
                        //}
                    }
                }
                else
                {
                    this.textBlock.Text = "Operation cancelled.";
                }
            }
            catch (Exception ex)
            {
                ListBox_Messages.Items.Add(string.Format("Exception: {0}", ex.Message));
            }

        }
    }
}
