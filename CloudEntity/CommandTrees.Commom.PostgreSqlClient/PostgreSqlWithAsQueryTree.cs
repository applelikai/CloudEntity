using System.Text;

namespace CloudEntity.CommandTrees.Commom.PostgreSqlClient
{
    /// <summary>
    /// 用于 Postgresql 的 With As 查询命令生成树
    /// 李凯 Apple_Li 15150598493
    /// </summary>
    internal class PostgreSqlWithAsQueryTree : WithAsQueryTree
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        /// <param name="innerQuerySql">查询sql</param>
        /// <param name="tableAlias">临时表名</param>
        public PostgreSqlWithAsQueryTree(char parameterMarker, string innerQuerySql, string tableAlias)
         : base(parameterMarker, innerQuerySql, tableAlias) { }
        /// <summary>
        /// 构建sql命令
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public override void Build(StringBuilder commandText)
        {
            commandText.AppendFormat("WITH \"{0}\" AS\n", base.TableAlias);
            commandText.AppendLine("(");
            commandText.AppendLine(base.InnerQuerySql);
            commandText.AppendLine(")");
            commandText.AppendLine("  SELECT *");
            commandText.AppendFormat("    FROM \"{0}\"\n", base.TableAlias);
            this.Where.Build(commandText);
            this.OrderBy.Build(commandText);
        }
    }
}