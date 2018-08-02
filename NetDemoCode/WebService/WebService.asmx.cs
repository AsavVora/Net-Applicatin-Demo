using BAL;
using BAL.Masters;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using MySql.Data.MySqlClient;
using System.Web.Script.Serialization;
using Twilio;
using System.Text.RegularExpressions;
using System.Net;
using System.Xml;
using Microsoft.Web.Administration;

namespace PMS.WebService
{
    /// <summary>
    /// Summary description for WebService
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    [System.Web.Script.Services.ScriptService]
    public class WebService : System.Web.Services.WebService
    {
        static String ConStr = System.Configuration.ConfigurationManager.ConnectionStrings["DatabaseConnetion"].ConnectionString;
        static MySqlConnection conn = new MySqlConnection(ConStr);
        static string database = conn.Database.ToString();
        private static String ServerLogFile = HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings.Get("ServerLogFile"));

        /* Subdomain Creation */
        ServerManager serverMgr = new ServerManager();
        private static String HostName = ConfigurationManager.AppSettings.Get("HostName");

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        // SAVE CUSTOMER
        [WebMethod(EnableSession = true)]
        public string SaveCustomer(clsCustomerMaster CustomerData, clsCustomerContactMaster[] ContactData)
        {
            string msg = "";


            /* Next Doc NO */
            int nextDocNo = 0;
            int Link12 = 0;
            string FormattedNo = "";

            // LOGIN COMPANY LINK(ID)
            clsCustomerMaster obj = CustomerData;
            obj.CompanyLink = HttpContext.Current.Request.Cookies["CompanyLink"].Value.ToString();

            // CHECK DUPLICATE
            DataTable objdup = clsMethods.GetDuplicateCustomer(obj.Username, obj.CompanyLink);
            if (objdup.Rows.Count > 0)
            {
                if (obj.Link.ToString() == "0")
                    return "Customer is already in use, Please Enter another Name.";
                else
                    if (objdup.Rows.Count >= 2)
                    return "Customer is already in use, Please Enter another Name.";
                else
                        if (objdup.Rows[0]["Link"].ToString() != obj.Link.ToString())
                    return "Customer is already in use, Please Enter another Name.";
            }

            obj.IsDeactivate = false;

            // LINK 0 THEN INSERT ELSE UPDATE RECORD
            if (obj.Link.ToString() == "0")
            {
                DataTable dtNext = clsMethods.GetDocNo("QA");
                if (dtNext.Rows.Count > 0)
                {
                    // GET NEXT CUSTOMER NUMBER , NUMBER WILL BE LIKE THIS : QA00001
                    nextDocNo = Convert.ToInt32(dtNext.Rows[0]["Next_Doc_No"].ToString());
                    Link12 = Convert.ToInt32(dtNext.Rows[0]["Link"].ToString());
                    FormattedNo = clsMethods.FormatDocumentNo("QA", Convert.ToDecimal(nextDocNo));
                }
                // CREATED BY , CREATED HOST NAME, DATE OF CREATION FOR ALL THE RECORD WHICH IS INSERT FOR ALL TABLE
                obj.CustomerNo = FormattedNo;
                obj.CreatedBy = Convert.ToInt32(HttpContext.Current.Request.Cookies["UserId"].Value);
                obj.CreatedHostName = HttpContext.Current.Request.UserHostName;
                obj.DateOfCreation = DateTime.Now;
            }
            else
            {
                // MODIFIED BY , MODIFIED HOST , MODIFIED DATE WHEN RECORD UPDATE.
                obj.LastModifiedBy = Convert.ToInt32(HttpContext.Current.Request.Cookies["UserId"].Value);
                obj.LastModifiedHost = HttpContext.Current.Request.UserHostName;
                obj.LastModifiedDate = DateTime.Now;
            }
            // THIS IS THE FUNCTION IN BAL PROJECT WHERE INSERT,UPDATE OR DELETE QUERY CREATE BASED ON THE OBJECT WHICH HAS DEFINE IN CLASS FILE.
            clsIFrameWork.SetIndependentTable(obj);

            // CREATE CONTACT
            for (int i = 0; i < ContactData.Length; i++)
            {
                clsCustomerContactMaster objM = ContactData[i];
                objM.CustomerNo = obj.CustomerNo;
                objM.CompanyLink = HttpContext.Current.Request.Cookies["CompanyLink"].Value.ToString();

                // LINK 0 THEN INSERT ELSE UPDATE RECORD
                if (objM.Link.ToString() == "0")
                {
                    // CREATED BY , CREATED HOST NAME, DATE OF CREATION FOR ALL THE RECORD WHICH IS INSERT FOR ALL TABLE
                    objM.CreatedBy = Convert.ToInt32(HttpContext.Current.Request.Cookies["UserId"].Value);
                    objM.CreatedHostName = HttpContext.Current.Request.UserHostName;
                    objM.DateOfCreation = DateTime.Now;
                }
                else
                {
                    // MODIFIED BY , MODIFIED HOST , MODIFIED DATE WHEN RECORD UPDATE.
                    objM.LastModifiedBy = Convert.ToInt32(HttpContext.Current.Request.Cookies["UserId"].Value);
                    objM.LastModifiedHost = HttpContext.Current.Request.UserHostName;
                    objM.LastModifiedDate = DateTime.Now;
                }
                // THIS IS THE FUNCTION IN BAL PROJECT WHERE INSERT,UPDATE OR DELETE QUERY CREATE BASED ON THE OBJECT WHICH HAS DEFINE IN CLASS FILE.
                clsIFrameWork.SetIndependentTable(objM);


            }

            bool issucc = false;
            try
            {
                // TRACACTION IS EXECUTE , SO ABOVE CREATED ALL QUERY WILL EXECUTE AT SAME TIME.
                issucc = clsIFrameWork.FireTransaction();
            }
            catch (Exception ex)
            {
                // ERROR LOG
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                msg = ex.Message;
            }
            if (issucc)
                return "Record Saved Successfully.";
            else
                return msg;
        }

