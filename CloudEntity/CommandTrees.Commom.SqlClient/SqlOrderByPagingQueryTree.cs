using System.Text;

namespace CloudEntity.CommandTrees.Commom.SqlClient
{
    /// <summary>
    /// SqlServer分页查询Sql生成树
    /// 李凯 Apple_Li 15150598493
    /// 最后修改时间：2023/02/17 19:57
    /// </summary>
    internal class SqlOrderByPagingQueryTree : QueryTree
    {
        /// <summary>
        /// 创建分页查询命令生成树
        /// </summary>
        /// <param name="parameterMarker">sql参数标识符号</param>
        public SqlOrderByPagingQueryTree(char parameterMarker)
            : base(parameterMarker) { }
        /// <summary>
        /// 构建sql命令
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public override void Build(StringBuilder commandText)
        {
            // 拼接WITH AS
            commandText.AppendLine("WITH [t] AS");
            commandText.AppendLine("(");
            // 拼接正常查询的sql
            this.Select.Build(commandText);
            this.From.Build(commandText);
            this.Where.Build(commandText);
            this.GroupBy.Build(commandText);
            // 拼接分页条件
            commandText.AppendLine("\n)");
            commandText.AppendLine("SELECT *");
            commandText.AppendLine("  FROM [t]");
            commandText.AppendLine(" WHERE [t].[RowNumber] > @SkipCount");
            commandText.Append("   AND [t].[RowNumber] <= @SkipCount + @NextCount");
        }
    }
}
