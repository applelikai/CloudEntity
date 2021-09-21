using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// Delete命令生成树
    /// 李凯 Apple_Li
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
        /// 拼接DELETE
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        protected virtual void AppendDelete(StringBuilder commandText)
        {
            //拼接DELETE
            commandText.AppendLine($"DELETE {_tableAlias}");
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
                commandText.AppendLine($"  FROM {tableName} {tableAlias}");
            //否则则拼接架构名.表名
            else
                commandText.AppendLine($"  FROM {schemaName}.{tableName} {tableAlias}");
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