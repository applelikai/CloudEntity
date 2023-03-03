using System.Collections.Generic;

namespace CloudEntity.CommandTrees
{
    /// <summary>
    /// 命令工厂接口
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    public interface ICommandFactory
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
        /// 获取为Table添加列的语句生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="columnNodes">列节点迭代器</param>
        /// <returns>为Table添加列的语句生成树</returns>
        ICommandTree GetAlterTableAddColumnsTree(string schemaName, string tableName, IEnumerable<IColumnNode> columnNodes);
        /// <summary>
        /// 获取Insert命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="insertNodes">Insert命令生成树子节点</param>
        /// <returns>Insert命令生成树</returns>
        ICommandTree GetInsertTree(string schemaName, string tableName, IEnumerable<KeyValuePair<string, string>> insertNodes);
        /// <summary>
        /// 获取Delete命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="whereChildBuilders">Where语句段子节点集合</param>
        /// <returns>Delete命令生成树</returns>
        ICommandTree GetDeleteTree(string schemaName, string tableName, string tableAlias, IEnumerable<ISqlBuilder> whereChildBuilders);
        /// <summary>
        /// 获取Update命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="updateChildBuilders">Update命令生成树的子节点集合</param>
        /// <returns>Update命令生成树</returns>
        ICommandTree GetUpdateTree(string schemaName, string tableName, string tableAlias, IEnumerable<INodeBuilder> updateChildBuilders);
        /// <summary>
        /// 获取查询sql生成器
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <returns>sql查询命令生成树</returns>
        ISelectCommandTree GetQueryTree(IEnumerable<INodeBuilder> queryChildBuilders);
        /// <summary>
        /// 获取TOP查询sql的生成树
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP查询sql的生成树</returns>
        ISelectCommandTree GetTopQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, int topCount);
        /// <summary>
        /// 获取生成Distinct查询sql的生成树
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <returns>Distinct查询sql的生成树</returns>
        ISelectCommandTree GetDistinctQueryTree(IEnumerable<INodeBuilder> queryChildBuilders);
        /// <summary>
        /// 获取分页查询命令生成树
        /// </summary>
        /// <param name="queryChildBuilders">分页查询命令生成树的子节点集</param>
        /// <returns>分页查询命令生成树</returns>
        ISelectCommandTree GetPagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders);
        /// <summary>
        /// 获取With As 查询命令生成树
        /// </summary>
        /// <param name="innerQuerySql">查询sql</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="queryChildBuilders">查询条件表达式节点集合</param>
        /// <returns>With As 查询命令生成树</returns>
        ICommandTree GetWithAsQueryTree(string innerQuerySql, string tableAlias, IEnumerable<INodeBuilder> queryChildBuilders);
        #endregion
        #region 获取SQL命令
        /// <summary>
        /// 获取建表的SQL语句
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="columnNodes">列节点列表</param>
        /// <returns>建表的SQL语句</returns>
        string GetCreateTableCommandText(string schemaName, string tableName, IEnumerable<IColumnNode> columnNodes);
        /// <summary>
        /// 获取删除表的SQL命令
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">完整表名</param>
        /// <returns>删除表的SQL命令</returns>
        string GetDropTableCommandText(string schemaName, string tableName);
        /// <summary>
        /// 获取重命名Table的SQL命令
        /// </summary>
        /// <param name="schemaName">数据库架构名（或用户名 或模式）</param>
        /// <param name="tableName">表名</param>
        /// <param name="oldTableName">原来的Table名</param>
        /// <returns>重命名Table的SQL命令</returns>
        string GetRenameTableCommandText(string schemaName, string tableName, string oldTableName);
        #endregion
    }
}
