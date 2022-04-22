using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BLL;
using Models;

namespace Mall.Controllers
{
    public class AdminController : Controller
    {
        private AdminUsersBLL bll = new AdminUsersBLL();

        // GET: Admin
        [AdminAuthentication]
        public ActionResult Index()
        {
            return View();
        }

        // GET: Admin/Login
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login
        [HttpPost]
        public ActionResult Login(AdminUsers u)
        {
            if (ModelState.IsValid)
            {
                AdminUsers user = bll.FindEntityByCondition(model => model.UserName == u.UserName && model.Pwd == u.Pwd);
                if (user != null)
                {
                    // 设置权限
                    MyAuthentication.SetAuthCookie(user.UserName, user.AdminID.ToString(), user.Role.ToString());
                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("","账号名或密码错误");
            }
            return View();
        }

        // GET: Admin/LoginOut
        public ActionResult LoginOut()
        {
            MyAuthentication.LoginOut();
            return RedirectToAction("Index");
        }
    }
}
