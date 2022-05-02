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
        public List<Products> ListEntity(string key, string cates, string orderBy, string sortBy, string priceMin, string priceMax)
        {
            IQueryable<Products> products = ListEntity();
            if (cates.Length > 0)
            {
                products = products.Where(p => p.Categories.CateName == cates || p.Categories.Categories2.CateName == cates);
            }
            if (key.Length > 0)
            {
                products = products.Where(n => n.Title.Contains(key) ||
                                                                   n.Title.Contains(key) ||
                                                                   n.Categories.CateName.Contains(key) ||
                                                                   n.States == (key == "下架" ? 0 : key == "上架" ? 1 : -1));
            }
            if (orderBy.Length > 0)
            {
                if (sortBy == "1")
                {
                    if (orderBy == "Price")
                    {
                        products = products.OrderByDescending(p => p.Price);
                    } 
                    else if (orderBy == "PostTime")
                    {
                        products = products.OrderByDescending(p => p.PostTime);
                    }
                    else if(orderBy == "Appraise")
                    {
                        products = products.OrderByDescending(p => p.Appraises.Count());
                    }
                    else 
                    {
                        products = products.OrderByDescending(p => p.OrdersDetails.Count());
                    }
                }
                else
                {
                    if (orderBy == "Price")
                    {
                        products = products.OrderBy(p => p.Price);
                    }
                    else if (orderBy == "PostTime")
                    {
                        products = products.OrderBy(p => p.PostTime);
                    }
                    else if (orderBy == "Appraise")
                    {
                        products = products.OrderBy(p => p.Appraises.Count());
                    }
                    else
                    {
                        products = products.OrderBy(p => p.OrdersDetails.Count());
                    }
                }
            }
            if (priceMin.Length > 0 && priceMax.Length > 0)
            {
                int min = int.Parse(priceMin);
                int max = int.Parse(priceMax);
                if (min < max)
                {
                    products = products.Where(p => p.Price >= min && p.Price <= max);
                }
            }
            else if(priceMin.Length > 0)
            {
                int min = int.Parse(priceMin);
                products = products.Where(p => p.Price >= min);
            }
            else if (priceMax.Length > 0)
            {
                int max = int.Parse(priceMax);
                products = products.Where(p => p.Price <= max);
            }
            return products.ToList();
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
                int id = int.Parse(item);
                var photos = pll.ListEntityByCondition(p => p.ProductID == id);
                if (photos.Count() > 0)
                {
                    pll.DeleteEntity(photos);
                }
            }
            return  dal.Delete("ProductID", idList);
        }
    }
}
