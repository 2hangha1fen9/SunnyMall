using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class BaseDAL<T> where T : class,new()
    {
        /// <summary>
        /// 数据库上下文
        /// </summary>
        private DbContext context = ContextFactory.GetContext();

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public T Add(T t)
        {
            context.Set<T>().Add(t);
            context.SaveChanges();
            return t;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Delete(T t)
        {
            context.Set<T>().Remove(t);
            return context.SaveChanges() > 0;
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Delete(IEnumerable<T> ts)
        {
            context.Set<T>().RemoveRange(ts);
            return context.SaveChanges() > 0;
        }

        /// <summary>
        /// 根据Id删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(string idColumnName,int id)
        {
            Type type = typeof(T);
            string tableName = type.Name;
            string sql = $"delete from {tableName} where {idColumnName} = @ID";
            SqlParameter[] paras = new SqlParameter[] {
                new SqlParameter("@ID",id)
            };
            return context.Database.ExecuteSqlCommand(sql, paras) > 0;
        }

        /// <summary>
        /// 根据id列表删除
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public bool Delete(string idColumnName,string idList)
        {
            Type type = typeof(T);
            string tableName = type.Name;
            string sql = $"delete from {tableName} where {idColumnName} in({idList})";
            return context.Database.ExecuteSqlCommand(sql) > 0;
        }

        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool Update(T t)
        {
            context.Set<T>().Attach(t);
            context.Entry(t).State = EntityState.Modified;
            return context.SaveChanges() > 0;
        }

        /// <summary>
        /// 列出所有实体
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> List()
        {
            return context.Set<T>().AsQueryable();
        }

        /// <summary>
        /// 按条件列出实体
        /// </summary>
        /// <param name="condition">lamdba表达式</param>
        /// <returns></returns>
        public IQueryable<T> List(Expression<Func<T, bool>> condition)
        {
            return context.Set<T>().Where(condition);
        }

        /// <summary>
        /// 查询实体
        /// </summary>
        /// <param name="condition">lamdba表达式</param>
        /// <returns></returns>
        public T Entity(Expression<Func<T, bool>> condition)
        {
            return context.Set<T>().Where(condition).FirstOrDefault();
        }

        public T Entity(int id)
        { 
            return context.Set<T>().Find(id);
        }

        /// <summary>
        /// 获取实体个数
        /// </summary>
        /// <returns></returns>
        public int Count()
        {
            return context.Set<T>().Count();
        }

        /// <summary>
        /// 按条件获取实体个数
        /// </summary>
        /// <param name="condition">lamdba表达式</param>
        /// <returns></returns>
        public int Count(Expression<Func<T, bool>> condition)
        {
            return context.Set<T>().Where(condition).Count();
        }

        /// <summary>
        /// 判断实体是否存在
        /// </summary>
        /// <param name="condition">lamdba表达式</param>
        /// <returns></returns>
        public bool Exist(Expression<Func<T, bool>> condition)
        {
            return context.Set<T>().Where(condition).Any();
        }
    }
}
