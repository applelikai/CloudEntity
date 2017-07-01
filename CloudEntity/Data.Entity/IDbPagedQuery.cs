using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 分页查询数据源接口
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
    }
}
