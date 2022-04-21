using BLL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Mall.Controllers
{
    public class OrdersController : Controller
    {
        private OrdersBLL bll = new OrdersBLL();
        // GET: Orders
        [AdminAuthentication]
        public ActionResult Index(int? id, int? states)
        {
            return View(bll.ListEntityByPage(id ?? 1,states));
        }

        
        [HttpPost]
        [AdminAuthentication]
        public ActionResult Index(string key)
        {
            if (key.Length == 0)
            {
                return RedirectToAction("Index");
            }
            var news = bll.FindEntity(key);
            TempData["Message"] = $"检索到{news.Count()}条数据";
            return View(news);
        }

        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            return View();
        }

        [AdminAuthentication]
        public ActionResult Close(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders order = bll.FindEntityById(id.Value);
            if (order == null)
            {
                TempData["Message"] = "无效的ID";
            }
            order.States = -1;
            if (bll.UpdateEntity(order))
            {
                TempData["Message"] = "关闭成功";
            }
            return RedirectToAction("Index");
        }

        [AdminAuthentication]
        public ActionResult Send(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Orders order = bll.FindEntityById(id.Value);
            if (order == null)
            {
                TempData["Message"] = "无效的ID";
            }
            order.States = 2;
            if (bll.UpdateEntity(order))
            {
                TempData["Message"] = "发货成功";
            }
            return RedirectToAction("Index");
        }
    }
}