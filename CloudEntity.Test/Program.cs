using CloudEntity.Core.Data.Entity;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Test.Entities;
using CloudEntity.Test.Models;
using CloudEntity.Test.MySqlClient;
using System;
using System.Reflection;
using System.Text;

public class Program
{
    private static void Main(string[] args)
    {
        //获取数据容器
        string connectionString = "Data Source=localhost;Initial Catalog=MemberSys;User Id=root;SslMode=None;";
        IDbContainer container = DbContainer.GetContainer<MySqlInitializer>(connectionString);
        //执行其他操作
        string searchName = "Apple";
        IDbQuery<Category> categories = container.List<Category>()
            .Where(c => !c.CategoryName.Equals(searchName));
        IDbQuery<Member> members = container.List<Member>()
            .Join(categories, m => m.MemberCategory, (m, c) => m.CategoryId == c.CategoryId)
            .Where(m => m.MemberName.Equals(searchName));
        foreach (Member member in members)
        {
            Console.WriteLine($"{member.MemberCategory.CategoryName} {member.MemberName}");
        }
    }
}