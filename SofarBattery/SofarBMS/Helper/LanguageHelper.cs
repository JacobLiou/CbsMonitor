using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Helper
{
    /// <summary>
    /// 语言翻译类
    /// </summary>
    public class LanguageHelper
    {
        
        /// <summary>
        /// 当前语言：1中文 2英文
        /// </summary>
        private static int languageIndex = 1;
        public static int LanaguageIndex
        {
            get { return languageIndex; }
            set { languageIndex = value; }
        }

        private static DataTable languageTable = SQLiteHelper.GetDataSet("select * from NationalLanguage").Tables[0];
        private static DataTable LanguageTable
        {
            get
            {
                return languageTable;
            }
        }

        /// <summary>
        /// 获取翻译名称
        /// </summary>
        /// <param name="key">传递的TitleName</param>
        /// <param name="index">1:中文，2:英文</param>
        /// <returns></returns>
        public static string GetLanguage(string key)
        {
            string result = String.Empty;

            for (int i = 0; i < LanguageTable.Rows.Count; i++)
            {
                if (key == LanguageTable.Rows[i][1].ToString())
                {
                    if (LanaguageIndex == 1)
                    {
                        result = LanguageTable.Rows[i][2].ToString();
                    }
                    else if (LanaguageIndex == 2)
                    {
                        result = LanguageTable.Rows[i][3].ToString();
                    }

                    string unit = LanguageTable.Rows[i][4].ToString();

                    if (unit != "")
                    {
                        result += $"({unit})";
                    }

                    break;
                }
            }
            return result;
        }
       
    }
}
