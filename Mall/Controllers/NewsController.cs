using BLL;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;

namespace Mall.Controllers
{
    public class NewsController : Controller
    {
        private NewsBLL bll = new NewsBLL();

        private List<News> GetNews(string key = "")
        {
            TempData["Search"] = key;
            return bll.ListEntity(key);
        }

        [AdminAuthentication]
        public ActionResult Index(int? id = 1, string key = "")
        {
            var news = GetNews(key);
            if (key.Length > 0)
            {
                TempData["Message"] = $"检索到{news.Count()}条数据";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Index", news.ToPagedList(id.Value, 10));
            }
            return View(news.ToPagedList(id.Value,10));
        }

        public ActionResult List(int? id = 1,string key="")
        {
            return Index(id,key);
        }

        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(bll.FindEntityById(id.Value));
        }

        [ValidateInput(false)]
        [AdminAuthentication]
        public ActionResult Update(int? id)
        {
            if (id.HasValue)
            {
                return View(bll.FindEntityById(id.Value));
            }
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [AdminAuthentication]
        public ActionResult Update(News n, int? id)
        {
            n.PhotoUrl = TempData["FileName"] == null ? n.PhotoUrl : TempData["FileName"].ToString();
            TempData["FileName"] = null;
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
        public ActionResult Delete(int? id,int? pageIndex = 1,string key = "")
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bll.DeleteEntityById(id.Value))
            {
                return PartialView("_Index", GetNews(key).ToPagedList(pageIndex.Value, 10));
            }
            else
            {
                TempData["Message"] = "无效的ID";
            }
            
            return RedirectToAction("Index");
        }  

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
                return PartialView("_Index", GetNews(key).ToPagedList(int.Parse(pageIndex), 10));
            }
            else
            {
                TempData["Message"] = "无效的ID列表";
            }
            return RedirectToAction("Index");
        }

        [AdminAuthentication]
        public ActionResult States(int? id, int? pageIndex = 1, string key = "")
        {
            if(!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = bll.FindEntityById(id.Value);
            if(news == null)
            {
                TempData["Message"] = "无效的ID";
            }
            news.States = news.States == 0 ? 1 : 0;
            if (bll.UpdateEntity(news))
            {
                return PartialView("_Index", GetNews(key).ToPagedList(pageIndex.Value, 10));
            }
            return RedirectToAction("Index");
        }   
    }
}