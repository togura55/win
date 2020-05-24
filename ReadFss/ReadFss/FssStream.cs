using System;
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
        public string description;
        public Boolean isString;
    }

    public class ByteStreamIDs
    {
        public readonly byte[] byteID = { 0x30, 0x30, 0x2F, 0x17, 0x16, 0x54, 0x05, 0x07, 0x09, 0x0A,
        0x03, 0x06, 0x08, 0x04, 0x0D, 0x0C, 0x4F, 0x14, 0x50, 0x1D, 0x1C,
        0x1A, 0x1B, 0x18, 0x15};
        public readonly Boolean[] isString =
        {
            false,  // 0x30
            false,  // 0x30
            true,   // 0x2F
            true,   // 0x17
            true,   // 0x16
            true,  // 0x54
            false,  // 0x05
            false,  // 0x07
            false,  // 0x09
            false,  // 0x0A
            false,  // 0x03
            false,  // 0x06
            false,  // 0x08
            false,  // 0x04
            false,  // 0x0D
            false,  // 0x0C
            false,  // 0x4F, STU
            false,  // 0x14
            false,  // 0x50, DTU
            false,  // 0x1D
            true,  // 0x1C
            true,  // 0x1A
            true,  // 0x1B
            false,  // 0x18
            false   // 0x15, STU
        };
        public readonly string[] description = {
            "ID List",  // 0x30
            "unknown",  // 0x30
            "SigObj.ExtraData",     // 0x2F
            "Signatory name",       // 0x17, DC.Capture_Who
            "Reason for signing",   // 0x16, DC.Capture_Why
            "Lic. UID", // 0x54, License key ID in jwt token
            "unknown",  // 0x05
            "unknown",  // 0x07
            "unknown",  // 0x09
            "unknown",  // 0x0A
            "unknown",  // 0x03
            "unknown",  // 0x06
            "unknown",  // 0x08
            "unknown",  // 0x04
            "unknown",  // 0x0D
            "unknown",  // 0x0C
            "unknown_STU",  // 0x4F, STU
            "unknown",  // 0x14
            "unknown_DTU", // 0x50, DTU
            "Network Interface Card",   // 0x1D
            "Operating system", // 0x1C
            "Digitizer type",   // 0x1A
            "Digitizer driver", // 0x1B
            "unknown",  // 0x18
            "unknown_STU"   // 0x15, STU
        };

        public ByteStreamIDs()
        {

        }
    }

    class FssStream
    {
        public byte[] byteStream = null;

        public int size = 0;
        public byte[] first_label = new byte[2];
        public byte[] first_data = new byte[2];
        public byte[] second_label = new byte[4];


        public byte[] key_19 = { 0x19 };

        public List<ByteStreamPart> byteStreamParts = null;

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

        public void Timestamp()
        {
            // 保存されているタイムスタンプがUnix timestampでHex 4byteとすると
            // 4F
            byte[] ts_array = new byte[8];
            foreach (ByteStreamPart bsp in byteStreamParts)
            {
                if (bsp.byteID == 0x4F)
                {
                    for (int i = 0; i<4; i++)
                    {
                        ts_array[i] = bsp.rawDataBytes[i+1];
                    }
 //                   Array.Copy(bsp.rawDataBytes, 1, ts_array, 0, ts_array.Length - 1);
                    break;
                }
            }
            ulong unixTime = BitConverter.ToUInt64(ts_array, 0);
            var dateTime = FromUnixTime((long)unixTime);

//            DateTime dt = DateTime.Now;

//            var timespan = DateTime.UtcNow;
        }
        private static DateTime FromUnixTime(long unixTime)
        {
            return DateTimeOffset.FromUnixTimeSeconds(unixTime).LocalDateTime;
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

                ByteStreamIDs id = new ByteStreamIDs();
                foreach (ByteStreamPart bsp in byteStreamParts)
                {
                    for (int i = 0; i<id.byteID.Length; i++)
                    {
                        if (bsp.byteID == id.byteID[i])
                        {
                            bsp.description = id.description[i];
                            bsp.isString = id.isString[i];
                            break;
                        }
                    }
                }

                current++;
 
            }

            catch (Exception ex)
            {
                throw new Exception(string.Format("Exception: Decode: {0}", ex.Message));
            }

            Timestamp();
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
    }
}
