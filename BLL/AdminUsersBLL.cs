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
        public PagedList<AdminUsers> FindEntity(string key)
        {
            IQueryable<AdminUsers> products = ListEntity().Where(n => n.UserName.Contains(key) ||
                                                                                 n.Role == (key == "管理员" ? 0 : key == "超级管理员" ? 1 : -9));
            return products.ToList().ToPagedList(1, products.Count());
        }

        public PagedList<AdminUsers> ListEntityByPage(int? id = 1)
        {
            return ListEntity()
                    .ToList()
                    .AsQueryable()
                    .ToPagedList(id ?? 1, 10);
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
