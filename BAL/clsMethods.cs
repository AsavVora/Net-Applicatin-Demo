using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAL;
using System.Configuration;
using System.Net.Mail;

namespace BAL
{
    public class clsMethods
    {
        private const int DOCUMENTNOPORTIONLENGTH = 5;

        public static object DeleteRecordByLink(string TableName, string ColumnName, string Value)
        {
            // DELETE RECORD FROM TABLE , USING TABLE NAME , COLUMN NAME (MOSTALLY LINK) AND VALUE OF THAT COLUMN
            MySqlParameter[] parameters = {
                new MySqlParameter("TableName",TableName),
                new MySqlParameter("FieldName",ColumnName),
                new MySqlParameter("FieldValue",Value)
            };
            // DeactivateRecordByLink is a stored procedure name and Above Parameter list for the stored procedure
            return DatabaseConnectionSqlServer.ExecuteNonQuery("DeactivateRecordByLink", parameters);
        }

        public static object ActivateRecordByLink(string TableName, string ColumnName, string Value)
        {
            // ACTIVATE RECORD FROM TABLE , USING TABLE NAME , COLUMN NAME (MOSTALLY LINK) AND VALUE OF THAT COLUMN
            MySqlParameter[] parameters = {
                new MySqlParameter("TableName",TableName),
                new MySqlParameter("FieldName",ColumnName),
                new MySqlParameter("FieldValue",Value)
            };
            return DatabaseConnectionSqlServer.ExecuteNonQuery("ActivateRecordByLink", parameters);
        }

        public static DataTable GetMenus(int PLink, string UserLink, string UserType, string IsTop)
        {
            // GET MENUS FOR LOGIN USER
            MySqlParameter[] parameters = {
                new MySqlParameter("ParentLink",PLink),
                new MySqlParameter("UserLink",UserLink),
                new MySqlParameter("UserType",UserType),
                new MySqlParameter("IsTop",IsTop)
            };
            return DatabaseConnectionSqlServer.GetDataTable("GetMenus", parameters);
        }

        public static DataTable GetMenus3(int PLink, string UserLink, string UserType, string IsUserType)
        {

            // GET MENUS FOR LOGIN USER
            MySqlParameter[] parameters = {
                new MySqlParameter("ParentLink",PLink),
                new MySqlParameter("UserLink",UserLink),
                new MySqlParameter("UserType",UserType),
                new MySqlParameter("IsUserType",IsUserType)
            };
            return DatabaseConnectionSqlServer.GetDataTable("GetMenus3", parameters);
        }

        public static DataTable GetMenusByUserType(int PLink, string UserLink, string UserType, string IsTop)
        {
            // GET MENUS FOR LOGIN USER USING USER TYPE
            MySqlParameter[] parameters = {
                new MySqlParameter("ParentLink",PLink),
                new MySqlParameter("UserLink",UserLink),
                new MySqlParameter("UserType",UserType),
                new MySqlParameter("IsTop",IsTop)
            };
            return DatabaseConnectionSqlServer.GetDataTable("GetMenusByUserType", parameters);
        }

        public static bool SendMail(string subject, string body, string MailTo, string CompanyLink)
        {
            // SEND MAIL
            bool issuccess = false;

            string emailfrom = string.Empty;
            string password = string.Empty;

            DataTable dt = DatabaseConnectionSqlServer.GetDataTable("Select Email.* from Email where Email.IsDeactivate=0 and Email.CompanyLink='" + CompanyLink + "'");


            if (dt.Rows.Count > 0)
            {
                emailfrom = dt.Rows[0]["Email"].ToString();
                password = dt.Rows[0]["Password"].ToString();
            }
            else
            {
                emailfrom = ConfigurationManager.AppSettings["EmailId"].ToString();
                password = ConfigurationManager.AppSettings["Password"].ToString();
            }
            string port = ConfigurationManager.AppSettings["Port"].ToString();
            string host = ConfigurationManager.AppSettings["Host"].ToString();
            string enablessl = ConfigurationManager.AppSettings["EnableSSL"].ToString();
            SmtpClient SmtpServer = new SmtpClient();
            SmtpServer.Credentials = new System.Net.NetworkCredential(emailfrom, password);

            SmtpServer.Port = Convert.ToInt32(port);
            SmtpServer.Host = host;

            SmtpServer.EnableSsl = Convert.ToBoolean(enablessl);
            MailMessage mail = new MailMessage();
            //mail.IsBodyHtml = true;

            mail.From = new MailAddress(emailfrom, emailfrom, System.Text.Encoding.UTF8);
            mail.Subject = subject;
            mail.To.Add(MailTo);
            //mail.To.Add("jeff@wallsandcrafts.com");
            //mail.To.Add("r.mcwhorter@hoppitydoodle.com");
            mail.IsBodyHtml = true;
            mail.Body = body;

            try
            {
                SmtpServer.Send(mail);
                issuccess = true;
            }
            catch (Exception ex)
            {
                issuccess = false;
                //File.AppendAllText(@"c:\projects\ScheduledService.txt", "\nError at :\n " + ex.Message + "\n" + DateTime.Now.ToString());
            }
            return issuccess;
        }

