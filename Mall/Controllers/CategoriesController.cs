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
    public class CategoriesController : Controller
    {
        private CategoriesBLL bll = new CategoriesBLL();
        
        // GET: Categories
        [AdminAuthentication]
        public ActionResult Index(int? id=1)
        {
            return View(bll.ListEntityGroupByPage(id));
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

        [AdminAuthentication]
        public ActionResult Delete(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            if (bll.FindEntityById(id.Value).Products.Count() > 0)
            {
                TempData["Message"] = "删除失败,该分类下还有商品,请删除对应商品在进行操作";              
            }
            else if (bll.DeleteEntityById(id.Value))
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
        public ActionResult Update(int? id)
        {
            ViewBag.CateGroups = bll.ListEntityGroup();
            var createsub = Request.QueryString["createsub"];
            if (id.HasValue)
            {
                Categories c = bll.FindEntityById(id.Value);
                return View(c);
            }
            if (createsub != null)
            {
                Categories c = new Categories();
                c.ParentID = int.Parse(createsub);
                return View(c);
            }
            return View();
        }

        [HttpPost]
        [AdminAuthentication]
        public ActionResult Update(Categories c,int? id)
        {
            if (id.HasValue)
            {
                if (ModelState.IsValid && bll.UpdateEntity(c))
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
                if (ModelState.IsValid && bll.AddEntity(c) != null)
                {
                    TempData["Message"] = "添加成功";
                }
                else
                {
                    TempData["Message"] = "添加失败";
                    return View();
                }
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
            Categories cates = bll.FindEntityById(id.Value);
            if (cates == null)
            {
                TempData["Message"] = "无效的ID";
            }
            cates.States = cates.States == 0 ? 1 : 0;
            if (bll.UpdateEntity(cates))
            {
                TempData["Message"] = "操作成功";
            }
            return RedirectToAction("Index");
        }
    }
}