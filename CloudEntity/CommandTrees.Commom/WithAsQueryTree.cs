using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// With As 查询命令生成树
    /// 李凯 Apple_Li 15150598493
    /// </summary>
    internal class WithAsQueryTree : CommandTree
    {
        /// <summary>
        /// 查询命令生成树的Where节点
        /// </summary>
        private IBuilderCollection whereBuilder;
        /// <summary>
        /// 查询命令生成树的Order By节点
        /// </summary>
        private IBuilderCollection orderByBuilder;

        /// <summary>
        /// 查询sql
        /// </summary>
        protected string InnerQuerySql { get; private set; }
        /// <summary>
        /// 临时表名
        /// </summary>
        protected string TableAlias { get; private set; }

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
                    TitleLeftSpace = "\n   WHERE ",
                    BodyLeftSpace = "     AND ",
                    BodyRightSpace = "\n",
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
                    TitleLeftSpace = "\nORDER BY ",
                    BodyLeftSpace = "         ",
                    BodyRightSpace = ",\n",
                    LastRightSpace = string.Empty
                };
                //回到Start
                goto Start;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        /// <param name="innerQuerySql">查询sql</param>
        /// <param name="tableAlias">临时表名</param>
        public WithAsQueryTree(char parameterMarker, string innerQuerySql, string tableAlias)
            : base(parameterMarker)
        {
            this.InnerQuerySql = innerQuerySql;
            this.TableAlias = tableAlias;
        }
        /// <summary>
        /// 构建sql命令
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public override void Build(StringBuilder commandText)
        {
            commandText.AppendFormat("WITH {0} AS\n", this.TableAlias);
            commandText.AppendLine("(");
            commandText.AppendLine(this.InnerQuerySql);
            commandText.AppendLine(")");
            commandText.AppendLine("  SELECT *");
            commandText.AppendFormat("    FROM {0}", this.TableAlias);
            this.Where.Build(commandText);
            this.OrderBy.Build(commandText);
        }
    }
}
