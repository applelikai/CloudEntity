using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity.Internal.WhereVisitors
{
    /// <summary>
    /// 列获取类
    /// </summary>
    internal class ColumnGetter : IColumnGetter
    {
        /// <summary>
        /// 获取列全名
        /// </summary>
        /// <param name="memberExpression">指定属性表达式</param>
        /// <returns>列的全名(临时表名.列名)</returns>
        public string GetColumnFullName(MemberExpression memberExpression)
        {
            return string.Format("t.{0}", memberExpression.Member.Name);
        }
    }
}
