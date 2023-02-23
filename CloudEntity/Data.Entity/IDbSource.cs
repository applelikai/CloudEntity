using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 基本数据源数据源
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IDbSource<TEntity> : IDbBase, IEnumerable<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 数据操作类的工厂
        /// </summary>
        IDbFactory Factory { get; }
    }
}
