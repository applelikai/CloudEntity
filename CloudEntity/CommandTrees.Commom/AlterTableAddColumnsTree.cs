using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 为Table添加列的语句生成树
    /// </summary>
    public class AlterTableAddColumnsTree : ICommandTree
    {
        /// <summary>
        /// 数据库架构名
        /// </summary>
        private string _schemaName;
        /// <summary>
        /// 表名
        /// </summary>
        private string _tableName;
        /// <summary>
        /// 获取列节点信息的Helper
        /// </summary>
        private ColumnNodeHelper _columnNodeHelper;
        /// <summary>
        /// 列节点集合
        /// </summary>
        private IList<IColumnNode> _columnNodes;

        /// <summary>
        /// 拼接ALTER TABLE
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="tableName">表名</param>
        /// <param name="schemaName">数据库架构名</param>
        protected virtual void AppendAlterTable(StringBuilder commandText, string tableName, string schemaName)
        {
            //若架构名为空 则直接拼接[ALTER 表名]
            if (string.IsNullOrEmpty(schemaName))
                commandText.Append($"ALTER TABLE {tableName}");
            //若不为空，则拼接[ALTER 架构名.表名]
            else
                commandText.Append($"ALTER TABLE {schemaName}.{tableName}");
        }
        /// <summary>
        /// 拼接添加列名
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="columnName">列名</param>
        protected virtual void AppendAddColumn(StringBuilder commandText, string columnName)
        {
            commandText.Append($" ADD {columnName}");
        }

        /// <summary>
        /// 创建为Table添加列的语句生成树
        /// </summary>
        /// <param name="columnNodeHelper">获取列节点信息的Helper</param>
        /// <param name="tableName">表名</param>
        /// <param name="schemaName">数据库架构名</param>
        public AlterTableAddColumnsTree(ColumnNodeHelper columnNodeHelper, string tableName, string schemaName)
        {
            _columnNodeHelper = columnNodeHelper;
            _tableName = tableName;
            _schemaName = schemaName;
            _columnNodes = new List<IColumnNode>();
        }
        /// <summary>
        /// 添加列节点
        /// </summary>
        /// <param name="columnNode">列节点</param>
        public void Add(IColumnNode columnNode)
        {
            if (_columnNodes.Count(n => n.ColumnName.Equals(columnNode.ColumnName)) == 0)
                _columnNodes.Add(columnNode);
        }
        /// <summary>
        /// 拼接为Table添加列的sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            //遍历所有的Column节点
            foreach (ColumnNode columnNode in _columnNodes)
            {
                //拼接表名
                this.AppendAlterTable(commandText, _tableName, _schemaName);
                //拼接列名
                this.AppendAddColumn(commandText, columnNode.ColumnName);
                //拼接数据类型
                commandText.AppendFormat(" {0}", columnNode.SqlDataType ?? this._columnNodeHelper.GetSqlType(columnNode.SourceType));
                //拼接数据类型长度
                if (columnNode.Length != null && columnNode.Decimals != null)
                    commandText.AppendFormat("({0}, {1})", columnNode.Length, columnNode.Decimals);
                else if (columnNode.Length != null)
                    commandText.AppendFormat("({0})", columnNode.Length);
                //拼接默认值
                if (columnNode.IsDefault)
                {
                    string defaultValue = _columnNodeHelper.GetDefaultValue(columnNode.SourceType);
                    if (!string.IsNullOrEmpty(defaultValue))
                        commandText.AppendFormat(" DEFAULT {0}", defaultValue);
                }
                //拼接换行
                commandText.AppendLine();
            }
        }
        /// <summary>
        /// 生成为Table添加列的sql
        /// </summary>
        /// <returns>为Table添加列的sql</returns>
        public string Compile()
        {
            StringBuilder commandText = new StringBuilder();
            this.Build(commandText);
            return commandText.ToString();
        }
    }
}
