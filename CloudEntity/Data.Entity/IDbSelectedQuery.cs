using System.Collections.Generic;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 选定项查询数据源接口
    /// Apple_Li 李凯 15150598493
    /// 2017/06/19 最后修改：2023/02/09
    /// </summary>
    /// <typeparam name="TElement">元素类型</typeparam>
    public interface IDbSelectedQuery<TElement> : IDbBase, IEnumerable<TElement>
    {
        /// <summary>
        /// 获取查询Sql字符串
        /// </summary>
        /// <returns>查询Sql字符串</returns>
        string ToSqlString();
    }
}
