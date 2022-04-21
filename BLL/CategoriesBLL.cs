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
        public PagedList<Categories> FindEntity(string key)
        {
            IQueryable<Categories> news = ListEntity().Where(n => n.CateName.Contains(key));
            return news.ToList().ToPagedList(1, news.Count());
        }

        public PagedList<Categories> ListEntityGroupByPage(int? id = 1)
        {
            IQueryable<Categories> cates = ListEntity().Where(c => c.Categories1.Count() != 0 || c.ParentID == null);
            return cates.ToList().ToPagedList(id ?? 1,5);
        }

        public List<Categories> ListEntityGroup()
        {
            return ListEntity().Where(c => c.Categories1.Count() != 0 || c.ParentID == null).ToList();
        }

        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("CateID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("CateID", idList);
        }
    }
}
