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
        // GET: Favorites
        [UserAuthentication]
        public ActionResult Index(int? id =1,string key = "")
        {
            var favorites = favoritesBLL.ListEntity(key);
            if (key.Length > 0)
            {
                TempData["Message"] = $"检索到{favorites.Count()}条数据";
            }
            TempData["Search"] = key;
            return View(favorites.ToPagedList(id.Value, 10));
        }

        [UserAuthentication]
        public ActionResult Like(int? id)
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
            if (Request.IsAjaxRequest())
            {
                return content;
            }
            return RedirectToAction("Index");
        }
    }
}