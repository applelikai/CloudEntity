using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 分页查询数据源接口
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    public interface IDbPagedQuery<TEntity> : IEnumerable<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 元素总数量
        /// </summary>
        int Count { get; }
        /// <summary>
        /// 页元素数量
        /// </summary>
        int PageSize { get; }
        /// <summary>
        /// 当前是第几页
        /// </summary>
        int PageIndex { get; }
        /// <summary>
        /// 总页数
        /// </summary>
        int PageCount { get; }

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
