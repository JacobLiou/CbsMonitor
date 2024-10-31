using System;

namespace SofarBMS.Helper
{
    public enum Crc16Model : ushort { Standard = 0xA001, CcittKermit = 0x8408 }

    public class Crc16
    {
        static ushort[] table = new ushort[256];

        public ushort ComputeChecksum(params byte[] bytes)
        {
            ushort crc = 0;
            for (int i = 0; i < bytes.Length; ++i)
            {
                byte index = (byte)(crc ^ bytes[i]);
                crc = (ushort)((crc >> 8) ^ table[index]);
            }
            return crc;
        }

        public byte[] ComputeChecksumBytes(params byte[] bytes)
        {
            ushort crc = ComputeChecksum(bytes);
            return BitConverter.GetBytes(crc);
        }

        public static String getCRC16_CCITT(byte[] bytes)
        {
            int crc = 0xFFFF;          // initial value
            int polynomial = 0x1021;   // 0001 0000 0010 0001  (0, 5, 12) 
            foreach (byte b in bytes)
            {
                for (int i = 0; i < 8; i++)
                {
                    bool bit = ((b >> (7 - i) & 1) == 1);
                    bool c15 = ((crc >> 15 & 1) == 1);
                    crc <<= 1;
                    if (c15 ^ bit) crc ^= polynomial;
                }
            }
            crc &= 0xffff;

            string strDest = crc.ToString("X");
            return strDest;
        }

        public static byte getCRC8(byte[] buffer)
        {
            byte crc = 0xFF;// Initial value
            for (int i = 0; i < buffer.Length; i++)
            {
                crc ^= buffer[i];
                for (int j = 0; j < 8; j++)
                {
                    if ((crc & 1) > 0)
                        crc = (byte)((crc >> 1) ^ 0xE0);// 0xE0 = reverse 0x07
                    else
                        crc = (byte)(crc >> 1);
                }
            }
            return crc;
        }


        public Crc16(Crc16Model model)
        {
            ushort polynomial = (ushort)model;
            ushort value;
            ushort temp;
            for (ushort i = 0; i < table.Length; ++i)
            {
                value = 0;
                temp = i;
                for (byte j = 0; j < 8; ++j)
                {
                    if (((value ^ temp) & 0x0001) != 0)
                    {
                        value = (ushort)((value >> 1) ^ polynomial);
                    }
                    else
                    {
                        value >>= 1;
                    }
                    temp >>= 1;
                }
                table[i] = value;
            }
        }
    }
}
