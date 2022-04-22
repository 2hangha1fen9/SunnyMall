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
        public ActionResult Index(int? id = 1, int? states = null, string key = "")
        {
            if (key.Length == 0)
            {
                return View(bll.ListEntityByPage(id.Value,states));
            }
            var news = bll.FindEntityByPage(id.Value,key);
            TempData["Search"] = key;
            TempData["Message"] = $"检索到{news.Count()}条数据";
            return View(news);
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

            return RedirectToAction("Index");
        }

        [UserAuthentication]
        [AdminAuthentication]
        public ActionResult Details(int? id)
        {
            return View();
        }

        [UserAuthentication]
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

        [UserAuthentication]
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