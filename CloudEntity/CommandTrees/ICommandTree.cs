namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// SQL命令生成树
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    public interface ICommandTree : ISqlBuilder
    {
        /// <summary>
        /// 生成Sql命令
        /// </summary>
        /// <returns>生成的Sql命令</returns>
        string Compile();
    }
}
