using Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class ContextFactory
    {
        /// <summary>
        /// 获取数据库上下文
        /// </summary>
        /// <returns></returns>
        public static DbContext GetContext()
        {
            DbContext context = CallContext.GetData("DbContext") as DbContext;
            if (context == null)
            {
                context = new Entities();
                CallContext.SetData("DbContext",context);
            }
            return context;
        }
    }
}
