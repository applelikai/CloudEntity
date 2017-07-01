using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// Sql生成树
    /// </summary>
    public abstract class CommandTree : ICommandTree
    {
        //Sql参数标识符
        private char parameterMarker;

        /// <summary>
        /// 创建CommandTree
        /// </summary>
        /// <param name="parameterMarker">Sql参数标识符</param>
        public CommandTree(char parameterMarker)
        {
            this.parameterMarker = parameterMarker;
        }

        /// <summary>
        /// 构建sql命令
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public abstract void Build(StringBuilder commandText);
        /// <summary>
        /// 生成sql命令
        /// </summary>
        /// <returns>sql命令</returns>
        public string Compile()
        {
            StringBuilder sql = new StringBuilder();    //创建sql
            this.Build(sql);                            //构建原始sql
            sql.Replace('$', this.parameterMarker);     //替换Sql中的$为sql参数表示符号
            return sql.ToString();                      //返回最终生成的sql
        }
    }
}
