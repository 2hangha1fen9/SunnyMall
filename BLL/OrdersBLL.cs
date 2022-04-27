using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Webdiyer.WebControls.Mvc;

namespace BLL
{
    public class OrdersBLL : BaseBLL<Orders>
    {
        ProductsBLL productsBLL = new ProductsBLL();

        public List<Orders> ListEntity(string key,int? states)
        {
            if(key.Length > 0)
            {
                IQueryable<Orders> news = ListEntity().Where(o => o.Users.UserName.Contains(key) ||
                                                               o.Deliveries.Consignee.Contains(key) ||
                                                               o.OrdersDetails.Any(a => a.Products.Title.Contains(key)) ||
                                                               o.States == (key == "未付款" ? 0 : key == "已付款" ? 1 : key == "已发货" ? 2 : key == "已收货" ? 3 : key == "已关闭" ? -1 : -1));
                return news.ToList().OrderByDescending(n => n.Orderdate).ToList();
            }
            if (states.HasValue)
            {
                return ListEntityByCondition(o => o.States == states.Value).OrderByDescending(n => n.Orderdate).ToList();
            }
            return ListEntity().OrderByDescending(n => n.Orderdate).ToList();
        }

        public Orders ConfirmOrder(Users user,int deliverieID)
        {
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    List<Cart> cart = user.Cart.Where(c => c.Checked == 1).ToList();
                    Orders orders = new Orders();
                    orders.Orderdate = DateTime.Now;
                    orders.DeliveryID = deliverieID;
                    orders.UserID = user.UserID;
                    foreach (Cart cartItem in cart)
                    {
                        OrdersDetails details = new OrdersDetails();
                        details.ProductID = cartItem.ProductID;
                        details.Quantity = cartItem.Quantity;
                        cartItem.Products.Stock -= cartItem.Quantity;
                        if (!productsBLL.UpdateEntity(cartItem.Products))
                        {
                            throw new Exception();
                        }
                        orders.OrdersDetails.Add(details);
                    }
                    orders.Total = orders.OrdersDetails.Sum(c => c.Products.Price * c.Quantity);
                    Orders confirm = AddEntity(orders);
                    if (confirm != null)
                    {
                        scope.Complete();
                        return confirm;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        public Orders ConfirmOrder(Users user, int pid,int quantity,int deliverieID)
        {
            
            using (TransactionScope scope = new TransactionScope())
            {
                try
                {
                    Orders orders = new Orders();
                    orders.Orderdate = DateTime.Now;
                    orders.DeliveryID = deliverieID;
                    orders.UserID = user.UserID;

                    Products products = productsBLL.FindEntityById(pid);
                    OrdersDetails details = new OrdersDetails();
                    details.ProductID = products.ProductID;
                    details.Quantity = quantity;
                    products.Stock -= quantity;
                    if (!productsBLL.UpdateEntity(products))
                    {
                        throw new Exception();
                    }
                    orders.OrdersDetails.Add(details);
                    orders.Total = orders.OrdersDetails.Sum(c => c.Products.Price * c.Quantity);
                    Orders confirm = AddEntity(orders);
                    if (confirm != null)
                    {
                        scope.Complete();
                        return confirm;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
                catch (Exception)
                {
                    return null;
                }
            }
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
