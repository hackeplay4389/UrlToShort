using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SQLite;

namespace UrlToShort
{
    /// <summary>
    /// 数据库辅助类
    /// </summary>
    public class DBHelper
    {
        private string SqlConn = "data source=" + AppDomain.CurrentDomain.BaseDirectory + "/App_Data/Data.db";

        private SQLiteConnection conn = null;

        /// <summary>
        /// 初始化数据库链接
        /// </summary>
        private void Open()
        {
            if (conn == null)
            {
                conn = new SQLiteConnection(SqlConn);
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            if (conn.State == ConnectionState.Broken)
            {
                conn.Close();
                conn.Open();
            }
        }

        /// <summary>
        /// 关闭数据库链接
        /// </summary>
        private void Close()
        {
            if (conn.State == ConnectionState.Open)
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 获取首行首列
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string GetFirst(string sql)
        {
            try
            {
                Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                object obj=cmd.ExecuteScalar();
                return obj==null?"":obj.ToString();
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 获取受影响行数
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public int GetLine(string sql)
        {
            try
            {
                Open();
                SQLiteCommand cmd = new SQLiteCommand(sql, conn);
                return cmd.ExecuteNonQuery();
            }
            finally
            {
                Close();
            }
        }

        /// <summary>
        /// 获取数据表
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable GetDT(string sql)
        {
            DataTable data = new DataTable();
            SQLiteDataAdapter sda = new SQLiteDataAdapter(sql, SqlConn);
            sda.Fill(data);
            return data;
        }

    }
}