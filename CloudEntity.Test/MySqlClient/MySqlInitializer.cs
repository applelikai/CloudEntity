using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom.MySqlClient;
using CloudEntity.Core.Data.Entity;
using CloudEntity.Data;
using CloudEntity.Mapping;
using CloudEntity.Mapping.Common;
using CloudEntity.Test.Entities;
using CloudEntity.Test.Mappers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEntity.Test.MySqlClient
{
    /// <summary>
    /// MySql数据源初始化器
    /// </summary>
    internal class MySqlInitializer : DbInitializer
    {
        /// <summary>
        /// 创建Table初始化器
        /// </summary>
        /// <returns>Table初始化器</returns>
        public override TableInitializer CreateTableInitializer()
        {
            return new MySqlTableInitializer();
        }
        /// <summary>
        /// 创建DbHelper对象
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>DbHelper对象</returns>
        public override DbHelper CreateDbHelper(string connectionString)
        {
            return new MySqlHelper(connectionString);
        }
        /// <summary>
        /// 获取创建sql命令生成树的工厂
        /// </summary>
        /// <returns>sql命令生成树的工厂</returns>
        public override ICommandTreeFactory CreateCommandTreeFactory()
        {
            return new MySqlCommandTreeFactory();
        }
        /// <summary>
        /// 创建Mapper容器
        /// </summary>
        /// <returns>Mapper容器</returns>
        public override IMapperContainer CreateMapperContainer()
        {
            return new InnerMapperContainer();
        }
    }
}
