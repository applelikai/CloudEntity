using System.Collections.Generic;

namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// 用于创建Command Tree的Factory
    /// </summary>
    public interface ICommandTreeFactory
    {
        /// <summary>
        /// sql 参数标识符
        /// </summary>
        char ParameterMarker { get; }

        /// <summary>
        /// 创建建表语句生成树
        /// </summary>
        /// <param name="tableFullName">table全名</param>
        /// <param name="columnNodes">列节点迭代器</param>
        /// <returns>建表语句生成树</returns>
        ICommandTree CreateBuildTableTree(string tableFullName, IEnumerable<IColumnNode> columnNodes);
        /// <summary>
        /// 创建为Table添加列的语句生成树
        /// </summary>
        /// <param name="tableFullName">table全名</param>
        /// <param name="columnNodes">列节点迭代器</param>
        /// <returns>为Table添加列的语句生成树</returns>
        ICommandTree CreateAlterTableAddColumnsTree(string tableFullName, IEnumerable<IColumnNode> columnNodes);
        /// <summary>
        /// 创建Insert命令生成树
        /// </summary>
        /// <param name="tableFullName">完整表名</param>
        /// <param name="insertNodes">Insert命令生成树子节点</param>
        /// <returns>Insert命令生成树</returns>
        ICommandTree CreateInsertTree(string tableFullName, IEnumerable<KeyValuePair<string, string>> insertNodes);
        /// <summary>
        /// 创建Delete命令生成树
        /// </summary>
        /// <param name="tableFullName">完整的表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="whereChildBuilders">Where语句段子节点集合</param>
        /// <returns>Delete命令生成树</returns>
        ICommandTree CreateDeleteTree(string tableFullName, string tableAlias, IEnumerable<ISqlBuilder> whereChildBuilders);
        /// <summary>
        /// 创建Update命令生成树
        /// </summary>
        /// <param name="tableFullName">完整表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="updateChildBuilders">Update命令生成树的子节点集合</param>
        /// <returns>Update命令生成树</returns>
        ICommandTree CreateUpdateTree(string tableFullName, string tableAlias, IEnumerable<INodeBuilder> updateChildBuilders);
        /// <summary>
        /// 创建查询sql生成器
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <returns>sql查询命令生成树</returns>
        ICommandTree CreateQueryTree(IEnumerable<INodeBuilder> queryChildBuilders);
        /// <summary>
        /// 创建生成Distinct查询sql的生成树
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <returns>Distinct查询sql的生成树</returns>
        ICommandTree CreateDistinctQueryTree(IEnumerable<INodeBuilder> queryChildBuilders);
        /// <summary>
        /// 创建分页查询命令生成树
        /// </summary>
        /// <param name="queryChildBuilders">分页查询命令生成树的子节点集</param>
        /// <param name="orderByColumn">排序的列</param>
        /// <param name="isAsc">True:升序(False为降序)</param>
        /// <returns>分页查询命令生成树</returns>
        ICommandTree CreatePagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, string orderByColumn, bool isAsc = true);
        /// <summary>
        /// 创建With As 查询命令生成树
        /// </summary>
        /// <param name="innerQuerySql">查询sql</param>
        /// <param name="queryChildBuilders">查询条件表达式节点集合</param>
        /// <returns>With As 查询命令生成树</returns>
        ICommandTree CreateWithAsQueryTree(string innerQuerySql, IEnumerable<INodeBuilder> queryChildBuilders);
    }
}
