namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// top查询命令生成树
    /// </summary>
    public class TopQueryTree : QueryTree
    {
        /// <summary>
        /// 查询的前几条的元素数量
        /// </summary>
        private int topCount;

        /// <summary>
        /// 创建Select节点
        /// </summary>
        /// <returns>Select节点</returns>
        protected override IBuilderCollection CreateSelectBuilder()
        {
            return new BuilderCollection()
            {
                TitleLeftSpace = string.Format("    SELECT TOP {0}\n           ", this.topCount),
                BodyLeftSpace = "           ",
                BodyRightSpace = ",\n",
                LastRightSpace = string.Empty
            };
        }
        
        /// <summary>
        /// 创建top查询命令生成树
        /// </summary>
        /// <param name="parameterMarker">sql参数标识符号</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        public TopQueryTree(char parameterMarker, int topCount)
            : base(parameterMarker)
        {
            this.topCount = topCount;
        }
    }
}