using System.Text;

namespace CloudEntity.CommandTrees.Commom.MySqlClient
{
    /// <summary>
    /// MySql分页Sql生成树
    /// 李凯 Apple_Li
    /// </summary>
    internal class MySqlPagingQueryTree : QueryTree
    {
        /// <summary>
        /// 创建MySql分页查询命令生成树
        /// </summary>
        /// <param name="parameterMarker">sql参数标识符号</param>
        public MySqlPagingQueryTree(char parameterMarker)
            : base(parameterMarker)
        {
        }
        /// <summary>
        /// 构建sql命令
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public override void Build(StringBuilder commandText)
        {
            base.Build(commandText);
            //拼接MySql分页条件
            commandText.AppendLine();
            commandText.Append("   LIMIT @SkipCount, @NextCount");
        }
    }
}
