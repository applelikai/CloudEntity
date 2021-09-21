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
            get { return this._setCollection ?? (this._setCollection = new BuilderCollection("   SET ", "       ", ",\n", "\n")); }
        }
        /// <summary>
        /// where语句生成器
        /// </summary>
        public IBuilderCollection Where
        {
            get
            {
                Start:
                //若whereCollection不为空,直接返回
                if (this._whereCollection != null)
                    return this._whereCollection;
                //创建whereCollection
                this._whereCollection = new BuilderCollection()
                {
                    TitleLeftSpace = " WHERE ",
                    BodyLeftSpace = "   AND ",
                    BodyRightSpace = "\n",
                    LastRightSpace = string.Empty
                };
                //回到Start
                goto Start;
            }
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
            _schemaName = schemaName;
            _tableName = tableName;
            _tableAlias = tableAlias;
        }
        /// <summary>
        /// 拼接sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public override void Build(StringBuilder commandText)
        {
            //拼接UPDATE
            commandText.AppendFormat("UPDATE {0}\n", _tableAlias);
            //拼接SET
            this.Set.Build(commandText);
            //拼接FROM
            if (string.IsNullOrEmpty(_schemaName))
                commandText.AppendFormat("  FROM {0} {1}\n", _tableName, _tableAlias);
            else
                commandText.AppendFormat("  FROM {0}.{1} {2}\n", _schemaName, _tableName, _tableAlias);
            //拼接WHERE
            this.Where.Build(commandText);
        }
    }
}