using CloudEntity.Data;
using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace CloudEntity.Test.MySqlClient
{
    /// <summary>
    /// 操作MySql数据库的DbHelper
    /// </summary>
    internal class MySqlHelper : DbHelper
    {
        /// <summary>
        /// 创建数据库连接
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>数据库连接</returns>
        protected override IDbConnection Connect(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
        /// <summary>
        /// 记录执行后的命令
        /// </summary>
        /// <param name="commandText">sql命令</param>
        protected override void RecordCommand(string commandText)
        {
            Console.WriteLine(commandText);
        }

        /// <summary>
        /// 创建操作MySql数据库的对象
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        public MySqlHelper(string connectionString)
            : base(connectionString)
        {
        }
        /// <summary>
        /// 创建sql参数
        /// </summary>
        /// <returns>sql参数</returns>
        public override IDbDataParameter Parameter()
        {
            return new MySqlParameter();
        }
    }
}
