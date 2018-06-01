namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 去除重复查询命令生成树
    /// </summary>
    public class DistinctQueryTree : QueryTree
    {
        /// <summary>
        /// 创建Select节点
        /// </summary>
        /// <returns>Select节点</returns>
        protected override IBuilderCollection CreateSelectBuilder()
        {
            return new BuilderCollection()
            {
                TitleLeftSpace = "    SELECT DISTINCT\n           ",
                BodyLeftSpace = "           ",
                BodyRightSpace = ",\n",
                LastRightSpace = string.Empty
            };
        }

        /// <summary>
        /// 创建去除重复查询命令生成树
        /// </summary>
        /// <param name="parameterMarker">sql参数标识符号</param>
        public DistinctQueryTree(char parameterMarker)
            : base(parameterMarker) { }
    }
}
