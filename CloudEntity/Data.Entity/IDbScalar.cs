namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 查询第一行第一列的值的接口
    /// </summary>
    public interface IDbScalar : IDbBase
    {
        /// <summary>
        /// 执行查询获取第一行第一列的值
        /// </summary>
        /// <returns>第一行第一列的值</returns>
        object Execute();
    }
}
