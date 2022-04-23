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
using Webdiyer.WebControls.Mvc;

namespace Mall.Controllers
{
    public class AdminController : Controller
    {
        private AdminUsersBLL bll = new AdminUsersBLL();
        private OrdersBLL oll = new OrdersBLL();
        private ProductsBLL pll = new ProductsBLL();
        private UsersBLL ull = new UsersBLL();

        // GET: Admin
        [AdminAuthentication]
        public ActionResult Index()
        {
            DateTime d = DateTime.Now;
            d.AddDays(-1);
            ViewBag.prepay = oll.ListEntityByCondition(o => o.States == 0).Count();
            ViewBag.presend = oll.ListEntityByCondition(o => o.States == 1).Count();
            ViewBag.send = oll.ListEntityByCondition(o => o.States == 2).Count();
            ViewBag.success = oll.ListEntityByCondition(o => o.States == 3).Count();
            ViewBag.complete = oll.ListEntityByCondition(o => o.States == 4).Count();
            ViewBag.up = pll.ListEntityByCondition(p => p.States == 1).Count();
            ViewBag.down = pll.ListEntityByCondition(p => p.States == 0).Count();
            ViewBag.warn = pll.ListEntityByCondition(p => p.Stock > 10).Count();
            ViewBag.count = pll.ListEntity().Count();
            ViewBag.today = ull.ListEntityByCondition(u => u.RegisterDate.Day == DateTime.Now.Day).Count();
            ViewBag.yesterday = ull.ListEntityByCondition(u => u.RegisterDate.Day == d.Day).Count();
            ViewBag.month = ull.ListEntityByCondition(u => u.RegisterDate.Month == DateTime.Now.Month).Count();
            ViewBag.count = ull.ListEntity().Count();
            return View();
        }

        // GET: Admin/Login
        public ActionResult Login()
        {
            return View();
        }

        [AdminAuthentication]
        public ActionResult Manager(int? id = 1, string key = "")
        {
            var users = bll.ListEntity(key);
            if(key.Length > 0)
            { 
                TempData["Message"] = $"检索到{users.Count()}条数据";
            }
            TempData["Search"] = key;
            return View(users.ToPagedList(id.Value, 10));
        }

        [AdminAuthentication]
        public ActionResult Update(int? id)
        {
            if (id.HasValue)
            {
                return View(bll.FindEntityById(id.Value));
            }
            return View();
        }

        [HttpPost]
        [AdminAuthentication]
        public ActionResult Update(AdminUsers n, int? id)
        {
            if (id.HasValue)
            {
                if (ModelState.IsValid && bll.UpdateEntity(n))
                {
                    TempData["Message"] = "更新成功";
                }
                else
                {
                    TempData["Message"] = "更新失败";
                    return View();
                }
            }
            else
            {
                if (ModelState.IsValid && bll.AddEntity(n) != null)
                {
                    TempData["Message"] = "提交成功";
                }
                else
                {
                    TempData["Message"] = "提交失败";
                    return View();
                }
            }
            return RedirectToAction("Manager");
        }

        [AdminAuthentication]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bll.DeleteEntityById(id.Value))
            {
                TempData["Message"] = "删除成功";

            }
            else
            {
                TempData["Message"] = "无效的ID";
            }

            return RedirectToAction("Manager");
        }

        [HttpPost]
        public ActionResult Delete(string ids)
        {
            if (ids.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bll.DeleteEntityByIdList(ids))
            {
                TempData["Message"] = "删除成功";
            }
            else
            {
                TempData["Message"] = "无效的ID列表";
            }
            return RedirectToAction("Manager");
        }


        // POST: Admin/Login
        [HttpPost]
        public ActionResult Login(AdminUsers u)
        {
            if (ModelState.IsValid)
            {
                AdminUsers user = bll.FindEntityByCondition(model => model.UserName == u.UserName && model.Pwd == u.Pwd && u.Role != -9);
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
