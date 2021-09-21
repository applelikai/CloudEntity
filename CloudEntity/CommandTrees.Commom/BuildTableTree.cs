using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 建表语句生成树
    /// </summary>
    public class BuildTableTree : ICommandTree
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
        /// 获取列节点信息的Helper
        /// </summary>
        protected ColumnNodeHelper ColumnNodeHelper
        {
            get { return _columnNodeHelper; }
        }

        /// <summary>
        /// 拼接CREATE TABLE语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="tableName">表名</param>
        /// <param name="schemaName">数据库架构名</param>
        protected virtual void AppendCreateTable(StringBuilder commandText, string tableName, string schemaName)
        {
            //若架构名为空 则直接拼接表名
            if (string.IsNullOrEmpty(schemaName))
                commandText.AppendLine($"CREATE TABLE {tableName}");
            //若不为空，则拼接 架构名.表名
            else
                commandText.AppendLine($"CREATE TABLE {schemaName}.{tableName}");
        }
        /// <summary>
        /// 拼接列名
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="columnName">列名</param>
        protected virtual void AppendColumnName(StringBuilder commandText, string columnName)
        {
            //拼接列名
            commandText.Append($"\t{columnName}");
        }
        /// <summary>
        /// 拼接列
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="columnNode">列</param>
        protected virtual void AppendColumn(StringBuilder commandText, IColumnNode columnNode)
        {
            //拼接列名
            this.AppendColumnName(commandText, columnNode.ColumnName);
            //拼接数据类型
            commandText.AppendFormat("\t{0}", columnNode.SqlDataType ?? _columnNodeHelper.GetSqlType(columnNode.SourceType));
            //拼接数据类型长度及小数点位数
            if (columnNode.Length != null && columnNode.Decimals != null)
                commandText.AppendFormat("({0}, {1})", columnNode.Length, columnNode.Decimals);
            else if (columnNode.Length != null)
                commandText.AppendFormat("({0})", columnNode.Length);
            //拼接默认值
            if (columnNode.IsDefault)
            {
                string defaultValue = _columnNodeHelper.GetDefaultValue(columnNode.SourceType);
                if (!string.IsNullOrEmpty(defaultValue))
                    commandText.AppendFormat("\tDEFAULT {0}", defaultValue);
            }
            //拼接主键
            if (columnNode.IsPrimary)
                commandText.Append("\tPRIMARY KEY");
            //拼接自增列
            if (columnNode.IsIdentity)
                commandText.AppendFormat("\t{0}", _columnNodeHelper.GetIdentity());
            //拼接是否为空(非主键列才可以定义NULL 或 NOT NULL)
            if (!columnNode.IsPrimary)
                commandText.AppendFormat("\t{0}", columnNode.IsNull ? "NULL" : "NOT NULL");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="columnNodeHelper">获取列节点信息的Helper</param>
        /// <param name="tableName">表名</param>
        /// <param name="schemaName">数据库架构名</param>
        public BuildTableTree(ColumnNodeHelper columnNodeHelper, string tableName, string schemaName = null)
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
        /// 拼接建表的sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            //拼接CREATE TABLE
            this.AppendCreateTable(commandText, _tableName, _schemaName);
            //拼接主体
            commandText.AppendLine("(");
            //遍历列节点，拼接每一列
            for (int i = 0; i < _columnNodes.Count; i++)
            {
                //拼接列
                this.AppendColumn(commandText, _columnNodes[i]);
                //拼接结尾
                commandText.AppendLine((i + 1) == _columnNodes.Count ? string.Empty : ",");
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