        [WebMethod(EnableSession = true)]
        public string SaveProjectStatus(string ProjectLink, string Flag)
        {
            // UPDATE STATUS OF THE PROJECT
            string msg = "";

            clsProjectMaster obj = new clsProjectMaster();
            // CREATE LIST OF THE FIELD WHICH ONLY NEED TO UPDATE.
            List<string> lst = new List<string>();
            lst.Add("Link");
            obj.Link = Convert.ToInt32(ProjectLink);
            lst.Add("Flag");
            obj.Flag = Flag;
            // THIS CREATE A QUERY OF THE COLUMN WHICH DEFINE INTO THE LIST.
            clsIFrameWork.SetDependentTable(obj, lst);


            bool issucc = false;
            try
            {
                // TRACACTION IS EXECUTE , SO ABOVE CREATED ALL QUERY WILL EXECUTE AT SAME TIME.
                issucc = clsIFrameWork.FireTransaction();
            }
            catch (Exception ex)
            {
                // ERROR LOG
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                msg = ex.Message;
            }
            if (issucc)
                return "";
            else
                return msg;
        }

        // GET PROJECT DETAIL USING PROJECT NUMBER
        [WebMethod(EnableSession = true)]
        public string GetItemByProjectNo(string ProjectNo)
        {
            string msg = "";

            try
            {
                // GET 
                DataTable dt = clsMethods.GetItemByProjectNo(ProjectNo);
                // NEWTONSOFT JSON FOR CONVERT DATA INTO THE JSON
                return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                return msg;
            }

        }

        // FILTER PROJECT
        [WebMethod(EnableSession = true)]
        public string GetAllProjectFilter(string filter)
        {
            string msg = "";

            try
            {
                // filter has where condition which used in stored procedure
                string CompanyLink = HttpContext.Current.Request.Cookies["CompanyLink"].Value.ToString();
                DataTable dt = clsMethods.GetAllProjectFilter(filter, CompanyLink);
                return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                return msg;
            }

        }

        // GET NEXT NUMBER. EX: PR00001

        [WebMethod(EnableSession = true)]
        public string GetNextNo(string DocType)
        {
            string msg = "";

            try
            {
                int nextDocNo = 0;
                int Link12 = 0;
                string FormattedNo = "";

                DataTable dtNext = clsMethods.GetDocNo(DocType);
                if (dtNext.Rows.Count > 0)
                {
                    // NEXT NUMBER.
                    nextDocNo = Convert.ToInt32(dtNext.Rows[0]["Next_Doc_No"].ToString());
                    FormattedNo = clsMethods.FormatDocumentNoWithoutUpdate(DocType, Convert.ToDecimal(nextDocNo));
                }

                return Newtonsoft.Json.JsonConvert.SerializeObject(FormattedNo);
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                return msg;
            }

        }

