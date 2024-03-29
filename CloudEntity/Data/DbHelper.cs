﻿using System;
using System.Collections.Generic;
using System.Data;

namespace CloudEntity.Data
{
    /// <summary>
    /// 操作数据库的DbHelper
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    public abstract class DbHelper : IDbHelper
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private readonly string _connectionString;
        /// <summary>
        /// 数据库默认架构名（或用户名 或模式 总之就是表名前缀）
        /// </summary>
        private readonly string _defaultSchemaName;

        /// <summary>
        /// 数据库默认架构名（或用户名 或模式 总之就是表名前缀）
        /// </summary>
        public string DefaultSchemaName
        {
            get { return _defaultSchemaName; }
        }

        /// <summary>
        /// 获取数据类型
        /// </summary>
        /// <param name="sourceType">源类型</param>
        /// <returns>数据类型</returns>
        private DbType GetDbType(Type sourceType)
        {
            DbType dbType;
            Enum.TryParse(sourceType.Name, out dbType);
            return dbType == default(DbType) ? DbType.Object : dbType;
        }

        /// <summary>
        /// 记录执行的sql命令
        /// </summary>
        /// <param name="commandText">sql命令</param>
        protected abstract void RecordCommand(string commandText);
        /// <summary>
        /// 创建Connection对象
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>Connection对象</returns>
        protected abstract IDbConnection Connect(string connectionString);
        /// <summary>
        /// 创建Command对象
        /// </summary>
        /// <param name="connection">连接</param>
        /// <returns>Command对象</returns>
        protected virtual IDbCommand CreateCommand(IDbConnection connection)
        {
            return connection.CreateCommand();
        }
        /// <summary>
        /// 创建数据适配器
        /// </summary>
        /// <returns>数据适配器</returns>
        protected abstract IDbDataAdapter CreateDataAdapter();

        /// <summary>
        /// 初始化DbHelper
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="defaultSchemaName">数据库默认架构名（或用户名 或模式 总之就是表名前缀）</param>
        public DbHelper(string connectionString, string defaultSchemaName = null)
        {
            //非空检查
            Check.ArgumentNull(connectionString, nameof(connectionString));
            //赋值
            _connectionString = connectionString;
            _defaultSchemaName = defaultSchemaName;
        }

