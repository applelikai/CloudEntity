using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Mapping;

namespace CloudEntity.Core.Data.Entity
{
    /// <summary>
    /// ColumnMapper扩展类
    /// </summary>
    internal static class ExtendColumnMapper
    {
        /// <summary>
        /// 转换获取Column节点
        /// </summary>
        /// <param name="columnMapper">Column元数据解析器</param>
        /// <returns>Column节点</returns>
        internal static IColumnNode ToColumnNode(this IColumnMapper columnMapper)
        {
            return new ColumnNode()
            {
                ColumnName = columnMapper.ColumnName,
                SourceType = columnMapper.Property.PropertyType,
                SqlDataType = columnMapper.DataType,
                Length = columnMapper.Length,
                Decimals = columnMapper.Decimals,
                IsDefault = columnMapper.ColumnAction.ToString().Contains("Default"),
                IsNull = columnMapper.AllowNull,
                IsPrimary = columnMapper.ColumnAction.ToString().StartsWith("Primary"),
                IsIdentity = columnMapper.ColumnAction == ColumnAction.PrimaryAndIdentity
            };
        }
    }
}
