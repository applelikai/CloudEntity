namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// sql命令及参数的生成器
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
