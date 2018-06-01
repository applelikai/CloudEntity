using System.Text;

namespace CloudEntity.CommandTrees.Commom.OracleClient
{
    /// <summary>
    /// 用于生成针对Oracle的分页查询sql
    /// 李凯 Apple_Li
    /// </summary>
    internal class OraclePagingQueryTree : QueryTree
    {
        /// <summary>
        /// 创建Oracle Sql分页生成树
        /// </summary>
        /// <param name="parameterMarker">sql参数标识符号</param>
        public OraclePagingQueryTree(char parameterMarker)
            : base(parameterMarker)
        {
            //添加列
            base.Select.Append(new SqlBuilder("ROWNUM RowNumber"));
        }
        /// <summary>
        /// 构建sql命令
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public override void Build(StringBuilder commandText)
        {
            commandText.AppendLine("WITH t AS");
            commandText.AppendLine("(");
            base.Build(commandText);
            commandText.AppendLine("\n)");
            commandText.AppendLine("SELECT *");
            commandText.AppendLine("  FROM t");
            commandText.AppendLine(" WHERE t.RowNumber > @SkipCount");
            commandText.Append("   AND t.RowNumber <= @SkipCount + @NextCount");
        }
    }
}
