using System;

namespace CloudEntity.CommandTrees.Commom.OracleClient
{
    /// <summary>
    /// 获取用于Oracle的列节点信息的帮助类
    /// </summary>
    public class OracleColumnNodeHelper : ColumnNodeHelper
    {
        /// <summary>
        /// 获取创建列时自增列表达式
        /// </summary>
        /// <returns>创建列时自增列表达式</returns>
        public override string GetIdentity()
        {
            return string.Empty;
        }
        /// <summary>
        /// 获取sql数据类型
        /// </summary>
        /// <param name="sourceType">源数据类型</param>
        /// <returns>sql数据类型</returns>
        public override string GetSqlType(Type sourceType)
        {
            switch (sourceType.SourceTypeName())
            {
                case "String":
                    return "VARCHAR";
                case "DateTime":
                    return "DATE";
                case "Decimal":
                case "Double":
                case "Int16":
                case "Int32":
                case "Int64":
                    return "NUMBER";
                default:
                    return "VARCHAR";
            }
        }
        /// <summary>
        /// 获取最终的sql数据类型
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <returns>最终的sql数据类型</returns>
        public override string GetSqlType(DataType dataType)
        {
            switch (dataType)
            {
                case DataType.SmallInt:
                case DataType.BigInt:
                    return "INTEGER";
                case DataType.Double:
                    return "BINARY_DOUBLE";
                case DataType.Money:
                    return "DECIMAL";
                case DataType.Text:
                case DataType.Ntext:
                case DataType.Json:
                    return "Long";
                case DataType.Nvarchar:
                    return "Nvarchar2";
                case DataType.Time:
                case DataType.DateTime:
                    return "TIMESTAMP";
                case DataType.Boolean:
                    return "INTEGER";
                case DataType.Xml:
                    return "XMLTYPE";
                default:
                    return dataType.ToString().ToUpper();
            }
        }
        /// <summary>
        /// 获取类型默认值
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <returns>默认值</returns>
        public override string GetDefaultValue(Type dataType)
        {
            switch (dataType.SourceTypeName())
            {
                //时间类型默认值
                case "DateTime":
                    return "SYSDATE";
                //数字类型默认值
                case "Int16":
                case "Int32":
                case "Int64":
                case "Decimal":
                    return "0";
                //其他
                default:
                    return string.Empty;
            }
        }
        /// <summary>
        /// 获取类型默认值
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <returns>默认值</returns>
        public override string GetDefaultValue(DataType dataType)
        {
            switch (dataType)
            {
                //数字类型默认值
                case DataType.SmallInt:
                case DataType.Integer:
                case DataType.BigInt:
                case DataType.Float:
                case DataType.Real:
                case DataType.Double:
                case DataType.Decimal:
                case DataType.Money:
                    return "0";
                //日期时间类型默认值
                case DataType.Date:
                case DataType.Time:
                case DataType.DateTime:
                    return "SYSDATE";
                //其他
                default:
                    return string.Empty;
            }
        }
    }
}
