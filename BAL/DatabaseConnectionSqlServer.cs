using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Text;
using System.Web;


namespace BAL
{
    public static class DatabaseConnectionSqlServer
    {
        private static string connectionString = ConfigurationManager.ConnectionStrings["DatabaseConnetion"].ConnectionString;
        private static string ServerName = ConfigurationManager.AppSettings["ServerName"].ToString();
        private static string ServerUser = ConfigurationManager.AppSettings["ServerUser"].ToString();
        private static string ServerPassword = ConfigurationManager.AppSettings["ServerPassword"].ToString();
        private static String ServerLogFile = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings.Get("ServerLogFile"));

        #region COMMON FUNCTION FOR INSERT,UPDATE AND DELETE OPERATION
        //Added by Jay Tankariya on 16-08-2013

        public enum OPERATIONTYPE
        {
            Insert = 1,
            Update = 2,
            Delete = 3,
            View = 4
        }

        public static string DatabaseConnection()
        {
            try
            {
                string NewConnection = string.Empty;

                if (HttpContext.Current.Request.Cookies["dbName"] != null)
                {
                    if (HttpContext.Current.Request.Cookies["dbName"].Value != "" && HttpContext.Current.Request.Cookies["dbName"].Value != null)
                    {
                        NewConnection = @"Database=" + HttpContext.Current.Request.Cookies["dbName"].Value.ToString() + ";Port=3306;Server=localhost;Uid=root;Pwd=root";
                        return NewConnection;
                    }
                    else
                        return NewConnection = connectionString;
                }
                else
                {
                    return NewConnection = connectionString;
                }
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                return connectionString;
            }
        }

        public static string ExecuteScalarInternal(string Query)
        {
            try
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(connectionString + System.DateTime.Now.ToString());
                sw.Close();

                connectionString = DatabaseConnection();
                Query = Query.Replace(",False", ",0");
                Query = Query.Replace(",True", ",1");
                MySqlConnection conn = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(Query);
                cmd.Connection = conn;
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                string result = "";
                result = Convert.ToString(cmd.ExecuteScalar());
                cmd.Connection.Close();
                return result;
            }
            catch (MySqlException ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                return "";
            }
        }

        public static int ExecuteNonQuery(string SPName, MySqlParameter[] parameters)
        {
            try
            {
                connectionString = DatabaseConnection();
                MySqlConnection conn = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(SPName);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        cmd.Parameters.Add(parameters[i]);
                    }
                }
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                int result = 0;
                result = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return result;
            }
            catch (SqlException ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                throw ex;
            }
        }

        public static int DeleteFromTableByLink(string SPName, string TableName, string FieldName, string FieldValue)
        {
            try
            {
                connectionString = DatabaseConnection();
                MySqlConnection conn = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(SPName);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                cmd.Parameters.AddWithValue("TableName", TableName);
                cmd.Parameters.AddWithValue("FieldName", FieldName);
                cmd.Parameters.AddWithValue("FieldValue", FieldValue);
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                int result = 0;
                result = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return result;
            }
            catch (MySqlException ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                throw ex;
            }
        }

        public static int ExecuteNonQueryInternal(string Query)
        {
            connectionString = DatabaseConnection();
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd = new MySqlCommand(Query);

            try
            {

                Query = Query.Replace(",False", ",0");
                Query = Query.Replace(",True", ",1");
                cmd.Connection = conn;
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                int result = 0;
                result = cmd.ExecuteNonQuery();
                cmd.Connection.Close();
                return result;
            }
            catch (MySqlException ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                throw ex;
            }
            finally
            {
                cmd.Connection.Close();
            }
        }

        public static DataTable GetDataTable(string SPName, MySqlParameter[] parameters)
        {
            try
            {
                connectionString = DatabaseConnection();

                MySqlConnection conn = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(SPName);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        cmd.Parameters.Add(parameters[i]);
                    }
                }
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                adp.Fill(dt);
                cmd.Connection.Close();
                return dt;
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                throw ex;
            }
        }

        public static DataTable GetDataTable(string Query)
        {
            try
            {
                connectionString = DatabaseConnection();
                MySqlConnection conn = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(Query);
                cmd.Connection = conn;
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                adp.Fill(dt);
                cmd.Connection.Close();
                return dt;
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                throw ex;
            }
        }

        public static DataSet GetDataSet(string SPName, MySqlParameter[] parameters)
        {
            try
            {
                connectionString = DatabaseConnection();
                MySqlConnection conn = new MySqlConnection(connectionString);
                MySqlCommand cmd = new MySqlCommand(SPName);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = conn;
                if (parameters != null)
                {
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        cmd.Parameters.Add(parameters[i]);
                    }
                }
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                MySqlDataAdapter adp = new MySqlDataAdapter(cmd);
                DataSet dt = new DataSet();
                if (cmd.Connection.State == ConnectionState.Closed)
                    cmd.Connection.Open();
                adp.Fill(dt);
                cmd.Connection.Close();
                return dt;
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                throw ex;
            }
        }

        public static Boolean ExecuteTransaction(ArrayList q)
        {
            connectionString = DatabaseConnection();
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            if (conn.State != ConnectionState.Open)
            { conn.Open(); }
            MySqlTransaction Tran = conn.BeginTransaction();
            try
            {
                foreach (string query in q)
                {
                    cmd = new MySqlCommand(query, conn, Tran);
                    cmd.ExecuteNonQuery();
                }
                Tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                Tran.Rollback();
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                throw ex;
            }

        }

        public static int CreateDatabase(string DBName, string script)
        {
            try
            {
                connectionString = DatabaseConnection();
                MySqlConnection connection = new MySqlConnection(connectionString);
                MySqlCommand command = new MySqlCommand("CREATE DATABASE  IF NOT EXISTS " + DBName + ";", connection);
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                int result = 0;
                result = command.ExecuteNonQuery();
                connection.Close();

                if (result > 0)
                {
                    string UseDB = "USE " + DBName + " ; ";

                    MySqlConnection connection1 = new MySqlConnection(connectionString);
                    MySqlScript script1 = new MySqlScript(connection, UseDB + script);
                    script1.Delimiter = ";;";
                    script1.Execute();
                }

                return result;

            }
            catch (SqlException ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                throw ex;
            }
        }

        #endregion
    }
}
