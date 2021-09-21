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

        #region 获取sql表达式节点
        /// <summary>
        /// 获取sql参数表达式
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>sql参数表达式</returns>
        ISqlBuilder GetParameterBuilder(string parameterName);
        /// <summary>
        /// 获取基础Column节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <returns>基础Column节点</returns>
        ISqlBuilder GetColumnBuilder(string tableAlias, string columnName);
        /// <summary>
        /// 获取sql column节点生成类
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="columnAlias">列别名</param>
        /// <returns>sql column节点生成类</returns>
        INodeBuilder GetColumnBuilder(string tableAlias, string columnName, string columnAlias);
        /// <summary>
        /// 获取Sql函数表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="functionName">sql函数名</param>
        /// <returns>Sql函数表达式节点</returns>
        INodeBuilder GetFunctionNodeBuilder(string tableAlias, string columnName, string functionName);
        /// <summary>
        /// 获取UPDATE SET节点的子sql表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="parameterName">参数名</param>
        /// <returns>UPDATE SET节点的子sql表达式节点</returns>
        INodeBuilder GetUpdateSetChildBuilder(string tableAlias, string columnName, string parameterName);
        /// <summary>
        /// 获取Table表达式节点
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">表别名</param>
        /// <param name="schemaName">数据库架构名</param>
        /// <returns>Table表达式节点</returns>
        INodeBuilder GetTableBuilder(string tableName, string tableAlias, string schemaName);
        /// <summary>
        /// 获取Where节点的子表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="rightSqlExpression">右边的sql条件表达式</param>
        /// <returns>Where节点的子表达式节点</returns>
        INodeBuilder GetWhereChildBuilder(string tableAlias, string columnName, string rightSqlExpression);
        /// <summary>
        /// 获取等于条件表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="parameterName">参数名</param>
        /// <returns>等于条件表达式节点</returns>
        INodeBuilder GetEqualsBuilder(string tableAlias, string columnName, string parameterName);
        /// <summary>
        /// 获取OrderBy节点的子节点表达式
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="isDesc">是否为降序[true:降序 false:升序]</param>
        /// <returns>OrderBy节点的子节点表达式</returns>
        INodeBuilder GetOrderByChildBuilder(string tableAlias, string columnName, bool isDesc);
        #endregion
        #region 获取sql命令生成树
        /// <summary>
        /// 创建建表语句生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">table名</param>
        /// <param name="columnNodes">列节点迭代器</param>
        /// <returns>建表语句生成树</returns>
        ICommandTree CreateBuildTableTree(string schemaName, string tableName, IEnumerable<IColumnNode> columnNodes);
        /// <summary>
        /// 创建为Table添加列的语句生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="columnNodes">列节点迭代器</param>
        /// <returns>为Table添加列的语句生成树</returns>
        ICommandTree CreateAlterTableAddColumnsTree(string schemaName, string tableName, IEnumerable<IColumnNode> columnNodes);
        /// <summary>
        /// 创建Insert命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="insertNodes">Insert命令生成树子节点</param>
        /// <returns>Insert命令生成树</returns>
        ICommandTree CreateInsertTree(string schemaName, string tableName, IEnumerable<KeyValuePair<string, string>> insertNodes);
        /// <summary>
        /// 创建Delete命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="whereChildBuilders">Where语句段子节点集合</param>
        /// <returns>Delete命令生成树</returns>
        ICommandTree CreateDeleteTree(string schemaName, string tableName, string tableAlias, IEnumerable<ISqlBuilder> whereChildBuilders);
        /// <summary>
        /// 创建Update命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="updateChildBuilders">Update命令生成树的子节点集合</param>
        /// <returns>Update命令生成树</returns>
        ICommandTree CreateUpdateTree(string schemaName, string tableName, string tableAlias, IEnumerable<INodeBuilder> updateChildBuilders);
        /// <summary>
        /// 创建查询sql生成器
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <returns>sql查询命令生成树</returns>
        ICommandTree CreateQueryTree(IEnumerable<INodeBuilder> queryChildBuilders);
        /// <summary>
        /// 创建TOP查询sql的生成树
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP查询sql的生成树</returns>
        ICommandTree CreateTopQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, int topCount);
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
        /// <param name="sortChildBuilders">排序的子节点集</param>
        /// <returns>分页查询命令生成树</returns>
        ICommandTree CreatePagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, IEnumerable<ISqlBuilder> sortChildBuilders);
        /// <summary>
        /// 创建With As 查询命令生成树
        /// </summary>
        /// <param name="innerQuerySql">查询sql</param>
        /// <param name="queryChildBuilders">查询条件表达式节点集合</param>
        /// <returns>With As 查询命令生成树</returns>
        ICommandTree CreateWithAsQueryTree(string innerQuerySql, IEnumerable<INodeBuilder> queryChildBuilders);
        #endregion
    }
}
