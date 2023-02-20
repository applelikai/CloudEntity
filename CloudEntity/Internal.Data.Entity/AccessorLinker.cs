using CloudEntity.Mapping;
using System.Reflection;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 对象访问关联类
    /// 李凯 Apple_Li 15150598493
    /// </summary>
    public class AccessorLinker
    {
        /// <summary>
        /// 关联属性的名称
        /// </summary>
        public string PropertyName { get; private set; }
        /// <summary>
        /// 查询列对应的的ColumnMapper数组
        /// </summary>
        public IColumnMapper[] ColumnMappers { get; private set; }
        /// <summary>
        /// 对象访问器
        /// </summary>
        public ObjectAccessor EntityAccessor { get; private set; }
        /// <summary>
        /// 子属性的访问关联数组
        /// </summary>
        public AccessorLinker[] AccessorLinkers { get; private set; }

        /// <summary>
        /// 创建对象访问关联
        /// </summary>
        /// <param name="property">关联对象属性</param>
        /// <param name="columnMappers">列对应的的ColumnMapper数组</param>
        /// <param name="accessorLinkers">子属性的访问关联数组</param>
        public AccessorLinker(PropertyInfo property, IColumnMapper[] columnMappers, params AccessorLinker[] accessorLinkers)
        {
            //非空检查
            Check.ArgumentNull(property, nameof(property));
            Check.ArgumentNull(columnMappers, nameof(columnMappers));
            //赋值
            this.PropertyName = property.Name;
            this.EntityAccessor = ObjectAccessor.GetAccessor(property.PropertyType);
            this.ColumnMappers = columnMappers;
            this.AccessorLinkers = accessorLinkers;
        }
    }
}
