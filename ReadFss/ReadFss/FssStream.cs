﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReadFss
{
    //    public struct ByteStreamPart
    public class ByteStreamPart
    {
        public byte byteID;
        public byte numBytes;
        public byte[] rawDataBytes;
    }

    class FssStream
    {
        public byte[] byteStream = null;

        public int size = 0;
        public byte[] first_label = new byte[2];
        public byte[] first_data = new byte[2];
        public byte[] second_label = new byte[4];

        //public byte[] second_label = new byte[32];
        //public byte[] second_data = new byte[16];
        //public byte[] unknown_1 = new byte[4];
        //public byte[] extraData_Key = null; // strings, indefinite length
        //public byte[] extraData_Sep = new byte[1]; // separater
        //public byte[] extraData_Value = null;  // strings, indefinite length
        //public byte[] captureWho_Sep = new byte[3]; // separater
        //public byte[] captureWho = null;// strings,  indefinite length
        //public byte[] captureWhy_Sep = new byte[3]; // separater
        //public byte[] captureWhy = null;// strings,  indefinite length
        //public byte[] unknown_2 = null; // data, indefinite length
        //public byte[] stroke_packets = null; // data, indefinite length
        //public byte[] unknown_3 = new byte[13];
        //public byte[] unknown_4 = new byte[4];
        //public byte[] unknown_5 = null; // strings and data, indefinite length, EOF

        public byte[] key_19 = { 0x19 };

        public ByteStreamPart key_30;

        public List<ByteStreamPart> byteStreamParts = null;



        //        public byte[] key_30 = { 0x30, 0 };      // 2bytes

        public byte[] key_2F = { 0x2F, 0, 0, 0 };
        public byte[] key_17 = { 0x17, 0, 0 };  // who separater
        public byte[] key_16 = { 0x16, 0, 0 };  // why separater
        public byte[] key_54 = { 0x54, 0, 0 };  // 5-parts-digit

        public byte[] key_05 = { 0x05, 0 };      // 2bytes
        public byte[] key_07 = { 0x07, 0 };      // 2bytes
        public byte[] key_09 = { 0x09, 0 };      // 2bytes

        public byte[] key_0A = { 0x0A, 0 };      // 2bytes
        public byte[] key_03 = { 0x03, 0 };      // 2bytes
        public byte[] key_06 = { 0x06, 0 };      // 2bytes
        public byte[] key_08 = { 0x08, 0 };      // 2bytes
        public byte[] key_04 = { 0x04, 0 };      // 2bytes
        public byte[] key_0D = { 0x0D, 0 };      // 2bytes

        public byte[] key_0C = { 0x0C, 0 };     // 2bytes
        public byte[] key_4F = { 0x4F, 0, 0 };  // 3bytes
        public byte[] key_14 = { 0x14, 0 };      // 2bytes
        public byte[] key_1D = { 0x1A, 0, 0 };  // 3bytes
        public byte[] key_1C = { 0x1C, 0, 0 };  // 3bytes
        public byte[] key_1A = { 0x1A, 0, 0 };  // 3bytes
        public byte[] key_1B = { 0x1B, 0, 0 };  // 3bytes
        public byte[] key_18 = { 0x18, 0 };      // 2bytes
        public byte[] key_15 = { 0x15, 0 };      // 2bytes



        public List<UInt16> strokeList = null;

        public FssStream()
        {
            strokeList = new List<UInt16>();
            byteStreamParts = new List<ByteStreamPart>();

            //           key_30.byteID = 0x30;
        }

        public void Read()
        {

        }

        public void Write()
        {

        }

        public void Decode()
        {
            //            int index = 0;
            int current = 0;

            try
            {
                Array.Copy(byteStream, current, first_label, 0, first_label.Length);
                current += first_label.Length;
                Array.Copy(byteStream, current, first_data, 0, first_data.Length);
                current += first_data.Length;
                Array.Copy(byteStream, current, second_label, 0, second_label.Length);
                current += second_label.Length;

                while (current < size)
                {
                    byteStreamParts.Add(new ByteStreamPart());
                    ByteStreamPart bsp = byteStreamParts[byteStreamParts.Count - 1];
                    bsp.byteID = byteStream[current];

                    current++;  // next
                    bsp.numBytes = byteStream[current];
                    current++;  // next
                    bsp.rawDataBytes = new byte[bsp.numBytes];
                    Array.Copy(byteStream, current, bsp.rawDataBytes, 0, bsp.numBytes);
                    current += bsp.numBytes;
                }
                /*
                                //Array.Copy(byteStream, 36, second_data, 0, 16);
                                //Array.Copy(byteStream, 52, unknown_1, 0, 4);

                                extraData_Sep[0] = (byte)0x19;

                                index = Array.IndexOf(byteStream, (byte)0x19, 56);
                                if (index > 0)
                                {
                                    extraData_Key = new byte[index - 56];
                                    Array.Copy(byteStream, 56, extraData_Key, 0, index - 56);
                                    current = index;
                                }
                                else
                                {// error
                                    throw new Exception(string.Format("Cannot find 0x{0:x}", 0x19));
                                }

                                index = Array.IndexOf(byteStream, (byte)0x17, current);
                                if (index > 0)
                                {
                                    extraData_Value = new byte[index - current];
                                    Array.Copy(byteStream, current, extraData_Value, 0, index - current);
                                    current = index;
                                }
                                else
                                {// error
                                    throw new Exception(string.Format("Cannot find 0x{0:x}", 0x17));
                                }

                                Array.Copy(byteStream, current, captureWho_Sep, 0, captureWho_Sep.Length);
                                current = current + captureWho_Sep.Length;

                                captureWho = new byte[captureWho_Sep.Last()];
                                Array.Copy(byteStream, current, captureWho, 0, captureWho_Sep.Last());
                                current = current + captureWho.Length;

                                Array.Copy(byteStream, current, captureWhy_Sep, 0, captureWhy_Sep.Length);
                                current = current + captureWhy_Sep.Length;

                                captureWhy = new byte[captureWhy_Sep.Last()];
                                Array.Copy(byteStream, current, captureWhy, 0, captureWhy_Sep.Last());
                                current = current + captureWhy.Length;

                                // 最後が0A 09 02を探す
                                byte[] searchBytes = { 0x0A, 0x09, 0x02 };
                                index = SearchElementBlock(byteStream, searchBytes, current);
                                if (index > 0)
                                {
                                    unknown_2 = new byte[index + searchBytes.Length - current];
                                    Array.Copy(byteStream, current, unknown_2, 0, index + searchBytes.Length - current);
                                    current = index + searchBytes.Length;
                                }
                                else
                                {// error
                                    string elements = string.Empty;
                                    foreach (int b in searchBytes)
                                        elements += string.Format("0x{0:X},", b);
                                    throw new Exception(string.Format("Cannot find {0}", elements));
                                }

                                // 最初が0C 08 01 を探す
                                byte[] searchBytes2 = { 0x0C, 0x08, 0x01 };
                                index = SearchElementBlock(byteStream, searchBytes2, current);
                                if (index > 0)
                                {
                                    stroke_packets = new byte[index - current];
                                    Array.Copy(byteStream, current, stroke_packets, 0, index - current);
                                    current = index;
                                }
                                else
                                {// error
                                    string elements = string.Empty;
                                    foreach (int b in searchBytes2)
                                        elements += string.Format("0x{0:X},", b);
                                    throw new Exception(string.Format("Cannot find {0}", elements));
                                }

                                // 13バイト読み込む
                                Array.Copy(byteStream, current, unknown_3, 0, unknown_3.Length);
                                current = current + unknown_3.Length;

                                // 4バイト読み込む
                                Array.Copy(byteStream, current, unknown_4, 0, unknown_4.Length);
                                current = current + unknown_4.Length;

                                // 最後まで読み込む
                                unknown_5 = new byte[size - current];
                                Array.Copy(byteStream, current, unknown_5, 0, unknown_5.Length);
                                current = current + unknown_5.Length;

                                DecodeStrokePart();
                 //               DecodeStrokePart2();
                 */
            }

            catch (Exception ex)
            {
                throw new Exception(string.Format("Exception: Decode: {0}", ex.Message));
            }
        }

        public void Encode()
        {

        }

        private int SearchElementBlock(byte[] targetBytes, byte[] searchBytes, int start = 0)
        {
            int pos = -1;
            byte[] buff = new byte[targetBytes.Length - start];
            Array.Copy(targetBytes, start, buff, 0, targetBytes.Length - start);
            List<int> positions = SearchBytePattern(searchBytes, buff);
            foreach (var item in positions)
            {
                //               string str = string.Format("Pattern matched at pos {0}", item + start);
                pos = item + start;
            }
            return pos;
        }

        static private List<int> SearchBytePattern(byte[] pattern, byte[] bytes)
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
        /*
                private void DecodeStrokePart()
                {
                    if (stroke_packets != null)
                    {
                        int unit = 2;
                        int num = stroke_packets.Length / unit;

                        for (int i = 0; i < num; i++)
                        {
                            strokeList.Add(BitConverter.ToUInt16(stroke_packets, i * unit));
                        }
                    }
                }

                private void DecodeStrokePart2()
                {
                    if (stroke_packets != null)
                    {
                        int unit = 2;
                        int num = stroke_packets.Length / unit;

                        for (int i = 0; i < num; i++)
                        {
                            //                    strokeList.Add(BitConverter.ToUInt16(stroke_packets, i * unit));
                            strokeList.Add((ushort)(stroke_packets[i*unit] << 8 | stroke_packets[i*unit + 1]));
                        }
                    }
                }
                */
    }
}
