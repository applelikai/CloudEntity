using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 实体操作接口
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IDbOperator <TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 创建查询数据源的工厂
        /// </summary>
        IDbFactory Factory { get; }

        /// <summary>
        /// 向数据库插入实体对象指定插入的列的值
        /// </summary>
        /// <param name="entity">待添加的实体对象</param>
        /// <returns>添加的实体对象的数量</returns>
        int Add(TEntity entity);
        /// <summary>
        /// 向数据库插入实体对象所有非自增标识的列的值
        /// </summary>
        /// <param name="entity">待添加的实体对象</param>
        /// <returns>添加的实体对象的数量</returns>
        int Insert(TEntity entity);

        /// <summary>
        /// 删除数据库中的某个实体对象
        /// </summary>
        /// <param name="entity">待删除的实体对象</param>
        /// <returns>删除的实体对象的数量</returns>
        int Remove(TEntity entity);
        /// <summary>
        /// 批量删除数据库中的实体对象
        /// </summary>
        /// <param name="entities">待删除的实体对象数据源</param>
        /// <returns>删除的实体对象的数量</returns>
        int RemoveAll(IDbQuery<TEntity> entities);

        /// <summary>
        /// 将实体对象的所有的非标识属性的值至数据库
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>修改的实体对象的数量</returns>
        int Modify(TEntity entity);
        /// <summary>
        /// 保存实体对象中所有指定修改的属性的值至数据库
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>修改的实体对象的数量</returns>
        int Save(TEntity entity);
        /// <summary>
        /// 批量修改符合条件的实体的信息
        /// </summary>
        /// <param name="setParameters">属性名及属性值字典</param>
        /// <param name="entities">待修改的实体的数据源</param>
        /// <returns>修改的实体对象的数量</returns>
        int SaveAll(IDictionary<string, object> setParameters, IDbQuery<TEntity> entities);
    }
}
