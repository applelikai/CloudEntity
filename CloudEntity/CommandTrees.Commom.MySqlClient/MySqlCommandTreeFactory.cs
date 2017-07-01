using System.Collections.Generic;

namespace CloudEntity.CommandTrees.Commom.MySqlClient
{
    /// <summary>
    /// 创建用于MySql的CommandTree的工厂
    /// 李凯 Apple_Li
    /// </summary>
    public class MySqlCommandTreeFactory : CommandTreeFactory
    {
        /// <summary>
        /// 创建生成MySql的命令生成树的工厂
        /// </summary>
        public MySqlCommandTreeFactory()
            : base('@') { }

        /// <summary>
        /// 创建读取节点信息的Helper
        /// </summary>
        /// <returns>读取节点信息的Helper</returns>
        protected override ColumnNodeHelper CreateColumnNodeHelper()
        {
            return new MySqlColumnNodeHelper();
        }
        /// <summary>
        /// 创建Update命令生成树
        /// </summary>
        /// <param name="tableFullName">完整表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <returns>Update命令生成树</returns>
        protected override UpdateTree CreateUpdateTree(string tableFullName, string tableAlias)
        {
            return new MySqlUpdateTree(tableFullName, tableAlias, base.ParameterMarker);
        }
        /// <summary>
        /// 创建分页查询命令生成树
        /// </summary>
        /// <param name="queryChildBuilders">分页查询命令生成树的子节点集</param>
        /// <param name="orderByColumn">排序的列</param>
        /// <param name="isAsc">True:升序(False为降序)</param>
        /// <returns>分页查询命令生成树</returns>
        public override ICommandTree CreatePagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, string orderByColumn, bool isAsc = true)
        {
            //创建MySql分页查询命令生成树
            MySqlPagingQueryTree queryTree = new MySqlPagingQueryTree(base.ParameterMarker);
            //填充MySql分页查询命令生成树的各个节点
            base.LoadQueryTree(queryTree, queryChildBuilders);
            queryTree.OrderBy.Append(new SqlBuilder("{0} {1}", orderByColumn, isAsc ? "ASC" : "DESC"));
            //返回MySql分页查询命令生成树
            return queryTree;
        }
    }
}