﻿using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 查询第一行第一列的值的类
    /// </summary>
    internal class DbScalar : DbQueryBase, IDbScalar
    {
        /// <summary>
        /// 创建统计查询类
        /// </summary>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbScalar(ICommandFactory commandFactory, IDbHelper dbHelper)
            : base(commandFactory, dbHelper){ }
        /// <summary>
        /// 执行查询获取第一行第一列的值
        /// </summary>
        /// <returns>第一行第一列的值</returns>
        public object Execute()
        {
            //创建CommandTree
            ICommandTree queryTree = base.CommandFactory.GetQueryTree(base.NodeBuilders);
            //执行查询获取第一行，第一列的值
            return base.DbHelper.GetScalar(queryTree.Compile(), parameters: base.Parameters.ToArray());
        }
    }
}
