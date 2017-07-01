using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 查询命令生成树
    /// 李凯 Apple_Li
    /// </summary>
    internal class InsertTree : CommandTree
    {
        /// <summary>
        /// 完整表名
        /// </summary>
        private string tableFullName;
        /// <summary>
        /// insert语句段生成器
        /// </summary>
        private IBuilderCollection insertCollection;
        /// <summary>
        /// Values语句段生成器
        /// </summary>
        private IBuilderCollection valueCollection;

        /// <summary>
        /// insert语句段生成器
        /// </summary>
        private IBuilderCollection InsertCollection
        {
            get { return this.insertCollection ?? (this.insertCollection = new BuilderCollection("            (", "             ", ",\n", ")\n")); }
        }
        /// <summary>
        /// Values语句段生成器
        /// </summary>
        private IBuilderCollection ValueCollection
        {
            get { return this.valueCollection ?? (this.valueCollection = new BuilderCollection("     VALUES (", "             ", ",\n", ")\n")); }
        }

        /// <summary>
        /// 拼接Column及Parameter节点
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="parameterName">参数名</param>
        internal void Append(string columnName, string parameterName)
        {
            this.InsertCollection.Append(new SqlBuilder(columnName));
            this.ValueCollection.Append(new SqlBuilder("${0}", parameterName));
        }

        /// <summary>
        /// 创建Insert命令生成树
        /// </summary>
        /// <param name="tableFullName">完整表名</param>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        public InsertTree(string tableFullName, char parameterMarker)
            : base(parameterMarker)
        {
            this.tableFullName = tableFullName;
        }
        /// <summary>
        /// 拼接sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public override void Build(StringBuilder commandText)
        {
            //开始拼接sql
            commandText.AppendFormat("INSERT INTO {0}\n", this.tableFullName);
            this.InsertCollection.Build(commandText);
            this.ValueCollection.Build(commandText);
        }
    }
}
