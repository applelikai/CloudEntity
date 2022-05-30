using System.Text;

namespace CloudEntity.CommandTrees.Commom.PostgreSqlClient
{
    /// <summary>
    /// PostgreSql分页Sql生成树
    /// 李凯 Apple_Li 2021/09/19
    /// </summary>
    internal class PostgreSqlPagingQueryTree : QueryTree
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="parameterMarker">sql参数标识符号</param>
        public PostgreSqlPagingQueryTree(char parameterMarker)
            : base(parameterMarker) { }
        /// <summary>
        /// 构建sql命令
        /// </summary>
        /// <param name="commandText">待构建的sql</param>
        public override void Build(StringBuilder commandText)
        {
            base.Build(commandText);
            //拼接MySql分页条件
            //  @SkipCount:跳过之前的多少行
            //  @NextCount:获取之后的多少行
            //  LIMIT 3 OFFSET 2: 查之后的3行，跳过前2行
            commandText.AppendLine();
            commandText.Append("     LIMIT @NextCount OFFSET @SkipCount");
        }
    }
}
