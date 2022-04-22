using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Mall
{
    public class MyAuthentication
    {
        public static bool isRedirect = false;
        /// <summary>
        /// 获取登录凭据
        /// </summary>
        /// <returns></returns>
        public static HttpCookie GetAuthCookie()
        {
            return HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
        }

        /// <summary>
        /// 设置登录凭证
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="userID"></param>
        /// <param name="auth"></param>
        public static void SetAuthCookie(string userName,string userID,string auth = "-1")
        {
            //拼接用户数据
            string userData = $"{userID}#{auth}";
            //构造用户凭据
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, userName, DateTime.Now, DateTime.Now.AddMinutes(60), false, userData);
            //加密用户凭据
            string enyTicket = FormsAuthentication.Encrypt(ticket);
            //将加密后的凭据添加到cookie
            HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,enyTicket);
            HttpContext.Current.Response.Cookies.Add(cookie);  
        }

        /// <summary>
        /// 判断用户是否登录
        /// </summary>
        /// <returns></returns>
        public static bool IsLogin()
        {
            return GetAuthCookie() != null;
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        public static void LoginOut()
        {
            FormsAuthentication.SignOut();
        }

        /// <summary>
        /// 从凭据中获取用户名
        /// </summary>
        /// <returns></returns>
        public static string GetUserName()
        {
            if (IsLogin())
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(GetAuthCookie().Value);
                return ticket.Name;
            }
            else
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 从凭据中获取用户ID
        /// </summary>
        /// <returns></returns>
        public static int GetUserID()
        {
            if (IsLogin())
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(GetAuthCookie().Value);
                string[] userData = ticket.UserData.Split('#');     
                if(userData.Length > 0){
                    return int.Parse(userData[0]);
                }
                else return 0;
            }else return 0;
        }

        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <returns></returns>
        public static string GetAuth()
        {
            if (IsLogin())
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(GetAuthCookie().Value);
                string[] userData = ticket.UserData.Split('#');
                if (userData.Length > 0)
                {
                    return userData[1];
                }else return string.Empty;
            }else return string.Empty;
        }

        /// <summary>
        /// 获取用户id
        /// </summary>
        /// <returns></returns>
        public static Users GetUser()
        {
            using(Entities context = new Entities())
            {
                return context.Users.Find(GetUserID());
            }
        }
    }
   

    public class AdminAuthentication : AuthorizeAttribute
    { 
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!MyAuthentication.IsLogin() || (MyAuthentication.GetAuth() != "1" && MyAuthentication.GetAuth() != "0"))
            {
                HttpContext.Current.Response.Redirect("~/Admin/Login", true);
            }
        }
    }

    public class UserAuthentication : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (!MyAuthentication.IsLogin() || (MyAuthentication.GetAuth() != "-1" && MyAuthentication.GetAuth() != "1" && MyAuthentication.GetAuth() != "0"))
            {
                HttpContext.Current.Response.Redirect("~/Home/Login", true);
            }
        }
    }
}