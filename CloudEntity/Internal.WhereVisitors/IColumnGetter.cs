using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity.Internal.WhereVisitors
{
    /// <summary>
    /// 列获取器
    /// </summary>
    public interface IColumnGetter
    {
        /// <summary>
        /// 获取列全名
        /// </summary>
        /// <param name="memberExpression">指定属性表达式</param>
        /// <returns>列的全名(临时表名.列名)</returns>
        string GetColumnFullName(MemberExpression memberExpression);
    }
}
