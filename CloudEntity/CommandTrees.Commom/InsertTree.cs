using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// Insert命令生成树
    /// 李凯 Apple_Li
    /// </summary>
    public class InsertTree : CommandTree
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
        /// insert语句段生成器
        /// </summary>
        private IBuilderCollection _insertCollection;
        /// <summary>
        /// Values语句段生成器
        /// </summary>
        private IBuilderCollection _valueCollection;

        /// <summary>
        /// insert语句段生成器
        /// </summary>
        protected IBuilderCollection InsertCollection
        {
            get { return _insertCollection ?? (_insertCollection = new BuilderCollection("            (", "             ", ",\n", ")\n")); }
        }
        /// <summary>
        /// Values语句段生成器
        /// </summary>
        protected IBuilderCollection ValueCollection
        {
            get { return _valueCollection ?? (_valueCollection = new BuilderCollection("     VALUES (", "             ", ",\n", ")\n")); }
        }

        /// <summary>
        /// 拼接INSERT INTO 表
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        protected virtual void AppendInsertTable(StringBuilder commandText, string schemaName, string tableName)
        {
            //若数据库架构名为空，则直接拼接表名
            if (string.IsNullOrEmpty(schemaName))
                commandText.AppendLine($"INSERT INTO {tableName}");
            //否则则拼接架构名.表名
            else
                commandText.AppendLine($"INSERT INTO {schemaName}.{tableName}");
        }

        /// <summary>
        /// 创建Insert命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        public InsertTree(string schemaName, string tableName, char parameterMarker)
            : base(parameterMarker)
        {
            _schemaName = schemaName;
            _tableName = tableName;
        }
        /// <summary>
        /// 拼接Column及Parameter节点
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="parameterName">参数名</param>
        public virtual void Append(string columnName, string parameterName)
        {
            //添加Columns节点
            this.InsertCollection.Append(new SqlBuilder(columnName));
            //添加Values节点
            this.ValueCollection.Append(base.GetParameterBuilder(parameterName));
        }
        /// <summary>
        /// 拼接sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public override void Build(StringBuilder commandText)
        {
            //拼接INSERT INTO 表
            this.AppendInsertTable(commandText, _schemaName, _tableName);
            //拼接列列表
            this.InsertCollection.Build(commandText);
            //拼接值参数列表
            this.ValueCollection.Build(commandText);
        }
    }
}
