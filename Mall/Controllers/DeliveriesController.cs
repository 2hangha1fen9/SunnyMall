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
    public class DeliveriesController : Controller
    {
        private DeliveriesBLL bll = new DeliveriesBLL();
        private UsersBLL ull = new UsersBLL();

        private List<Deliveries> GetDeliveries(string key = "")
        {
            TempData["Search"] = key;
            return bll.ListEntity(key);
        }

        [AdminAuthentication]
        public ActionResult Index(int? id = 1, string key = "")
        {
            var deliveries = GetDeliveries(key);
            if(key.Length > 0)
            {
                TempData["Message"] = $"检索到{deliveries.Count()}条数据";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Index", deliveries.ToPagedList(id.Value, 10));
            }
            return View(deliveries.ToPagedList(id.Value, 10));
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
                return PartialView("_Index", GetDeliveries(key).ToPagedList(pageIndex.Value, 10));
            }
            else
            {
                TempData["Message"] = "无效的ID";
            }
            return RedirectToAction("Index");
        }

        
        [AdminAuthentication]
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
                return PartialView("_Index", GetDeliveries(key).ToPagedList(int.Parse(pageIndex), 10));
            }
            else
            {
                TempData["Message"] = "无效的ID列表";
            }
            return RedirectToAction("Index");
        }

        [AdminAuthentication]
        [ValidateInput(false)]
        public ActionResult Update(int? id)
        {
            ViewBag.Users = ull.ListEntity();
            if (id.HasValue)
            {
                return View(bll.FindEntityById(id.Value));
            }
            return View();
        }

        [HttpPost]
        [AdminAuthentication]
        public ActionResult Update(Deliveries n, int? id)
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
                    TempData["Message"] = "创建成功";
                }
                else
                {
                    TempData["Message"] = "创建失败";
                    return View();
                }
            }
            return RedirectToAction("Index");
        }

        [UserAuthentication]
        public ActionResult Create(int? id)
        {
            Users user = FindDeliveries();
            if (id.HasValue)
            {
                ViewBag.Deliverie = bll.FindEntityById(id.Value);
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Create");
            }
            return View();
        }

        private Users FindDeliveries()
        {
            int uid = MyAuthentication.GetUserID();
            Users user = ull.FindEntityById(uid);
            ViewBag.Deliveries = bll.ListEntityByCondition(d => d.UserID == user.UserID).OrderByDescending(d => d.DeliveryID == user.DeliveryID);
            ViewBag.User = user;
            return user;
        }

        [HttpPost]
        [UserAuthentication]
        public ActionResult Create(Deliveries n)
        {
            Users user = FindDeliveries();
            if (user != null)
            {
                n.UserID = user.UserID;
                if(n.DeliveryID > 0)
                {
                    Deliveries d = user.Deliveries.FirstOrDefault(m => m.DeliveryID == n.DeliveryID);
                    if(d != null)
                    {
                        d.Consignee = n.Consignee;
                        d.Complete = n.Complete;
                        d.Phone = n.Phone;

                        if (!bll.UpdateEntity(d))
                        {
                            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                        }
                    }
                    else
                    {
                        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                    }
                }
                else if (bll.AddEntity(n) == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Create");
            }
            return RedirectToAction("Create");
        }

        [UserAuthentication]
        public ActionResult SetDefault(int? id)
        {
            Users user = FindDeliveries();
            if (id.HasValue)
            {
                user.DeliveryID = id.Value;
                if (!ull.UpdateEntity(user))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
            }
            return PartialView("_Create");
        }

        [UserAuthentication]
        public ActionResult Remove(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (!bll.DeleteEntityById(id.Value))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = FindDeliveries();
            return PartialView("_Create");
        }
    }
}