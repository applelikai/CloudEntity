using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// Update命令生成树
    /// 李凯 Apple_Li
    /// </summary>
    public class UpdateTree : CommandTree
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
        /// 临时表名
        /// </summary>
        private string _tableAlias;
        /// <summary>
        /// Set语句生成器
        /// </summary>
        private IBuilderCollection _setCollection;
        /// <summary>
        /// where语句生成器
        /// </summary>
        private IBuilderCollection _whereCollection;

        /// <summary>
        /// 数据库架构名
        /// </summary>
        protected string SchemaName
        {
            get { return _schemaName; }
        }
        /// <summary>
        /// 表名
        /// </summary>
        protected string TableName
        {
            get { return _tableName; }
        }
        /// <summary>
        /// 临时表名
        /// </summary>
        protected string TableAlias
        {
            get { return _tableAlias; }
        }
        /// <summary>
        /// Set语句生成器
        /// </summary>
        public IBuilderCollection Set
        {
            get { return _setCollection ; }
        }
        /// <summary>
        /// where语句生成器
        /// </summary>
        public IBuilderCollection Where
        {
            get { return _whereCollection; }
        }

        /// <summary>
        /// 拼接UPDATE
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="tableAlias">表别名</param>
        protected virtual void AppendUpdate(StringBuilder commandText, string tableAlias)
        {
            //拼接UPDATE
            commandText.AppendFormat("UPDATE {0}", tableAlias);
        }
        /// <summary>
        /// 拼接FROM表达式
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        protected virtual void AppendFrom(StringBuilder commandText, string schemaName, string tableName, string tableAlias)
        {
            //若数据库架构名为空，则直接拼接表名
            if (string.IsNullOrEmpty(schemaName))
                commandText.AppendFormat("\n  FROM {0} {1}", tableName, tableAlias);
            //否则则拼接架构名.表名
            else
                commandText.AppendFormat("\n  FROM {0}.{1} {2}", schemaName, tableName, tableAlias);
        }

        /// <summary>
        /// 创建Update命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="parameterMarker">sql参数标识符</param>
        public UpdateTree(string schemaName, string tableName, string tableAlias, char parameterMarker)
            : base(parameterMarker)
        {
            // 赋值
            _schemaName = schemaName;
            _tableName = tableName;
            _tableAlias = tableAlias;
            // 初始化
            _setCollection = new BuilderCollection("\n   SET ", "       ", ",\n", string.Empty);
            _whereCollection = new BuilderCollection("\n WHERE ", "   AND ", "\n", string.Empty);
        }
        /// <summary>
        /// 拼接sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public override void Build(StringBuilder commandText)
        {
            //拼接UPDATE
            this.AppendUpdate(commandText, _tableAlias);
            //拼接SET
            this.Set.Build(commandText);
            //拼接FROM
            this.AppendFrom(commandText, _schemaName, _tableName, _tableAlias);
            //拼接WHERE
            this.Where.Build(commandText);
        }
    }
}