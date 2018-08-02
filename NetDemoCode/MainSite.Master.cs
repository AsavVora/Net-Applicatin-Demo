using BAL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace PMS
{
    public partial class MainSite : System.Web.UI.MasterPage
    {
        string ChildStrHtml = "";
        string SubChildMenu = "";
        static int flaga = 0;
        static int flagb = 0;
        bool UserTypeRightFlag = false;
        bool UserTypeLeftFlag = false;
        string path = "";
        string getname = "";
        string[] last;
        string url = "";
        string ItHasUserTypeRights = "0";

        protected void Page_Load(object sender, EventArgs e)
        {
            if (HttpContext.Current.Request.Cookies["UserId"] != null)
                Session["UserId"] = HttpContext.Current.Request.Cookies["UserId"].Value;
            if (HttpContext.Current.Request.Cookies["UserName"] != null)
                Session["UserName"] = HttpContext.Current.Request.Cookies["UserName"].Value;
             
            if (Session["UserId"] != null)
            {
                if (!IsPostBack)
                {
                    //headerUserInfo.InnerHtml = Session["UserName"].ToString();
                    //// sideUserInfo.InnerHtml = Session["UserName"].ToString();
                    UserTypeLeftFlag = false;
                    UserTypeRightFlag = false;
                    flaga = 0;
                    flagb = 0;
                    BindMenu();
                    BindMenu2();
                }

                //if (HttpContext.Current.Request.Cookies["Color"].Value.ToString() != "" && HttpContext.Current.Request.Cookies["Color"].Value.ToString() != null)
                //{
                //    theme.Attributes.Add("class", "");
                //    theme.Attributes.Add("class", "theme-" + HttpContext.Current.Request.Cookies["Color"].Value.ToString());
                //}

                if (HttpContext.Current.Request.Cookies["UserImage"].Value.ToString() != "" && HttpContext.Current.Request.Cookies["UserImage"].Value.ToString() != null)
                {
                    userimage.Src = "savedimages/" + HttpContext.Current.Request.Cookies["UserImage"].Value.ToString();
                }
                else
                {
                    userimage.Src = "savedimages/NoImage.png";
                }

            }
            else
                Response.Redirect("Login.aspx");
        }

        private void BindMenu()
        {
            try
            {
                string path = Page.ResolveClientUrl("~/AdminDashBoard.aspx");

                //String Main = "";
                string menuhtml = "";
                menuhtml = menuhtml + "<ul class='nav navbar-nav-custom'><li><a href='javascript:void(0)' onclick='App.sidebar(\"toggle-sidebar\");'>"
                                    + "<i class='fa fa-ellipsis-v fa-fw animation-fadeInRight' id='sidebar-toggle-mini'></i>"
                                    + "<i class='fa fa-bars fa-fw animation-fadeInRight' id='sidebar-toggle-full'></i></a></li><li ><a href='Dashboard.aspx'><span>Dashboard</span></a></li>";
                DataTable dt = clsMethods.GetMenus(0, HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "1");
                if (dt.Rows.Count == 0)
                {
                    ItHasUserTypeRights = "1";
                    dt = clsMethods.GetMenusByUserType(0, HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "1");
                    UserTypeRightFlag = true;
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ChildStrHtml = "";
                    menuhtml = menuhtml + FillChildMenu(dt.Rows[i]["DisplayName"].ToString(), dt.Rows[i]["PageName"].ToString(), dt.Rows[i]["Link"].ToString(), url, dt.Rows[i]["ImageFile"].ToString()) + "</li>";
                }
                menuhtml = menuhtml + "</ul>";
                SideBarDiv.InnerHtml = menuhtml;
            }
            catch (Exception ex)
            {

            }
        }

        private string FillChildMenu(string ParentDisplayName, string PageName, string ParentId, string url, string ImageName)
        {
            DataTable dt = new DataTable();

            if (UserTypeRightFlag)
                dt = clsMethods.GetMenusByUserType(Convert.ToInt32(ParentId), HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "1");
            else
                dt = clsMethods.GetMenus(Convert.ToInt32(ParentId), HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "1");

            if (dt.Rows.Count > 0)
                ChildStrHtml = ChildStrHtml + "<li><a href='#' data-toggle='dropdown' class='dropdown-toggle'><span class='" + ImageName + "'></span>" + ParentDisplayName + "<span class='caret'></span></a>";
            else
                return "<li><a href='" + PageName + "'>" + ParentDisplayName + "</span></a></li>";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                    ChildStrHtml = ChildStrHtml + "<ul class='dropdown-menu'>";
                if (dt.Rows[i]["ItHasSub"].ToString() == "True")
                {
                    ChildStrHtml = ChildStrHtml + CheckChildMenu(dt.Rows[i]["Link"].ToString(), dt.Rows[i]["DisplayName"].ToString(), dt.Rows[i]["ImageFile"].ToString());
                }
                else
                    ChildStrHtml = ChildStrHtml + "<li><a href='" + dt.Rows[i]["PageName"].ToString() + "'><span class='" + dt.Rows[i]["ImageFile"].ToString() + "'></span><span lang='en'>" + dt.Rows[i]["DisplayName"].ToString() + "</span></a></li>";
                if (i == dt.Rows.Count - 1)
                {
                    ChildStrHtml = ChildStrHtml + "</li></ul>";
                }
            }
            return ChildStrHtml;
        }

        private string CheckChildMenu(string ParentId, string DispalyName, string ImageFile)
        {
            SubChildMenu = "";
            DataTable dt = new DataTable();
            if (UserTypeRightFlag)
                dt = clsMethods.GetMenusByUserType(Convert.ToInt32(ParentId), HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "1");
            else
                dt = clsMethods.GetMenus(Convert.ToInt32(ParentId), HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "1");

            if (dt.Rows.Count > 0)
                SubChildMenu = SubChildMenu + "<li class='dropdown-submenu'><a href='#' class='dropdown-toggle' data-toggle='dropdown'><span class='" + ImageFile + "'></span>" + DispalyName + "</a>";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                    SubChildMenu = SubChildMenu + "<ul class='dropdown-menu'>";

                SubChildMenu = SubChildMenu + "<li><a href='" + dt.Rows[i]["PageName"].ToString() + "'><span class='" + dt.Rows[i]["ImageFile"].ToString() + "'></span><span lang='en'>" + dt.Rows[i]["DisplayName"].ToString() + "</span></a></li>";
                if (i == dt.Rows.Count - 1)
                {
                    SubChildMenu = SubChildMenu + "</li></ul>";
                }
            }

            return SubChildMenu;
        }

        private void BindMenu2()
        {
            try
            {
                string path = Page.ResolveClientUrl("~/AdminDashBoard.aspx");
                getname = HttpContext.Current.Request.Url.AbsolutePath;
                last = getname.Split('/');
                url = last[last.Length - 1];
                //String Main = "";
                string menuhtml = "";
                menuhtml = menuhtml + " <div>";
                DataTable dt = clsMethods.GetMenus(0, HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "0");
                if (dt.Rows.Count == 0)
                {
                    if (ItHasUserTypeRights == "1")
                    {
                        dt = clsMethods.GetMenusByUserType(0, HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "0");
                        UserTypeLeftFlag = true;
                    }
                }


                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ChildStrHtml = "";
                    menuhtml = menuhtml + FillChildMenu2(dt.Rows[i]["DisplayName"].ToString(), dt.Rows[i]["PageName"].ToString(), dt.Rows[i]["Link"].ToString(), url, dt.Rows[i]["ImageFile"].ToString());
                }
                menuhtml = menuhtml + "</div>";
                leftSidebarDiv.InnerHtml = menuhtml;
            }
            catch (Exception ex)
            {

            }
        }

        private string FillChildMenu2(string ParentDisplayName, string PageName, string ParentId, string url, string ImageName)
        {
            flaga = 0;
            DataTable dt = new DataTable();

            if (UserTypeLeftFlag)
                dt = clsMethods.GetMenusByUserType(Convert.ToInt32(ParentId), HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "0");
            else
                dt = clsMethods.GetMenus(Convert.ToInt32(ParentId), HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "0");



            if (dt.Rows.Count > 0)
            {
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    if (url.Contains(dt.Rows[j]["PageName"].ToString()))
                    {
                        ChildStrHtml = ChildStrHtml + "<ul class='sidebar-nav'><li class='active'><a href='#' class='sidebar-nav-menu'><i class='fa fa-chevron-left sidebar-nav-indicator sidebar-nav-mini-hide'></i><i class='" + ImageName + "'></i><span class='sidebar-nav-mini-hide MenuStyle'>" + ParentDisplayName + "</span></a>";
                        flaga = 1;
                    }
                    else
                    {
                        DataTable dt123 = clsMethods.GetMenus(Convert.ToInt32(dt.Rows[j]["Link"].ToString()), HttpContext.Current.Request.Cookies["UserId"].Value.ToString(), HttpContext.Current.Request.Cookies["UserType"].Value.ToString(), "0");

                        if (dt123.Rows.Count > 0)
                        {

                            for (int Z = 0; Z < dt123.Rows.Count; Z++)
                            {
                                if (url.Contains(dt123.Rows[Z]["PageName"].ToString()))
                                {
                                    ChildStrHtml = ChildStrHtml + "<ul class='sidebar-nav'><li class='active'><a href='#' class='sidebar-nav-menu'><i class='fa fa-chevron-left sidebar-nav-indicator sidebar-nav-mini-hide'></i><i class='" + ImageName + "'></i><span class='sidebar-nav-mini-hide MenuStyle'>" + ParentDisplayName + "</span></a>";
                                    flaga = 1;
                                }
                            }
                        }
                    }
                }

                if (flaga == 0)
                    ChildStrHtml = ChildStrHtml + "<ul class='sidebar-nav'><li><a href='#' class='sidebar-nav-menu'><i class='fa fa-chevron-left sidebar-nav-indicator sidebar-nav-mini-hide'></i><i class='" + ImageName + "'></i><span class='sidebar-nav-mini-hide MenuStyle'>" + ParentDisplayName + "</span></a>";

            }
            else
            {
                if (url.Contains(PageName))
                {
                    return "<ul class='sidebar-nav'><li><a class='active' href='" + PageName + "'><i class='" + ImageName + "'></i><span class='sidebar-nav-mini-hide'>" + ParentDisplayName + "</span></a></li></ul>";
                }
                return "<ul class='sidebar-nav'><li><a href='" + PageName + "'><i class='" + ImageName + "'></i><span class='sidebar-nav-mini-hide'>" + ParentDisplayName + "</span></a></li></ul>";
            }


            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                    ChildStrHtml = ChildStrHtml + "<ul>";
                if (dt.Rows[i]["ItHasSub"].ToString() == "1")
                {
                    ChildStrHtml = ChildStrHtml + CheckChildMenu2(dt.Rows[i]["Link"].ToString(), dt.Rows[i]["DisplayName"].ToString(), dt.Rows[i]["ImageFile"].ToString());

                }
                else
                {
                    if (url.Contains(dt.Rows[i]["PageName"].ToString()))
                    {
                        ChildStrHtml = ChildStrHtml + "<li><a  class='active' href='" + dt.Rows[i]["PageName"].ToString() + "'>" + dt.Rows[i]["DisplayName"].ToString() + "</a></li>";
                    }
                    else
                    {
                        ChildStrHtml = ChildStrHtml + "<li><a href='" + dt.Rows[i]["PageName"].ToString() + "'>" + dt.Rows[i]["DisplayName"].ToString() + "</a></li>";
                    }

                }
                if (i == dt.Rows.Count - 1)
                {
                    ChildStrHtml = ChildStrHtml + "</ul>";
                }
            }
            ChildStrHtml += "</li></ul>";
            return ChildStrHtml;
        }

        private string CheckChildMenu2(string ParentId, string DispalyName, string ImageFile)
        {
            flagb = 0;
            SubChildMenu = "";
            DataTable dt = new DataTable();
            //DataTable dt = clsMethods.GetMenus(Convert.ToInt32(ParentId), HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "0");
            if (UserTypeLeftFlag)
                dt = clsMethods.GetMenusByUserType(Convert.ToInt32(ParentId), HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "0");
            else
                dt = clsMethods.GetMenus(Convert.ToInt32(ParentId), HttpContext.Current.Request.Cookies["UserId"].Value, HttpContext.Current.Request.Cookies["UserType"].Value, "0");



            if (dt.Rows.Count > 0)
            {

                for (int j = 0; j < dt.Rows.Count; j++)
                {

                    if (url.Contains(dt.Rows[j]["PageName"].ToString()))
                    {
                        SubChildMenu = SubChildMenu + "<li><a href='#' class='sidebar-nav-submenu open'><i class='fa fa-chevron-left sidebar-nav-indicator'></i><i class='" + ImageFile + "'></i><span class=''>" + DispalyName + "</span></a>";
                        flagb = 1;
                    }
                }

                if (flagb == 0)
                {
                    SubChildMenu = SubChildMenu + "<li><a href='#' class='sidebar-nav-submenu'><i class='fa fa-chevron-left sidebar-nav-indicator'></i><i class='" + ImageFile + "'></i><span class=''>" + DispalyName + "</span></a>";
                }
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (i == 0)
                    SubChildMenu = SubChildMenu + "<ul>";

                if (url.Contains(dt.Rows[i]["PageName"].ToString()))
                {
                    SubChildMenu = SubChildMenu + "<li><a class='active' href='" + dt.Rows[i]["PageName"].ToString() + "'><span class='" + dt.Rows[i]["ImageFile"].ToString() + "'></span><span lang='en'>" + dt.Rows[i]["DisplayName"].ToString() + "</span></a></li>";
                }
                else
                {
                    SubChildMenu = SubChildMenu + "<li><a href='" + dt.Rows[i]["PageName"].ToString() + "'><span class='" + dt.Rows[i]["ImageFile"].ToString() + "'></span><span lang='en'>" + dt.Rows[i]["DisplayName"].ToString() + "</span></a></li>";
                }



                if (i == dt.Rows.Count - 1)
                {
                    SubChildMenu = SubChildMenu + "</ul>";
                }
            }

            SubChildMenu += "</li>";

            return SubChildMenu;
        }

    }
}