using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 为Table添加列的语句生成树
    /// </summary>
    internal class AlterTableAddColumnsTree : ICommandTree
    {
        private string tableFullName;               //表名
        private ColumnNodeHelper columnNodeHelper;  //获取列节点信息的Helper
        private IList<IColumnNode> columnNodes;     //列节点集合
        
        /// <summary>
        /// 列节点集合
        /// </summary>
        private IList<IColumnNode> ColumnNodes
        {
            get { return this.columnNodes ?? (this.columnNodes = new List<IColumnNode>()); }
        }

        /// <summary>
        /// 创建为Table添加列的语句生成树
        /// </summary>
        /// <param name="tableFullName">完整的Table名</param>
        /// <param name="columnNodeHelper">获取列节点信息的Helper</param>
        internal AlterTableAddColumnsTree(string tableFullName, ColumnNodeHelper columnNodeHelper)
        {
            this.tableFullName = tableFullName;
            this.columnNodeHelper = columnNodeHelper;
        }
        
        /// <summary>
        /// 添加列节点
        /// </summary>
        /// <param name="columnNode">列节点</param>
        internal void Add(IColumnNode columnNode)
        {
            if (this.ColumnNodes.Count(n => n.ColumnName.Equals(columnNode.ColumnName)) == 0)
                this.ColumnNodes.Add(columnNode);
        }

        /// <summary>
        /// 拼接为Table添加列的sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            //遍历所有的Column节点
            foreach (ColumnNode columnNode in this.ColumnNodes)
            {
                //拼接表名
                commandText.AppendFormat("ALTER TABLE {0} ", this.tableFullName);
                //拼接列名
                commandText.AppendFormat("ADD {0} ", columnNode.ColumnName);
                //拼接数据类型
                commandText.AppendFormat("{0} ", columnNode.SqlDataType ?? this.columnNodeHelper.GetSqlType(columnNode.SourceType));
                //拼接数据类型长度
                if (columnNode.Length != null && columnNode.Decimals != null)
                    commandText.AppendFormat("({0}, {1}) ", columnNode.Length, columnNode.Decimals);
                //拼接默认值
                if (columnNode.IsDefault)
                {
                    string defaultValue = this.columnNodeHelper.GetDefaultValue(columnNode.SourceType);
                    if (!string.IsNullOrEmpty(defaultValue))
                        commandText.AppendFormat("DEFAULT {0}", defaultValue);
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
