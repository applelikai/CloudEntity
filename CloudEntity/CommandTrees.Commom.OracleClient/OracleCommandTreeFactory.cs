﻿using System.Collections.Generic;

namespace CloudEntity.CommandTrees.Commom.OracleClient
{
    /// <summary>
    /// 创建CommandTree用于生成查询Oracle的sql
    /// 李凯 Apple_Li
    /// </summary>
    public class OracleCommandTreeFactory : CommandTreeFactory
    {
        /// <summary>
        /// 创建读取节点信息的Helper
        /// </summary>
        /// <returns>读取节点信息的Helper</returns>
        protected override ColumnNodeHelper CreateColumnNodeHelper()
        {
            return new OracleColumnNodeHelper();
        }

        /// <summary>
        /// 创建针对Oracle的sql驱动
        /// </summary>
        public OracleCommandTreeFactory()
            : base(':')
        { }
        /// <summary>
        /// 创建分页查询命令生成树
        /// </summary>
        /// <param name="queryChildBuilders">分页查询命令生成树的子节点集</param>
        /// <param name="sortChildBuilders">排序的子节点集</param>
        /// <returns>分页查询命令生成树</returns>
        public override ICommandTree CreatePagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, IEnumerable<ISqlBuilder> sortChildBuilders)
        {
            //创建Oracle分页sql生成树
            OraclePagingQueryTree queryTree = new OraclePagingQueryTree(base.ParameterMarker);
            //加载Oracle分页sql生成树
            base.LoadQueryTree(queryTree, queryChildBuilders);
            //填充OrderBy节点
            foreach (ISqlBuilder nodeBuilder in sortChildBuilders)
                queryTree.OrderBy.Append(nodeBuilder);
            //返回Oracle分页sql生成树
            return queryTree;
        }
    }
}