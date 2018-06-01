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
        /// <param name="property">属性</param>
        /// <returns>列的全名(临时表名.列名)</returns>
        public string GetColumnFullName(PropertyInfo property)
        {
            return string.Format("t.{0}", property.Name);
        }
    }
}
