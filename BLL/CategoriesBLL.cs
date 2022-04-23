using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class CategoriesBLL : BaseBLL<Categories>
    {
        public List<Categories> ListEntity(string key)
        {
            if(key.Length > 0)
            {
                IQueryable<Categories> news = ListEntity().Where(n => n.CateName.Contains(key) || n.States == (key == "正常" ? 0 : key == "禁用" ? 1 : -1));
                return news.ToList();
            }
            return ListEntity().Where(c => c.Categories1.Count() != 0 || c.ParentID == null).ToList();
        }

        public override bool DeleteEntityById(int id)
        {
            Categories c = dal.Entity(id);
            if (c.Categories1.Count() > 0)
            {
                dal.Delete(c.Categories1);
            }
            return dal.Delete("CateID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("CateID", idList);
        }
    }
}
