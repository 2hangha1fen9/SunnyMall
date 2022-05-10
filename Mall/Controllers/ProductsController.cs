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


        private List<Products> GetProducts(string key = "",string cates = "", string orderBy = "Count", string sortBy = "1", string priceMin = "", string priceMax = "")
        {
            TempData["Search"] = key;
            TempData["cates"] = cates;
            TempData["orderBy"] = orderBy;
            TempData["sortBy"] = sortBy;
            TempData["priceMin"] = priceMin;
            TempData["priceMax"] = priceMax;
            return bll.ListEntity(key, cates, orderBy, sortBy, priceMin, priceMax);
        }

        /// <summary>
        /// GET /Products/Index/{id}?[id=][key=][cates=][sortBy=][orderBy=][priceMin=][priceMax=]
        /// 管理员商品列表首页
        /// </summary>
        /// <param name="id">页码</param>
        /// <param name="key">搜索关键字</param>
        /// <param name="cates">分类关键字</param>
        /// <param name="orderBy">排序关键字</param>
        /// <param name="sortBy">升序降序</param>
        /// <param name="priceMin">最低价格</param>
        /// <param name="priceMax">最高价格</param>
        /// <returns></returns>
        [AdminAuthentication]
        public ActionResult Index(int? id = 1, string key = "", string cates = "", string orderBy = "Count", string sortBy = "1", string priceMin = "", string priceMax = "")
        {
            var products = GetProducts(key, cates, orderBy, sortBy, priceMin, priceMax);
            if (key.Length > 0)
            {
                TempData["Message"] = $"检索到{products.Count()}条数据";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Index", products.ToPagedList(id.Value, 10));
            }
            return View(products.ToPagedList(id.Value,10));
        }

        /// <summary>
        /// GET /Products/List/{id}?[id=][key=][cates=][sortBy=][orderBy=][priceMin=][priceMax=]
        /// 商品列表
        /// </summary>
        /// <param name="id">页码</param>
        /// <param name="key">搜索关键字</param>
        /// <param name="cates">分类关键字</param>
        /// <param name="orderBy">排序关键字</param>
        /// <param name="sortBy">升序降序</param>
        /// <param name="priceMin">最低价格</param>
        /// <param name="priceMax">最高价格</param>
        /// <returns></returns>
        public ActionResult List(int? id = 1, string key = "", string cates = "", string orderBy = "Count", string sortBy = "1", string priceMin = "", string priceMax = "")
        {
            var products = bll.ListEntity(key, cates, orderBy, sortBy, priceMin, priceMax).Where(p => p.States == 1 && p.Categories.States == 1 && p.Categories.Categories2 != null ? p.Categories.Categories2.States == 1 : p.Categories.States == 1);
            if (key.Length > 0)
            {
                TempData["Message"] = $"找到{products.Count()}个关于 \"{key}\" 的商品";
            }
            TempData["Search"] = key;
            TempData["cates"] = cates;
            if (Request.IsAjaxRequest())
            {
                return PartialView("_CardList",products.ToPagedList(id.Value, 17));
            }
            return View(products.ToPagedList(id.Value, 17));
        }

        /// <summary>
        /// GET /Products/Details{id}?[appraiseLevel=][appraiseOrderBy=][appraiseSortBy=]
        /// 商品详情
        /// </summary>
        /// <param name="id">商品ID</param>
        /// <param name="appraiseLevel">评价等级过滤</param>
        /// <param name="appraiseOrderBy">评价等级排序方式</param>
        /// <param name="appraiseSortBy">评价等级升序降序</param>
        /// <returns></returns>
        public ActionResult Details(int? id,int? appraiseLevel = -1, int? appraiseOrderBy = 1, int? appraiseSortBy = 1)
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = bll.FindEntityById(id.Value);
            List<Appraises> appraise = products.Appraises.ToList();
            if (appraiseLevel != -1)
            {
                appraise = appraise.Where(a => a.Grade == appraiseLevel).ToList();
            }

            if (appraiseOrderBy == 1)
            {
                if (appraiseSortBy == 1)
                {
                    appraise = appraise.OrderByDescending(a => a.RateTime).ToList();
                }
                else
                {
                    appraise = appraise.OrderBy(a => a.RateTime).ToList();
                }
            }
            products.Appraises = appraise;
            TempData["appraiseLevel"] = appraiseLevel;
            TempData["appraiseOrderBy"] = appraiseOrderBy;
            TempData["appraiseSortBy"] = appraiseSortBy;

            if (MyAuthentication.GetAuth() == "2")
            {
                Users user = MyAuthentication.GetUser();
                ViewBag.User = user;
                ViewBag.Favorites = fll.ListEntityByCondition(f => f.UserID == user.UserID).ToList();
            }
            return View(products);
        }

        /// <summary>
        /// GET /Products/Appraise/{id}?[appraiseLevel=][appraiseOrderBy=][appraiseSortBy=]
        /// 商品评价列表
        /// </summary>
        /// <param name="id">商品id</param>
        /// <param name="appraiseLevel">评价列表</param>
        /// <param name="appraiseOrderBy">排序一依据</param>
        /// <param name="appraiseSortBy">升序降序</param>
        /// <returns></returns>
        public ActionResult AppraiseList(int? id, int? appraiseLevel = -1, int? appraiseOrderBy = 1, int? appraiseSortBy = 1)
        {
            Products products = bll.FindEntityById(id.Value);
            List<Appraises> appraise = products.Appraises.ToList();
            if (appraiseLevel != -1)
            {
                appraise = appraise.Where(a => a.Grade == appraiseLevel).ToList();
            }

            if (appraiseOrderBy == 1)
            {
                if (appraiseSortBy == 1)
                {
                    appraise = appraise.OrderByDescending(a => a.RateTime).ToList();
                }
                else
                {
                    appraise = appraise.OrderBy(a => a.RateTime).ToList();
                }
            }
            TempData["appraiseLevel"] = appraiseLevel;
            TempData["appraiseOrderBy"] = appraiseOrderBy;
            TempData["appraiseSortBy"] = appraiseSortBy;

            return PartialView("_DetailsAppraise", appraise);
        }

        /// <summary>
        /// 删除评价
        /// </summary>
        /// <param name="id">产品id</param>
        /// <param name="aid">评价id</param>
        /// <returns></returns>
        public ActionResult AppraiseDelete(int? id, string aid)
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
            return PartialView("_DetailsAppraise", bll.FindEntityById(id.Value).Appraises.ToList());
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
        public ActionResult Delete(int? id,int? pageIndex = 1, string key = "", string cates = "", string orderBy = "Count", string sortBy = "1", string priceMin = "", string priceMax = "")
        {
            if (!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (bll.DeleteEntityById(id.Value))
            {
                return PartialView("_Index", GetProducts(key, cates, orderBy, sortBy, priceMin, priceMax).ToPagedList(pageIndex.Value, 10));
            }
            else
            {
                TempData["Message"] = "无效的ID";
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [AdminAuthentication]
        public ActionResult Delete(string key = "", string cates = "", string orderBy = "Count", string sortBy = "1", string priceMin = "", string priceMax = "")
        {
            string ids = Request.Form["ids"];
            string pageIndex = Request.Form["pageIndex"];
            if (bll.DeleteEntityByIdList(ids))
            {
                return PartialView("_Index", GetProducts(key, cates, orderBy, sortBy, priceMin, priceMax).ToPagedList(int.Parse(pageIndex), 10));
            }
            else
            {
                TempData["Message"] = "无效的ID列表";
            }
            return RedirectToAction("Index");
        }


        [AdminAuthentication]
        public ActionResult States(int? id, int? pageIndex = 1, string key = "", string cates = "", string orderBy = "Count", string sortBy = "1", string priceMin = "", string priceMax = "")
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
                return PartialView("_Index", GetProducts(key, cates, orderBy, sortBy, priceMin, priceMax).ToPagedList(pageIndex.Value, 10));
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