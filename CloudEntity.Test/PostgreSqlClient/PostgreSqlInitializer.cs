using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom.PostgreSqlClient;
using CloudEntity.Core.Data.Entity;
using CloudEntity.Data;
using CloudEntity.Mapping;
using CloudEntity.Test.Mappers;

namespace CloudEntity.Test.PostgreSqlClient;

/// <summary>
/// 访问PostgreSql数据库的初始化器
/// </summary>
public class PostgreSqlInitializer : DbInitializer
{
    /// <summary>
    /// 获取Column初始化器(Code First时需要使用)
    /// </summary>
    /// <returns>Column初始化器</returns>
    public override ColumnInitializer CreateColumnInitializer()
    {
        return new PostgreSqlColumnInitializer();
    }
    /// <summary>
    /// 创建Table初始化器(Code First时需要使用)
    /// </summary>
    /// <returns>Table初始化器</returns>
    public override TableInitializer CreateTableInitializer()
    {
        return new PostgreSqlTableInitializer();
    }
    /// <summary>
    /// 创建DbHelper对象
    /// </summary>
    /// <param name="connectionString">连接字符串</param>
    /// <returns>DbHelper对象</returns>
    public override DbHelper CreateDbHelper(string connectionString)
    {
        return new PostgreSqlHelper(connectionString);
    }
    /// <summary>
    /// 获取创建sql命令生成树的工厂
    /// </summary>
    /// <returns>sql命令生成树的工厂</returns>
    public override ICommandTreeFactory CreateCommandTreeFactory()
    {
        return new PostgreSqlCommandTreeFactory();
    }
    /// <summary>
    /// 创建Mapper容器
    /// </summary>
    /// <returns>Mapper容器</returns>
    public override IMapperContainer CreateMapperContainer()
    {
        return new InnerMapperContainer();
    }
}
