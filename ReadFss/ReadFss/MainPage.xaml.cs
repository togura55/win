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
        byte[] buffBytes = null;
        string error_message = string.Empty;
        int size = 0;
        byte[] first_label = new byte[2];
        byte[] first_data = new byte[2];
        byte[] second_label = new byte[32];
        byte[] second_data = new byte[16];
        byte[] unknown_1 = new byte[4];
        byte[] extraData_Key = null; // strings, indefinite length
        byte[] extraData_Sep = new byte[1]; // separater
        byte[] extraData_Value = null;  // strings, indefinite length
        byte[] captureWho_Sep = new byte[3]; // separater
        byte[] captureWho = null;// strings,  indefinite length
        byte[] captureWhy_Sep = new byte[3]; // separater
        byte[] captureWhy = null;// strings,  indefinite length
        byte[] unknown_2 = null; // data, indefinite length
        byte[] stroke_packets = null; // data, indefinite length
        byte[] unknown_3 = new byte[13];
        byte[] unknown_4 = new byte[4];
        byte[] unknown_5 = null; // strings and data, indefinite length, EOF


        public MainPage()
        {
            this.InitializeComponent();
            Ptn_ReadFile.Content = "Read File";
            textBlock.Text = "";
            Pbtn_Clear.Content = "Clear";
            Pbtn_Copy.Content = "Copy";
        }

        private void Ptn_ReadFile_Click(object sender, RoutedEventArgs e)
        {
            // Open read file dialog
            // Read data stream
            string s = string.Empty;
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
            ListBox_Messages.Items.Clear();
        }

        List<UInt16> strokeList = new List<UInt16>();

        private void DecodeStrokePart()
        {
            if(stroke_packets != null)
            {
                int unit = 2;
                int num = stroke_packets.Length / unit;

                for (int i = 0; i < num; i++)
                {
                    strokeList.Add(BitConverter.ToUInt16(stroke_packets, i * unit));
                }
            }
        }

        private int DecodeFSS()
        {
            int index = 0;
            int current = 0;

            try
            {
                Array.Copy(buffBytes, 0, first_label, 0, 2);
                Array.Copy(buffBytes, 2, first_data, 0, 2);
                Array.Copy(buffBytes, 4, second_label, 0, 32);
                Array.Copy(buffBytes, 36, second_data, 0, 16);
                Array.Copy(buffBytes, 52, unknown_1, 0, 4);

                extraData_Sep[0] = (byte)0x19;

                index = Array.IndexOf(buffBytes, (byte)0x19, 56);
                if (index > 0)
                {
                    extraData_Key = new byte[index - 56];
                    Array.Copy(buffBytes, 56, extraData_Key, 0, index - 56);
                    current = index;
                }
                else
                {// error
                    error_message = string.Format("Cannot find 0x{0:x}", 0x19);
                    return -1;
                }

                index = Array.IndexOf(buffBytes, (byte)0x17, current);
                if (index > 0)
                {
                    extraData_Value = new byte[index - current];
                    Array.Copy(buffBytes, current, extraData_Value, 0, index - current);
                    current = index;
                }
                else
                {// error
                    error_message = string.Format("Cannot find 0x{0:x}", 0x17);
                    return -1;
                }

                Array.Copy(buffBytes, current, captureWho_Sep, 0, captureWho_Sep.Length);
                current = current + captureWho_Sep.Length;

                captureWho = new byte[captureWho_Sep.Last()];
                Array.Copy(buffBytes, current, captureWho, 0, captureWho_Sep.Last());
                current = current + captureWho.Length;

                Array.Copy(buffBytes, current, captureWhy_Sep, 0, captureWhy_Sep.Length);
                current = current + captureWhy_Sep.Length;

                captureWhy = new byte[captureWhy_Sep.Last()];
                Array.Copy(buffBytes, current, captureWhy, 0, captureWhy_Sep.Last());
                current = current + captureWhy.Length;

                // 最後が0A 09 02を探す
                byte[] searchBytes = { 0x0A, 0x09, 0x02 };
                index = SearchElementBlock(buffBytes, searchBytes, current);
                if (index > 0)
                {
                    unknown_2 = new byte[index + searchBytes.Length - current];
                    Array.Copy(buffBytes, current, unknown_2, 0, index + searchBytes.Length - current);
                    current = index + searchBytes.Length;
                }
                else
                {// error
                    string elements = string.Empty;
                    foreach (int b in searchBytes)
                        elements += string.Format("0x{0:X},", b);
                    error_message = string.Format("Cannot find {0}", elements);
                    return -1;
                }

                // 最初が0C 08 01 を探す
                byte[] searchBytes2 = { 0x0C, 0x08, 0x01 };
                index = SearchElementBlock(buffBytes, searchBytes2, current);
                if (index > 0)
                {
                    stroke_packets = new byte[index - current];
                    Array.Copy(buffBytes, current, stroke_packets, 0, index - current);
                    current = index;
                }
                else
                {// error
                    string elements = string.Empty;
                    foreach (int b in searchBytes2)
                        elements += string.Format("0x{0:X},", b);
                    error_message = string.Format("Cannot find {0}", elements);
                    return -1;
                }

                // 13バイト読み込む
                Array.Copy(buffBytes, current, unknown_3, 0, unknown_3.Length);
                current = current + unknown_3.Length;

                // 4バイト読み込む
                Array.Copy(buffBytes, current, unknown_4, 0, unknown_4.Length);
                current = current + unknown_4.Length;

                // 最後まで読み込む
                unknown_5 = new byte[size - current];
                Array.Copy(buffBytes, current, unknown_5, 0, unknown_5.Length);
                current = current + unknown_5.Length;

                DecodeStrokePart();
            }

            catch (Exception ex)
            {
                ListBox_Messages.Items.Add(string.Format("Exception: {0}", ex.Message));
            }
            return 0;
        }

        private int SearchElementBlock(byte[] targetBytes, byte[] searchBytes, int start = 0)
        {
            int pos = -1;
            byte[] buff = new byte[targetBytes.Length - start];
            Array.Copy(targetBytes, start, buff, 0, targetBytes.Length-start);
            List<int> positions = SearchBytePattern(searchBytes, buff);
            foreach (var item in positions)
            {
                //               string str = string.Format("Pattern matched at pos {0}", item + start);
                pos = item + start;
            }
            return pos;
        }

        static public List<int> SearchBytePattern(byte[] pattern, byte[] bytes)
        {
            List<int> positions = new List<int>();
            int patternLength = pattern.Length;
            int totalLength = bytes.Length;
            byte firstMatchByte = pattern[0];
            for (int i = 0; i < totalLength; i++)
            {
                if (firstMatchByte == bytes[i] && totalLength - i >= patternLength)
                {
                    byte[] match = new byte[patternLength];
                    Array.Copy(bytes, i, match, 0, patternLength);
                    if (match.SequenceEqual<byte>(pattern))
                    {
                        positions.Add(i);
                        i += patternLength - 1;
                    }
                }
            }
            return positions;
        }

        // ---- 表示部 ----------
        private void ShowFssData()
        {
            try
            {
                string buff = string.Empty;

                buff = string.Format("Stream data size: {0}", size);
                ListBox_Messages.Items.Add(buff);

                string first_label_text = System.Text.Encoding.ASCII.GetString(first_label); //ASCII encoding
                buff = string.Format("1st label({0}): {1}, 0x{2:X},0x{3:X} ", first_label.Length, first_label_text, first_label[0], first_label[1]);
                ListBox_Messages.Items.Add(buff);
                buff = string.Format("1st data({0}): {1},{2}({3}), 0x{1:X},0x{2:X} ",
                    first_data.Length, first_data[0], first_data[1], BitConverter.ToUInt16(first_data, 0));
                ListBox_Messages.Items.Add(buff);
 
                SetStringToListBox("2nd label({0}): ", "{0},", second_label);
                SetStringToListBox("2nd data({0}): ", "{0},", second_data);
                SetStringToListBox("unknown 1({0}): ", "{0:X2},", unknown_1);
                SetStringToListBox("ExtraData_Key({0}): ", "{0:X2},", extraData_Key); // strings, indefinite length
                SetStringToListBox("ExtraData_Sep({0}): ", "{0:X2},", extraData_Sep); // separater
                SetStringToListBox("ExtraData_Value({0}): ", "{0:X2},", extraData_Value);  // strings, indefinite length
                SetStringToListBox("CaptureWho_Sep({0}): ", "{0:X2},", captureWho_Sep); // separater
                SetStringToListBox("CaptureWho({0}): ", "{0:X2},", captureWho);// strings,  indefinite length
                SetStringToListBox("CaptureWhy_Sep({0}): ", "{0:X2},", captureWhy_Sep); // separater
                SetStringToListBox("CaptureWhy({0}): ", "{0:X2},", captureWhy);// strings,  indefinite length
                SetStringToListBox("unknown 2({0}): ", "{0:X2},", unknown_2); // data, indefinite length
                SetStringToListBox("stroke packets({0}): ", "{0:X2},", stroke_packets); // data, indefinite length
                SetStringToListBox("unknown 3({0}): ", "{0:X2},", unknown_3);
                SetStringToListBox("unknown 4({0}): ", "{0:X2},", unknown_4);
                SetStringToListBox("unknown 5({0}): ", "{0:X2},", unknown_5); // strings and data, indefinite length, EOF

                buff = string.Format("StrokePart({0}): ", strokeList.Count);
                foreach (int data in strokeList)
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
                        size = buffBytes.Length;
                        // Analyze it
                        if (DecodeFSS() < 0)
                        {
                            Windows.UI.Popups.MessageDialog md = new Windows.UI.Popups.MessageDialog(error_message);
                            await md.ShowAsync();
                        }
                        else
                        {
                            ShowFssData();
                        }

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
