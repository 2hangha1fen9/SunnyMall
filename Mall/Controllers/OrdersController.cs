using BLL;
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
        private OrdersDetailsBLL ordersDetailsBLL = new OrdersDetailsBLL();

        private IEnumerable<Orders> GetOrders(int? states = null, string key = "")
        {
            TempData["Search"] = key;
            TempData["States"] = states;
            int uid = MyAuthentication.GetUserID();
            return bll.ListEntity(key, states).Where(o => o.UserID == uid);
        }

        private IEnumerable<Orders> GetAllOrders(int? states = null, string key = "")
        {
            TempData["Search"] = key;
            TempData["States"] = states;
            int uid = MyAuthentication.GetUserID();
            return bll.ListEntity(key, states);
        }

        // GET: Orders
        [AdminAuthentication]
        public ActionResult Index(int? id = 1, int? states = null, string key = "")
        {
            var orders = GetAllOrders(states, key);
            if (key.Length > 0)
            {
                TempData["Message"] = $"检索到{orders.Count()}条数据";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Index", orders.ToPagedList(id.Value, 10));
            }
            return View(orders.ToPagedList(id.Value,10));
        }

        [UserAuthentication]
        public ActionResult MyOrder(int? id = 1, int? states = null, string key = "")
        {
            var orders = GetOrders(states, key);
            if (key.Length > 0)
            {
                TempData["Message"] = $"检索到{orders.Count()}条数据";
            }
            
            if (Request.IsAjaxRequest())
            {
                return PartialView("_MyOrder", orders.ToPagedList(id.Value, 10));
            }
            return View(orders.ToPagedList(id.Value, 10));
        }

        /// <summary>
        /// 购物车确认订单
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [UserAuthentication]
        public ActionResult CheckOrder()
        {
            Users user = MyAuthentication.GetUser();
            ViewBag.Deliveries = user.Deliveries.ToList();
            ViewBag.User = user;
            return View(ordersDetailsBLL.CheckDetail(user));
        }

        /// <summary>
        /// GET /Orders/CheckOrder/
        /// 一键购买确认订单
        /// </summary>
        /// <param name="id">产品id</param>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        [UserAuthentication]
        public ActionResult CheckOrder(int pid,int quantity)
        {
            Users user = MyAuthentication.GetUser();
            ViewBag.Deliveries = user.Deliveries.ToList();
            ViewBag.User = user;
            ViewBag.OnKeyBuy = true;
            ViewBag.Quantity = quantity;
            ViewBag.Pid = pid;
            return View(ordersDetailsBLL.CheckDetail(pid, quantity, user));
        }
        
        /// <summary>
        /// 创建订单
        /// </summary>
        /// <param name="id">收货地址</param>
        /// <param name="pid">产品id</param>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        [UserAuthentication]
        public ActionResult CreateOrder(int id,int? pid,int? quantity,string remark = "")
        {
            Users user = MyAuthentication.GetUser();
            Orders orders;
            if (pid.HasValue && quantity.HasValue)
            {
                orders = bll.CreateOrder(user,pid.Value,quantity.Value,id,remark);
            }
            else
            {
                orders = bll.CreateOrder(user, id,remark);
            }
            if(orders != null)
            {
                return RedirectToAction("Cashier", new { id = orders.OrdersID });
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [UserAuthentication]
        public ActionResult Cashier(int id)
        {
            return View(bll.FindEntityById(id));
        }

        [HttpPost]
        [UserAuthentication]
        public ActionResult Cashier(Orders o)
        {
            Orders orders = bll.FindEntityById(o.OrdersID);
            if(orders != null)
            {
                if(o.PayType == 0)
                {
                    return RedirectToAction("Index", "Alipay", orders);
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [UserAuthentication]
        public ActionResult Pay()
        {
            string tradeNo = Request.QueryString["out_trade_no"];
            decimal totalAmount = decimal.Parse(Request.QueryString["total_amount"]);
            Orders order = bll.FindEntityByCondition(o => o.SerialID == tradeNo);
            if (order != null && order.Total.Value == totalAmount)
            {
                order.PayType = 0;
                order.States = 1;
                if (bll.UpdateEntity(order))
                {
                    return View(order);
                }
            }

            return View();
        }

        [UserAuthentication]
        public ActionResult Confirm(int id,int? pageIndex = 1,int? states = null, string key = "")
        {
            Orders orders = bll.FindEntityById(id);
            orders.States = 3;
            orders.DeliveryDate = DateTime.Now;
            if (bll.UpdateEntity(orders))
            {
                return PartialView("_MyOrder", GetOrders(states,key).ToPagedList(pageIndex.Value, 10));
            }
            else
            {
                TempData["Message"] = "无效的ID";
                return RedirectToAction("MyOrder");
            }
            
        }

        [UserAuthentication]
        public ActionResult OrderDelete(int id,int? pageIndex = 1, int? states = null, string key = "")
        {
            if (bll.DeleteEntityById(id))
            {
                return PartialView("_MyOrder", GetOrders(states, key).ToPagedList(pageIndex.Value, 10));
            }
            else
            {
                TempData["Message"] = "无效的ID";
                return RedirectToAction("MyOrder");
            }
        }

        [UserAuthentication]
        public ActionResult OrderClose(int id, int? pageIndex = 1, int? states = null, string key = "")
        {
            Orders order = bll.FindEntityById(id);
            if (order == null)
            {
                TempData["Message"] = "无效的ID";
            }
            order.States = -1;
            if (bll.UpdateEntity(order))
            {
                return PartialView("_MyOrder", GetOrders(states, key).ToPagedList(pageIndex.Value, 10));
            }
            return RedirectToAction("MyOrder");
        }

        [AdminAuthentication]
        public ActionResult Delete(int id, int? pageIndex = 1, int? states = null, string key = "")
        {
            if (bll.DeleteEntityById(id))
            {
                return PartialView("_Index", GetAllOrders(states, key).ToPagedList(pageIndex.Value, 10));
            }
            else
            {
                TempData["Message"] = "无效的ID";
                return RedirectToAction("_Index");
            }
        }

        [AdminAuthentication]
        public ActionResult Send(int id)
        {
            Orders order = bll.FindEntityById(id);
            return View(order);
        }

        [HttpPost]
        [AdminAuthentication]
        public ActionResult Send(Orders o)
        {
            Orders order = bll.FindEntityById(o.OrdersID);
            if(order != null)
            {
                order.ExpressNumber = o.ExpressNumber;
                order.ExpressType = o.ExpressType;
                order.States = 2;
                if(bll.UpdateEntity(order))
                {
                    TempData["Message"] = "发货成功";
                }
                else
                {
                    TempData["Message"] = "发货失败";
                }
            }
            return RedirectToAction("Index");
        }


        public ActionResult OrderDetails(int id)
        {
            return View(bll.FindEntityById(id));
        }
    }
}