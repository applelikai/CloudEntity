using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// Join sql表达式节点
    /// </summary>
    public class JoinBuilder : INodeBuilder
    {
        /// <summary>
        /// 操作符(INNER JOIN 或 LEFT JOIN)
        /// </summary>
        private string joinOperator;
        /// <summary>
        /// Table表达式节点
        /// </summary>
        private ISqlBuilder tableBuilder;
        /// <summary>
        /// ON后面的表达式节点
        /// </summary>
        private IBuilderCollection onBuilders;

        /// <summary>
        /// ON后面的表达式节点
        /// </summary>
        public IBuilderCollection OnBuilders
        {
            get
            {
                if (this.onBuilders == null)
                {
                    this.onBuilders = new BuilderCollection()
                    {
                        TitleLeftSpace = "\n        ON ",
                        BodyLeftSpace = "       AND ",
                        BodyRightSpace = "\n",
                        LastRightSpace = string.Empty
                    };
                }
                return this.onBuilders;
            }
        }
        /// <summary>
        /// 当前节点类型
        /// </summary>
        public BuilderType BuilderType
        {
            get { return BuilderType.Join; }
        }
        /// <summary>
        /// 父节点类型
        /// </summary>
        public SqlType ParentNodeType
        {
            get { return SqlType.From; }
        }

        /// <summary>
        /// 创建Join sql表达式节点
        /// </summary>
        /// <param name="joinOperator">操作符(INNER JOIN 或 LEFT JOIN)</param>
        /// <param name="tableBuilder">Table表达式节点</param>
        private JoinBuilder(string joinOperator, ISqlBuilder tableBuilder)
        {
            this.joinOperator = joinOperator;
            this.tableBuilder = tableBuilder;
        }

        /// <summary>
        /// 拼接sql语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            commandText.Append(this.joinOperator);  //INNER JOIN 
            this.tableBuilder.Build(commandText);   //INNER JOIN dbo.Contracts contract
            this.OnBuilders.Build(commandText);     //		  ON contract.CompanyId = customer.CompanyId
        }
        /// <summary>
        /// 创建INNER JOIN表达式节点
        /// </summary>
        /// <param name="tableBuilder">Table表达式节点</param>
        /// <returns>INNER JOIN表达式节点</returns>
        public static JoinBuilder GetInnerJoinBuilder(ISqlBuilder tableBuilder)
        {
            return new JoinBuilder("INNER JOIN ", tableBuilder);
        }
        /// <summary>
        /// 创建LEFT JOIN表达式节点
        /// </summary>
        /// <param name="tableBuilder">Table表达式节点</param>
        /// <returns>LEFT JOIN表达式节点</returns>
        public static JoinBuilder GetLeftJoinBuilder(ISqlBuilder tableBuilder)
        {
            return new JoinBuilder(" LEFT JOIN ", tableBuilder);
        }
    }
}
