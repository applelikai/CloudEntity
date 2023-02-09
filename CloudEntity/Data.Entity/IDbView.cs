using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 视图查询数据源
    /// Apple_Li 李凯 15150598493
    /// </summary>
    /// <typeparam name="TModel">对象类型</typeparam>
    public interface IDbView<TModel> : IDbBase, IEnumerable<TModel>
        where TModel : class, new()
    {
        /// <summary>
        /// 查询数据源创建工厂
        /// </summary>
        IDbFactory Factory { get; }
        /// <summary>
        /// 查询sql
        /// </summary>
        string InnerQuerySql { get; }
    }
}
