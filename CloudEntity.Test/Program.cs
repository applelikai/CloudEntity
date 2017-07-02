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
        IDbContainer container = DbContainer.GetContainer<MySqlInitializer>();
        IDbQuery<Member> members = container.List<Member>();
        IDbQuery<Category> categories = container.List<Category>();
        members = members.Join(categories, m => m.MemberCategory, (m, c) => m.CategoryId == c.CategoryId);
        foreach (Member member in members)
        {
            Console.WriteLine("{0} {1}", member.MemberCategory.CategoryName, member.MemberName);
        }
    }
}