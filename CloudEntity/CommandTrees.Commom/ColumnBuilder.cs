using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// sql Column节点生成类
    /// </summary>
    public class ColumnBuilder : INodeBuilder
    {
        /// <summary>
        /// 列的全名
        /// </summary>
        private string columnFullName;

        /// <summary>
        /// 当前节点类型
        /// </summary>
        public BuilderType BuilderType
        {
            get { return BuilderType.Column; }
        }
        /// <summary>
        /// 父节点类型
        /// </summary>
        public SqlType ParentNodeType
        {
            get { return SqlType.Select; }
        }
        /// <summary>
        /// 列名
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// 创建sql Column节点生成对象
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="columnFullName">列的全名</param>
        public ColumnBuilder(string columnName, string columnFullName)
        {
            //非空检查
            Check.ArgumentNull(columnName, nameof(columnName));
            Check.ArgumentNull(columnFullName, nameof(columnFullName));
            //赋值
            this.ColumnName = columnName;
            this.columnFullName = columnFullName;
        }
        /// <summary>
        /// 拼接sql语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            commandText.Append(this.columnFullName);
        }
    }
}
