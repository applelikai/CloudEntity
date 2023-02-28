using System.Text;

namespace CloudEntity.CommandTrees.Commom.MySqlClient
{
    /// <summary>
    /// 用于My Sql的Update命令生成树
    /// Apple_Li 李凯 15150598493
    /// </summary>
    internal class MySqlUpdateTree : UpdateTree
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="parameterMarker">sql参数标识符</param>
        public MySqlUpdateTree(string schemaName, string tableName, string tableAlias, char parameterMarker)
            : base(schemaName, tableName, tableAlias, parameterMarker) { }
        /// <summary>
        /// 拼接sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public override void Build(StringBuilder commandText)
        {
            //拼接UPDATE
            if (string.IsNullOrEmpty(base.SchemaName))
                commandText.AppendFormat("UPDATE `{0}` `{1}`", base.TableName, base.TableAlias);
            else
                commandText.AppendFormat("UPDATE {0}.`{1}` `{2}`", base.SchemaName, base.TableName, base.TableAlias);
            //拼接SET
            this.Set.Build(commandText);
            //拼接WHERE
            this.Where.Build(commandText);
        }
    }
}
