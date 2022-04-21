using BLL;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mall.Controllers
{
    public class UploadController : Controller
    {

        [HttpPost]
        [AdminAuthentication]
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var res = new JsonResult();
            if (upload != null)
            {
                string generateName = DateTime.Now.ToFileTime().ToString() + Path.GetExtension(upload.FileName);
                string serverName = $"/Content/Images/{generateName}";
                upload.SaveAs(Server.MapPath(serverName));
                res.Data = new { uploaded = "1", url = serverName };

                if(Request.Form["main"] == "true") //上传资讯图片,传入图片服务器地址
                {
                    TempData["FileName"] = serverName;
                }               
                if(Request.Form["ProductID"] != null) //上传商品图片
                {
                    Photos p = new Photos();
                    p.ProductID = int.Parse(Request.Form["ProductID"]);
                    p.PhotoUrl = serverName;
                    PhotosBLL bll = new PhotosBLL();
                    Photos result;
                    if ((result = bll.AddEntity(p)) != null)
                    {
                        TempData["Message"] = "上传成功";
                        res.Data = new { PhotoID = result.PhotoID, ProductID = result.ProductID, PhotoUrl = result.PhotoUrl };
                    }                  
                }
            }
            return res;
        }
    }
}