using SofarBMS.model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SofarBMS.ui
{
    public partial class DownloadControl : UserControl
    {
        int flag = 0;
        int file_size = 0;
        int groupIndex = 0;

        FileStream fs = null;
        public static CancellationTokenSource cts = null;
        Crc16 _crc = new Crc16(Crc16Model.CcittKermit);

        public DownloadControl()
        {
            InitializeComponent();

            FrmMain.CANID = 0x1f;
            cts = new CancellationTokenSource();
        }

        private void DownloadControl_Load(object sender, EventArgs e)
        {
            listBox1.TopIndex = listBox1.Items.Count - 1;
            cbbChip_role.SelectedIndex = 3;

            #region 接收异步线程
            Task.Run(() =>
            {
                while (!cts.IsCancellationRequested)
                {
                    if (EcanHelper.IsConnection)
                    {
                        lock (EcanHelper._locker)
                        {
                            while (EcanHelper._task.Count > 0)
                            {
                                //出队
                                CAN_OBJ ch = (CAN_OBJ)EcanHelper._task.Dequeue();

                                //解析
                                this.Invoke(new Action(() => { analysisData(ch.ID, ch.Data); }));
                            }
                        }
                    }
                }
            });
            #endregion
        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = "C:\\Users\\admin\\Desktop\\Bin\\";
            openFileDialog.Filter = "bin文件|*.bin";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openFileDialog.FileName;
            }
        }

        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            try
            {
                file_size = 0;
                groupIndex = 0;
                listBox1.Items.Clear();

                if (flag == 0)
                {
                    btnUpgrade.Text = "取消升级";

                    flag = 1;

                    fs = new FileStream(textBox1.Text, FileMode.Open, FileAccess.Read, FileShare.Read);

                    file_size = (Convert.ToInt32(fs.Length / 1024) + Convert.ToInt32((file_size % 1024) != 0 ? 1 : 0) - 1);

                    startDownloadFlag1(Convert.ToByte(cbbChip_role.SelectedIndex), txtChip_code.Text, file_size, 1024);
                }
                else
                {
                    btnUpgrade.Text = "确认升级";

                    if (fs != null) fs.Close();

                    flag = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("启动升级出现异常，错误消息：" + ex.Message);

                if (fs != null)
                    fs.Close();

                flag = 0;
            }
        }

        private void startDownloadFlag1(byte chip_role, string chip_code, int file_size, int data_size)
        {
            int i = 0;
            byte[] data = new byte[8];
            try
            {
                byte[] id = new byte[] { 0x41, 0x3F, 0xFB, 0x07 };// 0x41,FrmMain.CANID

                /* ===下发0x7FB===
                 * 帧类型 1B 0:发送请求帧
                 * 芯片角色 1B 0:ARM 1:DSP_M 2:DSP_S 3:BMS
                 * 芯片型号编码 2B 芯片编码
                 * 文件数据块总数 2B 文件数据块总数=文件大小/数据块的大小+1
                 * 数据块的大小 2B 默认1025Byte
                 */
                data[i++] = 0x00;
                data[i++] = chip_role;
                data[i++] = asciiToByte(chip_code)[1];
                data[i++] = asciiToByte(chip_code)[0];
                data[i++] = Convert.ToByte(file_size & 0xff);
                data[i++] = Convert.ToByte(file_size >> 8);
                data[i++] = Convert.ToByte(data_size & 0xff);
                data[i++] = Convert.ToByte(data_size >> 8);

                EcanHelper.Send(data, id);
                DebugPrint(id, data);
            }
            catch (Exception ex)
            {
                throw new Exception("升级文件传输开始帧0x7FB，ERROR：" + ex.Message);
            }
        }

        private void startDownloadPack2(int serial_number, int data_size)
        {
            int i = 0;
            byte[] data = new byte[8];
            byte[] bytes = new byte[1024];

            try
            {
                byte[] id = new byte[] { 0x41, 0x3F, 0xFC, 0x07 };

                //序号总CRC计算
                readBinFile(serial_number * 1024, ref bytes);
                byte[] hexCRC = _crc.ComputeChecksumBytes(bytes);

                /* ===下发0x7FC===
                 * 帧类型 1B 0:发送请求帧
                 * 预留 1B 0
                 * 当前的数据块序号 2B 当前传输的数据块的序号(0开始)
                 * 数据块大小 2B 数据块的大小
                 * CRC 2B 数据块校验值
                 */
                data[i++] = 0x00;
                data[i++] = 0x00;
                data[i++] = Convert.ToByte(serial_number & 0xff);
                data[i++] = Convert.ToByte(serial_number >> 8);
                data[i++] = Convert.ToByte(data_size & 0xff);
                data[i++] = Convert.ToByte(data_size >> 8);
                data[i++] = hexCRC[0];
                data[i++] = hexCRC[1];

                EcanHelper.Send(data, id);
                DebugPrint(id, data);
                Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                throw new Exception("升级数据块开始帧0xFC，ERROR：" + ex.Message);
            }
        }

        private void startDownloadData3(long offset)
        {
            byte[] data = new byte[8];
            try
            {
                byte[] id = new byte[] { 0x41, 0x3F, 0xFD, 0x07 };
                /* ===下发0x7FD===
                * 文件数据 8B 文件数据
                */
                readBinFile(offset, ref data);

                EcanHelper.Send(data, id);
                Thread.Sleep(50);
                DebugPrint(id, data);
            }
            catch (Exception ex)
            {
                throw new Exception("升级数据块数据帧0xFD，ERROR：" + ex.Message);
            }
        }

        private void startDownloadCheck4(byte chip_role, string chip_code)
        {
            int i = 0;
            byte[] data = new byte[8];
            try
            {
                byte[] id = new byte[] { 0x41, 0x3F, 0xFE, 0x07 };
                /* ===下发0x7FE===
               * 帧类型 1B 0:发送请求帧
               * 芯片角色 1B 0:ARM 1:DSP_M 2:DSP_S 3:BMS
               * 芯片型号编码 2B 芯片编码
               */
                data[i++] = 0x00;
                data[i++] = chip_role;
                data[i++] = asciiToByte(chip_code)[1];
                data[i++] = asciiToByte(chip_code)[0];

                EcanHelper.Send(data, id);
                DebugPrint(id, data);
            }
            catch (Exception ex)
            {
                throw new Exception("升级文件接收结果查询帧0xFE，ERROR：" + ex.Message);
            }
        }

        private void startDownloadState5(byte chip_role, string chip_code)
        {
            int i = 0;
            byte[] data = new byte[8];
            try
            {
                byte[] id = new byte[] { 0x41, 0x3F, 0xFF, 0x07 };
                /* ===下发0x7FF===
                * 帧类型 1B 0:发送请求帧
                * 芯片角色 1B 0:ARM 1:DSP_M 2:DSP_S 3:BMS
                * 芯片型号编码 2B 芯片编码
                */
                data[i++] = 0x00;
                data[i++] = chip_role;
                data[i++] = asciiToByte(chip_code)[1];
                data[i++] = asciiToByte(chip_code)[0];

                DebugPrint(id, data);
                EcanHelper.Send(data, id);
            }
            catch (Exception ex)
            {
                throw new Exception("升级完成状态查询帧0xFF，ERROR：" + ex.Message);
            }
        }

        private void analysisData(uint id, byte[] data)
        {
            try
            {
                DebugPrint(BitConverter.GetBytes(id), data);

                //ID帧判断
                uint serial = id | 0xff;
                if (!(serial == 0x07fb41ff
                    || serial == 0x07fc41ff
                    || serial == 0x07fd41ff
                    || serial == 0x07fe41ff
                    || serial == 0x07ff41ff))
                    return;

                switch (flag)
                {
                    case 1:
                        /* ===升级文件传输开始帧0x7FB===
                         * 帧类型 1B 1：应答回复
                         * 应答码 1B 1：准备就绪
                         * 预留.. 4B
                         * 接收数据块最大值 2B 默认1024byte
                         */
                        if (data[0] == 0x01 && data[1] == 0x01 && serial == 0x07fb41ff)
                        {
                            //默认从0开始，总序号公式=bin文件长度/1024+1
                            flag = 2;
                            startDownloadPack2(0, 1024);
                        }
                        break;
                    case 2://升级数据块开始帧0x7FC
                        /* ===升级数据块开始帧===
                         * 帧类型 1B 1：应答回复帧
                         * 应答码 1B 1：准备就绪
                         * 预留...6B
                         */
                        if (data[0] == 0x01 && data[1] == 0x01)
                        {
                            long offset = groupIndex * 1024;//开始的位置

                            for (int j = 0; j < 1024; j += 8)
                            {
                                //flag = 3;
                                startDownloadData3(offset + j);
                            }
                        }

                        //进行下一轮
                        if (groupIndex < file_size)
                        {
                            groupIndex++;
                            flag = 2;

                            startDownloadPack2(groupIndex, 1024);
                        }
                        else if (groupIndex == file_size)//0x07fc4101
                        {
                            flag = 4;

                            for (int i = 0; i < 3; i++)
                            {
                                startDownloadCheck4(Convert.ToByte(cbbChip_role.SelectedIndex), txtChip_code.Text);
                            }
                        }
                        break;
                    case 4:
                        /* ===升级文件接收结果查询帧0x7FE===
                         * 帧类型 1B 1：应答回复帧
                         * 应答码 1B 0：接收成功，1：文件校验错误，2：芯片编码错误
                         * 芯片型号编码 2B 芯片编码
                         * 数据块序号组索引 1B 0~255，每组数据块的分为24块
                         * 每组序号的索引值 3B 24bit值，1：接收失败，0：接收成功
                         */
                        if (data[0] == 0x01 && serial == 0x07fe41ff)
                        {
                            if (data[1] == 0x00)
                            {
                                flag = 5;
                                startDownloadState5(Convert.ToByte(cbbChip_role.SelectedIndex), txtChip_code.Text);
                            }
                            else
                            {
                                int bitMask;
                                int groupIndex = data[4];

                                string byteStr = getBitValue(new byte[3] { data[7], data[6], data[5] });
                                char[] strBit = byteStr.ToCharArray();

                                for (int i = 0; i < strBit.Length; i++)
                                {
                                    if (strBit[(strBit.Length - 1) - i] == '1')
                                    {
                                        bitMask = i;

                                        //包ID下发
                                        startDownloadPack2(bitMask, 1024);
                                        //触发重试操作
                                        resendData(groupIndex, bitMask);
                                    }
                                }

                                //升级文件接收结果查询帧（0x7FEE）
                                flag = 4;
                                startDownloadCheck4(Convert.ToByte(cbbChip_role.SelectedIndex), txtChip_code.Text);
                            }
                        }
                        break;
                    case 5:
                        /* ===升级完成状态查询帧===
                         * 帧类型 1B 1：应答回复帧
                         * 状态 1B 0：成功，1：文件校验失败，2：其他原因失败
                         */
                        if (data[0] == 0x01 && serial == 0x07ff41ff)
                        {
                            flag = 0;
                            fs.Close();

                            if (data[1] == 0x00)
                            {
                                listBox1.Items.Add("升级成功！");
                            }
                            else
                            {
                                listBox1.Items.Add("升级失败，错误代码：" + data[1].ToString("X2"));
                            }
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new Exception("解析数据帧异常，错误信息：" + ex.Message);
            }
        }

        private void resendData(int group_index, int bit_Mask)
        {
            try
            {
                byte[] id = new byte[4] { 0x41, FrmMain.CANID, 0xFD, 0x07 };
                /* ===重发该序号的1024byte数据===
                 * 步骤1：获取1024Byte
                 * 步骤2：升级数据块开始帧（0x7FC）
                 * 步骤3：升级数据块数据帧（0x7FD）
                 * 步骤4：升级文件接收结果查询帧（0x7FE）
                 */
                long offset = ((group_index * 24) + bit_Mask) * 1024;
                for (int i = 0; i < 128; i++)
                {
                    startDownloadData3(offset);
                    offset += 8;
                }

                /*byte[] buffer = new byte[1024];

                if (readBinFile(offset, ref buffer))
                {
                    for (int i = 0; i < buffer.Length; i += 8)
                    {
                        byte[] data = new byte[8];

                        Buffer.BlockCopy(buffer, i, data, 0, data.Length);

                        EcanHelper.Send(data, id);
                        DebugPrint(id, data);
                    }
                }*/
            }
            catch (Exception ex)
            {
                throw new Exception("重新验证出现错误，error:" + ex.Message);
            }
        }

        private bool readBinFile(long offset, ref byte[] data)
        {
            try
            {
                fs.Seek(offset, SeekOrigin.Begin);
                fs.Read(data, 0, data.Length);

                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("读取文件错误：" + ex.Message);
            }
        }

        private string getBitValue(byte[] data)
        {
            string str = "";
            for (int i = 0; i < data.Length; i++)
            {
                str += byteToBit(data[i]);
            }
            return str;
        }

        private static String byteToBit(byte b)
        {
            return "" + (byte)((b >> 7) & 0x1) +
                (byte)((b >> 6) & 0x1) +
                (byte)((b >> 5) & 0x1) +
                (byte)((b >> 4) & 0x1) +
                (byte)((b >> 3) & 0x1) +
                (byte)((b >> 2) & 0x1) +
                (byte)((b >> 1) & 0x1) +
                (byte)((b >> 0) & 0x1);
        }

        private static byte[] asciiToByte(string str)
        {
            return System.Text.ASCIIEncoding.Default.GetBytes(str);
        }

        private static byte[] hexStringToByte(string str)
        {
            byte[] bytes = new byte[str.Length / 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                bytes[i] = (byte)Int16.Parse(str.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;
        }

        private void DebugPrint(byte[] id, byte[] sendData)
        {
            //string str = "";
            //for (int i = 0; i < sendData.Length; i++)
            //{
            //    str += " " + sendData[i].ToString("X2");
            //}
            //listBox1.Items.Add(string.Format("ID帧：{0}，数据帧：{1}，发送时间：{2}", BitConverter.ToUInt32(id, 0).ToString("X8"), str, DateTime.Now.ToString("HH:mm:ss:fffff")));

            //listBox1.TopIndex = listBox1.Items.Count - 1;
        }
    }
}
