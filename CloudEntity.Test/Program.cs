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
        container.List<Member>().Add(new Member()
        {
            MemberName = "Sarah",
            CategoryId = 1,
            Sex = 0,
            Age = 28
        });
    }
}