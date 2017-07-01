using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 选定项查询数据源接口
    /// Apple_Li 李凯 2017/06/19
    /// </summary>
    /// <typeparam name="TElement">元素类型</typeparam>
    public interface IDbSelectedQuery<TElement> : IDbBase, IEnumerable<TElement>
    {
        /// <summary>
        /// 获取条件查询Sql
        /// </summary>
        /// <returns>条件查询Sql</returns>
        string ToWhereSqlString();
    }
}
