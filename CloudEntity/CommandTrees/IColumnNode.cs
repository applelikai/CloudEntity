using System;

namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// 生成建表语句所需的Column节点
    /// </summary>
    public interface IColumnNode
    {
        /// <summary>
        /// 列名
        /// </summary>
        string ColumnName { get; }
        /// <summary>
        /// 源数据类型
        /// </summary>
        Type SourceType { get; }
        /// <summary>
        /// 数据类型
        /// </summary>
        string SqlDataType { get; }
        /// <summary>
        /// 长度
        /// </summary>
        int? Length { get; }
        /// <summary>
        /// 是否设置默认值
        /// </summary>
        bool IsDefault { get; }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        bool IsNull { get; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        bool IsPrimary { get; }
        /// <summary>
        /// 是否为自增列
        /// </summary>
        bool IsIdentity { get; }
    }
}
