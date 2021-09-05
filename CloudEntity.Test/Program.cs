using CloudEntity.Core.Data.Entity;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Test.Entities;
using CloudEntity.Test.Models;
using CloudEntity.Test.MySqlClient;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

public class Program
{
    /// <summary>
    /// 数据容器
    /// </summary>
    private static IDbContainer _container;

    /// <summary>
    /// 分页获取分类列表
    /// </summary>
    /// <typeparam name="TCategory">分类类型</typeparam>
    /// <returns>分类分页列表</returns>
    private static IDbPagedQuery<TCategory> GetCategories<TCategory>()
        where TCategory : CategoryBase
    {
        //获取分类数据源
        IDbQuery<TCategory> categories = _container.List<TCategory>().Like(c => c.CategoryName, "%商");
        //返回分类分页数据源
        return categories.PagingByDescending(c => c.CreatedTime, 10);
    }
    /// <summary>
    /// 开始执行
    /// </summary>
    /// <param name="args">控制台参数</param>
    private static void Main(string[] args)
    {
        //获取公司分类列表
        IEnumerable<CompanyCategory> categories = Program.GetCategories<CompanyCategory>();
        //遍历公司分类列表
        foreach (CompanyCategory category in categories)
        {
            Console.WriteLine(category.CategoryName);
        }
    }

    /// <summary>
    /// 静态初始化
    /// </summary>
    static Program()
    {
        //获取连接字符串
        string connectionString = "Data Source=39.106.212.169;User Id=root;Password=123456;Initial Catalog=cheij_management;";
        //初始化数据容器
        DbContainer.Init<MySqlInitializer>(connectionString);
        //获取数据容器
        _container = DbContainer.Get(connectionString);
    }
}