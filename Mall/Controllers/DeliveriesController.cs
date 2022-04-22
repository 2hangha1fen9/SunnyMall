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
    public class DeliveriesController : Controller
    {
        private DeliveriesBLL bll = new DeliveriesBLL();
        private UsersBLL ull = new UsersBLL();

        [AdminAuthentication]
        public ActionResult Index(int? id = 1)
        {
            return View(bll.ListEntityByPage(id));
        }

        [AdminAuthentication]
        [HttpPost]
        public ActionResult Index(string key)
        {
            if (key.Length == 0)
            {
                return RedirectToAction("Index");
            }
            var deliveries = bll.FindEntity(key);
            TempData["Message"] = $"检索到{deliveries.Count()}条数据";
            return View(deliveries);
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

        [UserAuthentication]
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
        [UserAuthentication]
        [AdminAuthentication]
        [ValidateInput(false)]
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
    }
}