using System.Text;

namespace CloudEntity.CommandTrees.Commom.PostgreSqlClient
{
    /// <summary>
    /// 用于PostgreSql的建表语句生成树
    /// Apple_Li 李凯 15150598493
    /// </summary>
    public class PostgreSqlBuildTableTree : BuildTableTree
    {
        /// <summary>
        /// 拼接CREATE TABLE语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="tableName">表名</param>
        /// <param name="schemaName">数据库架构名</param>
        protected override void AppendCreateTable(StringBuilder commandText, string tableName, string schemaName)
        {
            //若架构名为空 则直接拼接表名
            if (string.IsNullOrEmpty(schemaName))
                commandText.AppendLine($"CREATE TABLE \"{tableName}\"");
            //若不为空，则拼接 架构名.表名
            else
                commandText.AppendLine($"CREATE TABLE {schemaName}.\"{tableName}\"");
        }
        /// <summary>
        /// 拼接列名
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="columnName">列名</param>
        protected override void AppendColumnName(StringBuilder commandText, string columnName)
        {
            //拼接列名
            commandText.Append($"\t\"{columnName}\"");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="columnNodeHelper">获取列节点信息的Helper</param>
        /// <param name="tableName">表名</param>
        /// <param name="schemaName">数据库架构名</param>
        public PostgreSqlBuildTableTree(ColumnNodeHelper columnNodeHelper, string tableName, string schemaName = null)
            : base(columnNodeHelper, tableName, schemaName) { }
    }
}
