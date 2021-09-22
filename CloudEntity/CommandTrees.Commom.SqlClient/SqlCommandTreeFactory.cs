using System.Collections.Generic;
using System.Linq;

namespace CloudEntity.CommandTrees.Commom.SqlClient
{
    /// <summary>
    /// 创建用于Sql Server的CommandTree的工厂类
    /// 李凯 Apple_Li
    /// </summary>
    public class SqlCommandTreeFactory : CommandTreeFactory
    {
        /// <summary>
        /// 创建读取节点信息的Helper
        /// </summary>
        /// <returns>读取节点信息的Helper</returns>
        protected override ColumnNodeHelper CreateColumnNodeHelper()
        {
            return new SqlColumnNodeHelper();
        }

        /// <summary>
        /// 创建针对sql server的sql驱动
        /// </summary>
        public SqlCommandTreeFactory()
            : base('@') { }
        /// <summary>
        /// 获取分页查询命令生成树
        /// </summary>
        /// <param name="queryChildBuilders">分页查询命令生成树的子节点集</param>
        /// <param name="sortChildBuilders">排序的子节点集</param>
        /// <returns>分页查询命令生成树</returns>
        public override ICommandTree GetPagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, IEnumerable<ISqlBuilder> sortChildBuilders)
        {
            //创建分页查询命令生成树
            SqlOrderByPagingQueryTree queryTree = new SqlOrderByPagingQueryTree(base.ParameterMarker, sortChildBuilders.ToArray());
            //填充分页查询命令生成树各个节点
            base.LoadQueryTree(queryTree, queryChildBuilders);
            //返回分页查询命令生成树
            return queryTree;
        }
    }
}