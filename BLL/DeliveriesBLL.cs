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
        public List<Deliveries> ListEntity(string key)
        {
            if(key.Length > 0)
            {
                IQueryable<Deliveries> desliveries = ListEntity().Where(d => d.Complete.Contains(key) ||
                                                                                    d.Consignee.Contains(key) ||
                                                                                    d.Phone.Contains(key) ||
                                                                                    d.Users.UserName.Contains(key));
                return desliveries.ToList();
            }
            return ListEntity().ToList();
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
