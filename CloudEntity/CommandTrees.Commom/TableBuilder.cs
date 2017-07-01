using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// sql Table节点生成类
    /// </summary>
    public class TableBuilder : INodeBuilder
    {
        private string tableFullName;   //Table全名

        /// <summary>
        /// 当前节点类型
        /// </summary>
        public BuilderType BuilderType
        {
            get { return BuilderType.Table; }
        }
        /// <summary>
        /// 父节点类型
        /// </summary>
        public SqlType ParentNodeType
        {
            get { return SqlType.From; }
        }
        /// <summary>
        /// Table名
        /// </summary>
        public string TableName { get; private set; }

        /// <summary>
        /// 创建sql Table节点生成对象
        /// </summary>
        /// <param name="tableName">Table名</param>
        /// <param name="tableFullName">Table全名</param>
        public TableBuilder(string tableName, string tableFullName)
        {
            this.TableName = tableName;
            this.tableFullName = tableFullName;
        }

        /// <summary>
        /// 拼接sql语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            commandText.Append(this.tableFullName);
        }
    }
}
