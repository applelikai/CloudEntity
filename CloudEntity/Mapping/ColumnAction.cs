namespace CloudEntity.Mapping
{
    /// <summary>
    /// 对当前列执行的操作
    /// </summary>
    public enum ColumnAction
    {
        /// <summary>
        /// 执行Insert或Update时都包含该列
        /// </summary>
        InsertAndEdit = 0,
        /// <summary>
        /// 执行Insert时包含该列
        /// </summary>
        Insert = 1,
        /// <summary>
        /// Update时包含该列
        /// </summary>
        Edit = 2,
        /// <summary>
        /// Update时包含该列
        /// </summary>
        EditAndDefault = 3,
        /// <summary>
        /// 默认值
        /// </summary>
        Default = 4,
        /// <summary>
        /// 该列为主键,但Insert时必须包含该列
        /// </summary>
        PrimaryAndInsert = 5,
        /// <summary>
        /// 该列为主键,也是自增列
        /// </summary>
        PrimaryAndIdentity = 6
    }
}
