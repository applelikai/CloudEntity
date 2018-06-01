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
    }
}
