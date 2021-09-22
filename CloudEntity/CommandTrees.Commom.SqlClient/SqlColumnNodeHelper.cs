using System;

namespace CloudEntity.CommandTrees.Commom.SqlClient
{
    /// <summary>
    /// 获取用于Sql Server的列节点信息的帮助类
    /// </summary>
    public class SqlColumnNodeHelper : ColumnNodeHelper
    {
        /// <summary>
        /// 获取创建列时自增列表达式
        /// </summary>
        /// <returns>创建列时自增列表达式</returns>
        public override string GetIdentity()
        {
            return "IDENTITY (1, 1)";
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
                case "Boolean":
                    return "BIT";
                case "Decimal":
                case "Double":
                    return "DECIMAL";
                case "Single":
                    return "FLOAT";
                case "Int16":
                case "Int32":
                case "Int64":
                    return "INT";
                case "DateTime":
                    return "DATETIME";
                case "String":
                    return "VARCHAR";
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
                case DataType.Integer:
                    return "INT";
                case DataType.Double:
                    return "DECIMAL";
                case DataType.DateTime:
                    return "DATETIME2";
                case DataType.Boolean:
                    return "BIT";
                case DataType.Json:
                    return "TEXT";
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
                    return "getdate()";
                //数字类型默认值
                case "Int16":
                case "Int32":
                case "Int64":
                case "Decimal":
                    return "0";
                //默认为空
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
                    return "GETDATE()";
                //其他
                default:
                    return string.Empty;
            }
        }
    }
}
