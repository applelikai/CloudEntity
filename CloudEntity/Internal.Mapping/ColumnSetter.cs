using CloudEntity.Mapping;

namespace CloudEntity.Internal.Mapping
{
    /// <summary>
    /// 列信息设置器
    /// </summary>
    internal class ColumnSetter : IColumnSetter
    {
        /// <summary>
        /// 列与属性的映射对象
        /// </summary>
        private ColumnMapper columnMapper;

        /// <summary>
        /// 创建列信息设置器
        /// </summary>
        /// <param name="columnMapper">列与属性的映射对象</param>
        public ColumnSetter(ColumnMapper columnMapper)
        {
            this.columnMapper = columnMapper;
        }
        /// <summary>
        /// 设置当前列的别名
        /// </summary>
        /// <param name="columnAlias">列的别名</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter As(string columnAlias = null)
        {
            this.columnMapper.ColumnAlias = columnAlias ?? this.columnMapper.Property.Name;
            return this;
        }
        /// <summary>
        /// 设置数据类型
        /// </summary>
        /// <param name="dataType">数据类型(如 VARCHAR INT等),可以不设置</param>
        /// <param name="length">类型长度</param>
        /// <param name="decimals">小数点位数</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter DataType(string dataType = null, int? length = null, int? decimals = null)
        {
            this.columnMapper.DataType = dataType;
            this.columnMapper.Length = length;
            this.columnMapper.Decimals = decimals;
            return this;
        }
    }
}
