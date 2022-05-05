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
        private OrdersBLL ordersBLL = new OrdersBLL();
        private UsersBLL usersBLL = new UsersBLL();
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
            var deliveries = FindEntityById(id);
            if(deliveries != null)
            {
                foreach (var item in deliveries.Orders.ToList())
                {
                    item.Deliveries = null;
                    ordersBLL.UpdateEntity(item);
                }
                if (deliveries.Users1.Count() != 0)
                {
                    deliveries.Users.DeliveryID = null;
                    usersBLL.UpdateEntity(deliveries.Users);
                }
            }
           
            return dal.Delete("DeliveryID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            string[] ids = idList.Split(',');
            foreach (string id in ids)
            {
                var deliveries = FindEntityById(int.Parse(id));
                if (deliveries != null)
                {
                    foreach (var item in deliveries.Orders.ToList())
                    {
                        item.Deliveries = null;
                        ordersBLL.UpdateEntity(item);
                    }
                    if (deliveries.Users1.Count() != 0)
                    {
                        deliveries.Users.DeliveryID = null;
                        usersBLL.UpdateEntity(deliveries.Users);
                    }
                }
            }
            return dal.Delete("DeliveryID", idList);
        }
    }
}
