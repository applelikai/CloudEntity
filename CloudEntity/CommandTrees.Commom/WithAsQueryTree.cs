using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// With As 查询命令生成树
    /// 李凯 Apple_Li 15150598493
    /// </summary>
    public class WithAsQueryTree : CommandTree
    {
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
        public IBuilderCollection Where { get; private set; }
        /// <summary>
        /// Order by nodes
        /// </summary>
        public IBuilderCollection OrderBy { get; private set; }

        /// <summary>
        /// 拼接WITH AS语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="tableAlias">临时表名</param>
        protected virtual void AppendWithAs(StringBuilder commandText, string tableAlias)
        {
            commandText.AppendFormat("WITH {0} AS", tableAlias);
        }
        /// <summary>
        /// 拼接FROM语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="tableAlias">临时表名</param>
        protected virtual void AppendFrom(StringBuilder commandText, string tableAlias)
        {
            commandText.AppendFormat("    FROM {0}", tableAlias);
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
            this.Where = new BuilderCollection("\n   WHERE ", "     AND ", "\n", string.Empty);
            this.OrderBy = new BuilderCollection("\nORDER BY ", "         ", ",\n", string.Empty);
        }
        /// <summary>
        /// 构建sql命令
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public override void Build(StringBuilder commandText)
        {
            this.AppendWithAs(commandText, this.TableAlias);
            commandText.AppendLine("\n(");
            commandText.AppendLine(this.InnerQuerySql);
            commandText.AppendLine(")");
            commandText.AppendLine("  SELECT *");
            this.AppendFrom(commandText, this.TableAlias);
            this.Where.Build(commandText);
            this.OrderBy.Build(commandText);
        }
    }
}
