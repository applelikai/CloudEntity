using System.Reflection;

namespace CloudEntity.Mapping
{
    /// <summary>
    /// 列与属性映射接口
    /// </summary>
    public interface IColumnMapper
    {
        /// <summary>
        /// 当前Mapping的属性
        /// </summary>
        PropertyInfo Property { get; }
        /// <summary>
        /// 可对改属性所Mapping的列的操作
        /// </summary>
        ColumnAction ColumnAction { get; }
        /// <summary>
        /// 列名
        /// </summary>
        string ColumnName { get; }
        /// <summary>
        /// 列的别名（可以为空）
        /// </summary>
        string ColumnAlias { get; }
        /// <summary>
        /// 数据类型
        /// </summary>
        string DataType { get; }
        /// <summary>
        /// 长度
        /// </summary>
        int? Length { get; }
        /// <summary>
        /// 小数点位数
        /// </summary>
        int? Decimals { get; }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        bool AllowNull { get; }
    }
}