        public static bool SendMail2(string subject, string body, string MailTo, string AttPath, string CompanyLink)
        {
            // SEND MAIL WITH ATTACHMENT

            bool issuccess = false;
            string emailfrom = string.Empty;
            string password = string.Empty;

            DataTable dt = DatabaseConnectionSqlServer.GetDataTable("Select Email.* from Email where Email.IsDeactivate=0 and Email.CompanyLink='" + CompanyLink + "'");


            if (dt.Rows.Count > 0)
            {
                emailfrom = dt.Rows[0]["Email"].ToString();
                password = dt.Rows[0]["Password"].ToString();
            }
            else
            {
                emailfrom = ConfigurationManager.AppSettings["EmailId"].ToString();
                password = ConfigurationManager.AppSettings["Password"].ToString();
            }
            string port = ConfigurationManager.AppSettings["Port"].ToString();
            string host = ConfigurationManager.AppSettings["Host"].ToString();
            string enablessl = ConfigurationManager.AppSettings["EnableSSL"].ToString();
            SmtpClient SmtpServer = new SmtpClient();
            SmtpServer.Credentials = new System.Net.NetworkCredential(emailfrom, password);

            SmtpServer.Port = Convert.ToInt32(port);
            SmtpServer.Host = host;

            SmtpServer.EnableSsl = Convert.ToBoolean(enablessl);
            MailMessage mail = new MailMessage();
            //mail.IsBodyHtml = true;

            mail.From = new MailAddress(emailfrom, emailfrom, System.Text.Encoding.UTF8);
            mail.Subject = subject;
            mail.To.Add(MailTo);
            //mail.To.Add("jeff@wallsandcrafts.com");
            //mail.To.Add("r.mcwhorter@hoppitydoodle.com");
            mail.IsBodyHtml = true;
            mail.Body = body;

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment(AttPath);
            mail.Attachments.Add(attachment);



            try
            {
                SmtpServer.Send(mail);
                issuccess = true;
            }
            catch (Exception ex)
            {
                issuccess = false;
                //File.AppendAllText(@"c:\projects\ScheduledService.txt", "\nError at :\n " + ex.Message + "\n" + DateTime.Now.ToString());
            }
            return issuccess;
        }

        public static DataTable GetDuplicateCustomer(string Customer, string CompanyLink)
        {
            // GET DUPLICATE CUSTOMER USING NAME
            MySqlParameter[] parameters = {
                new MySqlParameter("Customer",Customer),
                new MySqlParameter("CompanyLink",CompanyLink)
            };
            return DatabaseConnectionSqlServer.GetDataTable("GetDuplicateCustomer", parameters);
        }

        public static DataTable GetDocNo(string DocType)
        {
            // GET DOCUMENT NUMBER
            MySqlParameter[] parameters = {
                new MySqlParameter("DocType",DocType)
            };
            DataTable dt = DatabaseConnectionSqlServer.GetDataTable("GetDocNo", parameters);
            return dt;
        }

        public static String FormatDocumentNo(String DocumentType, Decimal doc_no)
        {
            // GET NEXT DOCUMENT NUMBER AND DOCUMENT NUMBER SHOULD BE LIST THIS PR00001

            int A = DatabaseConnectionSqlServer.ExecuteNonQueryInternal("update next_doc_no set next_doc_no=" + doc_no + "+ 1 where Doc_type ='" + DocumentType + "'");

            String FormattedNo = "";
            FormattedNo = DocumentType.Substring(0, 1);
            String year = System.DateTime.Today.Year.ToString();
            FormattedNo += year.Substring(2);
            year = System.DateTime.Today.AddYears(1).Year.ToString().Substring(2);
            FormattedNo += year;

            int LengthOfCurrentDocNo = doc_no.ToString().Length;
            for (int i = 1; i <= DOCUMENTNOPORTIONLENGTH - LengthOfCurrentDocNo; i++)
                FormattedNo += "0";
            FormattedNo += doc_no.ToString();

            return FormattedNo;
        }

        public static DataTable GetItemByProjectNo(string ProjectNo)
        {
            // GET PROJECT DETAIL USING PROJECT NUMBER
            MySqlParameter[] parameters = {
                new MySqlParameter("ProjectNo",ProjectNo)
            };

            return DatabaseConnectionSqlServer.GetDataTable("GetItemByProjectNo", parameters);
        }

        public static DataTable GetAllProjectFilter(string filter, string CompanyLink)
        {
            MySqlParameter[] parameters = {
                new MySqlParameter("filter",filter),
                new MySqlParameter("CompanyLink",CompanyLink),
            };
            return DatabaseConnectionSqlServer.GetDataTable("GetAllProjectFilter", parameters);
        }

        // THIS IS FOR GET NEXT NUMBER , IT WILL NOT UPDATE IN DATABASE.
        public static String FormatDocumentNoWithoutUpdate(String DocumentType, Decimal doc_no)
        {
            String FormattedNo = "";
            FormattedNo = DocumentType.Substring(0, 1);

            int LengthOfCurrentDocNo = doc_no.ToString().Length;
            for (int i = 1; i <= DOCUMENTNOPORTIONLENGTH - LengthOfCurrentDocNo; i++)
                FormattedNo += "0";
            FormattedNo += doc_no.ToString();

            return FormattedNo;
        } 

        public static DataTable GetAllCustomer(string CompanyLink)
        {
            MySqlParameter[] parameters = {
                new MySqlParameter("CompanyLink",CompanyLink)
            };
            return DatabaseConnectionSqlServer.GetDataTable("GetAllCustomer", parameters);
        }

    }
}