        #region 创建sql参数
        /// <summary>
        /// 创建sql参数
        /// </summary>
        /// <returns>sql参数</returns>
        public abstract IDbDataParameter CreateParameter();
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        public virtual IDbDataParameter CreateParameter(string name, object value)
        {
            IDbDataParameter parameter = this.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        public virtual IDbDataParameter CreateParameter(string name, Type dataType, ParameterDirection direction)
        {
            return this.CreateParameter(name, this.GetDbType(dataType), direction);
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        public virtual IDbDataParameter CreateParameter(string name, DbType dataType, ParameterDirection direction)
        {
            IDbDataParameter parameter = this.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = dataType;
            parameter.Direction = direction;
            return parameter;
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        public virtual IDbDataParameter CreateParameter(string name, Type dataType, int size, ParameterDirection direction)
        {
            return this.CreateParameter(name, this.GetDbType(dataType), size, direction);
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        public virtual IDbDataParameter CreateParameter(string name, DbType dataType, int size, ParameterDirection direction)
        {
            IDbDataParameter parameter = this.CreateParameter(name, dataType, direction);
            parameter.Size = size;
            return parameter;
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        public virtual IDbDataParameter CreateParameter(string name, Type dataType, object value)
        {
            return this.CreateParameter(name, this.GetDbType(dataType), value);
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        public virtual IDbDataParameter CreateParameter(string name, DbType dataType, object value)
        {
            IDbDataParameter parameter = this.CreateParameter();
            parameter.ParameterName = name;
            parameter.DbType = dataType;
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        public virtual IDbDataParameter CreateParameter(string name, Type dataType, ParameterDirection direction, object value)
        {
            return this.CreateParameter(name, this.GetDbType(dataType), direction, value);
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        public virtual IDbDataParameter CreateParameter(string name, DbType dataType, ParameterDirection direction, object value)
        {
            IDbDataParameter parameter = this.CreateParameter(name, dataType, direction);
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }
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
        public object GetScalar(string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            using (IDbConnection connection = this.Connect(_connectionString))
            {
                //打开连接
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                //创建Command对象
                using (IDbCommand command = this.CreateCommand(connection))
                {
                    //指定并记录sql命令
                    command.CommandText = commandText;
                    this.RecordCommand(command.CommandText);
                    //添加参数
                    foreach (IDbDataParameter parameter in parameters)
                        command.Parameters.Add(parameter);
                    //获取命令类型
                    command.CommandType = commandType;
                    //获取结果
                    object result = command.ExecuteScalar();
                    //清空sql参数
                    command.Parameters.Clear();
                    //返回执行结果
                    return result;
                }
            }
        }
        /// <summary>
        /// Execute and get dataset
        /// 执行并获取DataSet
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>DataSet对象</returns>
        public virtual DataSet GetDataSet(string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            // 创建数据库连接
            using (IDbConnection connection = this.Connect(_connectionString))
            {
                // 打开连接
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                // 创建Command对象
                using (IDbCommand command = this.CreateCommand(connection))
                {
                    // 指定并记录sql命令
                    command.CommandText = commandText;
                    this.RecordCommand(command.CommandText);
                    // 添加参数
                    foreach (IDbDataParameter parameter in parameters)
                        command.Parameters.Add(parameter);
                    // 指定命令类型
                    command.CommandType = commandType;
                    // 创建数据适配器
                    IDbDataAdapter dataAdapter = this.CreateDataAdapter();
                    // 为数据适配器指定查询Command
                    dataAdapter.SelectCommand = command;
                    // 创建DataSet对象
                    DataSet dataSet = new DataSet();
                    // 填充DataSet对象
                    dataAdapter.Fill(dataSet);
                    // 最终获取DataSet对象
                    return dataSet;
                }
            }
        }
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
        public IEnumerable<TResult> GetResults<TResult>(Func<IDataReader, TResult> getResult, string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            using (IDbConnection connection = this.Connect(_connectionString))
            {
                //打开连接
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                //创建Command对象
                using (IDbCommand command = this.CreateCommand(connection))
                {
                    //指定并记录sql命令
                    command.CommandText = commandText;
                    this.RecordCommand(command.CommandText);
                    //添加参数
                    foreach (IDbDataParameter parameter in parameters)
                        command.Parameters.Add(parameter);
                    //获取命令类型
                    command.CommandType = commandType;
                    //获取DataReader并转换获取结果
                    using (IDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        while (reader.Read())
                            yield return getResult(reader);
                    }
                    //清空sql参数
                    command.Parameters.Clear();
                }
            }
        }
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
        public IEnumerable<TResult> GetResults<TResult>(Func<IDataReader, string[], TResult> getResult, string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            using (IDbConnection connection = this.Connect(_connectionString))
            {
                //打开连接
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                //创建Command对象
                using (IDbCommand command = this.CreateCommand(connection))
                {
                    //指定并记录sql命令
                    command.CommandText = commandText;
                    this.RecordCommand(command.CommandText);
                    //添加参数
                    foreach (IDbDataParameter parameter in parameters)
                        command.Parameters.Add(parameter);
                    //获取命令类型
                    command.CommandType = commandType;
                    //获取DataReader并转换获取结果
                    using (IDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        // 获取查询的列名数组
                        string[] columnNames = reader.GetColumns();
                        // 读取DataReader,转换获取TResult
                        while (reader.Read())
                            yield return getResult(reader, columnNames);
                    }
                    //清空sql参数
                    command.Parameters.Clear();
                }
            }
        }
        #endregion

        #region 执行带事务和不带事务的修改和删除
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        public void BeginTransaction(out IDbConnection connection, out IDbTransaction transaction)
        {
            connection = this.Connect(_connectionString);   //创建数据库连接对象
            if (connection.State != ConnectionState.Open)       //若连接没有打开
                connection.Open();                                  //打开连接
            transaction = connection.BeginTransaction();        //获取事务
        }
        /// <summary>
        /// 结束事务处理
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事务</param>
        public void EndTransaction(IDbConnection connection, IDbTransaction transaction)
        {
            transaction.Dispose();  //释放事务
            connection.Dispose();   //释放连接
        }
        /// <summary>
        /// Execute and get changed rows numbers
        /// 执行获取DB受影响行数
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>DB受影响行数</returns>
        public int ExecuteUpdate(string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            using (IDbConnection connection = this.Connect(_connectionString))
            {
                //打开连接
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                //创建Command对象
                using (IDbCommand command = this.CreateCommand(connection))
                {
                    //指定并记录sql命令
                    command.CommandText = commandText;
                    this.RecordCommand(command.CommandText);
                    //填充sql参数
                    foreach (IDbDataParameter parameter in parameters)
                        command.Parameters.Add(parameter);
                    //获取命令类型
                    command.CommandType = commandType;
                    //获取DB受影响行数
                    int result = command.ExecuteNonQuery();
                    //清空sql参数
                    command.Parameters.Clear();
                    //返回受影响行数
                    return result;
                }
            }
        }
        /// <summary>
        /// Execute and get changed rows numbers
        /// 执行并获取DB受影响行数
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="transaction">事务处理对象</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>DB受影响行数</returns>
        public int ExecuteUpdate(string commandText, IDbTransaction transaction, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            //开始执行命令
            using (IDbCommand command = this.CreateCommand(transaction.Connection))
            {
                //指定处理事务
                command.Transaction = transaction;
                //指定并记录sql命令
                command.CommandText = commandText;
                this.RecordCommand(command.CommandText);
                //填充sql参数
                foreach (IDbDataParameter parameter in parameters)
                    command.Parameters.Add(parameter);
                //获取命令类型
                command.CommandType = commandType;
                //获取DB受影响行数
                int result = command.ExecuteNonQuery();
                //清空sql参数
                command.Parameters.Clear();
                //返回受影响行数
                return result;
            }
        }
        #endregion
    }
}
