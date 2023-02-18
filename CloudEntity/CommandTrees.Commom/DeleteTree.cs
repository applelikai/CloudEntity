using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// Delete命令生成树
    /// 李凯 Apple_Li
    /// 最后修改时间：2023/02/17 20:14
    /// </summary>
    public class DeleteTree : CommandTree
    {
        /// <summary>
        /// 数据库架构名
        /// </summary>
        private string _schemaName;
        /// <summary>
        /// 表全名
        /// </summary>
        private string _tableName;
        /// <summary>
        /// 临时表名
        /// </summary>
        private string _tableAlias;
        /// <summary>
        /// where语句生成器
        /// </summary>
        private IBuilderCollection _whereCollection;

        /// <summary>
        /// 临时表名
        /// </summary>
        protected string TableAlias
        {
            get { return _tableAlias; }
        }

        /// <summary>
        /// where语句生成器
        /// </summary>
        public IBuilderCollection Where
        {
            get { return _whereCollection; }
        }

        /// <summary>
        /// 拼接DELETE
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        protected virtual void AppendDelete(StringBuilder commandText)
        {
            //拼接DELETE
            commandText.AppendFormat("DELETE {0}", _tableAlias);
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
        /// 初始化
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        public DeleteTree(string schemaName, string tableName, string tableAlias, char parameterMarker)
            : base(parameterMarker)
        {
            _schemaName = schemaName;
            _tableName = tableName;
            _tableAlias = tableAlias;
            // 初始化
            _whereCollection = new BuilderCollection("\n WHERE ", "   AND ", "\n", string.Empty);
        }
        /// <summary>
        /// 拼接Delete sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public override void Build(StringBuilder commandText)
        {
            //拼接DELETE
            this.AppendDelete(commandText);
            //拼接FROM
            this.AppendFrom(commandText, _schemaName, _tableName, _tableAlias);
            //拼接WHERE
            this.Where.Build(commandText);
        }
    }
}