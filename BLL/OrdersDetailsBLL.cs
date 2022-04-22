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
