using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 查询命令生成树
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
	public class QueryTree : CommandTree
    {
        /// <summary>
        /// Select子节点集合
        /// </summary>
        private IBuilderCollection _selectBuilders;

        /// <summary>
        /// Select节点
        /// </summary>
        protected IBuilderCollection Select
        {
            get
            {
                // 若Select子节点集合为空
                if (_selectBuilders == null)
                    // 则创建Select子节点集合
                    _selectBuilders = this.CreateSelectBuilder();
                // 最后获取Select子节点集合
                return _selectBuilders;
            }
        }
        /// <summary>
        /// From节点
        /// </summary>
        protected IBuilderCollection From { get; private set; }
        /// <summary>
        /// Where节点
        /// </summary>
        protected IBuilderCollection Where { get; private set; }
        /// <summary>
        /// Group by nodes
        /// </summary>
        protected IBuilderCollection GroupBy { get; private set; }
        /// <summary>
        /// Order by nodes
        /// </summary>
        protected IBuilderCollection OrderBy { get; private set; }

        /// <summary>
        /// 创建Select节点
        /// </summary>
        /// <returns>Select节点</returns>
        protected virtual IBuilderCollection CreateSelectBuilder()
        {
            return new BuilderCollection()
            {
                TitleLeftSpace = "    SELECT ",
                BodyLeftSpace = "           ",
                BodyRightSpace = ",\n",
                LastRightSpace = string.Empty
            };
        }

        /// <summary>
        /// 查询命令生成树
        /// </summary>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        public QueryTree(char parameterMarker)
            : base(parameterMarker) 
        {
            // 初始化各个子节点
            this.From = new BuilderCollection("\n      FROM ", string.Empty, "\n", string.Empty);
            this.Where = new BuilderCollection("\n     WHERE ", "       AND ", "\n", string.Empty);
            this.GroupBy = new BuilderCollection("\n  GROUP BY", "           ", ",\n", string.Empty);
            this.OrderBy = new BuilderCollection("\n  ORDER BY ", "           ", ",\n", string.Empty);
        }

        /// <summary>
        /// 添加SELECT子节点
        /// </summary>
        /// <param name="sqlBuilder">sql子节点</param>
        public void AppendSelect(ISqlBuilder sqlBuilder)
        {
            // 为SELECT节点添加子节点
            this.Select.Append(sqlBuilder);
        }
        /// <summary>
        /// 添加FROM子节点
        /// </summary>
        /// <param name="sqlBuilder">sql子节点</param>
        public void AppendFrom(ISqlBuilder sqlBuilder)
        {
            this.From.Append(sqlBuilder);
        }
        /// <summary>
        /// 添加WHERE子节点
        /// </summary>
        /// <param name="sqlBuilder">sql子节点</param>
        public void AppendWhere(ISqlBuilder sqlBuilder)
        {
            this.Where.Append(sqlBuilder);
        }
        /// <summary>
        /// 添加GROUP BY子节点
        /// </summary>
        /// <param name="sqlBuilder">sql子节点</param>
        public void AppendGroupBy(ISqlBuilder sqlBuilder)
        {
            this.GroupBy.Append(sqlBuilder);
        }
        /// <summary>
        /// 添加ORDER BY子节点
        /// </summary>
        /// <param name="sqlBuilder">sql子节点</param>
        public void AppendOrderBy(ISqlBuilder sqlBuilder)
        {
            this.OrderBy.Append(sqlBuilder);
        }
        /// <summary>
        /// 构建sql命令
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public override void Build(StringBuilder commandText)
        {
            this.Select.Build(commandText);
            this.From.Build(commandText);
            this.Where.Build(commandText);
            this.GroupBy.Build(commandText);
            this.OrderBy.Build(commandText);
        }
    }
}