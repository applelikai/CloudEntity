using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 建表语句生成树
    /// </summary>
    internal class BuildTableTree : ICommandTree
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
        /// 创建建表语句生成树
        /// </summary>
        /// <param name="tableFullName">表名</param>
        /// <param name="columnNodeHelper">获取列节点信息的Helper</param>
        internal BuildTableTree(string tableFullName, ColumnNodeHelper columnNodeHelper)
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
        /// 拼接建表的sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            commandText.AppendFormat("CREATE TABLE {0}\n", this.tableFullName);
            commandText.AppendLine("(");
            //遍历列节点，拼接每一列
            for (int i = 0; i < this.ColumnNodes.Count; i++)
            {
                //拼接表名
                commandText.AppendFormat("\t{0}", this.ColumnNodes[i].ColumnName);
                //拼接数据类型
                commandText.AppendFormat("\t{0}", this.ColumnNodes[i].SqlDataType ?? this.columnNodeHelper.GetSqlType(this.ColumnNodes[i].SourceType));
                //拼接长度
                if (this.ColumnNodes[i].Length > 0)
                    commandText.AppendFormat(" ({0})", this.ColumnNodes[i].Length);
                //拼接默认值
                if (this.ColumnNodes[i].IsDefault)
                {
                    string defaultValue = this.columnNodeHelper.GetDefaultValue(this.ColumnNodes[i].SourceType);
                    if (!string.IsNullOrEmpty(defaultValue))
                        commandText.AppendFormat("\tDEFAULT {0}", defaultValue);
                }
                //拼接主键
                if (this.ColumnNodes[i].IsPrimary)
                    commandText.Append("\tPRIMARY KEY");
                //拼接自增列
                if (this.ColumnNodes[i].IsIdentity)
                    commandText.AppendFormat("\t{0}", this.columnNodeHelper.GetIdentity());
                //拼接是否为空(非主键列才可以定义NULL 或 NOT NULL)
                if (!this.ColumnNodes[i].IsPrimary)
                    commandText.AppendFormat("\t{0}", this.ColumnNodes[i].IsNull ? "NULL" : "NOT NULL");
                //拼接结尾
                commandText.AppendLine((i + 1) == this.ColumnNodes.Count ? string.Empty : ",");

            }
            commandText.AppendLine(")");
        }
        /// <summary>
        /// 创建建表的sql
        /// </summary>
        /// <returns>建表的sql</returns>
        public string Compile()
        {
            StringBuilder commandText = new StringBuilder();
            this.Build(commandText);
            return commandText.ToString();
        }
    }
}
