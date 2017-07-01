using System;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 生成建表语句所需的Column节点
    /// </summary>
    public class ColumnNode : IColumnNode
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// 源数据类型
        /// </summary>
        public Type SourceType { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public string SqlDataType { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int? Length { get; set; }
        /// <summary>
        /// 是否为默认值
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool IsNull { get; set; }
        /// <summary>
        /// 是否为主键
        /// </summary>
        public bool IsPrimary { get; set; }
        /// <summary>
        /// 是否为自增列
        /// </summary>
        public bool IsIdentity { get; set; }
    }
}
