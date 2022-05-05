using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class UsersBLL : BaseBLL<Users>
    {
        private OrdersBLL ordersBLL = new OrdersBLL();
        public List<Users> ListEntity(string key)
        {
            if(key.Length > 0)
            {
                IQueryable<Users> users = ListEntity().Where(u => u.UserName.Contains(key) ||
                                                                       u.Nick.Contains(key) ||
                                                                       u.Email.Contains(key));
                return users.ToList();
            }
            return ListEntity().ToList();
        }
        public override bool DeleteEntityById(int id)
        {   
            var user = FindEntityById(id);
            if(user != null)
            {
                foreach(var item in user.Orders.ToList())
                {
                    item.UserID = null;
                    item.DeliveryID = null;
                    item.States = 3;
                    ordersBLL.UpdateEntity(item);
                }
            }
            return dal.Delete("UserID",id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            string[] ids = idList.Split(',');
            foreach (string id in ids)
            {
                var user = FindEntityById(int.Parse(id));
                if (user != null)
                {
                    foreach (var item in user.Orders.ToList())
                    {
                        item.UserID = null;
                        item.DeliveryID = null;
                        item.States = 3;
                        ordersBLL.UpdateEntity(item);
                    }
                }
            }
            return dal.Delete("UserID", idList);
        }   
    }
}
