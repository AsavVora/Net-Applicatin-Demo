using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PMS.Handlers
{
    /// <summary>
    /// Summary description for FileUploader
    /// </summary>
    public class FileUploader : IHttpHandler
    {

        int mFileSize = 0;
        public void ProcessRequest(HttpContext context)
        {
            try
            {
                string file1 = context.Request.Files[0].FileName;
                string myName = context.Request.Form["firstName"];
                string ServerFileName = Guid.NewGuid().ToString() + Path.GetExtension(context.Request.Files[0].FileName).ToLower();
                //For uploading new File
                string Serverpath = System.Configuration.ConfigurationManager.AppSettings["FolderPath"];
                var postedFile = context.Request.Files[0];
                string filesize = System.Configuration.ConfigurationManager.AppSettings["FileSize"];
                mFileSize = postedFile.ContentLength / 1048576;

                //if (mFileSize <= Convert.ToInt32(filesize))
                //{
                // Get Server Folder to upload file        
                string Savepath = context.Server.MapPath(Serverpath);
                //string Savepath = Serverpath;
                string file;

                //For IE to get file name
                if (HttpContext.Current.Request.Browser.Browser.ToUpper() == "IE")
                {
                    string[] files = postedFile.FileName.Split(new char[] { '\\' });
                    file = files[files.Length - 1];

                }
                //For Other Browser to get file name
                else
                {
                    file = postedFile.FileName;
                }

                string fileDirectory = Savepath + "\\" + ServerFileName;
                postedFile.SaveAs(fileDirectory);

                string promsg = "";

                //Set response message
                string msg = "{";
                msg += string.Format("error:'{0}',\n", promsg);
                msg += string.Format("upfile:'{0}'\n", ServerFileName);
                msg += "}";

                context.Response.Write(ServerFileName);
                //}

            }
            catch (Exception ex)
            {
                //context.Response.Write("Error: " + ex.Message);
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}