namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 基本Sql节点
    /// Apple_Li 李凯
    /// </summary>
    public class NodeBuilder : SqlBuilder, INodeBuilder
    {
        /// <summary>
        /// /父节点的类型
        /// </summary>
        public SqlType ParentNodeType { get; private set; }
        /// <summary>
        /// 当前节点类型
        /// </summary>
        public BuilderType BuilderType
        {
            get { return BuilderType.Normal; }
        }

        /// <summary>
        /// 创建基本sql节点
        /// </summary>
        /// <param name="parentNodeType">父节点类型</param>
        /// <param name="teamplate">字符串模板</param>
        /// <param name="arguments">格式化参数</param>
        public NodeBuilder(SqlType parentNodeType, string teamplate, params object[] arguments)
            : base(teamplate, arguments)
        {
            this.ParentNodeType = parentNodeType;
        }
    }
}
