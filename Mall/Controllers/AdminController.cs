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

        private List<AdminUsers> GetAdmin(string key = "")
        {
            TempData["Search"] = key;
            return bll.ListEntity(key);
        }

        // GET: Admin
        [AdminAuthentication]
        public ActionResult Index()
        {
            DateTime d = DateTime.Now;
            d.AddDays(-1);
            DateTime d2 = DateTime.Now.AddDays(0 - Convert.ToInt16(DateTime.Now.DayOfWeek));
            DateTime d3 = DateTime.Now.AddDays(6 - Convert.ToInt16(DateTime.Now.DayOfWeek));
            ViewBag.prepay = oll.ListEntityByCondition(o => o.States == 0).Count();
            ViewBag.presend = oll.ListEntityByCondition(o => o.States == 1).Count();
            ViewBag.send = oll.ListEntityByCondition(o => o.States == 2).Count();
            ViewBag.success = oll.ListEntityByCondition(o => o.States == 3).Count();
            ViewBag.complete = oll.ListEntityByCondition(o => o.States == 4).Count();
            ViewBag.up = pll.ListEntityByCondition(p => p.States == 1).Count();
            ViewBag.down = pll.ListEntityByCondition(p => p.States == 0).Count();
            ViewBag.warn = pll.ListEntityByCondition(p => p.Stock > 10).Count();
            ViewBag.ProductCount = pll.ListEntity().Count();
            ViewBag.today = ull.ListEntityByCondition(u => u.RegisterDate.Value.Day == DateTime.Now.Day).Count();
            ViewBag.yesterday = ull.ListEntityByCondition(u => u.RegisterDate.Value.Day == d.Day).Count();
            ViewBag.month = ull.ListEntityByCondition(u => u.RegisterDate.Value.Month == DateTime.Now.Month).Count();
            ViewBag.count = ull.ListEntity().Count();
            ViewBag.SalesToday = (oll.ListEntityByCondition(o => o.Orderdate.Day == DateTime.Now.Day).Sum(o => o.Total) ?? 0).ToString("0.00");
            ViewBag.SalesYesterday = (oll.ListEntityByCondition(o => o.Orderdate.Day == d.Day).Sum(o => o.Total) ?? 0).ToString("0.00");
            ViewBag.SalesWeek = (oll.ListEntityByCondition(o => o.Orderdate.Day >= d2.Day && o.Orderdate.Day <= d3.Day).Sum(o => o.Total) ?? 0).ToString("0.00");
            ViewBag.SalesMonth = (oll.ListEntityByCondition(o => o.Orderdate.Month == DateTime.Now.Month).Sum(o => o.Total) ?? 0).ToString("0.00");
            ViewBag.SalesYear = (oll.ListEntityByCondition(o => o.Orderdate.Year == DateTime.Now.Year).Sum(o => o.Total) ?? 0).ToString("0.00");
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
            var users = GetAdmin(key);
            if(key.Length > 0)
            { 
                TempData["Message"] = $"检索到{users.Count()}条数据";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Manager", users.ToPagedList(id.Value, 10));
            }
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
        public ActionResult Delete(int? id, int? pageIndex = 1, string key = "")
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bll.DeleteEntityById(id.Value))
            {
                return PartialView("_Manager", GetAdmin(key).ToPagedList(pageIndex.Value, 10));
            }
            else
            {
                TempData["Message"] = "无效的ID";
            }

            return RedirectToAction("Manager");
        }

        [HttpPost]
        public ActionResult Delete(string key = "")
        {
            string ids = Request.Form["ids"];
            string pageIndex = Request.Form["pageIndex"];
            if (ids.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bll.DeleteEntityByIdList(ids))
            {
                return PartialView("_Manager", GetAdmin(key).ToPagedList(int.Parse(pageIndex), 10));
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
