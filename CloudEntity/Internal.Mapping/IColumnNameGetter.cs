namespace CloudEntity.Internal.Mapping
{
    /// <summary>
    /// 列名获取接口
    /// </summary>
    internal interface IColumnNameGetter
    {
        /// <summary>
        /// 获取列名
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns>列名</returns>
        string GetColumnName(string propertyName);
    }
}
