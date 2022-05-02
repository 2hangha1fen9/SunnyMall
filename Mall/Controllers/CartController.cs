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
    public class CartController : Controller
    {
        private CartBLL cartBLL = new CartBLL();
        private ProductsBLL productsBLL = new ProductsBLL();

        // GET: Cart
        [UserAuthentication]
        public ActionResult Index()
        {
            Users user = MyAuthentication.GetUser();
            return View(cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
        }

        [UserAuthentication]
        public ActionResult Add(int id, int quantity)
        {
            Users user = MyAuthentication.GetUser();
            if (!cartBLL.AddQuantity(id,quantity))
            {
                TempData["Message"] = "操作失败";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Cart", cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
            }
            return View(cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
        }

        [UserAuthentication]    
        public ActionResult AddProduct(int id,int? quantity = 1)
        {
            Users user = MyAuthentication.GetUser();
            if (cartBLL.AddProduct(id, quantity.Value, user))
            {
                return new HttpStatusCodeResult(HttpStatusCode.OK);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
        }

        [UserAuthentication]
        public ActionResult Checked(int id)
        {
            Users user = MyAuthentication.GetUser();
            if (!cartBLL.CheckProduct(id))
            {
                TempData["Message"] = "操作失败";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Cart", cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
            }
            return View(cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
        }

        [HttpPost]
        [UserAuthentication]
        public ActionResult Checked()
        {
            string cids = Request.Form["cid"];
            Users user = MyAuthentication.GetUser();
            if (!cartBLL.CheckProduct(cids))
            {
                TempData["Message"] = "操作失败";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Cart", cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
            }
            return View(cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
        }

        [UserAuthentication]
        public ActionResult RemoveProduct(int id)
        {
            Users user = MyAuthentication.GetUser();
            if (cartBLL.DeleteEntityById(id))
            {
                TempData["Message"] = "删除成功";
            }
            else
            {
                TempData["Message"] = "删除失败";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Cart", cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
            }
            return View(cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
        }

        [HttpPost]
        [UserAuthentication]
        public ActionResult RemoveProduct()
        {
            string cids = Request.Form["cid"];
            Users user = MyAuthentication.GetUser();
            if (cartBLL.DeleteEntityByIdList(cids))
            {
                TempData["Message"] = "删除成功";
            }
            else
            {
                TempData["Message"] = "删除失败";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Cart", cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
            }
            return View(cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
        }

        [UserAuthentication]
        public ActionResult RemoveToFavories(int id)
        {
            Users user = MyAuthentication.GetUser();
            if (cartBLL.RemoveToFavories(id, user))
            {
                TempData["Message"] = "操作成功";
            }
            else
            {
                TempData["Message"] = "操作失败";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Cart", cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
            }
            return View(cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
        }

        [HttpPost]
        [UserAuthentication]
        public ActionResult RemoveToFavories()
        {
            string cids = Request.Form["cid"];
            Users user = MyAuthentication.GetUser();
            if (cartBLL.RemoveToFavories(cids, user))
            {
                TempData["Message"] = "操作成功";
            }
            else
            {
                TempData["Message"] = "操作失败";
            }
            if (Request.IsAjaxRequest())
            {
                return PartialView("_Cart", cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
            }
            return View(cartBLL.ListEntityByCondition(c => c.UserID == user.UserID).ToList());
        }
    }
}