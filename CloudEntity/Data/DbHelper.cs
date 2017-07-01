using System;
using System.Collections.Generic;
using System.Data;

namespace CloudEntity.Data
{
    /// <summary>
    /// 操作数据库的DbHelper
    /// </summary>
    public abstract class DbHelper : IParameterFactory
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string connectionString;

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
        /// 初始化DbHelper
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public DbHelper(string connectionString)
        {
            //非空检查
            Check.ArgumentNull(connectionString, nameof(connectionString));
            //赋值
            this.connectionString = connectionString;
        }

        /// <summary>
        /// 创建sql参数
        /// </summary>
        /// <returns>sql参数</returns>
        public abstract IDbDataParameter Parameter();
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        public IDbDataParameter Parameter(string name, object value)
        {
            IDbDataParameter parameter = this.Parameter();
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
        public IDbDataParameter Parameter(string name, Type dataType, ParameterDirection direction)
        {
            return this.Parameter(name, this.GetDbType(dataType), direction);
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        public IDbDataParameter Parameter(string name, DbType dataType, ParameterDirection direction)
        {
            IDbDataParameter parameter = this.Parameter();
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
        public IDbDataParameter Parameter(string name, Type dataType, int size, ParameterDirection direction)
        {
            return this.Parameter(name, this.GetDbType(dataType), size, direction);
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="size">长度</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <returns>参数</returns>
        public IDbDataParameter Parameter(string name, DbType dataType, int size, ParameterDirection direction)
        {
            IDbDataParameter parameter = this.Parameter(name, dataType, direction);
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
        public IDbDataParameter Parameter(string name, Type dataType, object value)
        {
            return this.Parameter(name, this.GetDbType(dataType), value);
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        public IDbDataParameter Parameter(string name, DbType dataType, object value)
        {
            IDbDataParameter parameter = this.Parameter();
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
        public IDbDataParameter Parameter(string name, Type dataType, ParameterDirection direction, object value)
        {
            return this.Parameter(name, this.GetDbType(dataType), direction, value);
        }
        /// <summary>
        /// 创建参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="dataType">参数值类型</param>
        /// <param name="direction">参数类型(输入输出)</param>
        /// <param name="value">参数值</param>
        /// <returns>参数</returns>
        public IDbDataParameter Parameter(string name, DbType dataType, ParameterDirection direction, object value)
        {
            IDbDataParameter parameter = this.Parameter(name, dataType, direction);
            parameter.Value = value ?? DBNull.Value;
            return parameter;
        }

        /// <summary>
        /// Execute and get result
        /// 执行并获取单个结果
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>单个结果</returns>
        public object GetScalar(string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            using (IDbConnection connection = this.Connect(this.connectionString))
            {
                //打开连接
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                //创建Command对象
                using (IDbCommand command = connection.CreateCommand())
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
            using (IDbConnection connection = this.Connect(this.connectionString))
            {
                //打开连接
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                //创建Command对象
                using (IDbCommand command = connection.CreateCommand())
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
        /// <typeparam name="TResult">结果类型</typeparam>
        /// <param name="commandText">sql命令</param>
        /// <param name="getResult">method: 传入DataReader,及当前所选的列名数组,获取结果</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>TResult类型的迭代器</returns>
        public IEnumerable<TResult> GetResults<TResult>(Func<IDataReader, string[], TResult> getResult, string commandText, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            using (IDbConnection connection = this.Connect(this.connectionString))
            {
                //打开连接
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                //创建Command对象
                using (IDbCommand command = connection.CreateCommand())
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
                        //读取DataReader,转换获取TResult
                        while (reader.Read())
                            yield return getResult(reader, reader.GetColumns());
                    }
                    //清空sql参数
                    command.Parameters.Clear();
                }
            }
        }
        
        /// <summary>
        /// 开始事故
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事故</param>
        public void BeginTransaction(out IDbConnection connection, out IDbTransaction transaction)
        {
            connection = this.Connect(this.connectionString);   //创建数据库连接对象
            if (connection.State != ConnectionState.Open)       //若连接没有打开
                connection.Open();                                  //打开连接
            transaction = connection.BeginTransaction();        //获取事故
        }
        /// <summary>
        /// 结束事故处理
        /// </summary>
        /// <param name="connection">数据库连接</param>
        /// <param name="transaction">事故</param>
        public void EndTransaction(IDbConnection connection, IDbTransaction transaction)
        {
            transaction.Dispose();  //释放事故
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
            using (IDbConnection connection = this.Connect(this.connectionString))
            {
                //打开连接
                if (connection.State != ConnectionState.Open)
                    connection.Open();
                //创建Command对象
                using (IDbCommand command = connection.CreateCommand())
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
        /// <param name="transaction">事故处理对象</param>
        /// <param name="commandType">命令类型</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>DB受影响行数</returns>
        public int ExecuteUpdate(string commandText, IDbTransaction transaction, CommandType commandType = CommandType.Text, params IDbDataParameter[] parameters)
        {
            //开始执行命令
            using (IDbCommand command = transaction.Connection.CreateCommand())
            {
                //指定处理事故
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
    }
}
