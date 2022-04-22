using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class DeliveriesBLL : BaseBLL<Deliveries>
    {
        public PagedList<Deliveries> FindEntityByPage(int? id, string key)
        {
            IQueryable<Deliveries> desliveries = ListEntity().Where(d => d.Complete.Contains(key) ||
                                                                                    d.Consignee.Contains(key) ||
                                                                                    d.Phone.Contains(key) ||
                                                                                    d.Users.UserName.Contains(key));
            return desliveries.ToList().ToPagedList(id ?? 1, 10);
        }

        public PagedList<Deliveries> ListEntityByPage(int? id = 1)
        {
            return ListEntity()
                    .ToList()
                    .AsQueryable()
                    .ToPagedList(id ?? 1, 10);
        }
        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("DeliveryID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("DeliveryID", idList);
        }
    }
}
