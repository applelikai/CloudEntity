using System.Text;

namespace CloudEntity.CommandTrees.Commom.SqlClient
{
    /// <summary>
    /// 用于 Sql Server 的 With As 查询命令生成树
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    internal class SqlWithAsQueryTree : WithAsQueryTree
    {
        /// <summary>
        /// 拼接WITH AS语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="tableAlias">临时表名</param>
        protected override void AppendWithAs(StringBuilder commandText, string tableAlias)
        {
            commandText.AppendFormat("WITH [{0}] AS", tableAlias);
        }
        /// <summary>
        /// 拼接FROM语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="tableAlias">临时表名</param>
        protected override void AppendFrom(StringBuilder commandText, string tableAlias)
        {
            commandText.AppendFormat("\n    FROM [{0}]", tableAlias);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        /// <param name="innerQuerySql">查询sql</param>
        /// <param name="tableAlias">临时表名</param>
        public SqlWithAsQueryTree(char parameterMarker, string innerQuerySql, string tableAlias)
         : base(parameterMarker, innerQuerySql, tableAlias) { }
    }
}