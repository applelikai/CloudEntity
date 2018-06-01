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
    }
}
