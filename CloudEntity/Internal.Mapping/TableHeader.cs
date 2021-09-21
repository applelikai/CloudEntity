using CloudEntity.Mapping;

namespace CloudEntity.Internal.Mapping
{
    /// <summary>
    /// 表的基础数据
    /// </summary>
    internal class TableHeader : ITableHeader
    {
        /// <summary>
        /// 架构名
        /// </summary>
        public string SchemaName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 表的别名(sql语句中使用)
        /// </summary>
        public string TableAlias { get; set; }
    }
}
