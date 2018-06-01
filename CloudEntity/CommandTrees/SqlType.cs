namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// sql类型
    /// </summary>
    public enum SqlType
    {
        /// <summary>
        /// SELECT语句
        /// </summary>
        Select,
        /// <summary>
        /// FROM语句
        /// </summary>
        From,
        /// <summary>
        /// 条件表达式节点
        /// </summary>
        Where,
        /// <summary>
        /// 排序节点
        /// </summary>
        OrderBy,
        /// <summary>
        /// 分组节点
        /// </summary>
        GroupBy,
        /// <summary>
        /// UPDATE命令的Set节点
        /// </summary>
        UpdateSet
    }
}
