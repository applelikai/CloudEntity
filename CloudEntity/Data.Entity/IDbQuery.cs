using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 查询数据源
    /// Apple_Li 李凯 15150598493
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IDbQuery<TEntity> : IDbSource<TEntity>, IDbBase
        where TEntity : class
    {
        /// <summary>
        /// 当前对象的关联对象属性链接数组
        /// </summary>
        IEnumerable<PropertyLinker> PropertyLinkers { get; }

        /// <summary>
        /// 获取sql字符串
        /// </summary>
        /// <returns>sql字符串</returns>
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
