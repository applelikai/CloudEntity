using CloudEntity.CommandTrees;
using System.Collections.Generic;

namespace CloudEntity.Internal.CommandTreeGetters
{
    /// <summary>
    /// 排序分页查询命令生成树获取器
    /// 李凯 Apple_Li
    /// </summary>
    internal class PagingOrderByQueryTreeGetter : CommandTreeGetter
    {
        private string orderByColumn;   //排序的列
        private bool isAsc;             //升序 还是 降序

        /// <summary>
        /// 创建分页查询命令生成树获取器
        /// </summary>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="orderByColumn">排序的列</param>
        /// <param name="isAsc">升序 还是 降序</param>
        public PagingOrderByQueryTreeGetter(ICommandTreeFactory commandTreeFactory, string orderByColumn, bool isAsc)
            : base(commandTreeFactory)
        {
            this.orderByColumn = orderByColumn;
            this.isAsc = isAsc;
        }
        /// <summary>
        /// 获取分页查询命令生成树
        /// </summary>
        /// <param name="nodeBuilders">查询命令生成树的子节点集</param>
        /// <returns>分页查询命令生成树</returns>
        public override ICommandTree Get(IEnumerable<INodeBuilder> nodeBuilders)
        {
            return base.CommandTreeFactory.CreatePagingQueryTree(nodeBuilders, this.orderByColumn, this.isAsc);
        }
    }
}
