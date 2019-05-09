using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 查询数据源
    /// Apple_Li 李凯
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IDbQuery<TEntity> : IEnumerable<TEntity>, IDbBase
        where TEntity : class
    {
        /// <summary>
        /// 数据操作类的工厂
        /// </summary>
        IDbFactory Factory { get; }
        /// <summary>
        /// Sql参数创建对象
        /// </summary>
        IParameterFactory ParameterFactory { get; }
        /// <summary>
        /// 当前对象的关联对象属性链接数组
        /// </summary>
        PropertyLinker[] PropertyLinkers { get; }
    }
}
