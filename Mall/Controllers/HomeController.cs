using BLL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Webdiyer.WebControls.Mvc;
using Common;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Mall.Controllers
{
    public class HomeController : Controller
    {
        UsersBLL UsersBLL = new UsersBLL();
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

       [HttpPost]
        public ActionResult Login(Users u, string code)
        {
            if (ModelState.IsValid)
            {
                if(!TempData["code"].ToString().Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", "验证码错误");
                    return View();
                }
                Users user = UsersBLL.FindEntityByCondition(model => model.UserName == u.UserName && model.Pwd == u.Pwd && (model.States != 1 || model.States != -1));
                if (user != null)
                {
                    if (user.States != 2 || user.States == 3)
                    {
                        ModelState.AddModelError("", "账号不可用");
                        return View();
                    }
                    // 设置权限
                    MyAuthentication.SetAuthCookie(user.UserName, user.UserID.ToString(), user.States.ToString());
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "账号名或密码错误");
                }
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(Users user, string code)
        {
            if (ModelState.IsValid)
            {
                if (!TempData["code"].ToString().Equals(code, StringComparison.OrdinalIgnoreCase))
                {
                    ModelState.AddModelError("", "验证码错误");
                    return View();
                }
                if(UsersBLL.FindEntityByCondition(m => m.UserName == user.UserName || m.Email == user.Email) == null)
                {
                    user.States = -1;
                    user.ActivationCode = Guid.NewGuid().ToString();
                    user.RegisterDate = DateTime.Now;
                    if (UsersBLL.AddEntity(user) != null)
                    {
                        MailHelper.SendMail(user.Email, $"[阳光商城]邮箱验证", MailHelper.Register(user.UserName, $"https://{Request.Url.Host}/UserCenter/Activation/{user.ActivationCode}"));
                        TempData["Message"] = "注册成功,请访问你的邮箱进行激活!";
                        return RedirectToAction("Login");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "用户已存在");
                    return View();
                }
            }
            return View();
        }

        public ActionResult LoginOut()
        {
            MyAuthentication.LoginOut();
            return RedirectToAction("Index");
        }

        public ActionResult CheckCode()
        {
            Random r = new Random();
            string letter = "1Aa2Bb3Cc4Dd5Ee6Ff7Gg8Hh9Ii0JjKkLlMmNnOoPpQqRrSsTtUuVvWwXxYyZz";
            string code = "";
            int codeLength = 5;
            for (int i = 0; i < codeLength; i++)
            {
                code += letter[r.Next(letter.Length - 1)];
            }
            TempData["code"] = code;
            return CreateImages(code);
        }

        [UserAuthentication]
        public ActionResult UserCenter()
        {
            return View();
        }

        public FileContentResult CreateImages(string checkCode)
        {
            int imageWidth = (int)(checkCode.Length * 10) * 10;
            int imageHeight = 200;
            Bitmap image = new Bitmap(imageWidth, imageHeight); //创建一个位图
            Graphics g = Graphics.FromImage(image); //创建一个画布
            g.Clear(Color.White); //设置背景
            Random r = new Random();

            //颜色
            Color[] colors = { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Aqua, Color.Blue, Color.Purple };
            

            //输出字符
            for (int i = 0; i < checkCode.Length; i++)
            {
                string code = checkCode[i].ToString();
                Font font = new Font("宋体", 100);
                SolidBrush solidBrush = new SolidBrush(colors[r.Next(colors.Length - 1)]);
                int x = i * (imageWidth / checkCode.Length);
                int y = r.Next(imageHeight - font.Height);
                g.DrawString(code, font, solidBrush, x, y);
            }
            //输出噪点
            int dotCount = r.Next(50, 100);
            for (int i = 0; i <= dotCount; i++)
            {
                int x = r.Next(image.Width);
                int y = r.Next(image.Height);
                g.DrawRectangle(new Pen(colors[r.Next(colors.Length - 1)], 3), x, y, 1, 1);
            }
            //输出线条
            int lineCount = r.Next(10, 20);
            for (int i = 0; i <= lineCount; i++)
            {
                int x1 = r.Next(image.Width);
                int y1 = r.Next(image.Height);
                int x2 = r.Next(image.Width);
                int y2 = r.Next(image.Height);
                g.DrawLine(new Pen(colors[r.Next(colors.Length - 1)]), x1, y1, x2, y2);
            }
            //输出图像
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, ImageFormat.Jpeg);
                FileContentResult result = new FileContentResult(ms.ToArray(), "image/jpeg");
                g.Dispose();
                image.Dispose();
                return result;
            }
        }
    }
}