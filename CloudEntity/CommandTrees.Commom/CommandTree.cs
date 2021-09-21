using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// Sql生成树
    /// </summary>
    public abstract class CommandTree : ICommandTree
    {
        /// <summary>
        /// Sql参数标识符
        /// </summary>
        private char _parameterMarker;

        /// <summary>
        /// 获取sql参数表达式节点
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <returns>sql参数表达式节点</returns>
        protected ISqlBuilder GetParameterBuilder(string parameterName)
        {
            //获取sql表达式字符串
            string sqlExpression = string.Concat(_parameterMarker, parameterName);
            //获取sql参数表达式节点
            return new SqlBuilder(sqlExpression);
        }

        /// <summary>
        /// 创建CommandTree
        /// </summary>
        /// <param name="parameterMarker">Sql参数标识符</param>
        public CommandTree(char parameterMarker)
        {
            _parameterMarker = parameterMarker;
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
            sql.Replace('$', this._parameterMarker);     //替换Sql中的$为sql参数表示符号
            return sql.ToString();                      //返回最终生成的sql
        }
    }
}
