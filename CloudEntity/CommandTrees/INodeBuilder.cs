namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// 基本Sql节点
    /// Apple_Li 李凯
    /// </summary>
    public interface INodeBuilder : ISqlBuilder
    {
        /// <summary>
        /// 当前节点类型
        /// </summary>
        BuilderType BuilderType { get; }
        /// <summary>
        /// 父节点类型
        /// </summary>
        SqlType ParentNodeType { get; }
    }
}
