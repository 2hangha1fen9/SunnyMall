using DAL;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class PhotosBLL : BaseBLL<Photos>
    {
        private BaseDAL<Photos> dal = new BaseDAL<Photos>();
        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("PhotoID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("PhotoID", idList);
        }
    }
}
