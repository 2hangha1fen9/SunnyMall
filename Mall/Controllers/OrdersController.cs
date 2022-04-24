﻿using BLL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Mall.Controllers
{
    public class OrdersController : Controller
    {
        private OrdersBLL bll = new OrdersBLL();

        // GET: Orders
        [AdminAuthentication]
        public ActionResult Index(int? id = 1, int? states = null, string key = "")
        {
            var orders = bll.ListEntity(key,states);
            if (key.Length > 0)
            {
                TempData["Message"] = $"检索到{orders.Count()}条数据";
            }
            TempData["Search"] = key;
            return View(orders.ToPagedList(id.Value,10));
        }

        [UserAuthentication]
        public ActionResult MyOrder(int? id = 1, int? states = null, string key = "")
        {
            int uid = MyAuthentication.GetUserID();
            var orders = bll.ListEntity(key, states).Where(o => o.UserID == uid);
            if (key.Length > 0)
            {
                TempData["Message"] = $"检索到{orders.Count()}条数据";
            }
            TempData["Search"] = key;
            return View(orders.ToPagedList(id.Value, 10));
        }

        [UserAuthentication]
        public ActionResult Pay(int id)
        {
            return View();
        }

        [UserAuthentication]
        public ActionResult Confirm(int id)
        {
            Orders orders = bll.FindEntityById(id);
            orders.States = 3;
            if (bll.UpdateEntity(orders))
            {
                TempData["Message"] = "确认收货成功";
            }
            return RedirectToAction("MyOrder");
        }

        [UserAuthentication]
        public ActionResult OrderDelete(int id)
        {
            if (bll.DeleteEntityById(id))
            {
                TempData["Message"] = "删除成功";
            }
            else
            {
                TempData["Message"] = "无效的ID";
            }
            return RedirectToAction("MyOrder");
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