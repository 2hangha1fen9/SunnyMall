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
    public class FavoritesController : Controller
    {
        FavoritesBLL favoritesBLL = new FavoritesBLL();

        private IEnumerable<Favorites> GetFavorites(string key = "")
        {
            TempData["Search"] = key;
            int uid = MyAuthentication.GetUserID();
            return favoritesBLL.ListEntity(key).Where(o => o.UserID == uid);
        }

        // GET: Favorites
        [UserAuthentication]
        public ActionResult Index(int? id =1,string key = "")
        {
            var favorites = GetFavorites(key);
            if (key.Length > 0)
            {
                TempData["Message"] = $"检索到{favorites.Count()}条数据";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Index", favorites.ToPagedList(id.Value, 10));
            }
            return View(favorites.ToPagedList(id.Value, 10));
        }

        /// <summary>
        /// GET /Favorites/Like/{id}[detail][pageIndex][key]
        /// 收藏
        /// </summary>
        /// <param name="id">产品id</param>
        /// <param name="detail">是否是详情页面请求</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="key">搜索关键字</param>
        /// <returns></returns>
        [UserAuthentication]
        public ActionResult Like(int? id, int? detail, int? pageIndex = 1,string key = "")
        {
            int uid = MyAuthentication.GetUserID();
            Favorites favorites = new Favorites();
            favorites.UserID = uid;
            favorites.ProductID = id.Value;
            Favorites reslut = favoritesBLL.FindEntityByCondition(f => f.UserID == uid && f.ProductID == id.Value);
            ContentResult content = new ContentResult();
            if (reslut != null)
            {
                if (favoritesBLL.DeleteEntity(reslut))
                {
                    content.Content = "添加收藏";
                }
                else
                {
                    content.Content = "取消收藏";
                }
                
            }
            else if ((reslut = favoritesBLL.AddEntity(favorites)) != null)
            {
                content.Content = "取消收藏";
            }
            else
            {
                content.Content = "添加收藏";
            }
            if (detail.HasValue)
            {
                return content;
            }
            else
            {
                return PartialView("_Index", GetFavorites(key).ToPagedList(pageIndex.Value, 10));
            }
            
        }
    }
}