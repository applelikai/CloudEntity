using System.Text;

namespace CloudEntity.CommandTrees.Commom.OracleClient
{
    /// <summary>
    /// 用于Oracle的Delete命令生成树
    /// 李凯 Apple_Li 2021/09/21
    /// </summary>
    internal class OracleDeleteTree : DeleteTree
    {
        /// <summary>
        /// 拼接DELETE
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        protected override void AppendDelete(StringBuilder commandText)
        {
            //拼接DELETE
            commandText.AppendLine("DELETE");
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        public OracleDeleteTree(string schemaName, string tableName, string tableAlias, char parameterMarker)
            : base(schemaName, tableName, tableAlias, parameterMarker) { }
    }
}
