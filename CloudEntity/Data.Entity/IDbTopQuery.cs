using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// TOP实体查询数据源接口(查询前几条实体的数据)
    /// Apple_Li 李凯 15150598493
    /// 2023/02/19 19:54
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IDbTopQuery<TEntity> : IDbBase, IEnumerable<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 查询的前几条的元素数量
        /// </summary>
        int TopCount { get; }
        /// <summary>
        /// 获取查询Sql字符串
        /// </summary>
        /// <returns>查询Sql字符串</returns>
        string ToSqlString();
        /// <summary>
        /// 将此数据源的查询结果映射为TModel对象数据源
        /// </summary>
        /// <typeparam name="TModel">TModel对象（只要是有无参构造函数的类就可以）</typeparam>
        /// <returns>TModel对象数据源</returns>
        IEnumerable<TModel> Cast<TModel>()
            where TModel : class, new();
    }
}