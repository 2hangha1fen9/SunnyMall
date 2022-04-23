using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;
namespace BLL
{
    public class AdminUsersBLL : BaseBLL<AdminUsers>
    {

        public List<AdminUsers> ListEntity(string key)
        {
            if(key.Length > 0)
            {
                IQueryable<AdminUsers> products = ListEntity().Where(n => n.UserName.Contains(key) ||
                                                                                n.Role == (key == "管理员" ? 0 : key == "超级管理员" ? 1 : -9));
                return products.ToList();
            }
            return ListEntity().ToList();
        }

        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("AdminID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("AdminID", idList);
        }
    }
}
