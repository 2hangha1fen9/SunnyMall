using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public class NewsBLL : BaseBLL<News>
    {
        public override bool DeleteEntityById(int id)
        {
            return dal.Delete("NewsID", id);
        }

        public override bool DeleteEntityByIdList(string idList)
        {
            return dal.Delete("NewsID", idList);
        }
    }
}
