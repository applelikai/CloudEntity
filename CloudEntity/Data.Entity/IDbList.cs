using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 可读可写的操作DB的对象数据源
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IDbList<TEntity> : IDbOperator<TEntity>, IDbSource<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 获取当前实体对象
        /// </summary>
        /// <param name="id">实体的ID</param>
        /// <returns>实体对象</returns>
        TEntity this[object id] { get; }

        /// <summary>
        /// 判断当前ID的实体是否存在
        /// </summary>
        /// <param name="id">实体的ID</param>
        /// <returns>1:存在 0:不存在</returns>
        int Exist(object id);
        /// <summary>
        /// Add many entities to databse
        /// </summary>
        /// <param name="entities">entities</param>
        /// <returns>How many rows changed</returns>
        int AddRange(IEnumerable<TEntity> entities);
        /// <summary>
        /// 批量保存实体信息
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>保存的实体数量</returns>
        int SaveAll(IEnumerable<TEntity> entities);
        /// <summary>
        /// 获取查询sql字符串
        /// </summary>
        /// <returns>查询sql字符串</returns>
        string ToSqlString();
    }
}
