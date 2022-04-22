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
    public class ProductsController : Controller
    {
        private ProductsBLL bll = new ProductsBLL();
        private CategoriesBLL cll = new CategoriesBLL();
        private PhotosBLL pll = new PhotosBLL();

        [AdminAuthentication]
        public ActionResult Index(int? id = 1, string key = "")
        {
            if (key.Length == 0)
            {
                return View(bll.ListEntityByPage(id));
            }
            var products = bll.FindEntityByPage(id, key);
            TempData["Search"] = key;
            TempData["Message"] = $"检索到{products.Count()}条数据";
            return View(products);
        }

        [UserAuthentication]
        [AdminAuthentication]
        public ActionResult Details(int? id)
        {
            return View();
        }

        [ValidateInput(false)]
        [AdminAuthentication]
        public ActionResult Update(int? id)
        {
            ViewBag.Categories = cll.ListEntity();
            if (id.HasValue)
            {
                return View(bll.FindEntityById(id.Value));
            }
            return View();
        }

        [HttpPost]
        [AdminAuthentication]
        [ValidateInput(false)]
        public ActionResult Update(Products n, int? id)
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
                    TempData["Message"] = "发布成功";
                }
                else
                {
                    TempData["Message"] = "发布失败";
                    return View();
                }
            }
            return RedirectToAction("Index");
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

        [HttpPost]
        [AdminAuthentication]
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
        public ActionResult States(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products news = bll.FindEntityById(id.Value);
            if (news == null)
            {
                TempData["Message"] = "无效的ID";
            }
            news.States = news.States == 0 ? 1 : 0;
            if (bll.UpdateEntity(news))
            {
                TempData["Message"] = "操作成功";
            }
            return RedirectToAction("Index");
        }

        [AdminAuthentication]
        public ActionResult Photos(int? id)
        {
            if (id.HasValue)
            {
                return View(bll.FindEntityById(id.Value));
            }
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        [AdminAuthentication]
        public ActionResult DeletePhotos(int? id)
        {
            if (!id.HasValue && Request.QueryString["id"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (pll.DeleteEntityById(int.Parse(Request.QueryString["id"])))
            {
                TempData["Message"] = "删除成功";
            }
            else
            {
                TempData["Message"] = "无效的ID";
            }
            return RedirectToAction("Photos", new { id = id.Value });
        }

        [AdminAuthentication]
        public ActionResult TopPhotos(int? id)
        {
            if (!id.HasValue && Request.QueryString["id"] == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Photos p = pll.FindEntityById(int.Parse(Request.QueryString["id"]));
            if (p == null)
            {
                TempData["Message"] = "无效的ID";
            }
            p.States = p.States == 0 ? 1 : 0;
            if (pll.UpdateEntity(p))
            {
                TempData["Message"] = "操作成功";
            }
 
            return RedirectToAction("Photos", new { id = id.Value });
        }
    }
}