using BLL;
using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Mall.Controllers
{
    public class UserCenterController : Controller
    {
        UsersBLL usersBLL = new UsersBLL();
        // GET: UserCenter
        [UserAuthentication]
        public ActionResult Index()
        {
            int uid = MyAuthentication.GetUserID();
            OrdersBLL ordersBLL = new OrdersBLL();
            ViewBag.NoPayOrders = ordersBLL.ListEntity("",0).Where(o => o.UserID == uid).ToList();
            ViewBag.NoSendOrders = ordersBLL.ListEntity("",1).Where(o => o.UserID == uid).ToList();
            ViewBag.NoGiveOrders = ordersBLL.ListEntity("",2).Where(o => o.UserID == uid).ToList();
            ViewBag.NoAppraiseOrders = ordersBLL.ListEntity("", 3).Where(o => o.UserID == uid).ToList();
            return View();
        }

        [UserAuthentication]
        public ActionResult MyInfo()
        {
            int uid = MyAuthentication.GetUserID();
            return View(usersBLL.FindEntityById(uid));
        }

        [HttpPost]
        [UserAuthentication]
        public ActionResult MyInfo(Users u)
        {
            int uid = MyAuthentication.GetUserID();
            Users user = usersBLL.FindEntityById(uid);
            if(user != null)
            {
                user.Nick = u.Nick;
                if (usersBLL.UpdateEntity(user))
                {
                    TempData["Message"] = "更新成功";
                }
                else
                {
                    TempData["Message"] = "更新失败";
                }
            }
            return View(user);
        }

        [UserAuthentication]
        public ActionResult ChangePwd()
        {
            int uid = MyAuthentication.GetUserID();
            return View();
        }

        [HttpPost]
        [UserAuthentication]
        public ActionResult ChangePwd(Users u,string newPwd)
        {
            int uid = MyAuthentication.GetUserID();
            Users user = usersBLL.FindEntityById(uid);
            if (user != null && user.Pwd == u.Pwd)
            {
                user.Pwd = newPwd;
                if (usersBLL.UpdateEntity(user))
                {
                    TempData["Message"] = "修改成功";
                }
                else
                {
                    TempData["Message"] = "修改失败";
                }
            }
            else
            {
                TempData["Message"] = "原密码错误";
            }
            return View();
        }


        public ActionResult Activation(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = usersBLL.FindEntityByCondition(u => u.ActivationCode == id);
            if (user != null)
            {
                user.States = 2;
                if (usersBLL.UpdateEntity(user))
                {

                    TempData["Message"] = "激活成功";
                }
                else
                {
                    TempData["Message"] = "激活失败";
                }
                return RedirectToAction("Login", "Home");
            }
            ContentResult result = new ContentResult();
            result.Content = "激活失败,URL非法";
            result.ContentType = "text/html";
            return result;
        }
    }
}