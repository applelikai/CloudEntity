using CloudEntity.Core.Data.Entity;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Test.Entities.SysManagement;
using CloudEntity.Test.Models;
using CloudEntity.Test.PostgreSqlClient;
using CloudEntity.Test.Utils;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;

public class Program
{
    /// <summary>
    /// 数据容器
    /// </summary>
    private static IDbContainer _container;

    /// <summary>
    /// 静态初始化
    /// </summary>
    static Program()
    {
        //获取配置
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json")
            .Build();
        //获取连接字符串
        string connectionName = configuration["ConnectionName"];
        string connectionString = configuration.GetConnectionString(connectionName);
        //初始化数据容器
        DbContainer.Init<PostgreSqlInitializer>(connectionString);
        //获取数据容器
        _container = DbContainer.Get(connectionString);
    }
    /// <summary>
    /// 初始化所有表
    /// </summary>
    private static void InitTables()
    {
        _container.InitTable<Role>();
        _container.InitTable<User>();
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    private static void InitData()
    {
        // 创建角色id
        string roleId = GuidHelper.NewOrdered().ToString();
        // 添加角色
        _container.List<Role>().Add(new Role(roleId, "管理员"));
        // 添加用户
        _container.List<User>().Add(new User(GuidHelper.NewOrdered().ToString())
        {
            UserName = "admin",
            Password = "000000",
            RoleId = roleId
        });
        // 添加用户
        _container.List<User>().Add(new User(GuidHelper.NewOrdered().ToString())
        {
            UserName = "apple",
            Password = "000000",
            RoleId = roleId
        });
    }
    /// <summary>
    /// 打印用户信息
    /// </summary>
    /// <param name="users">用户列表</param>
    private static void PrintUsers(IEnumerable<User> users)
    {
        // 遍历用户并打印
        foreach (User user in users)
        {
            Console.WriteLine("{0} {1}", user.Role.RoleName, user.UserName);
        }
    }
    /// <summary>
    /// 打印用户信息
    /// </summary>
    /// <param name="users">用户列表</param>
    private static void PrintUsers(IEnumerable<WechatUser> users)
    {
        // 遍历用户并打印
        foreach (WechatUser user in users)
        {
            Console.WriteLine("{0} {1}", user.RoleName, user.UserName);
        }
    }
    /// <summary>
    /// 查询测试
    /// </summary>
    private static void QueryTest()
    {
        // 获取角色数据源
        IDbQuery<Role> roles = _container.CreateQuery<Role>()
            .IncludeBy(r => r.RoleName);
        // 获取用户数据源
        IDbQuery<User> users = _container.CreateQuery<User>()
            .IncludeBy(u => new { u.UserName, u.Password})
            .Join(roles, u => u.Role, (u, r) => u.RoleId == r.RoleId)
            .Like(u => u.UserName, "a%");
        // 第一次打印用户列表
        Program.PrintUsers(users);
        // 第二次映射为微信用户列表并打印
        Program.PrintUsers(users.Cast<WechatUser>());
        // 获取用户分页查询数据源
        IDbPagedQuery<User> pagedUsers = users.Like(u => u.UserName, "ad%").PagingByDescending(u => u.CreatedTime, 10, 1);
        // 第三次打印用户列表
        Program.PrintUsers(pagedUsers);
        // 第四次映射为微信用户列表并打印
        Program.PrintUsers(pagedUsers.Cast<WechatUser>());
    }
    /// <summary>
    /// 开始执行
    /// </summary>
    /// <param name="args">控制台参数</param>
    private static void Main(string[] args)
    {
        string name0 = "UserName";
        string name1 = "UserName";
        Console.WriteLine(name0.StartsWith(name1));
    }
}