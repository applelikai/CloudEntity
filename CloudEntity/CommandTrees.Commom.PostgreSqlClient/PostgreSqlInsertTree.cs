using System.Text;

namespace CloudEntity.CommandTrees.Commom.PostgreSqlClient
{
    /// <summary>
    /// 用于PostgreSql的Insert命令生成树
    /// Apple_Li 李凯 15150598493
    /// 2021/09/21
    /// </summary>
    internal class PostgreSqlInsertTree : InsertTree
    {
        /// <summary>
        /// 拼接INSERT INTO 表
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        protected override void AppendInsertTable(StringBuilder commandText, string schemaName, string tableName)
        {
            //若数据库架构名为空，则直接拼接表名
            if (string.IsNullOrEmpty(schemaName))
                commandText.AppendFormat("INSERT INTO \"{0}\"", tableName);
            //否则则拼接架构名.表名
            else
                commandText.AppendFormat("INSERT INTO {0}.\"{1}\"", schemaName, tableName);
        }

        /// <summary>
        /// 创建Insert命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        public PostgreSqlInsertTree(string schemaName, string tableName, char parameterMarker)
            : base(schemaName, tableName, parameterMarker) { }
        /// <summary>
        /// 拼接Column及Parameter节点
        /// </summary>
        /// <param name="columnName">列名</param>
        /// <param name="parameterName">参数名</param>
        public override void Append(string columnName, string parameterName)
        {
            //添加Columns节点
            base.InsertCollection.Append(new SqlBuilder("\"{0}\"", columnName));
            //添加Values节点
            base.ValueCollection.Append(base.GetParameterBuilder(parameterName));
        }
    }
}
