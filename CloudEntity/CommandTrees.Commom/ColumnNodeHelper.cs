using System;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 获取列节点信息的帮助类
    /// </summary>
    public abstract class ColumnNodeHelper
    {
        /// <summary>
        /// 获取创建列时自增列表达式
        /// </summary>
        /// <returns>创建列时自增列表达式</returns>
        public abstract string GetIdentity();
        /// <summary>
        /// 获取sql数据类型
        /// </summary>
        /// <param name="sourceType">源数据类型</param>
        /// <returns>sql数据类型</returns>
        public abstract string GetSqlType(Type sourceType);
        /// <summary>
        /// 获取最终的sql数据类型
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <returns>最终的sql数据类型</returns>
        public abstract string GetSqlType(DataType dataType);
        /// <summary>
        /// 获取类型默认值
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <returns>默认值</returns>
        public abstract string GetDefaultValue(Type dataType);
        /// <summary>
        /// 获取类型默认值
        /// </summary>
        /// <param name="dataType">数据类型</param>
        /// <returns>默认值</returns>
        public abstract string GetDefaultValue(DataType dataType);
    }
}
