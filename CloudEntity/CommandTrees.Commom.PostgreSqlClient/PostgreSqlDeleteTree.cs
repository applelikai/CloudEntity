using System.Text;

namespace CloudEntity.CommandTrees.Commom.PostgreSqlClient
{
    /// <summary>
    /// 用于PostgreSql的Delete命令生成树
    /// 李凯 Apple_Li 2021/09/21
    /// 最后修改时间：2023/02/05
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
            commandText.AppendLine("DELETE");
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
                commandText.AppendLine($"  FROM \"{tableName}\" \"{tableAlias}\"");
            //否则则拼接架构名.表名
            else
                commandText.AppendLine($"  FROM {schemaName}.\"{tableName}\" \"{tableAlias}\"");
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
