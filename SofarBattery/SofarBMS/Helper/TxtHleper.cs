using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Helper
{
    public class TxtHleper
    {
        public static void FileWrite(string path, string content)
        {
            if (!File.Exists(path))
            {
                using (StreamWriter sw = new StreamWriter(path, true, Encoding.UTF8))
                {
                    sw.Write(content);
                }
            }
            else
            {
                FileStream fs = new FileStream(path, FileMode.Append);
                StreamWriter sw1 = new StreamWriter(fs, Encoding.UTF8);
                sw1.Write(content);
                sw1.Close();
            }
        }
    }
}
