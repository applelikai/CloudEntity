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
        private ColumnMapper _columnMapper;

        /// <summary>
        /// 创建列信息设置器
        /// </summary>
        /// <param name="columnMapper">列与属性的映射对象</param>
        public ColumnSetter(ColumnMapper columnMapper)
        {
            this._columnMapper = columnMapper;
        }
        /// <summary>
        /// 设置当前列的别名
        /// </summary>
        /// <param name="columnAlias">列的别名</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter As(string columnAlias = null)
        {
            this._columnMapper.ColumnAlias = columnAlias ?? this._columnMapper.Property.Name;
            return this;
        }
        /// <summary>
        /// 设置类型长度
        /// </summary>
        /// <param name="length">类型长度</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter Length(int length)
        {
            //设置类型长度
            _columnMapper.Length = length;
            //获取当前设置对象
            return this;
        }
        /// <summary>
        /// 设置类型精度(长度和小数点位数)
        /// </summary>
        /// <param name="length">类型长度</param>
        /// <param name="decimals">小数点位数</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter Round(int length, int decimals)
        {
            //设置类型长度
            _columnMapper.Length = length;
            //设置小数点位数
            _columnMapper.Decimals = decimals;
            //获取当前设置对象
            return this;
        }
        /// <summary>
        /// 设置数据类型
        /// </summary>
        /// <param name="dataType">数据类型,可以不设置</param>
        /// <param name="length">类型长度</param>
        /// <param name="decimals">小数点位数</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter DataType(DataType? dataType = null, int? length = null, int? decimals = null)
        {
            //设置数据类型
            if (dataType != null)
                _columnMapper.DataType = dataType.Value;
            //设置类型长度
            _columnMapper.Length = length;
            //设置小数点位数
            _columnMapper.Decimals = decimals;
            //获取当前设置对象
            return this;
        }
    }
}
