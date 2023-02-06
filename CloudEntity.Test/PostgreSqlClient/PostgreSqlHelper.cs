using CloudEntity.Data;
using Npgsql;
using System;
using System.Data;

namespace CloudEntity.Test.PostgreSqlClient;

/// <summary>
/// 操作PostgreSq的帮助类
/// </summary>
public class PostgreSqlHelper : DbHelper
{
    /// <summary>
    /// 创建数据库连接
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <returns>数据库连接</returns>
    protected override IDbConnection Connect(string connectionString)
    {
        return new NpgsqlConnection(connectionString);
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
    /// 初始化
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    public PostgreSqlHelper(string connectionString)
        : base(connectionString, "public") { }
    /// <summary>
    /// 创建sql参数
    /// </summary>
    /// <returns>sql参数</returns>
    public override IDbDataParameter Parameter()
    {
        return new NpgsqlParameter();
    }
}
