using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class OrdersDetailsBLL : BaseBLL<OrdersDetails>
    {
        private CartBLL cartBLL = new CartBLL();
        private ProductsBLL productsBLL = new ProductsBLL();

        public List<OrdersDetails> CheckDetail(Users user)
        {
            List<OrdersDetails> ordersDetails = new List<OrdersDetails>();
            var carts = user.Cart.Where(c => c.Checked == 1);
            foreach (var c in carts)
            {
                OrdersDetails detail = new OrdersDetails();
                detail.Products = c.Products;
                detail.ProductID = c.ProductID;
                detail.Products = c.Products;
                detail.Quantity = c.Quantity;
                ordersDetails.Add(detail);
            }
            return ordersDetails;
        }

        public List<OrdersDetails>CheckDetail(int pid,int quantity, Users user)
        {
            List<OrdersDetails> ordersDetails = new List<OrdersDetails>();
            Products products = productsBLL.FindEntityById(pid);
            OrdersDetails detail = new OrdersDetails();
            detail.ProductID = products.ProductID;
            detail.Products = products;
            detail.Quantity = quantity;
            ordersDetails.Add(detail);
            return ordersDetails;
        }

        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("DetailID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("DetailID", idList);
        }
    }
}
