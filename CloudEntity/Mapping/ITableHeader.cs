namespace CloudEntity.Mapping
{
    /// <summary>
    /// 表的基础数据
    /// </summary>
    public interface ITableHeader
    {
        /// <summary>
        /// 架构名
        /// </summary>
        string SchemaName { get; }
        /// <summary>
        /// 表名
        /// </summary>
        string TableName { get; }
        /// <summary>
        /// 表的别名(sql语句中使用)
        /// </summary>
        string TableAlias { get; }
    }
}
