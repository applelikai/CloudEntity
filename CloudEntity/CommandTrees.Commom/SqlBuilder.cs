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
        private string _template;
        /// <summary>
        /// 格式化参数
        /// </summary>
        private object[] _arguments;

        /// <summary>
        /// 创建Sql节点
        /// </summary>
        /// <param name="teamplate">字符串模板</param>
        /// <param name="arguments">格式化参数</param>
        public SqlBuilder(string teamplate, params object[] arguments)
        {
            //非空检查
            Check.ArgumentNull(teamplate, nameof(teamplate));
            //赋值
            _template = teamplate;
            _arguments = arguments;
        }
        /// <summary>
        /// 拼接sql语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            commandText.AppendFormat(_template, _arguments);
        }
    }
}
