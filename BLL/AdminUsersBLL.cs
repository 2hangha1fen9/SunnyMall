using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Webdiyer.WebControls.Mvc;
namespace BLL
{
    public class AdminUsersBLL : BaseBLL<AdminUsers>
    {

        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("AdminID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("AdminID", idList);
        }
    }
}
