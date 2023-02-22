using System;
using System.Collections.Generic;
using System.Data;

namespace CloudEntity.Data
{
    /// <summary>
    /// 操作数据库的接口
    /// Apple_Li 李凯 15150598493
    /// 2023/02/21 22：39
    /// </summary>
    public interface IDbHelper
    {
        /// <summary>
        /// 数据库默认架构名（或用户名 或模式 总之就是表名前缀）
        /// </summary>
        string DefaultSchemaName { get; }

        #region 创建sql参数
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter CreateParameter();
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter CreateParameter(string name, object value);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        IDbDataParameter CreateParameter(string name, Type dataType, ParameterDirection direction);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        IDbDataParameter CreateParameter(string name, DbType dataType, ParameterDirection direction);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        IDbDataParameter CreateParameter(string name, Type dataType, int size, ParameterDirection direction);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        IDbDataParameter CreateParameter(string name, DbType dataType, int size, ParameterDirection direction);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter CreateParameter(string name, Type dataType, object value);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter CreateParameter(string name, DbType dataType, object value);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter CreateParameter(string name, Type dataType, ParameterDirection direction, object value);
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        IDbDataParameter CreateParameter(string name, DbType dataType, ParameterDirection direction, object value);
        #endregion

        #region 执行查询
        /// <summary>
        /// Execute and get scaler result
        /// 执行并获取第一行第一列的值
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>第一行第一列的值</returns>
        object GetScalar(string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters);
        /// <summary>
        /// Execute and get dataset
        /// 执行并获取DataSet
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>DataSet对象</returns>
        DataSet GetDataSet(string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters);
        /// <summary>
        /// Execute and get results
        /// 执行并获取结果
        /// </summary>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="getResult">method: 传入DataReader,获取结果</param>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>所有的结果</returns>
        IEnumerable<TResult> GetResults<TResult>(Func<IDataReader, TResult> getResult, string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters);
        /// <summary>
        /// Execute and get results
        /// 执行并获取结果
        /// </summary>
        /// <param name="getResult">匿名函数: 传入DataReader,及当前所选的列名数组,获取结果</param>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <returns>TResult类型的迭代器</returns>
        IEnumerable<TResult> GetResults<TResult>(Func<IDataReader, string[], TResult> getResult, string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters);
        /// <summary>
        /// Execute and get results
        /// 执行并获取结果
        /// </summary>
        /// <param name="getResults">匿名函数: 传入DataReader,及当前所选的列名数组,获取结果列表</param>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <returns>TResult类型的迭代器</returns>
        IEnumerable<TResult> GetResults<TResult>(Func<IDataReader, string[], IEnumerable<TResult>> getResults, string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters);
        #endregion

        #region 执行带事务和不带事务的修改和删除
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        void BeginTransaction(out IDbConnection connection, out IDbTransaction transaction);
        /// <summary>
        /// 结束事务处理
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        void EndTransaction(IDbConnection connection, IDbTransaction transaction);
        /// <summary>
        /// Execute and get changed rows numbers
        /// 执行获取DB受影响行数
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>DB受影响行数</returns>
        int ExecuteUpdate(string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters);
        /// <summary>
        /// Execute and get changed rows numbers
        /// 执行并获取DB受影响行数
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="transaction">事务处理对象</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>DB受影响行数</returns>
        int ExecuteUpdate(string commandText, IDbTransaction transaction, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters);
        #endregion
    }
}