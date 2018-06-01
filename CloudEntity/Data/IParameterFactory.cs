using System;
using System.Data;

namespace CloudEntity.Data
{
    /// <summary>
    /// sql参数工厂接口
    /// </summary>
    public interface IParameterFactory
    {
        /// <summary>
        /// 创建sql参数
        /// </summary>
        /// <returns>sql参数</returns>
        IDbDataParameter Parameter();
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter Parameter(string name, object value);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        IDbDataParameter Parameter(string name, Type dataType, ParameterDirection direction);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        IDbDataParameter Parameter(string name, DbType dataType, ParameterDirection direction);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        IDbDataParameter Parameter(string name, Type dataType, int size, ParameterDirection direction);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        IDbDataParameter Parameter(string name, DbType dataType, int size, ParameterDirection direction);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter Parameter(string name, Type dataType, object value);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter Parameter(string name, DbType dataType, object value);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter Parameter(string name, Type dataType, ParameterDirection direction, object value);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter Parameter(string name, DbType dataType, ParameterDirection direction, object value);
    }
}
