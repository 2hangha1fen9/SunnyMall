using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class AppraisesBLL : BaseBLL<Appraises>
    {
        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("AppraiseID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("AppraiseID", idList);
        }
    }
}
