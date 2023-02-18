using System.Text;

namespace CloudEntity.CommandTrees.Commom.SqlClient
{
    /// <summary>
    /// 用于Sql Server的Update命令生成树
    /// Apple_Li 李凯 15150598493
    /// 2023/02/17 23:23
    /// </summary>
    internal class SqlUpdateTree : UpdateTree
    {
        /// <summary>
        /// 拼接UPDATE
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="tableAlias">表别名</param>
        protected override void AppendUpdate(StringBuilder commandText, string tableAlias)
        {
            //拼接UPDATE
            commandText.AppendFormat("UPDATE [{0}]", tableAlias);
        }
        /// <summary>
        /// 拼接FROM表达式
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        protected override void AppendFrom(StringBuilder commandText, string schemaName, string tableName, string tableAlias)
        {
            //若数据库架构名为空，则直接拼接表名
            if (string.IsNullOrEmpty(schemaName))
                commandText.AppendFormat("\n  FROM [{0}] [{1}]", tableName, tableAlias);
            //否则则拼接架构名.表名
            else
                commandText.AppendFormat("\n  FROM {0}.[{1}] [{2}]", schemaName, tableName, tableAlias);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">完整表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="parameterMarker">sql参数标识符</param>
        public SqlUpdateTree(string schemaName, string tableName, string tableAlias, char parameterMarker)
         : base(schemaName, tableName, tableAlias, parameterMarker) { }
    }
}