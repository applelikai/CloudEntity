using CloudEntity.Mapping;
using System.Reflection;

namespace CloudEntity.Internal.Mapping
{
    /// <summary>
    /// 列与属性映射类
    /// </summary>
    internal class ColumnMapper : IColumnMapper
    {
        /// <summary>
        /// 列名(或者列的别名)
        /// </summary>
        private string columnName;

        /// <summary>
        /// 当前Mapping的属性
        /// </summary>
        public PropertyInfo Property { get; private set; }
        /// <summary>
        /// 可对改属性所Mapping的列的操作(默认为InsertAndEdit)
        /// </summary>
        public ColumnAction ColumnAction { get; set; }
        /// <summary>
        /// 列名(为空时取属性名)
        /// </summary>
        public string ColumnName
        {
            get { return this.columnName ?? (this.columnName = this.Property.Name); }
            set { this.columnName = value; }
        }
        /// <summary>
        /// 列的全名
        /// </summary>
        public string ColumnFullName { get; set; }
        /// <summary>
        /// 列的别名（可以为空）
        /// </summary>
        public string ColumnAlias { get; set; }
        /// <summary>
        /// 数据类型(可以为空)
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 长度(可以为空)
        /// </summary>
        public int? Length { get; set; }
        /// <summary>
        /// 小数点位数
        /// </summary>
        public int? Decimals { get; set; }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool AllowNull { get; set; }

        /// <summary>
        /// 创建ColumnMapper
        /// </summary>
        /// <param name="property">属性</param>
        public ColumnMapper(PropertyInfo property)
        {
            Check.ArgumentNull(property, nameof(property));
            this.Property = property;
            this.AllowNull = true;
        }
    }
}
