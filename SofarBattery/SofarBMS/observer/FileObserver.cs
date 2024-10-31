using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SofarBMS.Helper;
using SofarBMS.Model;

namespace SofarBMS.Observer
{
    public class FileObserver : RealtimeDataObserver
    {
        public override void SaveData(RealtimeData_BTS5K model)
        {
            try
            {
                var filePath = $"{System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase}//Log//{DateTime.Now.ToString("yyyy-MM-dd")}.csv";

                //用于确定指定文件是否存在
                if (!File.Exists(filePath))
                {
                    //打开一个文件，向其中追加指定的字符串，然后关闭该文件。
                    //如果文件不存在，此方法将创建一个文件，将指定的字符串写入文件，然后关闭该文件。
                    File.AppendAllText(filePath, model.GetHeader() + "\r\n");
                }

                File.AppendAllText(filePath, model.GetValue() + "\r\n");
            }
            catch (Exception)
            {

            }
        }
    }
}
