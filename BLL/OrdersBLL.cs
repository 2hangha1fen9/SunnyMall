using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class OrdersBLL : BaseBLL<Orders>
    {
        public List<Orders> ListEntity(string key,int? states)
        {
            if(key.Length > 0)
            {
                IQueryable<Orders> news = ListEntity().Where(o => o.Users.UserName.Contains(key) ||
                                                               o.Deliveries.Consignee.Contains(key) ||
                                                               o.States == (key == "未付款" ? 0 : key == "已付款" ? 1 : key == "已发货" ? 2 : key == "已收货" ? 3 : key == "已关闭" ? -1 : -1));
                return news.ToList().OrderByDescending(n => n.Orderdate).ToList();
            }
            if (states.HasValue)
            {
                return ListEntityByCondition(o => o.States == states.Value).OrderByDescending(n => n.Orderdate).ToList();
            }
            return ListEntity().OrderByDescending(n => n.Orderdate).ToList();
        }

        public override bool DeleteEntityById(int id)
        {
            Orders od = FindEntityById(id);
            if (od.OrdersDetails.Count() > 0)
            {
                new OrdersDetailsBLL().DeleteEntity(od.OrdersDetails);
            }
            return dal.Delete("OrdersID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("OrdersID", idList);
        }
    }
}
