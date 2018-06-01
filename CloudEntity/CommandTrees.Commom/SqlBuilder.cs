using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// Sql节点
    /// 李凯 Apple_Li
    /// </summary>
    public class SqlBuilder : ISqlBuilder
    {
        /// <summary>
        /// 字符串模板
        /// </summary>
        private string template;
        /// <summary>
        /// 格式化参数
        /// </summary>
        private object[] arguments;

        /// <summary>
        /// 创建Sql节点
        /// </summary>
        /// <param name="teamplate">字符串模板</param>
        /// <param name="arguments">格式化参数</param>
        public SqlBuilder(string teamplate, params object[] arguments)
        {
            //非空检查
            Check.ArgumentNull(teamplate, nameof(template));
            //赋值
            this.template = teamplate;
            this.arguments = arguments;
        }
        /// <summary>
        /// 拼接sql语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            commandText.AppendFormat(this.template, this.arguments);
        }
    }
}
