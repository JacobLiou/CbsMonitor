using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS
{
    public class MysqlHelper
    {
        private static string connectionStrings = ConfigurationManager.AppSettings["ConnectionSQLString"] ?? "server=172.30.16.147;database=sh_db;uid=zhaokai;pwd=zhaokai;";

        static MySqlConnection conn = new MySqlConnection(connectionStrings);

        public static bool IsConnectioned { get; set; } = false;

        public MysqlHelper()
        {
            InitConn();
        }

        private void InitConn()
        {
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
