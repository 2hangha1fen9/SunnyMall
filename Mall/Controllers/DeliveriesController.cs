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

        [AdminAuthentication]
        public ActionResult Index(int? id = 1, string key = "")
        {
            var deliveries = bll.ListEntity(key);
            if(key.Length > 0)
            {
                TempData["Message"] = $"检索到{deliveries.Count()}条数据";
            }
            TempData["Search"] = key;
            return View(deliveries.ToPagedList(id.Value, 10));
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

        
        [AdminAuthentication]
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
            int uid = MyAuthentication.GetUserID();
            Users user = ull.FindEntityById(uid);
            ViewBag.Deliveries = user.Deliveries.OrderByDescending(d => d.DeliveryID == user.DeliveryID);
            ViewBag.User = user;
            if (id.HasValue)
            {
                ViewBag.Deliverie = bll.FindEntityById(id.Value);
            }
            return View();
        }

        [HttpPost]
        [UserAuthentication]
        public ActionResult Create(Deliveries n)
        {
            int uid = MyAuthentication.GetUserID();
            Users user = ull.FindEntityById(uid);
            if(user != null)
            {
                n.UserID = user.UserID;
                if(n.DeliveryID > 0)
                {
                    if (bll.UpdateEntity(n))
                    {
                        TempData["Message"] = "更新成功";
                    }
                    else
                    {
                        TempData["Message"] = "更新失败";
                    }
                }
                else if (bll.AddEntity(n) != null)
                {
                    TempData["Message"] = "添加成功";
                }
                else
                {
                    TempData["Message"] = "添加失败";
                }
            }
            return RedirectToAction("Create");
        }

        [UserAuthentication]
        public ActionResult SetDefault(int? id)
        {
            if (id.HasValue)
            {
                int uid = MyAuthentication.GetUserID();
                Users user = ull.FindEntityById(uid);
                user.DeliveryID = id.Value;
                if (ull.UpdateEntity(user))
                {
                    TempData["Message"] = "设置成功";
                }
                else
                {
                    TempData["Message"] = "设置失败";
                }
            }
            return RedirectToAction("Create");
        }

        [UserAuthentication]
        public ActionResult Remove(int? id)
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
                TempData["Message"] = "无效的ID列表";
            }
            return RedirectToAction("Create");
        }
    }
}