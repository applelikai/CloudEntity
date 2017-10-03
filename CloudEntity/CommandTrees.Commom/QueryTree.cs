using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 查询命令生成树
    /// 李凯 Apple_Li 2017/05/21
    /// </summary>
	public class QueryTree : CommandTree
    {
        private IBuilderCollection selectBuilder;   //查询命令生成树的Select节点
        private IBuilderCollection fromBuilder;     //查询命令生成树的From节点
        private IBuilderCollection whereBuilder;    //查询命令生成树的Where节点
        private IBuilderCollection groupByBuilder;  //查询命令生成树的Group By节点
        private IBuilderCollection orderByBuilder;  //查询命令生成树的Order By节点

        /// <summary>
        /// Select节点
        /// </summary>
        public IBuilderCollection Select
        {
            get
            {
            Start:
                //若selectBuilder不为空,直接返回
                if (this.selectBuilder != null)
                    return this.selectBuilder;
                //创建selectBuilder并回到Start
                this.selectBuilder = this.CreateSelectBuilder();
                goto Start;
            }
        }
        /// <summary>
        /// From节点
        /// </summary>
        public IBuilderCollection From
        {
            get
            {
            Start:
                //若fromBuilder不为空,直接返回
                if (this.fromBuilder != null)
                    return this.fromBuilder;
                //创建fromBuilder
                this.fromBuilder = new BuilderCollection()
                {
                    TitleLeftSpace = "\n      FROM ",
                    BodyLeftSpace = string.Empty,
                    BodyRightSpace = "\n",
                    LastRightSpace = string.Empty
                };
                //回到Start
                goto Start;
            }
        }
        /// <summary>
        /// Where节点
        /// </summary>
        public IBuilderCollection Where
        {
            get
            {
            Start:
                //若whereBuilder不为空,直接返回
                if (this.whereBuilder != null)
                    return this.whereBuilder;
                //创建whereBuilder
                this.whereBuilder = new BuilderCollection()
                {
                    TitleLeftSpace = "\n     WHERE ",
                    BodyLeftSpace = "       AND ",
                    BodyRightSpace = "\n",
                    LastRightSpace = string.Empty
                };
                //回到Start
                goto Start;
            }
        }
        /// <summary>
        /// Group by nodes
        /// </summary>
        public IBuilderCollection GroupBy
        {
            get
            {
            Start:
                //若groupByBuilder不为空,直接返回
                if (this.groupByBuilder != null)
                    return this.groupByBuilder;
                //创建groupByBuilder
                this.groupByBuilder = new BuilderCollection()
                {
                    TitleLeftSpace = "\n  GROUP BY",
                    BodyLeftSpace = "           ",
                    BodyRightSpace = ",\n",
                    LastRightSpace = string.Empty
                };
                //回到Start
                goto Start;
            }
        }
        /// <summary>
        /// Order by nodes
        /// </summary>
        public IBuilderCollection OrderBy
        {
            get
            {
            Start:
                //若orderByBuilder不为空,直接返回
                if (this.orderByBuilder != null)
                    return this.orderByBuilder;
                //创建orderByBuilder
                this.orderByBuilder = new BuilderCollection()
                {
                    TitleLeftSpace = "\n  ORDER BY ",
                    BodyLeftSpace = "           ",
                    BodyRightSpace = ",\n",
                    LastRightSpace = string.Empty
                };
                //回到Start
                goto Start;
            }
        }

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
            : base(parameterMarker) { }

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