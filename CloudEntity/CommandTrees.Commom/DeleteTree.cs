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
        /// 临时表名
        /// </summary>
        private string tableAlias;
        /// <summary>
        /// 表全名
        /// </summary>
        private string tableFullName;
        /// <summary>
        /// where语句生成器
        /// </summary>
        private IBuilderCollection whereCollection;

        /// <summary>
        /// where语句生成器
        /// </summary>
        public IBuilderCollection Where
        {
            get
            {
                Start:
                //若whereCollection不为空,直接返回
                if (this.whereCollection != null)
                    return this.whereCollection;
                //创建whereCollection
                this.whereCollection = new BuilderCollection()
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
        /// 创建Delete命令生成树
        /// </summary>
        /// <param name="tableFullName">表全名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="parameterMarker">Sql参数标识符号</param>
        public DeleteTree(string tableFullName, string tableAlias, char parameterMarker)
            : base(parameterMarker)
        {
            this.tableFullName = tableFullName;
            this.tableAlias = tableAlias;
        }
        /// <summary>
        /// 拼接Delete sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public override void Build(StringBuilder commandText)
        {
            commandText.AppendFormat("DELETE {0}\n", this.tableAlias);
            commandText.AppendFormat("  FROM {0} {1}\n", this.tableFullName, this.tableAlias);
            this.Where.Build(commandText);
        }
    }
}