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
using System.Data;
using System.Linq;
using System.Text;

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
            Console.WriteLine("{0} {1}", user.Role?.RoleName, user.UserName);
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
        //获取时间
        DateTime start = DateTime.Parse("2023/02/06 00:00:00");
        // 获取角色数据源
        IDbQuery<Role> roles = _container.CreateQuery<Role>()
            .IncludeBy(r => r.RoleName);
        // 获取用户数据源
        IDbQuery<User> users = _container.CreateQuery<User>()
            .IncludeBy(u => new { u.UserName, u.Password})
            .Join(roles, u => u.Role, (u, r) => u.RoleId == r.RoleId)
            .Where(u => u.UserName.StartsWith("a"));
        // 第一次打印用户列表
        Program.PrintUsers(users);
        // 第二次映射为微信用户列表并打印
        Program.PrintUsers(users.Cast<WechatUser>());
        // 获取用户分页查询数据源
        string[] names = new string[] {"apple", "admin"};
        IDbPagedQuery<User> pagedUsers = users.Where(u => names.Contains(u.UserName)).PagingByDescending(u => u.UserName, 10, 1);
        // 第三次打印用户列表
        Program.PrintUsers(pagedUsers);
        // 第四次映射为微信用户列表并打印
        Program.PrintUsers(pagedUsers.Cast<WechatUser>());
    }
    /// <summary>
    /// IN查询测试
    /// </summary>
    /// <param name="users">用户数据源</param>
    private static void TestIn(IDbView<WechatUser> users)
    {
        // 获取用户姓名数组
        string[] names = new string[] { "apple", "admin", "orange" };
        // 添加查询条件
        users = users.In(u => u.UserName, names);
        // 打印
        Program.PrintUsers(users);
        // 获取角色名称数据源
        IDbSelectedQuery<string> roleNames = _container.CreateQuery<Role>()
            .Select(r => r.RoleName);
        // 添加查询条件
        users = users.In(u => u.RoleName, roleNames);
        // 打印
        Program.PrintUsers(users);
    }
    /// <summary>
    /// BETWEEN查询测试
    /// </summary>
    /// <param name="users">用户数据源</param>
    private static void TestBetween(IDbView<WechatUser> users)
    {
        // 获取开始时间
        DateTime start = DateTime.Parse("2023/02/07 00:00:00");
        // 添加查询条件
        users = users.Between(u => u.CreatedTime, start, DateTime.Now);
        // 打印
        Program.PrintUsers(users);
    }
    /// <summary>
    /// 视图查询测试
    /// </summary>
    private static void ViewQueryTest()
    {
        // 获取角色数据源
        IDbQuery<Role> roles = _container.CreateQuery<Role>()
            .IncludeBy(r => r.RoleName);
        // 获取用户数据源
        IDbQuery<User> users = _container.CreateQuery<User>()
            .IncludeBy(u => new { u.UserName, u.Password, u.CreatedTime})
            .Join(roles, u => u.Role, (u, r) => u.RoleId == r.RoleId);
        // 获取sql和参数
        string querySql = users.ToSqlString();
        IDbDataParameter[] sqlParameters = users.Parameters.ToArray();
        // 获取查询视图
        IDbView<WechatUser> wechatUsers = _container.CreateView<WechatUser>(querySql, sqlParameters);
        // 测试IN查询
        Program.TestIn(wechatUsers);
    }
    /// <summary>
    /// 开始执行
    /// </summary>
    /// <param name="args">控制台参数</param>
    private static void Main(string[] args)
    {
        // 查询测试
        Program.ViewQueryTest();
    }
}