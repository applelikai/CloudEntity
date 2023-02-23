using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 选定项查询数据源接口
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TElement">元素类型</typeparam>
    public interface IDbSelectedQuery<TElement> : IDbBase, IEnumerable<TElement>
    {
        /// <summary>
        /// 获取查询Sql字符串
        /// </summary>
        /// <returns>查询Sql字符串</returns>
        string ToSqlString();
        /// <summary>
        /// 获取的查询列名列表
        /// </summary>
        /// <returns>查询列名列表</returns>
        IEnumerable<string> GetSelectNames();
    }
}
