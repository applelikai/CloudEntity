using CloudEntity.Core.Data.Entity;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Test.Entities;
using CloudEntity.Test.MySqlClient;
using System;
using System.Reflection;
using System.Text;

public class Program
{
    private static void Main(string[] args)
    {
        string connectionString = "Data Source=localhost;Initial Catalog=MemberSys;User Id=root;SslMode=None;";
        IDbContainer container = DbContainer.GetContainer<MySqlInitializer>(connectionString);
        IDbQuery<Member> members = container.List<Member>()
            .LeftJoin(container.List<Category>(), m => m.MemberCategory, (m, c) => m.CategoryId == c.CategoryId);
        foreach (Member member in members)
        {
            Console.WriteLine("{0} {1}", member.MemberCategory.CategoryName, member.MemberName);
        }
    }
}