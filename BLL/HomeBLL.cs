using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class HomeBLL
    {
        private UsersBLL usersBLL = new UsersBLL();

        public Users UserLogin(Users user)
        {
            return usersBLL.FindEntityByCondition(u => u.UserName == user.UserName && u.Pwd == user.Pwd);
        }

        public Users RegisterUser(Users user)
        {
            return usersBLL.AddEntity(user);
        }
    }
}
