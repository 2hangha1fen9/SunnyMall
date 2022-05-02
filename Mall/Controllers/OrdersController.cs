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
            TempData["States"] = states;
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
            TempData["States"] = states;
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
                if (bll.UpdateEntity(orders))
                {
                    return RedirectToAction("Pay", new { id = orders.OrdersID });
                }
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [UserAuthentication]
        public ActionResult Pay(int id)
        {
            Orders orders = bll.FindEntityById(id);
            if (orders != null)
            {
                orders.States = 1;
                if (bll.UpdateEntity(orders))
                {
                    TempData["PayResult"] = "支付成功";
                }
                else
                {
                    TempData["PayResult"] = "支付失败";
                }
            }
            return View(orders);
        }

        [UserAuthentication]
        public ActionResult Confirm(int id)
        {
            Orders orders = bll.FindEntityById(id);
            orders.States = 3;
            orders.DeliveryDate = DateTime.Now;
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

        [UserAuthentication]
        public ActionResult OrderClose(int? id)
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