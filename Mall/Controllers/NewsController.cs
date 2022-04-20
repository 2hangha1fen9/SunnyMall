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
        // GET: News
        [AdminAuthentication]
        public ActionResult Index(int? id=1)
        {
            return View(bll.ListEntity().ToList().AsQueryable().OrderByDescending(n => n.PushTime).OrderByDescending(n => n.States).ToPagedList(id ?? 1,10));
        }

        [AdminAuthentication]
        [HttpPost]
        public ActionResult Index(string key)
        {
            if(key.Length == 0)
            {
                return Index();
            }
            var news = bll.ListEntity().Where(n => n.Title.Contains(key) || n.NType.Contains(key) || n.Content.Contains(key) || n.States == (key == "置顶" ? 1 : -1) || n.States == (key == "未置顶" ? 0 : -1));
            TempData["Message"] = $"检索到{news.Count()}条数据";
            return View(news.ToList().OrderByDescending(n => n.PushTime).OrderByDescending(n => n.States).ToPagedList(1,news.Count()));
        }

        [AllowAnonymous]
        public ActionResult Details(int? id)
        {
            return View();
        }

        [AdminAuthentication]
        [ValidateInput(false)]
        public ActionResult Update(int? id)
        {
            if (id.HasValue)
            {
                return View(bll.FindEntityById((int)id));
            }
            return View();
        }

        [HttpPost]
        [AdminAuthentication]
        [ValidateInput(false)]
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
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bll.DeleteEntityById((int)id))
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
            if(ids.Length == 0)
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
            if(id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            News news = bll.FindEntityById((int)id);
            if(news == null)
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
    }
}