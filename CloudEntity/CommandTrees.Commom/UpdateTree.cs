using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// Update命令生成树
    /// 李凯 Apple_Li
    /// </summary>
    public class UpdateTree : CommandTree
    {
        private string tableAlias;                  //临时表名
        private string tableFullName;               //表全名
        private IBuilderCollection setCollection;   //Set语句生成器
        private IBuilderCollection whereCollection; //where语句生成器

        /// <summary>
        /// 临时表名
        /// </summary>
        protected string TableAlias
        {
            get { return this.tableAlias; }
        }
        /// <summary>
        /// 完整表名
        /// </summary>
        protected string TableFullName
        {
            get { return this.tableFullName; }
        }
        /// <summary>
        /// Set语句生成器
        /// </summary>
        public IBuilderCollection Set
        {
            get { return this.setCollection ?? (this.setCollection = new BuilderCollection("   SET ", "       ", ",\n", "\n")); }
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
        /// 创建Update命令生成树
        /// </summary>
        /// <param name="tableFullName">表全名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="parameterMarker">sql参数标识符</param>
        internal UpdateTree(string tableFullName, string tableAlias, char parameterMarker)
            : base(parameterMarker)
        {
            this.tableFullName = tableFullName;
            this.tableAlias = tableAlias;
        }

        /// <summary>
        /// 拼接sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public override void Build(StringBuilder commandText)
        {
            commandText.AppendFormat("UPDATE {0}\n", this.tableAlias);
            this.Set.Build(commandText);
            commandText.AppendFormat("  FROM {0} {1}\n", this.tableFullName, this.tableAlias);
            this.Where.Build(commandText);
        }
    }
}