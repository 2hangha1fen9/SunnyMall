using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AppraisesBLL : BaseBLL<Appraises>
    {
        public List<Appraises> ListEntity(string key)
        {
            if (key.Length > 0)
            {
                IQueryable<Appraises> news = ListEntity().Where(a => a.Products.Title.Contains(key) || a.Content.Contains(key));
                return news.ToList();
            }
            return ListEntity().OrderByDescending(n => n.RateTime).ToList();
        }
        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("AppraiseID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("AppraiseID", idList);
        }
    }
}
