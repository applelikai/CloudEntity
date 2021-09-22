using System.Collections.Generic;

namespace CloudEntity.CommandTrees.Commom.PostgreSqlClient
{
    /// <summary>
    /// 创建用于PostgreSql的CommandTree的工厂
    /// 李凯 Apple_Li 2021/09/19
    /// </summary>
    public class PostgreSqlCommandTreeFactory : CommandTreeFactory
    {
        /// <summary>
        /// 创建读取节点信息的Helper
        /// </summary>
        /// <returns>读取节点信息的Helper</returns>
        protected override ColumnNodeHelper CreateColumnNodeHelper()
        {
            return new PostgreSqlColumnNodeHelper();
        }
        /// <summary>
        /// 创建建表语句生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <returns>建表语句生成树</returns>
        protected override BuildTableTree CreateBuildTableTree(string schemaName, string tableName)
        {
            return new PostgreSqlBuildTableTree(base.ColumnNodeHelper, tableName, schemaName);
        }
        /// <summary>
        /// 创建为Table添加列的语句生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <returns>为Table添加列的语句生成树</returns>
        protected override AlterTableAddColumnsTree CreateAlterTableAddColumnsTree(string schemaName, string tableName)
        {
            return new PostgreSqlAlterTableAddColumnsTree(base.ColumnNodeHelper, tableName, schemaName);
        }
        /// <summary>
        /// 创建Insert命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <returns>Insert命令生成树</returns>
        protected override InsertTree CreateInsertTree(string schemaName, string tableName)
        {
            return new PostgreSqlInsertTree(schemaName, tableName, base.ParameterMarker); ;
        }
        /// <summary>
        /// 创建Update命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <returns>Update命令生成树</returns>
        protected override UpdateTree CreateUpdateTree(string schemaName, string tableName, string tableAlias)
        {
            return new PostgreSqlUpdateTree(schemaName, tableName, tableAlias, base.ParameterMarker);
        }
        /// <summary>
        /// 创建Delete命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <returns>Delete命令生成树</returns>
        protected override DeleteTree CreateDeleteTree(string schemaName, string tableName, string tableAlias)
        {
            return new PostgreSqlDeleteTree(schemaName, tableName, tableAlias, base.ParameterMarker);
        }

        /// <summary>
        /// 创建生成MySql的命令生成树的工厂
        /// </summary>
        public PostgreSqlCommandTreeFactory()
            : base('@') { }
        /// <summary>
        /// 获取基础Column节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <returns>基础Column节点</returns>
        public override ISqlBuilder GetColumnBuilder(string tableAlias, string columnName)
        {
            return new SqlBuilder("{0}.\"{1}\"", tableAlias, columnName);
        }
        /// <summary>
        /// 获取sql column节点生成类
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="columnAlias">列别名</param>
        /// <returns>sql column节点生成类</returns>
        public override INodeBuilder GetColumnBuilder(string tableAlias, string columnName, string columnAlias)
        {
            //若列的别名为空，则不使用别名
            if (string.IsNullOrEmpty(columnAlias))
                return new ColumnBuilder(columnName, $"{tableAlias}.\"{columnName}\"");
            //若列的别名不为空, 则使用别名
            return new ColumnBuilder(columnAlias, $"{tableAlias}.\"{columnName}\" \"{columnAlias}\"");
        }
        /// <summary>
        /// 获取Sql函数表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="functionName">sql函数名</param>
        /// <returns>Sql函数表达式节点</returns>
        public override INodeBuilder GetFunctionNodeBuilder(string tableAlias, string columnName, string functionName)
        {
            return new NodeBuilder(SqlType.Select, "{0}({1}.\"{2}\")", functionName, tableAlias, columnName);
        }
        /// <summary>
        /// 获取UPDATE SET节点的子sql表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="parameterName">参数名</param>
        /// <returns>UPDATE SET节点的子sql表达式节点</returns>
        public override INodeBuilder GetUpdateSetChildBuilder(string tableAlias, string columnName, string parameterName)
        {
            return new NodeBuilder(SqlType.UpdateSet, $"\"{columnName}\" = {this.ParameterMarker}{parameterName}");
        }
        /// <summary>
        /// 获取Table表达式节点
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">表别名</param>
        /// <param name="schemaName">数据库架构名</param>
        /// <returns>Table表达式节点</returns>
        public override INodeBuilder GetTableBuilder(string tableName, string tableAlias, string schemaName)
        {
            //若架构名为空，则获取空架构名的Table节点
            if (string.IsNullOrEmpty(schemaName))
                return new TableBuilder(tableAlias, $"\"{tableName}\" {tableAlias}");
            //若架构名不为空，则获取有架构名的Table节点
            return new TableBuilder(tableAlias, $"{schemaName}.\"{tableName}\" {tableAlias}");
        }
        /// <summary>
        /// 获取Where节点的子表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="rightSqlExpression">右边的sql条件表达式</param>
        /// <returns>Where节点的子表达式节点</returns>
        public override INodeBuilder GetWhereChildBuilder(string tableAlias, string columnName, string rightSqlExpression)
        {
            return new NodeBuilder(SqlType.Where, $"{tableAlias}.\"{columnName}\" {rightSqlExpression}");
        }
        /// <summary>
        /// 获取等于条件表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="parameterName">参数名</param>
        /// <returns>等于条件表达式节点</returns>
        public override INodeBuilder GetEqualsBuilder(string tableAlias, string columnName, string parameterName)
        {
            return new NodeBuilder(SqlType.Where, $"{tableAlias}.\"{columnName}\" = {this.ParameterMarker}{parameterName}");
        }
        /// <summary>
        /// 获取OrderBy节点的子节点表达式
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="isDesc">是否为降序[true:降序 false:升序]</param>
        /// <returns>OrderBy节点的子节点表达式</returns>
        public override INodeBuilder GetOrderByChildBuilder(string tableAlias, string columnName, bool isDesc)
        {
            return new NodeBuilder(SqlType.OrderBy, "{0}.\"{1}\"{2}", tableAlias, columnName, isDesc ? " DESC" : string.Empty);
        }
        /// <summary>
        /// 获取TOP查询sql的生成树
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP查询sql的生成树</returns>
        public override ICommandTree GetTopQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, int topCount)
        {
            //创建top查询命令生成树
            QueryTree queryTree = new PostgreSqlTopQueryTree(this.ParameterMarker, topCount);
            //填充查询命令生成树各个节点
            this.LoadQueryTree(queryTree, queryChildBuilders);
            //返回查询命令生成树
            return queryTree;
        }
        /// <summary>
        /// 获取分页查询命令生成树
        /// </summary>
        /// <param name="queryChildBuilders">分页查询命令生成树的子节点集</param>
        /// <param name="sortChildBuilders">排序的子节点集</param>
        /// <returns>分页查询命令生成树</returns>
        public override ICommandTree GetPagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, IEnumerable<ISqlBuilder> sortChildBuilders)
        {
            //创建MySql分页查询命令生成树
            PostgreSqlPagingQueryTree queryTree = new PostgreSqlPagingQueryTree(base.ParameterMarker);
            //填充MySql分页查询命令生成树的各个节点
            base.LoadQueryTree(queryTree, queryChildBuilders);
            //填充OrderBy节点
            foreach (ISqlBuilder nodeBuilder in sortChildBuilders)
                queryTree.OrderBy.Append(nodeBuilder);
            //返回MySql分页查询命令生成树
            return queryTree;
        }
    }
}
