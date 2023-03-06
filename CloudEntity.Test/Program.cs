using CloudEntity.Core.Data.Entity;
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

public class Program
{
    /// <summary>
    /// 连接字符串
    /// </summary>
    private static string _connectionString;
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
        _connectionString = configuration.GetConnectionString(connectionName);
        //初始化数据容器
        DbContainer.Init<PostgreSqlInitializer>(_connectionString);
        //获取数据容器
        _container = DbContainer.Get(_connectionString);
    }
    /// <summary>
    /// 获取微信用户视图查询数据源
    /// </summary>
    /// <returns>微信用户视图查询数据源</returns>
    private static IDbAsView<WechatUser> GetWechatUsers()
    {
        // 构建角色查询数据源
        IDbQuery<Role> roles = _container.CreateQuery<Role>()
            .IncludeBy(r => r.RoleName);
        // 构建用户查询数据源
        IDbQuery<User> users = _container.CreateQuery<User>()
            .IncludeBy(u => new { u.UserName, u.Password, u.CreatedTime})
            .Join(roles, u => u.Role, (u, r) => u.RoleId == r.RoleId);
        // 获取sql和参数
        string querySql = users.ToSqlString();
        IDbDataParameter[] sqlParameters = users.Parameters.ToArray();
        // 获取视图查询数据源
        return _container.CreateView<WechatUser>(querySql, sqlParameters);
    }
    /// <summary>
    /// 初始化所有表
    /// </summary>
    private static void InitTables()
    {
        // 获取数据容器
        IDbContainer container = DbContainer.Get(_connectionString);
        // 初始化表
        container.InitTable<Role>();
        container.InitTable<User>();
    }
    /// <summary>
    /// 删除所有表
    /// </summary>
    private static void DropTables()
    {
        // 获取数据容器
        IDbContainer container = DbContainer.Get(_connectionString);
        // 删除表
        container.DropTable<Role>();
        container.DropTable<User>();
    }
    /// <summary>
    /// 初始化数据
    /// </summary>
    private static void InitData()
    {
        // 获取数据容器
        IDbContainer container = DbContainer.Get(_connectionString);
        // 创建角色id
        string roleId = GuidHelper.NewOrdered().ToString();
        // 添加角色
        container.List<Role>().Add(new Role(roleId, "管理员"));
        // 添加用户
        container.List<User>().Add(new User(GuidHelper.NewOrdered().ToString())
        {
            UserName = "admin",
            Password = "000000",
            RoleId = roleId
        });
        // 添加用户
        container.List<User>().Add(new User(GuidHelper.NewOrdered().ToString())
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
            Console.WriteLine("{0} {1} {2:yyyy/MM/dd HH:mm:ss}", user.Role?.RoleName, user.UserName, user.CreatedTime);
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
            Console.WriteLine("{0} {1} {2:yyyy/MM/dd HH:mm:ss}", user.RoleName, user.UserName, user.CreatedTime);
        }
    }
    /// <summary>
    /// 查询测试
    /// </summary>
    private static void QueryTest()
    {
        // 获取角色数据源
        IDbQuery<Role> roles = _container.CreateQuery<Role>()
            .SetIncludeBy(r => r.RoleName);
        // 获取用户数据源
        IDbQuery<User> users = _container.CreateQuery<User>()
            .SetIncludeBy(u => new { u.UserName, u.CreatedTime})
            .SetJoin(roles, u => u.Role, (u, r) => u.RoleId == r.RoleId)
            .SetSort(u => u.Role.RoleName);
        // 第一次打印用户列表
        // Program.PrintUsers(users);
        // 第二次映射为微信用户列表并打印
        // Program.PrintUsers(users.Cast<WechatUser>());
        // 获取用户分页查询数据源
        IDbPagedQuery<User> pagedUsers = users.PagingByDescending(u => u.CreatedTime, 10, 1);
        // 第三次打印用户列表
        Program.PrintUsers(pagedUsers);
        // 第四次映射为微信用户列表并打印
        // Program.PrintUsers(pagedUsers.Cast<WechatUser>());
    }
    /// <summary>
    /// TOP查询测试
    /// </summary>
    private static void TestTopQuery()
    {
        // 获取角色数据源
        IDbQuery<Role> roles = _container.CreateQuery<Role>()
            .SetIncludeBy(r => r.RoleName);
        // 获取用户数据源
        IDbTopQuery<User> users = _container.CreateQuery<User>()
            .SetIncludeBy(u => new { u.UserName, u.CreatedTime})
            .SetJoin(roles, u => u.Role, (u, r) => u.RoleId == r.RoleId)
            .Top(10);
        // 打印用户列表
        Program.PrintUsers(users);
        // 获取微信用户列表
        IEnumerable<WechatUser> wechatUsers = users.Cast<WechatUser>();
        // 打印微信用户列表
        Program.PrintUsers(wechatUsers);

    }
    /// <summary>
    /// 测试IN语句查询
    /// </summary>
    private static void QueryTestIn()
    {
        // 获取角色id数据源
        IDbSelectedQuery<string> roleIds = _container.CreateQuery<Role>().Top(10, r => r.RoleId);
        // 获取用户数据源
        IDbQuery<User> users = _container.CreateQuery<User>().OrderByDescending(u => u.CreatedTime);
        // 打印用户列表
        Program.PrintUsers(users.Top(10));
    }
    /// <summary>
    /// 选定项查询测试
    /// </summary>
    private static void QueryTestSelect()
    {
        // 获取用户数据源
        IDbQuery<User> users = _container.CreateQuery<User>();
        // 获取
    }
    /// <summary>
    /// 测试Between查询
    /// </summary>
    private static void TestBetween()
    {
        //获取时间
        DateTime start = DateTime.Parse("2023/01/01 00:00:00");
        // 获取用户数据源
        IDbQuery<User> users = _container.CreateQuery<User>()
            .Between(u => u.CreatedTime, start, DateTime.Now);
        // 第一次打印用户列表
        Program.PrintUsers(users);
    }
    /// <summary>
    /// BETWEEN查询测试
    /// </summary>
    /// <param name="users">用户数据源</param>
    private static void TestBetween(IDbAsView<WechatUser> users)
    {
        // 获取开始时间
        DateTime start = DateTime.Parse("2023/02/05 00:00:00");
        // 添加查询条件
        users = users.Between(u => u.CreatedTime, start, DateTime.Now);
        // 打印参数列表
        // 打印
        Program.PrintUsers(users);
    }
    /// <summary>
    /// 测试IN查询
    /// </summary>
    /// <param name="users">用户数据源</param>
    private static void TestIn(IDbAsView<WechatUser> users)
    {
        // 获取用户名数组
        string[] names = new string[] { "admin", "apple", "bob" };
        // 添加数据源检索条件
        users.SetIn(u => u.UserName, names);
        // 打印用户列表
        Program.PrintUsers(users);
    }
    /// <summary>
    /// 测试LIKE查询
    /// </summary>
    /// <param name="users">用户数据源</param>
    private static void TestLike(IDbAsView<WechatUser> users)
    {
        // 添加数据源检索条件
        users.SetLike(u => u.UserName, "ap%", false);
        // 打印用户列表
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
        IDbAsView<WechatUser> wechatUsers = _container.CreateView<WechatUser>(querySql, sqlParameters)
            .SetIsNull(u => u.UserName, false)
            .OrderByDescending(u => u.CreatedTime)
            .ThenBy(u => u.UserName);
        // 打印用户列表
        Program.PrintUsers(wechatUsers);
    }
    /// <summary>
    /// 统计测试
    /// </summary>
    private static void TestCount()
    {
        // 获取数据容器
        IDbContainer container = DbContainer.Get(_connectionString);
        // 打印统计信息
        Console.WriteLine(container.List<Role>().Count());
        Console.WriteLine(container.List<User>().Max(u => u.CreatedTime));
    }
    /// <summary>
    /// 测试事务执行
    /// </summary>
    private static void TestTransaction()
    {
        // 创建事务处理对象
        using (IDbExecutor executor = _container.CreateExecutor())
        {
            // 获取角色id
            string roleId = "000";
            // 删除角色
            executor.Operator<Role>().RemoveAll(r => r.RoleId.Equals(roleId));
            // 删除角色下的所有用户
            executor.Operator<User>().RemoveAll(u => u.RoleId.Equals(roleId));
            // 提交修改
            executor.Commit();
        }
    }
    /// <summary>
    /// 开始执行
    /// </summary>
    /// <param name="args">控制台参数</param>
    private static void Main(string[] args)
    {
        // 查询测试
        Program.QueryTest();
    }
}