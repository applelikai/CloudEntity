using System.Text;

namespace CloudEntity.CommandTrees.Commom.PostgreSqlClient
{
    /// <summary>
    /// 用于PostgreSql的Delete命令生成树
    /// [Apple_Li 李凯 15150598493]
    /// </summary>
    internal class PostgreSqlDeleteTree : DeleteTree
    {
        /// <summary>
        /// 拼接DELETE
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        protected override void AppendDelete(StringBuilder commandText)
        {
            //拼接DELETE
            commandText.Append("DELETE");
        }
        /// <summary>
        /// 拼接FROM表达式
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        protected override void AppendFrom(StringBuilder commandText, string schemaName, string tableName, string tableAlias)
        {
            //若数据库架构名为空，则直接拼接表名
            if (string.IsNullOrEmpty(schemaName))
                commandText.AppendFormat("\n  FROM \"{0}\" \"{1}\"", tableName, tableAlias);
            //否则则拼接架构名.表名
            else
                commandText.AppendFormat("\n  FROM {0}.\"{1}\" \"{2}\"", schemaName, tableName, tableAlias);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        public PostgreSqlDeleteTree(string schemaName, string tableName, string tableAlias, char parameterMarker)
            : base(schemaName, tableName, tableAlias, parameterMarker) { }
    }
}
