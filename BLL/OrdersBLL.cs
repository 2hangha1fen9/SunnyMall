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
        public PagedList<Orders> FindEntityByPage(int? id,string key)
        {
            IQueryable<Orders> news = ListEntity().Where(o => o.Users.UserName.Contains(key) || 
                                                                o.Deliveries.Consignee.Contains(key) || 
                                                                o.States == (key == "未付款" ? 0 : key == "已付款" ? 1 : key == "已发货" ? 2 : key == "已收货" ? 3 : key == "已关闭" ? -1 : -1));
            return news.ToList().OrderByDescending(n => n.Orderdate)
                                    .ToPagedList(id ?? 1, 10);
        }

        public PagedList<Orders> ListEntityByPage(int id,int? states)
        {
            if (states.HasValue)
            {
                return ListEntityByCondition(o => o.States == states.Value)
                       .ToList()
                       .AsQueryable()
                       .OrderByDescending(n => n.Orderdate)
                       .ToPagedList(id, 10);
            }
            return ListEntity()
                    .ToList()
                    .AsQueryable()
                    .OrderByDescending(n => n.Orderdate)
                    .ToPagedList(id, 10);
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
