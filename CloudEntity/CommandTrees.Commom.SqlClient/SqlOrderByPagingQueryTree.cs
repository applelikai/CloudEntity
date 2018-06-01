using System.Text;

namespace CloudEntity.CommandTrees.Commom.SqlClient
{
    /// <summary>
    /// SqlServer分页查询Sql生成树
    /// 李凯 Apple_Li
    /// </summary>
    internal class SqlOrderByPagingQueryTree : QueryTree
    {
        /// <summary>
        /// 创建分页查询命令生成树
        /// </summary>
        /// <param name="parameterMarker">sql参数标识符号</param>
        /// <param name="orderByColumn">排序列</param>
        /// <param name="isAsc">是否是升序</param>
        public SqlOrderByPagingQueryTree(char parameterMarker, string orderByColumn, bool isAsc)
            : base(parameterMarker)
        {
            //添加RowNumber查询列
            ISqlBuilder rowNumberBuilder = new SqlBuilder("ROW_NUMBER() OVER(ORDER BY {0} {1}) RowNumber", orderByColumn, isAsc ? "ASC" : "DESC");
            base.Select.Append(rowNumberBuilder);
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
