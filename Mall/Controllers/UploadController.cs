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
        public JsonResult UploadFile(HttpPostedFileBase upload)
        {
            var res = new JsonResult();
            if (upload != null)
            {
                var r = Request;
                string generateName = DateTime.Now.ToFileTime().ToString() + Path.GetExtension(upload.FileName);
                string serverName = $"/Content/Images/{generateName}";
                upload.SaveAs(Server.MapPath(serverName));
                res.Data = new { uploaded = "1", url = serverName };
                if(Request.Form["main"] == "true")
                {
                    TempData["FileName"] = serverName;
                }               
            }
            return res;
        }
    }
}