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
        public PagedList<Users> FindEntity(string key)
        {
            IQueryable<Users> users = ListEntity().Where(u => u.UserName.Contains(key) ||
                                                                       u.Nick.Contains(key) || 
                                                                       u.Email.Contains(key));
            return users.ToList().ToPagedList(1, users.Count());
        }

        public PagedList<Users> ListEntityByPage(int? id = 1)
        {
            return ListEntity()
                    .ToList()
                    .AsQueryable()
                    .ToPagedList(id ?? 1, 10);
        }
        public override bool DeleteEntityById(int id)
        {
            Users user = FindEntityById(id);
            if(user.Favorites.Count() > 0)
            {
                new FavoriesBLL().DeleteEntity(user.Favorites);
            }
            if (user.Appraises.Count() > 0)
            {
                new AppraisesBLL().DeleteEntity(user.Appraises);
            }
            if (user.Orders.Count() > 0)
            {
                new OrdersBLL().DeleteEntity(user.Orders);
            }
            if (user.Deliveries.Count() > 0)
            {
                new DeliveriesBLL().DeleteEntity(user.Deliveries);
            }        
            return dal.Delete("UserID",id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            string[] ids = idList.Split(',');
            foreach (var item in ids)
            {
                Users user = FindEntityById(int.Parse(item));
                if (user.Favorites.Count() > 0)
                {
                    new FavoriesBLL().DeleteEntity(user.Favorites);
                }
                if (user.Appraises.Count() > 0)
                {
                    new AppraisesBLL().DeleteEntity(user.Appraises);
                }
                if (user.Deliveries.Count() > 0)
                {
                    new DeliveriesBLL().DeleteEntity(user.Deliveries);
                }
                if (user.Orders.Count() > 0)
                {
                    new OrdersBLL().DeleteEntity(user.Orders);
                }
            }
            return dal.Delete("UserID", idList);
        }   
    }
}
