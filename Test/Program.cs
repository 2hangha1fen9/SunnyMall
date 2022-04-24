using BLL;
using Common;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            MailHelper.SendMail("207168599@qq.com", $"[阳光商城]邮箱验证", MailHelper.Register("注册", $"/UserCenter/"));
        }
    }
}
