namespace CloudEntity.Mapping
{
    /// <summary>
    /// 列信息设置器
    /// </summary>
    public interface IColumnSetter
    {
        /// <summary>
        /// 设置当前列的别名
        /// </summary>
        /// <param name="columnAlias">列的别名</param>
        /// <returns>列信息设置器</returns>
        IColumnSetter As(string columnAlias = null);
        /// <summary>
        /// 设置类型长度
        /// </summary>
        /// <param name="length">类型长度</param>
        /// <returns>列信息设置器</returns>
        IColumnSetter Length(int length);
        /// <summary>
        /// 设置类型精度(长度和小数点位数)
        /// </summary>
        /// <param name="length">类型长度</param>
        /// <param name="decimals">小数点位数</param>
        /// <returns>列信息设置器</returns>
        IColumnSetter Round(int length, int decimals);
        /// <summary>
        /// 设置数据类型
        /// </summary>
        /// <param name="dataType">数据类型,可以不设置</param>
        /// <param name="length">类型长度</param>
        /// <param name="decimals">小数点位数</param>
        /// <returns>列信息设置器</returns>
        IColumnSetter DataType(DataType? dataType = null, int? length = null, int? decimals = null);
    }
}
