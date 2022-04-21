using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class ProductsBLL : BaseBLL<Products>
    {
        public PagedList<Products> FindEntity(string key)
        {
            IQueryable<Products> news = ListEntity().Where(n => n.Title.Contains(key) ||
                                                                   n.Title.Contains(key) ||
                                                                   n.MarketPrice.ToString().Contains(key) ||
                                                                   n.Price.ToString().Contains(key) ||
                                                                   n.Stock.ToString().Contains(key) || 
                                                                   n.Categories.CateName.Contains(key));
            return news.ToList().OrderByDescending(n => n.PostTime).ToPagedList(1, news.Count());
        }

        public PagedList<Products> ListEntityByPage(int? id = 1)
        {
            return ListEntity()
                    .ToList()
                    .AsQueryable()
                    .OrderByDescending(n => n.PostTime)
                    .ToPagedList(id ?? 1, 10);
        }

        PhotosBLL pll = new PhotosBLL();
        public override bool DeleteEntityById(int id)
        {
            var photos = pll.ListEntityByCondition(p => p.ProductID == id);
            if(photos.Count() > 0)
            {
                pll.DeleteEntity(photos);
            }
            return dal.Delete("ProductID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            string[] ids = idList.Split(',');
            foreach (var item in ids)
            {
                var photos = pll.ListEntityByCondition(p => p.ProductID == int.Parse(item));
                if (photos.Count() > 0)
                {
                    pll.DeleteEntity(photos);
                }
            }
            return  dal.Delete("ProductID", idList);
        }
    }
}
