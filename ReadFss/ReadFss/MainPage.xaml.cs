using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
        byte[] buffBytes = null;

        public MainPage()
        {
            this.InitializeComponent();
            Ptn_ReadFile.Content = "Read File";
        }

        private void Ptn_ReadFile_Click(object sender, RoutedEventArgs e)
        {
            // Open read file dialog
            // Read data stream
            string s = string.Empty;
            ImportInkData(s);


        }

        private void DecodeFSS()
        {
            string buff = string.Empty;

            int size = buffBytes.Length;
            byte[] first_label = new byte[2];
            byte[] first_data = new byte[2];
            byte[] second_label = new byte[32];
            byte[] second_data = new byte[16];

            Array.Copy(buffBytes, 0, first_label, 0, 2);
            Array.Copy(buffBytes, 2, first_data, 0, 2);
            Array.Copy(buffBytes, 4, second_label, 0, 32);
            Array.Copy(buffBytes, 36, second_data, 0, 16);

            buff = string.Format("Stream data size: {0}", size);
            ListBox_Messages.Items.Add(buff);

            string first_label_text = System.Text.Encoding.ASCII.GetString(first_label); //ASCII encoding
            buff = string.Format("1st label({0}): {1}, 0x{2:X},0x{3:X} ", first_label.Length, first_label_text, first_label[0], first_label[1]);
            ListBox_Messages.Items.Add(buff);
            buff = string.Format("1st data({0}): {1},{2}({3}), 0x{1:X},0x{2:X} ", 
                first_data.Length, first_data[0], first_data[1], BitConverter.ToUInt16(first_data, 0));
            ListBox_Messages.Items.Add(buff);
            int i = 0;
            buff = string.Format("2nd label({0}): ", second_label.Length);
            for (i=0; i<second_label.Length; i++)
            {
                buff += string.Format("{0},", second_label[i]);
            }
            ListBox_Messages.Items.Add(buff);

            buff = string.Format("2nd data({0}): ", second_data.Length);
            for (i = 0; i < second_data.Length; i++)
            {
                buff += string.Format("{0},", second_data[i]);
            }
            ListBox_Messages.Items.Add(buff);
        }

        private async void ImportInkData(string s)
        {
            try
            {
                var picker = new Windows.Storage.Pickers.FileOpenPicker();
                picker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                picker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.PicturesLibrary;
                picker.FileTypeFilter.Add(".fss");
//                picker.FileTypeFilter.Add(".jpeg");
//                picker.FileTypeFilter.Add(".png");

                Windows.Storage.StorageFile file = await picker.PickSingleFileAsync();
                if (file != null)
                {
                    // Application now has read/write access to the picked file
                    this.textBlock.Text = "Picked fss: " + file.Name;

                    var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
                    buffBytes = new byte[buffer.Length];

                    using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                    {
                        dataReader.ReadBytes(buffBytes);

                        // Analyze it
                        DecodeFSS();
                    }
                }
                else
                {
                    this.textBlock.Text = "Operation cancelled.";
                }
            }
            catch (Exception ex)
            {

            }

            //try
            //{
            //    var folderPicker = new Windows.Storage.Pickers.FolderPicker();

            //    folderPicker.FileTypeFilter.Add("*");
            //    Windows.Storage.StorageFolder folder =
            //        await folderPicker.PickSingleFolderAsync();

            //    if (folder == null)
            //    {
            //        return;
            //    }

            //    string path = folder.Path.ToString();
            //    string filename = "data.txt";

            //    // Create a data stored file; replace if exists.
            //    Windows.Storage.StorageFile dataFile =
            //        await folder.CreateFileAsync(filename,
            //            Windows.Storage.CreationCollisionOption.ReplaceExisting);

            //    // string dataString = string.Empty;
            //    //foreach (String item in ListBox_Messages.Items)
            //    //{
            //    //    dataString += item + System.Environment.NewLine;
            //    //}

            //    if (dataFile != null)
            //    {
            //        await Windows.Storage.FileIO.WriteTextAsync(dataFile, s);
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }
    }
}
