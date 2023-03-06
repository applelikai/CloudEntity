using CloudEntity.Mapping;
using System;
using System.Reflection;

namespace CloudEntity.Internal.Mapping
{
    /// <summary>
    /// 列与属性映射类
    /// </summary>
    internal class ColumnMapper : IColumnMapper
    {
        /// <summary>
        /// 列名
        /// </summary>
        private string _columnName;

        /// <summary>
        /// 当前Mapping的属性
        /// </summary>
        public PropertyInfo Property { get; private set; }
        /// <summary>
        /// 可对改属性所Mapping的列的操作(默认为InsertAndEdit)
        /// </summary>
        public ColumnAction ColumnAction { get; set; }
        /// <summary>
        /// 列名(为空时取属性名)
        /// </summary>
        public string ColumnName
        {
            get { return _columnName ?? (_columnName = this.Property.Name); }
            set { _columnName = value; }
        }
        /// <summary>
        /// 列的别名（可以为空）
        /// </summary>
        public string ColumnAlias { get; set; }
        /// <summary>
        /// 数据类型
        /// </summary>
        public DataType DataType { get; set; }
        /// <summary>
        /// 长度(可以为空)
        /// </summary>
        public int? Length { get; set; }
        /// <summary>
        /// 小数点位数
        /// </summary>
        public int? Decimals { get; set; }
        /// <summary>
        /// 是否允许为空
        /// </summary>
        public bool AllowNull { get; set; }

        /// <summary>
        /// 获取数据类型
        /// </summary>
        /// <returns>数据类型</returns>
        private DataType GetDataType()
        {
            //获取属性类型名称
            string propertyTypeName = this.Property.PropertyType.SourceTypeName();
            //获取属性类型
            switch (propertyTypeName)
            {
                case "Int16":
                    return DataType.SmallInt;
                case "Int32":
                    return DataType.Integer;
                case "Int64":
                    return DataType.BigInt;
                case "Single":
                    return DataType.Float;
                case "String":
                    return DataType.Varchar;
                case "DateOnly":
                    return DataType.Date;
                default:
                    //尝试获取相同类型名称的数据类型，获取失败则获取默认数据类型
                    DataType dataType;
                    Enum.TryParse(propertyTypeName, out dataType);
                    return dataType;
            };
        }

        /// <summary>
        /// 创建ColumnMapper
        /// </summary>
        /// <param name="property">属性</param>
        public ColumnMapper(PropertyInfo property)
        {
            //非空检查
            Check.ArgumentNull(property, nameof(property));
            //赋值
            this.Property = property;
            this.DataType = this.GetDataType();
            this.AllowNull = true;
        }
    }
}
