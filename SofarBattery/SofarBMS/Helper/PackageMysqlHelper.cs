using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Helper
{
    /// <summary>
    /// 包装
    /// </summary>
    public class PackageMysqlHelper
    {
        private static string connectionStrings = ConfigurationManager.AppSettings["ConnectionSQLString"]?? "server=172.30.16.147;database=sh_db;uid=zhaokai;pwd=zhaokai;";

        static MySqlConnection conn = new MySqlConnection(connectionStrings);

        public static bool IsConnectioned { get; set; } = false;

        /*private static bool isConnectioned;

        public static bool IsConnectioned { get { return isConnectioned; } }

        static PackageMysqlHelper()
        {
            InitConn();
        }*/
        public PackageMysqlHelper()
        {
            InitConn();
        }

        private void InitConn()
        {
#pragma warning disable CS0168 // 声明了变量“ex”，但从未使用过
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                else if (conn.State == ConnectionState.Broken)
                {
                    conn.Close();
                    conn.Open();
                }
                IsConnectioned = true;
            }
            catch (Exception ex)
            {
                IsConnectioned = false;
                //throw;
            }
#pragma warning restore CS0168 // 声明了变量“ex”，但从未使用过

        }

        //增删改
        public bool ExecuteNonQuery(string sqlStr)
        {
            if (IsConnectioned)
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                int result = cmd.ExecuteNonQuery();

                return result > 0;
            }
            return false;

        }

        //执行集合函数
        public object ExecuteScalar(string sqlStr)
        {
            if (IsConnectioned)
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                object result = cmd.ExecuteScalar();

                return result;
            }
            return false;
        }

        //查询，获取DataTable
        public DataTable GetTable(string sqlStr)
        {
            if (IsConnectioned)
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr);
                cmd.Connection = conn;
                MySqlDataAdapter da = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                return dt;
            }
            return new DataTable();
        }

        //二进制数据增删改
        public bool Binary(string sqlStr, Dictionary<string, byte[]> parameters)
        {
            if (IsConnectioned)
            {
                MySqlCommand cmd = new MySqlCommand(sqlStr, conn);
                int i = 0;
                foreach (KeyValuePair<string, byte[]> kvp in parameters)
                {
                    cmd.Parameters.Add("@" + kvp.Key, MySqlDbType.LongBlob);
                    cmd.Parameters[i].Value = kvp.Value;
                    i++;
                }
                int result = cmd.ExecuteNonQuery();

                return result > 0;
            }
            return false;
        }
    }
}
