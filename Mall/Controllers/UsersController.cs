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
    public class UsersController : Controller
    {
        private UsersBLL bll = new UsersBLL();

        [AdminAuthentication]
        public ActionResult Index(int? id = 1, string key = "")
        {
            var users = bll.ListEntity(key);
            if(key.Length > 0)
            {
                TempData["Message"] = $"检索到{users.Count()}条数据";
            }
            TempData["Search"] = key;
            return View(users.ToPagedList(id.Value,10));
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
        [AdminAuthentication]
        [ValidateInput(false)]
        public ActionResult Update(Users n, int? id)
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
            Users users = bll.FindEntityById(id.Value);
            if (users == null)
            {
                TempData["Message"] = "无效的ID";
            }
            users.States = users.States == 2 ? 3 : 2;
            if (bll.UpdateEntity(users))
            {
                TempData["Message"] = "操作成功";
            }
            return RedirectToAction("Index");
        }

        public ActionResult Activation(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users user = bll.FindEntityByCondition(u => u.ActivationCode == id);
            if (user != null)
            {
                user.States = 2;
                if (bll.UpdateEntity(user))
                {

                    TempData["Message"] = "激活成功";
                }
                else
                {
                    TempData["Message"] = "激活失败";
                }
                return RedirectToAction("Login", "Home");
            }
            ContentResult result = new ContentResult();
            result.Content = "激活失败,URL非法";
            result.ContentType = "text/html";
            return result;
        }
    }
}