        // GET ALL CUSTOMER

        [WebMethod(EnableSession = true)]
        public string GetAllCustomer()
        {
            string msg = "";

            try
            {
                string CompanyLink = HttpContext.Current.Request.Cookies["CompanyLink"].Value.ToString();
                DataTable dt = clsMethods.GetAllCustomer(CompanyLink);
                return Newtonsoft.Json.JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                return msg;
            }

        }

        // SAVE PROJECT
        [WebMethod(EnableSession = true)]
        public string SaveProject(clsProjectMaster ProjectMaster, string notify)
        {
            string msg = "";
            int nextDocNo = 0;
            string FormattedNo = "";

            string CompanyLink = HttpContext.Current.Request.Cookies["CompanyLink"].Value.ToString();

            // PROJECT OBJECT
            clsProjectMaster obj = ProjectMaster;
            obj.IsDeactivate = false;
            obj.CompanyLink = HttpContext.Current.Request.Cookies["CompanyLink"].Value;
            obj.AreaLink = DatabaseConnectionSqlServer.ExecuteScalarInternal("Select Link from AreaMaster where areamaster.isdeactivate=0 and areamaster.name Like '%" + obj.City + "%'");

            // LINK 0 THEN INSERT ELSE UPDATE RECORD
            if (obj.Link == 0)
            {
                DataTable dtNext = clsMethods.GetDocNo("PR");
                if (dtNext.Rows.Count > 0)
                {
                    // GET NEXT CUSTOMER NUMBER , NUMBER WILL BE LIKE THIS : PR00001
                    nextDocNo = Convert.ToInt32(dtNext.Rows[0]["Next_Doc_No"].ToString());
                    FormattedNo = clsMethods.FormatDocumentNo("PR", Convert.ToDecimal(nextDocNo));
                }
                obj.ProjectNo = FormattedNo;

                // CREATED BY , CREATED HOST NAME, DATE OF CREATION FOR ALL THE RECORD WHICH IS INSERT FOR ALL TABLE
                obj.CreatedBy = Convert.ToInt32(HttpContext.Current.Request.Cookies["UserId"].Value);
                obj.CreatedHostName = HttpContext.Current.Request.UserHostName;
                obj.DateOfCreation = DateTime.Now;

            }
            else
            {
                // MODIFIED BY , MODIFIED HOST , MODIFIED DATE WHEN RECORD UPDATE.
                obj.ModifiedBy = Convert.ToInt32(HttpContext.Current.Request.Cookies["UserId"].Value);
                obj.ModifiedHostName = HttpContext.Current.Request.UserHostName;
                obj.DateOfModification = DateTime.Now;
            }

            clsIFrameWork.SetIndependentTable(obj);

            if (notify == "1")
            {
                // SEND MAIL
                DataTable dtEmail = DatabaseConnectionSqlServer.GetDataTable("Select Email from UserMaster where usermaster.Link = " + obj.SalesRepresentive);
                if (dtEmail.Rows.Count > 0)
                {
                    string Email = dtEmail.Rows[0]["Email"].ToString();
                    bool mail = clsMethods.SendMail("Project Creation", obj.InitialRequirement, Email, CompanyLink);
                }
            }


            bool issucc = false;
            try
            {
                // TRACACTION IS EXECUTE , SO ABOVE CREATED ALL QUERY WILL EXECUTE AT SAME TIME.
                issucc = clsIFrameWork.FireTransaction();
            }
            catch (Exception ex)
            {
                // ERROR LOG
                FileStream fs = new FileStream(ServerLogFile, FileMode.Append, FileAccess.Write, FileShare.Write);
                fs.Close();
                StreamWriter sw = new StreamWriter(ServerLogFile, true, Encoding.ASCII);
                sw.WriteLine(ex.ToString() + System.DateTime.Now.ToString());
                sw.Close();
                msg = ex.Message;
            }
            return msg;
        }

    }

}
