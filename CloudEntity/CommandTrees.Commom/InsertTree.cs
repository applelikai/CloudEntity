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
            get { return _insertCollection; }
        }
        /// <summary>
        /// Values语句段生成器
        /// </summary>
        protected IBuilderCollection ValueCollection
        {
            get { return _valueCollection; }
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
                commandText.AppendFormat("INSERT INTO {0}", tableName);
            //否则则拼接架构名.表名
            else
                commandText.AppendFormat("INSERT INTO {0}.{1}", schemaName, tableName);
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
            // 赋值
            _schemaName = schemaName;
            _tableName = tableName;
            // 初始化
            _insertCollection = new BuilderCollection("\n            (", "             ", ",\n", ")");
            _valueCollection = new BuilderCollection("\n     VALUES (", "             ", ",\n", ")");
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
