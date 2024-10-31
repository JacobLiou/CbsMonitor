using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace SofarBMS
{
    /// <summary>
    /// CSV文件特点: 
    /// 1. 遇到内容包含双引号会替换成两个双引号 
    /// 2. 内容包含逗号或双引号或\r或\n会在外面包裹双引号
    /// 该工具类目前保证内容的英文逗号可以被正确替换为中文逗号，但是双引号会被清除掉
    /// </summary>
    public class CsvFileHelper
    {

        public static void SaveCsv<T>(IEnumerable<T> modelList, string fullPath)
        {
            SaveCsv(ModelHelper.ToTable<T>(modelList), fullPath);
        }

        private static string SolveCSVColumn(string column)
        {
            column = column.Replace("\"", "\"\""); //替换英文冒号 英文冒号需要换成两个冒号
            if (column.Contains(',') || column.Contains('"')
                                     || column.Contains('\r') || column.Contains('\n')) //含逗号 冒号 换行符的需要放到引号中
            {
                column = $"\"{column}\"";
            }

            return column;
        }

        private static void Assert(bool result, string msg)
        {
            if (!result)
            {
                throw new ArgumentException(msg);
            }
        }

        /// <summary>
        /// 将DataTable中数据写入到CSV文件中
        /// </summary>
        /// <param name="dt">提供保存数据的DataTable</param>
        /// <param name="fullPath">全路径</param>
        /// <param name="encodeName">编码类型名称</param>
        public static DataTable SaveCsv(DataTable dt, string fullPath, string encodeName = "utf-8")
        {
            Assert(fullPath.EndsWith(".csv"), "目标文件路径无效!");

            FileInfo fi = new FileInfo(fullPath);
            if (fi.Directory != null && !fi.Directory.Exists)
            {
                fi.Directory.Create();
            }

            using (FileStream fs = new FileStream(fullPath, FileMode.Create, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding(encodeName)))
            {
                string data = "";
                //写出列名称
                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    data += SolveCSVColumn(dt.Columns[i].ColumnName);
                    if (i < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }

                sw.WriteLine(data);
                //写出各行数据
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    data = "";
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        string str = SolveCSVColumn(dt.Rows[i][j].ToString());

                        data += str;
                        if (j < dt.Columns.Count - 1)
                        {
                            data += ",";
                        }
                    }

                    sw.WriteLine(data);
                }
            }

            return dt;
        }

        // csv文件遇到英文单引号默认会添加双引号转义
        private static string ReplaceEnSingleQuotation(string str)
        {
            char[] chars = str.ToArray();
            str = string.Empty;

            bool changeChar = false;
            for (int i = 0; i < chars.Length; i++)
            {
                if (chars[i] == '\"')
                {
                    changeChar = !changeChar;
                }

                if (changeChar && chars[i] == ',')
                {
                    chars[i] = '，';
                }

                str += chars[i];
            }

            str = str.Replace("\"", "");

            return str;
        }

        /// <summary>
        /// 将CSV文件的数据读取到DataTable中
        /// 为避免数据出错，会默认将读取的英文逗号转为中文逗号
        /// </summary>
        /// <returns>返回读取了CSV数据的DataTable</returns>
        public static DataTable OpenCsv(string filePath)
        {
            if (Path.GetExtension(filePath).Length == 0)
            {
                throw new InvalidDataException("非法文件路径");
            }

            Encoding encoding = GetType(filePath);
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            dt.TableName = fs.Name;

            StreamReader sr = new StreamReader(fs, encoding);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine = null;
            string[] tableHead = null;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool isFirst = true;
            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                // 偶数个 " 替换英文 ',' 为'，'
                if (strLine.Contains("\"") && strLine.Split('\"').Length % 2 == 1)
                {
                    strLine = ReplaceEnSingleQuotation(strLine);
                }

                if (isFirst)
                {
                    tableHead = strLine.Split(',');
                    isFirst = false;
                    columnCount = tableHead.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(tableHead[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                    aryLine = strLine.Split(',');
                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < columnCount; j++)
                    {
                        dr[j] = aryLine[j];
                    }

                    dt.Rows.Add(dr);
                }
            }

            if (aryLine != null && aryLine.Length > 0)
            {
                dt.DefaultView.Sort = tableHead[0] + " " + "asc";
            }

            sr.Close();
            fs.Close();
            return dt;
        }


        /// 给定文件的路径，读取文件的二进制数据，判断文件的编码类型
        /// <param name="fileName">文件路径</param>
        /// <returns>文件的编码类型</returns>
        private static Encoding GetType(string fileName)
        {
            FileStream fs = new FileStream(fileName, FileMode.Open,
                FileAccess.Read);
            Encoding r = GetType(fs);
            fs.Close();
            return r;
        }

        /// 通过给定的文件流，判断文件的编码类型
        /// <param name="fs">文件流</param>
        /// <returns>文件的编码类型</returns>
        private static Encoding GetType(Stream fs)
        {
            byte[] unicode = new byte[] {0xFF, 0xFE, 0x41};
            byte[] unicodeBig = new byte[] {0xFE, 0xFF, 0x00};
            byte[] utf8 = new byte[] {0xEF, 0xBB, 0xBF}; //带BOM
            Encoding reVal = Encoding.Default;

            BinaryReader r = new BinaryReader(fs, Encoding.Default);
            int i;
            int.TryParse(fs.Length.ToString(), out i);
            byte[] ss = r.ReadBytes(i);
            if (IsUtf8Bytes(ss) || (ss[0] == 0xEF && ss[1] == 0xBB && ss[2] == 0xBF))
            {
                reVal = Encoding.UTF8;
            }
            else if (ss[0] == 0xFE && ss[1] == 0xFF && ss[2] == 0x00)
            {
                reVal = Encoding.BigEndianUnicode;
            }
            else if (ss[0] == 0xFF && ss[1] == 0xFE && ss[2] == 0x41)
            {
                reVal = Encoding.Unicode;
            }

            r.Close();
            return reVal;
        }

        private static Encoding GetEncoding(Stream stream)
        {
            if (!stream.CanSeek)
                throw new ArgumentException("require a stream that can seek!");

            Encoding encoding = Encoding.ASCII;
            byte[] bom = new byte[5];
            int nRead = stream.Read(bom, offset: 0, count: 5);

            if (bom[0] == 0xff && bom[1] == 0xfe && bom[2] == 0 && bom[3] == 0)
            {
                stream.Seek(4, SeekOrigin.Begin);
                return Encoding.UTF32;
            }
            else if (bom[0] == 0xff && bom[1] == 0xfe)
            {
                stream.Seek(2, SeekOrigin.Begin);
                return Encoding.Unicode;
            }
            else if (bom[0] == 0xfe && bom[1] == 0xff)
            {
                stream.Seek(2, SeekOrigin.Begin);
                return Encoding.BigEndianUnicode;
            }
            else if (bom[0] == 0xef && bom[1] == 0xbb && bom[2] == 0xbf)
            {
                stream.Seek(3, SeekOrigin.Begin);
                return Encoding.UTF8;
            }

            stream.Seek(0, SeekOrigin.Begin);
            return encoding;
        }

        /// 判断是否是不带 BOM 的 UTF8 格式
        /// <param name="data"></param>
        /// <returns></returns>
        private static bool IsUtf8Bytes(byte[] data)
        {
            int charByteCounter = 1; //计算当前正分析的字符应还有的字节数
            foreach (var t in data)
            {
                var curByte = t; //当前分析的字节.
                if (charByteCounter == 1)
                {
                    if (curByte >= 0x80)
                    {
                        //判断当前
                        while (((curByte <<= 1) & 0x80) != 0)
                        {
                            charByteCounter++;
                        }

                        //标记位首位若为非0 则至少以2个1开始 如:110XXXXX...........1111110X　
                        if (charByteCounter == 1 || charByteCounter > 6)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    //若是UTF-8 此时第一位必须为1
                    if ((curByte & 0xC0) != 0x80)
                    {
                        return false;
                    }

                    charByteCounter--;
                }
            }

            if (charByteCounter > 1)
            {
                 //throw new Exception("非预期的byte格式");
            }

            return true;
        }
    }
}