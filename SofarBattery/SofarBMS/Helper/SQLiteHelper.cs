using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SofarBMS.Helper
{
    public static class SQLiteHelper
    {
        public static string ConStr = "";
        /// <summary>
        /// 执行增删改
        /// </summary>
        /// <param name="sql"><
        /// ram>
        /// <returns></returns>
        public static int Update(string sql)
        {
            SQLiteConnection DBConnection = new SQLiteConnection(ConStr);
            SQLiteCommand cmd = new SQLiteCommand(sql, DBConnection);
            try
            {
                DBConnection.Open();
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                DBConnection.Close();
            }
        }
        /// <summary>
        /// 获取单一结果
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static object GetSingleResult(string sql)
        {
            SQLiteConnection DBConnection = new SQLiteConnection(ConStr);
            SQLiteCommand cmd = new SQLiteCommand(sql, DBConnection);
            try
            {
                DBConnection.Open();
                return cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                DBConnection.Close();
            }
        }
        /// <summary>
        /// 返回结果集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static SQLiteDataReader GetReader(string sql)
        {
            SQLiteConnection DBConnection = new SQLiteConnection(ConStr);
            SQLiteCommand cmd = new SQLiteCommand(sql, DBConnection);
            try
            {
                DBConnection.Open();
                return cmd.ExecuteReader(CommandBehavior.CloseConnection);
            }
            catch (Exception ex)
            {
                DBConnection.Close();
                throw ex;
            }


        }
        /// <summary>
        /// 返回数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static DataSet GetDataSet(string sql)
        {
            SQLiteConnection DBConnection = new SQLiteConnection(ConStr);
            SQLiteCommand cmd = new SQLiteCommand(sql, DBConnection);
            SQLiteDataAdapter da = new SQLiteDataAdapter(cmd);
            DataSet ds = new DataSet();
            try
            {
                DBConnection.Open();
                da.Fill(ds);
                return ds;
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                DBConnection.Close();
            }
        }
    }
}
