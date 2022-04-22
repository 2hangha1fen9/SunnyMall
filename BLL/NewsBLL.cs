using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class NewsBLL : BaseBLL<News>
    {
        public PagedList<News> FindEntityByPage(int? id,string key)
        {
            IQueryable<News> news = ListEntity().Where(n => n.Title.Contains(key) ||
                                                               n.NType.Contains(key) ||
                                                               n.Content.Contains(key) ||
                                                               n.States == (key == "置顶" ? 1 : -1) ||
                                                               n.States == (key == "未置顶" ? 0 : -1));
            return news.ToList().OrderByDescending(n => n.PushTime)
                                    .OrderByDescending(n => n.States)
                                    .ToPagedList(id ?? 1,10);
        }

        public PagedList<News> ListEntityByPage(int? id = 1)
        {
            return ListEntity()
                    .ToList()
                    .AsQueryable()
                    .OrderByDescending(n => n.PushTime)
                    .OrderByDescending(n => n.States)
                    .ToPagedList(id ?? 1, 10);
        }

        
        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("NewsID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("NewsID", idList);
        }
    }
}
