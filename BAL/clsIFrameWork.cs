using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public class clsIFrameWork
    {
        private static ArrayList FinalQueries = new ArrayList();

        public static void SetIndependentTable(object obj, List<string> UpdateParameters = null)
        {
            String ConStr = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseConnetion"].ConnectionString;
            MySqlConnection conn = new MySqlConnection(ConStr);
            string database = conn.Database.ToString();

            ArrayList SubQueries = new ArrayList();
            List<string> UpdateParam = null;
            if (UpdateParameters == null)
                UpdateParam = new List<string>();
            else
                UpdateParam = UpdateParameters;
            string Query = "", OperationMode = "", PrimaryKeyColumn = "", TableName = "";
            //Type type = obj.GetType();
            //Type listType = typeof(List<>).MakeGenericType(new[] { type });
            //IList list = (IList)Activator.CreateInstance(listType);
            //list.Add(obj);
            foreach (var item in obj.GetType().GetProperties())
            {
                if (item.Name == "TableName")
                {
                    TableName = item.GetValue(obj, null).ToString();
                    break;
                }
            }

            //MySqlParameter[] parameters = {
            //    new MySqlParameter("TableName",TableName) 
            //};
            DataTable dtPrimary = DatabaseConnectionSqlServer.GetDataTable("SHOW INDEX FROM " + database + ".`" + TableName + "` WHERE `Key_name` = 'PRIMARY'");
            PrimaryKeyColumn = dtPrimary.Rows[0]["Column_name"].ToString();

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.Name == PrimaryKeyColumn)
                {
                    if (Convert.ToInt32(prop.GetValue(obj, null)) == 0)
                    {
                        OperationMode = "Insert";
                    }
                    else
                    {
                        OperationMode = "Update";
                    }
                    break;
                }
            }


            if (OperationMode == "Insert")
            {
                string FieldName = "", FieldValue = "";
                foreach (var prop in obj.GetType().GetProperties())
                {
                    if (prop.Name != PrimaryKeyColumn)
                    {
                        if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(obj.GetType())).Name)
                        {
                            if (prop.PropertyType.Name.Contains("List"))
                            {
                                foreach (string subq in BuildTransactionQuery(prop.GetValue(obj, null), TableName))
                                {
                                    SubQueries.Add(subq);
                                }
                                continue;
                            }
                            FieldName = FieldName + prop.Name + ",";
                            if (prop.PropertyType.Name == "String")
                            {
                                if (Convert.ToString(prop.GetValue(obj, null)).Contains("IDENT_CURRENT"))
                                    FieldValue = FieldValue + Convert.ToString(prop.GetValue(obj, null)) + ",";
                                else
                                    FieldValue = FieldValue + "'" + Convert.ToString(prop.GetValue(obj, null)).Replace("'", "''") + "',";
                            }
                            if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                            {
                                if (prop.Name.Contains("DateOfCreation"))
                                {
                                    FieldValue = FieldValue + "UTC_TIMESTAMP()" + ",";
                                }
                                else
                                {
                                    DateTime newdate = new DateTime();
                                    if (prop.GetValue(obj, null) != null)
                                        newdate = Convert.ToDateTime(prop.GetValue(obj, null));
                                    FieldValue = FieldValue + (prop.GetValue(obj, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";
                                }
                            }
                            if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                            {
                                FieldValue = FieldValue + prop.GetValue(obj, null) + ",";
                            }
                            if (prop.PropertyType.Name == "Boolean")
                            {
                                FieldValue = FieldValue + (Convert.ToBoolean(prop.GetValue(obj, null)) == false ? "0" : "1") + ",";
                            }

                        }

                    }
                    //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
                }
                Query = "Insert into " + TableName + " (" + FieldName.TrimEnd(',') + ") values(" + FieldValue.TrimEnd(',') + ")";
            }
            else
            {
                #region UPDATE

                Query = "Update " + TableName + " set ";
                string LinkCond = "";
                foreach (var prop in obj.GetType().GetProperties())
                {
                    if (prop.Name != PrimaryKeyColumn)
                    {
                        if (prop.Name != "DateOfCreation" && prop.Name != "CreatedBy" && prop.Name != "CreatedHostName")
                        {
                            if (UpdateParam != null && UpdateParam.Count > 0)
                            {
                                foreach (string strvalue in UpdateParam)
                                {
                                    if (strvalue == prop.Name)
                                    {
                                        if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(obj.GetType())).Name)
                                        {
                                            if (prop.PropertyType.Name.Contains("List"))
                                            {
                                                foreach (string subq in BuildTransactionQuery(prop.GetValue(obj, null), TableName))
                                                {
                                                    SubQueries.Add(subq);
                                                }
                                                continue;
                                            }
                                            if (prop.PropertyType.Name == "String")
                                            {
                                                if (Convert.ToString(prop.GetValue(obj, null)).Contains("IDENT_CURRENT"))
                                                    Query = Query + prop.Name + "=" + Convert.ToString(prop.GetValue(obj, null)) + ",";
                                                else
                                                    Query = Query + prop.Name + "='" + Convert.ToString(prop.GetValue(obj, null)).Replace("'", "''") + "',";
                                            }
                                            if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                                            {

                                                if (prop.Name.Contains("DateOfCreation"))
                                                {
                                                    Query = Query + prop.Name + "=" + "UTC_TIMESTAMP()" + ",";
                                                }
                                                else
                                                {
                                                    DateTime newdate = new DateTime();
                                                    if (prop.GetValue(obj, null) != null)
                                                        newdate = Convert.ToDateTime(prop.GetValue(obj, null));
                                                    Query = Query + prop.Name + "=" + (prop.GetValue(obj, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";
                                                }

                                            }
                                            if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                                            {
                                                Query = Query + prop.Name + "=" + prop.GetValue(obj, null) + ",";
                                            }
                                            if (prop.PropertyType.Name == "Boolean")
                                            {
                                                Query = Query + prop.Name + "=" + (Convert.ToBoolean(prop.GetValue(obj, null)) == false ? "0" : "1") + ",";
                                            }
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(obj.GetType())).Name)
                                {
                                    if (prop.PropertyType.Name.Contains("List"))
                                    {
                                        foreach (string subq in BuildTransactionQuery(prop.GetValue(obj, null), TableName))
                                        {
                                            SubQueries.Add(subq);
                                        }
                                        continue;
                                    }
                                    if (prop.PropertyType.Name == "String")
                                    {
                                        Query = Query + prop.Name + "='" + Convert.ToString(prop.GetValue(obj, null)).Replace("'", "''") + "',";
                                    }
                                    if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                                    {
                                        if (prop.Name.Contains("DateOfCreation"))
                                        {
                                            Query = Query + prop.Name + "=" + "UTC_TIMESTAMP()" + ",";
                                        }
                                        else
                                        {
                                            DateTime newdate = new DateTime();
                                            if (prop.GetValue(obj, null) != null)
                                                newdate = Convert.ToDateTime(prop.GetValue(obj, null));
                                            Query = Query + prop.Name + "=" + (prop.GetValue(obj, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";
                                        }
                                    }
                                    if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                                    {
                                        Query = Query + prop.Name + "=" + prop.GetValue(obj, null) + ",";
                                    }
                                    if (prop.PropertyType.Name == "Boolean")
                                    {
                                        Query = Query + prop.Name + "=" + (Convert.ToBoolean(prop.GetValue(obj, null)) == false ? "0" : "1") + ",";
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        LinkCond = " where Link=" + prop.GetValue(obj, null);
                    }
                    //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
                }
                Query = Query.TrimEnd(',') + LinkCond;

                #endregion
            }
            FinalQueries.Add(Query);
            foreach (string query in SubQueries)
            {
                FinalQueries.Add(query);
            }
        }

        public static void SetIndependentQuery()
        {
            FinalQueries.Add("Select  @str  := LAST_INSERT_ID()");

        }

        private static ArrayList BuildTransactionQueryAttr(object transobj, string MasterTable)
        {
            ArrayList subqueries = new ArrayList();
            IList collection = (IList)transobj;
            //List<string> UpdateParam = null;
            string TransTableName = "", PrimaryKeyColumn = "", TransOpMode = "", Query = "";
            foreach (var parentitem in collection)
            {
                foreach (var prop in parentitem.GetType().GetProperties())
                {

                    if (prop.Name == "TableName")
                    {
                        TransTableName = prop.GetValue(parentitem, null).ToString();
                        break;
                    }
                }

                MySqlParameter[] parameters = {
                    new MySqlParameter("TableName",TransTableName) 
                    };
                DataTable dtPrimary = DatabaseConnectionSqlServer.GetDataTable("GetPrimaryKeyName", parameters);
                PrimaryKeyColumn = dtPrimary.Rows[0]["Column_name"].ToString();

                foreach (var prop in parentitem.GetType().GetProperties())
                {
                    if (prop.Name == PrimaryKeyColumn)
                    {
                        if (Convert.ToInt32(prop.GetValue(parentitem, null)) == 0)
                            TransOpMode = "Insert";
                        else
                            TransOpMode = "Update";
                        break;
                    }
                }

                if (TransOpMode == "Insert")
                {

                    #region INSERT MODE

                    string FieldName = "", FieldValue = "";
                    foreach (var prop in parentitem.GetType().GetProperties())
                    {
                        if (prop.Name != PrimaryKeyColumn)
                        {
                            if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(parentitem.GetType())).Name)
                            {
                                FieldName = FieldName + prop.Name + ",";
                                if (prop.PropertyType.Name == "String")
                                {
                                    FieldValue = FieldValue + "'" + Convert.ToString(prop.GetValue(parentitem, null)).Replace("'", "''") + "',";
                                }
                                if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                                {
                                    if (prop.Name.Contains("DateOfCreation"))
                                    {
                                        FieldValue = FieldValue + "UTC_TIMESTAMP()" + ",";
                                    }
                                    else
                                    {
                                        DateTime newdate = new DateTime();
                                        if (prop.GetValue(parentitem, null) != null)
                                            newdate = Convert.ToDateTime(prop.GetValue(parentitem, null));
                                        FieldValue = FieldValue + (prop.GetValue(parentitem, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";
                                    }
                                }
                                if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                                {
                                    if (prop.Name == "MasterLink")
                                        FieldValue = FieldValue + (prop.GetValue(parentitem, null).ToString() == "0" ? "IDENT_CURRENT('" + MasterTable + "')," : (prop.GetValue(parentitem, null).ToString() + ","));
                                    else
                                        FieldValue = FieldValue + prop.GetValue(parentitem, null) + ",";
                                }
                                if (prop.PropertyType.Name == "Boolean")
                                {
                                    FieldValue = FieldValue + (Convert.ToBoolean(prop.GetValue(parentitem, null)) == false ? "0" : "1") + ",";
                                }
                                if (prop.PropertyType.Name.Contains("List"))
                                {
                                    BuildTransactionQuery(prop.GetValue(parentitem, null), TransTableName);
                                }
                            }

                        }
                        //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
                    }
                    Query = "Insert into " + TransTableName + " (" + FieldName.TrimEnd(',') + ") values(" + FieldValue.TrimEnd(',') + ")";

                    #endregion

                }
                else
                {

                    #region UPDATE BLOCK

                    Query = "Update " + TransTableName + " set ";
                    string LinkCond = "";
                    foreach (var prop in parentitem.GetType().GetProperties())
                    {
                        if (prop.Name != PrimaryKeyColumn)
                        {
                            if (prop.Name != "MasterLink")
                            {
                                object[] str = parentitem.GetType().GetProperty(prop.Name).GetCustomAttributes(false);
                                if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(parentitem.GetType())).Name && ((IsToUpdate)(str[0])).flag)
                                {
                                    if (prop.PropertyType.Name == "String")
                                    {
                                        if (Convert.ToString(prop.GetValue(parentitem, null)).Contains("IDENT_CURRENT"))
                                            Query = Query + prop.Name + "=" + Convert.ToString(prop.GetValue(parentitem, null)) + ",";
                                        else
                                            Query = Query + prop.Name + "='" + Convert.ToString(prop.GetValue(parentitem, null)).Replace("'", "''") + "',";
                                    }
                                    if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                                    {
                                        if (prop.Name.Contains("DateOfCreation"))
                                        {
                                            Query = Query + prop.Name + "=" + "UTC_TIMESTAMP()" + ",";
                                        }
                                        else
                                        {
                                            DateTime newdate = new DateTime();
                                            if (prop.GetValue(parentitem, null) != null)
                                                newdate = Convert.ToDateTime(prop.GetValue(parentitem, null));
                                            Query = Query + prop.Name + "=" + (prop.GetValue(parentitem, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";
                                        }

                                    }
                                    if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                                    {
                                        Query = Query + prop.Name + "=" + prop.GetValue(parentitem, null) + ",";
                                    }
                                    if (prop.PropertyType.Name == "Boolean")
                                    {
                                        Query = Query + prop.Name + "=" + (Convert.ToBoolean(prop.GetValue(parentitem, null)) == false ? "0" : "1") + ",";
                                    }

                                }
                            }

                        }
                        else
                        {
                            LinkCond = " where Link=" + prop.GetValue(parentitem, null);
                        }
                        //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
                    }
                    Query = Query.TrimEnd(',') + LinkCond;

                    #endregion

                }
                subqueries.Add(Query);
            }
            return subqueries;
        }

        private static ArrayList BuildTransactionQuery(object transobj, string MasterTable)
        {
            ArrayList subqueries = new ArrayList();
            IList collection = (IList)transobj;
            //List<string> UpdateParam = null;
            string TransTableName = "", PrimaryKeyColumn = "", TransOpMode = "", Query = "";
            foreach (var parentitem in collection)
            {
                foreach (var prop in parentitem.GetType().GetProperties())
                {

                    if (prop.Name == "TableName")
                    {
                        TransTableName = prop.GetValue(parentitem, null).ToString();
                        break;
                    }
                }

                MySqlParameter[] parameters = {
                    new MySqlParameter("TableName",TransTableName) 
                    };
                DataTable dtPrimary = DatabaseConnectionSqlServer.GetDataTable("GetPrimaryKeyName", parameters);
                PrimaryKeyColumn = dtPrimary.Rows[0]["Column_name"].ToString();

                foreach (var prop in parentitem.GetType().GetProperties())
                {
                    if (prop.Name == PrimaryKeyColumn)
                    {
                        if (Convert.ToInt32(prop.GetValue(parentitem, null)) == 0)
                        {
                            TransOpMode = "Insert";
                        }
                        else
                        {
                            TransOpMode = "Update";
                        }
                        break;
                    }
                }

                if (TransOpMode == "Insert")
                {
                    string FieldName = "", FieldValue = "";
                    foreach (var prop in parentitem.GetType().GetProperties())
                    {
                        if (prop.Name != PrimaryKeyColumn)
                        {
                            if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(parentitem.GetType())).Name)
                            {
                                FieldName = FieldName + prop.Name + ",";
                                if (prop.PropertyType.Name == "String")
                                {
                                    FieldValue = FieldValue + "'" + Convert.ToString(prop.GetValue(parentitem, null)).Replace("'", "''") + "',";
                                }
                                if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                                {
                                    if (prop.Name.Contains("DateOfCreation"))
                                    {
                                        FieldValue = FieldValue + "UTC_TIMESTAMP()" + ",";
                                    }
                                    else
                                    {
                                        DateTime newdate = new DateTime();
                                        if (prop.GetValue(parentitem, null) != null)
                                            newdate = Convert.ToDateTime(prop.GetValue(parentitem, null));
                                        FieldValue = FieldValue + (prop.GetValue(parentitem, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";
                                    }
                                }
                                if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                                {
                                    if (prop.Name == "MasterLink")
                                        FieldValue = FieldValue + (prop.GetValue(parentitem, null).ToString() == "0" ? "IDENT_CURRENT('" + MasterTable + "')," : (prop.GetValue(parentitem, null).ToString() + ","));
                                    else
                                        FieldValue = FieldValue + prop.GetValue(parentitem, null) + ",";
                                }
                                if (prop.PropertyType.Name == "Boolean")
                                {
                                    FieldValue = FieldValue + (Convert.ToBoolean(prop.GetValue(parentitem, null)) == false ? "0" : "1") + ",";
                                }
                                if (prop.PropertyType.Name.Contains("List"))
                                {
                                    BuildTransactionQuery(prop.GetValue(parentitem, null), TransTableName);
                                }
                            }

                        }
                        //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
                    }
                    Query = "Insert into " + TransTableName + " (" + FieldName.TrimEnd(',') + ") values(" + FieldValue.TrimEnd(',') + ")";
                }
                else
                {
                    Query = "Update " + TransTableName + " set ";
                    string LinkCond = "";
                    foreach (var prop in parentitem.GetType().GetProperties())
                    {
                        if (prop.Name != PrimaryKeyColumn)
                        {
                            if (prop.Name != "MasterLink")
                            {
                                if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(parentitem.GetType())).Name)
                                {
                                    if (prop.PropertyType.Name == "String")
                                    {
                                        if (Convert.ToString(prop.GetValue(parentitem, null)).Contains("IDENT_CURRENT"))
                                            Query = Query + prop.Name + "=" + Convert.ToString(prop.GetValue(parentitem, null)) + ",";
                                        else
                                            Query = Query + prop.Name + "='" + Convert.ToString(prop.GetValue(parentitem, null)).Replace("'", "''") + "',";
                                    }
                                    if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                                    {
                                        if (prop.Name.Contains("DateOfCreation"))
                                        {
                                            Query = Query + prop.Name + "=" + "UTC_TIMESTAMP()" + ",";
                                        }
                                        else
                                        {
                                            DateTime newdate = new DateTime();
                                            if (prop.GetValue(parentitem, null) != null)
                                                newdate = Convert.ToDateTime(prop.GetValue(parentitem, null));
                                            Query = Query + prop.Name + "=" + (prop.GetValue(parentitem, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";
                                        }

                                    }
                                    if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                                    {
                                        Query = Query + prop.Name + "=" + prop.GetValue(parentitem, null) + ",";
                                    }
                                    if (prop.PropertyType.Name == "Boolean")
                                    {
                                        Query = Query + prop.Name + "=" + (Convert.ToBoolean(prop.GetValue(parentitem, null)) == false ? "0" : "1") + ",";
                                    }

                                }
                            }

                        }
                        else
                        {
                            LinkCond = " where Link=" + prop.GetValue(parentitem, null);
                        }
                        //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
                    }
                    Query = Query.TrimEnd(',') + LinkCond;
                }
                subqueries.Add(Query);
            }
            return subqueries;
        }

        public static string SetIndependentTableAttr(object obj)
        {
            ArrayList SubQueries = new ArrayList();
            string Query = "", OperationMode = "", PrimaryKeyColumn = "", TableName = "";
            DataTable dtPrimary = new DataTable();
            foreach (var item in obj.GetType().GetProperties())
            {
                if (item.Name == "TableName")
                {
                    TableName = item.GetValue(obj, null).ToString();
                    break;
                }
            }

            if (TableName == "")
            {
                return "Please Inherit TableName Property and Set it to Respected Table Name of your Database";
            }

            MySqlParameter[] parameters = {
                new MySqlParameter("TableName",TableName) 
            };

            try
            {
                dtPrimary = DatabaseConnectionSqlServer.GetDataTable("GetPrimaryKeyName", parameters);
            }
            catch (Exception ex)
            {
                return "Error while Executing Procedure 'GetPrimaryKey' : " + ex.Message;
            }

            PrimaryKeyColumn = dtPrimary.Rows[0]["Column_name"].ToString();

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.Name == PrimaryKeyColumn)
                {
                    if (Convert.ToInt32(prop.GetValue(obj, null)) == 0)
                        OperationMode = "Insert";
                    else
                        OperationMode = "Update";
                    break;
                }
            }

            if (OperationMode == "")
            {
                return "Please set value of property of your Primary Key.";
            }

            try
            {

                if (OperationMode == "Insert")
                {
                    #region INSERT BLOCK

                    string FieldName = "", FieldValue = "";
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        if (prop.Name != PrimaryKeyColumn)
                        {
                            if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(obj.GetType())).Name)
                            {
                                if (prop.PropertyType.Name.Contains("List"))
                                {
                                    foreach (string subq in BuildTransactionQueryAttr(prop.GetValue(obj, null), TableName))
                                    {
                                        SubQueries.Add(subq);
                                    }
                                    continue;
                                }
                                FieldName = FieldName + prop.Name + ",";
                                if (prop.PropertyType.Name == "String")
                                {
                                    if (Convert.ToString(prop.GetValue(obj, null)).Contains("IDENT_CURRENT"))
                                        FieldValue = FieldValue + Convert.ToString(prop.GetValue(obj, null)) + ",";
                                    else
                                        FieldValue = FieldValue + "'" + Convert.ToString(prop.GetValue(obj, null)).Replace("'", "''") + "',";
                                }
                                if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                                {

                                    if (prop.Name.Contains("DateOfCreation"))
                                    {
                                        FieldValue = FieldValue + "UTC_TIMESTAMP()" + ",";
                                    }
                                    else
                                    {
                                        DateTime newdate = new DateTime();
                                        if (prop.GetValue(obj, null) != null)
                                            newdate = Convert.ToDateTime(prop.GetValue(obj, null));
                                        FieldValue = FieldValue + (prop.GetValue(obj, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";
                                    }

                                }
                                if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                                {
                                    FieldValue = FieldValue + prop.GetValue(obj, null) + ",";
                                }
                                if (prop.PropertyType.Name == "Boolean")
                                {
                                    FieldValue = FieldValue + (Convert.ToBoolean(prop.GetValue(obj, null)) == false ? "0" : "1") + ",";
                                }

                            }

                        }
                        //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
                    }
                    Query = "Insert into " + TableName + " (" + FieldName.TrimEnd(',') + ") values(" + FieldValue.TrimEnd(',') + ")";

                    #endregion
                }
                else
                {
                    #region UPDATE

                    Query = "Update " + TableName + " set ";
                    string LinkCond = "";
                    foreach (var prop in obj.GetType().GetProperties())
                    {
                        if (prop.Name != PrimaryKeyColumn)
                        {

                            object[] str = obj.GetType().GetProperty(prop.Name).GetCustomAttributes(false);
                            if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(obj.GetType())).Name && ((IsToUpdate)(str[0])).flag)
                            {
                                if (prop.PropertyType.Name.Contains("List"))
                                {
                                    foreach (string subq in BuildTransactionQuery(prop.GetValue(obj, null), TableName))
                                    {
                                        SubQueries.Add(subq);
                                    }
                                    continue;
                                }
                                if (prop.PropertyType.Name == "String")
                                {
                                    if (Convert.ToString(prop.GetValue(obj, null)).Contains("IDENT_CURRENT"))
                                        Query = Query + prop.Name + "=" + Convert.ToString(prop.GetValue(obj, null)) + ",";
                                    else
                                        Query = Query + prop.Name + "='" + Convert.ToString(prop.GetValue(obj, null)).Replace("'", "''") + "',";
                                }
                                if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                                {

                                    if (prop.Name.Contains("DateOfCreation"))
                                    {
                                        Query = Query + prop.Name + "=" + "UTC_TIMESTAMP()" + ",";
                                    }
                                    else
                                    {
                                        DateTime newdate = new DateTime();
                                        if (prop.GetValue(obj, null) != null)
                                            newdate = Convert.ToDateTime(prop.GetValue(obj, null));
                                        Query = Query + prop.Name + "=" + (prop.GetValue(obj, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";
                                    }

                                }
                                if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                                {
                                    Query = Query + prop.Name + "=" + prop.GetValue(obj, null) + ",";
                                }
                                if (prop.PropertyType.Name == "Boolean")
                                {
                                    Query = Query + prop.Name + "=" + (Convert.ToBoolean(prop.GetValue(obj, null)) == false ? "0" : "1") + ",";
                                }

                            }
                        }
                        else
                        {
                            LinkCond = " where Link=" + prop.GetValue(obj, null);
                        }
                        //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
                    }
                    Query = Query.TrimEnd(',') + LinkCond;

                    #endregion
                }

            }
            catch (Exception ex)
            {
                return "Error : " + ex.Message;
            }
            FinalQueries.Add(Query);
            foreach (string query in SubQueries)
            {
                FinalQueries.Add(query);
            }
            return "Success";
        }

        public static bool FireTransaction()
        {
            bool issucc = false;
            try
            {
                issucc = DatabaseConnectionSqlServer.ExecuteTransaction(FinalQueries);
            }
            catch
            {
                FinalQueries = new ArrayList();
                return false;
            }
            FinalQueries = new ArrayList();
            return issucc;
        }

        public static bool ClearTransaction()
        {
            FinalQueries = new ArrayList();
            return true;
        }

        /* Update Single Cell */

        public static void SetDependentTable(object obj, List<string> UpdateParameters)
        {
            String ConStr = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseConnetion"].ConnectionString;
            MySqlConnection conn = new MySqlConnection(ConStr);
            string database = conn.Database.ToString();

            ArrayList SubQueries = new ArrayList();
            List<string> UpdateParam = null;
            if (UpdateParameters == null)
                UpdateParam = new List<string>();
            else
                UpdateParam = UpdateParameters;
            string Query = "", OperationMode = "", PrimaryKeyColumn = "", TableName = "";
            //Type type = obj.GetType();
            //Type listType = typeof(List<>).MakeGenericType(new[] { type });
            //IList list = (IList)Activator.CreateInstance(listType);
            //list.Add(obj);
            foreach (var item in obj.GetType().GetProperties())
            {
                if (item.Name == "TableName")
                {
                    TableName = item.GetValue(obj, null).ToString();
                    break;
                }
            }

            //MySqlParameter[] parameters = {
            //    new MySqlParameter("TableName",TableName) 
            //};
            DataTable dtPrimary = DatabaseConnectionSqlServer.GetDataTable("SHOW INDEX FROM " + database + ".`" + TableName + "` WHERE `Key_name` = 'PRIMARY'");
            PrimaryKeyColumn = dtPrimary.Rows[0]["Column_name"].ToString();

            foreach (var prop in obj.GetType().GetProperties())
            {
                if (prop.Name == PrimaryKeyColumn)
                {
                    if (Convert.ToInt32(prop.GetValue(obj, null)) == 0)
                    {
                        OperationMode = "Insert";
                    }
                    else
                    {
                        OperationMode = "Update";
                    }
                    break;
                }
            }


            if (OperationMode == "Insert")
            {
                string FieldName = "", FieldValue = "";
                foreach (var prop in obj.GetType().GetProperties())
                {
                    if (prop.Name != PrimaryKeyColumn)
                    {
                        if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(obj.GetType())).Name)
                        {
                            if (prop.PropertyType.Name.Contains("List"))
                            {
                                foreach (string subq in BuildTransactionQuery(prop.GetValue(obj, null), TableName))
                                {
                                    SubQueries.Add(subq);
                                }
                                continue;
                            }
                            FieldName = FieldName + prop.Name + ",";
                            if (prop.PropertyType.Name == "String")
                            {
                                if (Convert.ToString(prop.GetValue(obj, null)).Contains("IDENT_CURRENT"))
                                    FieldValue = FieldValue + Convert.ToString(prop.GetValue(obj, null)) + ",";
                                else
                                    FieldValue = FieldValue + "'" + Convert.ToString(prop.GetValue(obj, null)).Replace("'", "''") + "',";
                            }
                            if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                            {
                                DateTime newdate = new DateTime();
                                if (prop.GetValue(obj, null) != null)
                                    newdate = Convert.ToDateTime(prop.GetValue(obj, null));
                                FieldValue = FieldValue + (prop.GetValue(obj, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";

                            }
                            if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                            {
                                FieldValue = FieldValue + prop.GetValue(obj, null) + ",";
                            }
                            if (prop.PropertyType.Name == "Boolean")
                            {
                                FieldValue = FieldValue + (Convert.ToBoolean(prop.GetValue(obj, null)) == false ? "0" : "1") + ",";
                            }

                        }

                    }
                    //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
                }
                Query = "Insert into " + TableName + " (" + FieldName.TrimEnd(',') + ") values(" + FieldValue.TrimEnd(',') + ")";
            }
            else
            {
                #region UPDATE

                Query = "Update " + TableName + " set ";
                string LinkCond = "";
                foreach (var prop in obj.GetType().GetProperties())
                {
                    if (prop.Name != PrimaryKeyColumn)
                    {
                        if (prop.Name != "CreatedBy" && prop.Name != "CreatedHostName")
                        {
                            if (UpdateParam != null && UpdateParam.Count > 0)
                            {
                                foreach (string strvalue in UpdateParam)
                                {
                                    if (strvalue == prop.Name)
                                    {
                                        if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(obj.GetType())).Name)
                                        {
                                            if (prop.PropertyType.Name.Contains("List"))
                                            {
                                                foreach (string subq in BuildTransactionQuery(prop.GetValue(obj, null), TableName))
                                                {
                                                    SubQueries.Add(subq);
                                                }
                                                continue;
                                            }
                                            if (prop.PropertyType.Name == "String")
                                            {
                                                if (Convert.ToString(prop.GetValue(obj, null)).Contains("IDENT_CURRENT"))
                                                    Query = Query + prop.Name + "=" + Convert.ToString(prop.GetValue(obj, null)) + ",";
                                                else
                                                    Query = Query + prop.Name + "='" + Convert.ToString(prop.GetValue(obj, null)).Replace("'", "''") + "',";
                                            }
                                            if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                                            {

                                                DateTime newdate = new DateTime();
                                                if (prop.GetValue(obj, null) != null)
                                                    newdate = Convert.ToDateTime(prop.GetValue(obj, null));
                                                Query = Query + prop.Name + "=" + (prop.GetValue(obj, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";

                                            }
                                            if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                                            {
                                                Query = Query + prop.Name + "=" + prop.GetValue(obj, null) + ",";
                                            }
                                            if (prop.PropertyType.Name == "Boolean")
                                            {
                                                Query = Query + prop.Name + "=" + (Convert.ToBoolean(prop.GetValue(obj, null)) == false ? "0" : "1") + ",";
                                            }
                                            break;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (((System.Reflection.MemberInfo)(((System.Reflection.MemberInfo)(prop)).DeclaringType)).Name == ((System.Reflection.MemberInfo)(obj.GetType())).Name)
                                {
                                    if (prop.PropertyType.Name.Contains("List"))
                                    {
                                        foreach (string subq in BuildTransactionQuery(prop.GetValue(obj, null), TableName))
                                        {
                                            SubQueries.Add(subq);
                                        }
                                        continue;
                                    }
                                    if (prop.PropertyType.Name == "String")
                                    {
                                        Query = Query + prop.Name + "='" + Convert.ToString(prop.GetValue(obj, null)).Replace("'", "''") + "',";
                                    }
                                    if (prop.PropertyType.Name == "DateTime" || prop.PropertyType.Name == "Nullable`1")
                                    {
                                        if (prop.Name.Contains("DateOfCreation"))
                                        {
                                            Query = Query + prop.Name + "=" + "UTC_TIMESTAMP()" + ",";
                                        }
                                        else
                                        {
                                            DateTime newdate = new DateTime();
                                            if (prop.GetValue(obj, null) != null)
                                                newdate = Convert.ToDateTime(prop.GetValue(obj, null));
                                            Query = Query + prop.Name + "=" + (prop.GetValue(obj, null) != null ? "'" + newdate.ToString("yyyy-MM-dd h:mm:ss") + "'" : "NULL") + ",";
                                        }
                                    }
                                    if (prop.PropertyType.Name == "Int32" || prop.PropertyType.Name == "Decimal")
                                    {
                                        Query = Query + prop.Name + "=" + prop.GetValue(obj, null) + ",";
                                    }
                                    if (prop.PropertyType.Name == "Boolean")
                                    {
                                        Query = Query + prop.Name + "=" + (Convert.ToBoolean(prop.GetValue(obj, null)) == false ? "0" : "1") + ",";
                                    }
                                }

                            }
                        }

                    }
                    else
                    {
                        LinkCond = " where Link=" + prop.GetValue(obj, null);
                    }
                    //Console.WriteLine("{0}={1}", prop.Name, prop.GetValue(foo, null));
                }
                Query = Query.TrimEnd(',') + LinkCond;

                #endregion
            }
            FinalQueries.Add(Query);
            foreach (string query in SubQueries)
            {
                FinalQueries.Add(query);
            }
        }

        /* End Of Update Single Row*/

        //public static bool SetUpdateToSelected(object obj, string Params)
        //{
        //    IList collection = (IList)obj;
        //    foreach (var parentitem in collection)
        //    {
        //        foreach (var prop in parentitem.GetType().GetProperties())
        //        {
        //            object[] str = typeof(clsProjectMaster).GetProperty(prop.Name).GetCustomAttributes(false);
        //            if (!prop.PropertyType.Name.Contains("List"))
        //            {
        //                if (Params.Contains(prop.Name))
        //                {
        //                    ((IsToUpdate)(str[0])).flag = true;
        //                }
        //                else
        //                {
        //                    ((IsToUpdate)(str[0])).flag = false;
        //                }
        //            }
        //            else
        //            {
        //                SetUpdateToSelected(prop.GetValue(parentitem, null), Params);
        //            }
        //        }
        //    }
        //    return true;
        //}
    }
}
