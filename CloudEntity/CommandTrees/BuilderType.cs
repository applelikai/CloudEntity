namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// sql表达式节点的类型
    /// 李凯 Apple_Li
    /// </summary>
    public enum BuilderType
    {
        /// <summary>
        /// 任意类型的节点
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 当前节点为ColumnBuilder
        /// </summary>
        Column = 1,
        /// <summary>
        /// 当前节点为TableBuilder
        /// </summary>
        Table = 2,
        /// <summary>
        /// 当前节点为JoinBuilder
        /// </summary>
        Join = 3,
        /// <summary>
        /// 当前节点为BinaryBuilder
        /// </summary>
        Binary = 4
    }
}
