using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class FavoritesBLL : BaseBLL<Favorites>
    {
        public List<Favorites> ListEntity(string key)
        {
            if (key.Length > 0)
            {
                IQueryable<Favorites> favorites = ListEntity().Where(n => n.Products.Title.Contains(key));    
                return favorites.ToList();
            }
            return ListEntity().ToList();
        }
        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("FavoriteID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("FavoriteID", idList);
        }
    }
}
