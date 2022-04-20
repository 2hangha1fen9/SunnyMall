using DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BLL
{
    public abstract class BaseBLL<T> where T:class,new()
    {
        /// <summary>
        /// 数据访问层对象
        /// </summary>
        protected BaseDAL<T> dal = new BaseDAL<T>();

        /// <summary>
        /// 添加实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public T AddEntity(T t)
        {
            return dal.Add (t);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool DeleteEntity(T t)
        {
            return dal.Delete (t);
        }

        /// <summary>
        /// 根据Id删除实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public abstract bool DeleteEntityById(int id);

        /// <summary>
        /// 根据ID列表删除实体
        /// </summary>
        /// <param name="idList"></param>
        /// <returns></returns>
        public abstract bool DeleteEntityByIdList(string idList);


        /// <summary>
        /// 更新实体
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool UpdateEntity(T t)
        {
            return dal.Update (t);
        }

        /// <summary>
        /// 获取所有实体
        /// </summary>
        /// <returns></returns>
        public IQueryable<T> ListEntity()
        {
            return dal.List();
        }

        /// <summary>
        /// 根据条件列出实体
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public IQueryable<T> ListEntityByCondition(Expression<Func<T, bool>> condition)
        {
            return dal.List(condition);
        }

        /// <summary>
        /// 根据条件查询实体
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public T FindEntityByCondition(Expression<Func<T, bool>> condition)
        {
            return dal.Entity(condition);
        }

        /// <summary>
        /// 根据Id查询实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public T FindEntityById(int id)
        {
            return dal.Entity(id);
        }

        /// <summary>
        /// 获取实体个数
        /// </summary>
        /// <returns></returns>
        public int GetEntityCount()
        {
            return dal.Count();
        }

        /// <summary>
        /// 根据条件获取实体个数
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public int GetEntityCountByCondition(Expression<Func<T, bool>> condition)
        {
            return dal.Count(condition);
        }

        /// <summary>
        /// 判读实体是否存在
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public bool IsEntityExist(Expression<Func<T, bool>> condition)
        {
            return dal.Exist(condition);
        }
    }
}
