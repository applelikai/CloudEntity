﻿using System;

namespace CloudEntity.CommandTrees.Commom.MySqlClient
{
    /// <summary>
    /// 获取用于MySql的列节点信息的帮助类
    /// </summary>
    public class MySqlColumnNodeHelper : ColumnNodeHelper
    {
        /// <summary>
        /// 获取创建列时自增列表达式
        /// </summary>
        /// <returns>创建列时自增列表达式</returns>
        public override string GetIdentity()
        {
            return "AUTO_INCREMENT";
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
                case "DateTime":
                    return "DATETIME";
                case "Decimal":
                    return "DECIMAL";
                case "Int16":
                    return "SMALLINT";
                case "Int32":
                    return "INT";
                case "Int64":
                    return "BIGINT";
                case "String":
                    return "VARCHAR";
                default:
                    return "VARCHAR";
            };
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
                case DataType.Real:
                    return "FLOAT";
                case DataType.Money:
                    return "DECIMAL";
                case DataType.Nchar:
                    return "CHAR";
                case DataType.Nvarchar:
                    return "VARCHAR";
                case DataType.Ntext:
                case DataType.Xml:
                    return "TEXT";
                case DataType.Boolean:
                    return "TINYINT";
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
                    return "now()";
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
                    return "CURRENT_DATE()";
                case DataType.Time:
                    return "CURRENT_TIME";
                case DataType.DateTime:
                    return "NOW()";
                //其他
                default:
                    return string.Empty;
            }
        }
    }
}
