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
        StringBuilder sqlBuilder = new StringBuilder();
        sqlBuilder.AppendLine("SELECT category.CatId CategoryId,");
        sqlBuilder.AppendLine("       category.CatName CategoryName");
        sqlBuilder.AppendLine("  FROM memberSys.Categories category");
        sqlBuilder.AppendLine(" WHERE category.CatName LIKE @Name");
        foreach (Category category in container.DbHelper.Query<Category>(sqlBuilder.ToString(), new { Name = "%会员" }))
        {
            Console.WriteLine(category.CategoryName);
        }
    }
}