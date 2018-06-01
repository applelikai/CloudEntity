using System.Text;

namespace CloudEntity.CommandTrees.Commom.MySqlClient
{
    /// <summary>
    /// 用于My Sql的Update命令生成树
    /// 李凯 Apple_Li
    /// </summary>
    internal class MySqlUpdateTree : UpdateTree
    {
        /// <summary>
        /// 创建用于MySql的Update命令生成树
        /// </summary>
        /// <param name="tableFullName">完整表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="parameterMarker">sql参数标识符</param>
        internal MySqlUpdateTree(string tableFullName, string tableAlias, char parameterMarker)
            : base(tableFullName, tableAlias, parameterMarker) { }

        /// <summary>
        /// 拼接sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public override void Build(StringBuilder commandText)
        {
            commandText.AppendFormat("UPDATE {0} {1}\n", base.TableFullName, base.TableAlias);
            this.Set.Build(commandText);
            this.Where.Build(commandText);
        }
    }
}
