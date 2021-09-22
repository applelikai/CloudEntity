namespace CloudEntity
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DataType
    {
        #region 数值类型
        /// <summary>
        /// 小范围整数
        /// </summary>
        SmallInt,
        /// <summary>
        /// 常用的整数
        /// </summary>
        Integer,
        /// <summary>
        /// 大范围整数
        /// </summary>
        BigInt,
        /// <summary>
        /// 可变精度，不精确
        /// </summary>
        Float,
        /// <summary>
        /// 可变精度，不精确
        /// </summary>
        Real,
        /// <summary>
        /// 可变精度，不精确
        /// </summary>
        Double,
        /// <summary>
        /// 用户指定的精度，精确
        /// </summary>
        Decimal,
        /// <summary>
        /// 货币金额
        /// </summary>
        Money,
        #endregion
        #region 字符串类型
        /// <summary>
        /// 定长字符串
        /// </summary>
        Char,
        /// <summary>
        /// 变长字符串(有长度限制)
        /// </summary>
        Varchar,
        /// <summary>
        /// 变长字符串(无长度限制)
        /// </summary>
        Text,
        /// <summary>
        /// 定长Unicode字符串
        /// </summary>
        Nchar,
        /// <summary>
        /// 变长Unicode字符串(有长度限制)
        /// </summary>
        Nvarchar,
        #endregion
        #region 日期/时间类型
        /// <summary>
        /// 日期
        /// </summary>
        Date,
        /// <summary>
        /// 时间
        /// </summary>
        Time,
        /// <summary>
        /// 日期时间
        /// </summary>
        DateTime,
        #endregion
        #region 布尔类型
        /// <summary>
        /// [1:true 0:false]
        /// </summary>
        Boolean,
        #endregion
        #region 扩展类型
        /// <summary>
        /// xml数据
        /// </summary>
        Xml,
        /// <summary>
        /// json数据
        /// </summary>
        Json
        #endregion
    }
}
