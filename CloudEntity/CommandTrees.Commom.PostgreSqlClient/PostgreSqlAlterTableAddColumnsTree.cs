using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEntity.CommandTrees.Commom.PostgreSqlClient
{
    /// <summary>
    /// 用于PostgreSql的为Table添加列的语句生成树
    /// </summary>
    public class PostgreSqlAlterTableAddColumnsTree : AlterTableAddColumnsTree
    {
        /// <summary>
        /// 拼接ALTER TABLE
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="tableName">表名</param>
        /// <param name="schemaName">数据库架构名</param>
        protected override void AppendAlterTable(StringBuilder commandText, string tableName, string schemaName)
        {
            //若架构名为空 则直接拼接[ALTER 表名]
            if (string.IsNullOrEmpty(schemaName))
                commandText.Append($"ALTER TABLE \"{tableName}\"");
            //若不为空，则拼接[ALTER 架构名.表名]
            else
                commandText.Append($"ALTER TABLE {schemaName}.\"{tableName}\"");
        }
        /// <summary>
        /// 拼接添加列名
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="columnName">列名</param>
        protected override void AppendAddColumn(StringBuilder commandText, string columnName)
        {
            commandText.Append($" ADD \"{columnName}\"");
        }

        /// <summary>
        /// 创建为Table添加列的语句生成树
        /// </summary>
        /// <param name="columnNodeHelper">获取列节点信息的Helper</param>
        /// <param name="tableName">表名</param>
        /// <param name="schemaName">数据库架构名</param>
        public PostgreSqlAlterTableAddColumnsTree(ColumnNodeHelper columnNodeHelper, string tableName, string schemaName)
            : base(columnNodeHelper, tableName, schemaName) { }
    }
}
