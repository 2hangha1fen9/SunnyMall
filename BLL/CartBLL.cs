using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class CartBLL : BaseBLL<Cart>
    {
        public bool AddQuantity(int cid,int quantity)
        {
            Cart cart = FindEntityById(cid);
            if (cart != null)
            {
                if (cart.Quantity + quantity >= 1)
                {
                    cart.Quantity += quantity;
                }  
                return UpdateEntity(cart);
            }
            return false;
        }

        public bool AddProduct(int pid,int quantity, Users user)
        {
            Cart cart = user.Cart.FirstOrDefault(c => c.ProductID == pid);
            if (cart != null)
            {
                return AddQuantity(cart.CartID, 1);
            }
            else
            {
                cart = new Cart();
                cart.ProductID = pid;
                cart.UserID = user.UserID;
                cart.Quantity = quantity;
                return AddEntity(cart) != null;
            }
        }

        public bool CheckProduct(string cids)
        {
            bool flag = false;
            string[] ids = cids.Split(',');
            foreach (string item in ids)
            {
                flag = CheckProduct(int.Parse(item));
            }
            return flag;
        }

        public bool CheckProduct(int cid)
        {
            Cart cart = FindEntityById(cid);
            if (cart != null)
            {
                if(cart.Checked == 0)
                {
                    cart.Checked = 1;
                }
                else
                {
                    cart.Checked = 0;
                }
                return UpdateEntity(cart);
            }
            return false;
        }

        public bool RemoveToFavories(int cid,Users user)
        {
            Cart cart = user.Cart.FirstOrDefault(c => c.CartID == cid);
            if(!user.Favorites.Any(f => f.ProductID == cart.ProductID))
            {
                Favorites favorites = new Favorites();
                favorites.UserID = user.UserID;
                favorites.ProductID = cart.ProductID;
                new FavoritesBLL().AddEntity(favorites);
            }
            return DeleteEntity(cart);
        }

        public bool RemoveToFavories(string cids, Users user)
        {
            bool flag = false;
            string[] ids = cids.Split(',');
            foreach (string id in ids)
            {
                flag = RemoveToFavories(int.Parse(id),user);
            }
            return flag;
        }

        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("CartID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("CartID", idList);
        }
    }
}
