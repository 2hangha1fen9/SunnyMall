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
    public class ProductsController : Controller
    {
        private ProductsBLL bll = new ProductsBLL();
        private CategoriesBLL cll = new CategoriesBLL();
        private PhotosBLL pll = new PhotosBLL();
        private FavoritesBLL fll = new FavoritesBLL();
        private CartBLL cartBLL = new CartBLL();

        [AdminAuthentication]
        public ActionResult Index(int? id = 1, string key = "", string cates = "", string orderBy = "Count", string sortBy = "1", string pricecMin = "", string priceMax = "")
        {
            var products = bll.ListEntity(key, cates, orderBy, sortBy, pricecMin, priceMax);
            if(key.Length > 0)
            {
                TempData["Message"] = $"检索到{products.Count()}条数据";
            }
            TempData["Search"] = key;
            return View(products.ToPagedList(id.Value,10));
        }

        public ActionResult List(int? id = 1, string key = "", string cates = "", string orderBy = "Count", string sortBy = "1", string priceMin = "", string priceMax = "")
        {
            var products = bll.ListEntity(key, cates, orderBy, sortBy, priceMin, priceMax).Where(p => p.States == 1);
            if (key.Length > 0)
            {
                TempData["Message"] = $"找到{products.Count()}个关于 \"{key}\" 的商品";
            }
            TempData["key"] = key;
            TempData["cates"] = cates;
            TempData["orderBy"] = orderBy;
            TempData["sortBy"] = sortBy;
            TempData["priceMin"] = priceMin;
            TempData["priceMax"] = priceMax;
            return View(products.ToPagedList(id.Value, 17));
        }

        public ActionResult Details(int? id,int? appraiseLevel = -1, int? appraiseOrderBy = 1, int? appraiseSortBy = 1)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TempData["appraiseLevel"] = appraiseLevel;
            TempData["appraiseOrderBy"] = appraiseOrderBy;
            TempData["appraiseSortBy"] = appraiseSortBy;
            Products products = bll.FindEntityById(id.Value);
            List<Appraises> appraise = products.Appraises.ToList();
            if(appraiseLevel != -1)
            {
                appraise = appraise.Where(a => a.Grade == appraiseLevel).ToList();
            }

            if(appraiseOrderBy == 1)
            {
                if(appraiseSortBy == 1)
                {
                    appraise = appraise.OrderByDescending(a => a.RateTime).ToList();
                }
                else
                {
                    appraise = appraise.OrderBy(a => a.RateTime).ToList();
                }
            }

            ViewBag.Appraise = appraise;

            if(MyAuthentication.GetAuth() == "2")
            {
                Users user = MyAuthentication.GetUser();
                ViewBag.User = user;
                ViewBag.Favorites = fll.ListEntityByCondition(f => f.UserID == user.UserID).ToList();
            }
            return View(products);
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

        public ActionResult AppraiseDelete(int? id,string aid)
        {
            if (!id.HasValue || aid.Length == 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AppraisesBLL appraisesBLL = new AppraisesBLL();
            if (appraisesBLL.DeleteEntityById(int.Parse(aid)))
            {
                TempData["Message"] = "删除成功";
            }
            else
            {
                TempData["Message"] = "无效的ID列表";
            }
            return RedirectToAction("Details",new { id = id.Value });
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