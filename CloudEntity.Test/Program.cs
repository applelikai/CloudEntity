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
        string connectionString = "Data Source=localhost;Initial Catalog=MemberSys;User Id=root;SslMode=None;";
        IDbContainer container = DbContainer.GetContainer<MySqlInitializer>(connectionString);
        //获取sql
        StringBuilder sqlBuilder = new StringBuilder();
        sqlBuilder.AppendLine("    SELECT member.MemberId UserId,");
        sqlBuilder.AppendLine("           member.MemberName UserName,");
        sqlBuilder.AppendLine("           category.CatName CategoryName");
        sqlBuilder.AppendLine("      FROM MemberSys.Members member");
        sqlBuilder.AppendLine("INNER JOIN MemberSys.categories category");
        sqlBuilder.AppendLine("        ON member.CatId = category.CatId");
        //获取视图查询数据源
        IDbView<User> users = container.View<User>(sqlBuilder.ToString())
            .In(u => u.CategoryName, new string[] { "金卡会员", "银卡会员" })
            .OrderByDescending(u => u.UserName);
        foreach (User user in users)
        {
            Console.WriteLine("{0} {1}", user.CategoryName, user.UserName);
        }
    }
}