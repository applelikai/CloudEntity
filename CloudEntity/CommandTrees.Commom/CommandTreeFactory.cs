using System.Collections.Generic;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 创建CommandTree的工厂
    /// Apple_Li 李凯 15150598493
    /// 最后修改日期：2023/02/05
    /// </summary>
    public abstract class CommandTreeFactory : ICommandTreeFactory
    {
        /// <summary>
        /// 获取列节点信息的Helper
        /// </summary>
        private ColumnNodeHelper columnNodeHelper;

        /// <summary>
        /// 获取列节点信息的Helper
        /// </summary>
        protected ColumnNodeHelper ColumnNodeHelper
        {
            get { return this.columnNodeHelper ?? (this.columnNodeHelper = this.CreateColumnNodeHelper()); }
        }

        /// <summary>
        /// sql参数标识符
        /// </summary>
        public char ParameterMarker { get; private set; }

        /// <summary>
        /// 创建sql驱动器
        /// </summary>
        /// <param name="marker">sql参数标识符</param>
        protected CommandTreeFactory(char marker)
        {
            this.ParameterMarker = marker;
        }
        /// <summary>
        /// 创建读取节点信息的Helper
        /// </summary>
        /// <returns>读取节点信息的Helper</returns>
        protected abstract ColumnNodeHelper CreateColumnNodeHelper();
        /// <summary>
        /// 创建建表语句生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <returns>建表语句生成树</returns>
        protected virtual BuildTableTree CreateBuildTableTree(string schemaName, string tableName)
        {
            return new BuildTableTree(this.ColumnNodeHelper, tableName, schemaName);
        }
        /// <summary>
        /// 创建为Table添加列的语句生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <returns>为Table添加列的语句生成树</returns>
        protected virtual AlterTableAddColumnsTree CreateAlterTableAddColumnsTree(string schemaName, string tableName)
        {
            return new AlterTableAddColumnsTree(this.ColumnNodeHelper, tableName, schemaName);
        }
        /// <summary>
        /// 创建Insert命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <returns>Insert命令生成树</returns>
        protected virtual InsertTree CreateInsertTree(string schemaName, string tableName)
        {
            return new InsertTree(schemaName, tableName, this.ParameterMarker);
        }
        /// <summary>
        /// 创建Delete命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <returns>Delete命令生成树</returns>
        protected virtual DeleteTree CreateDeleteTree(string schemaName, string tableName, string tableAlias)
        {
            return new DeleteTree(schemaName, tableName, tableAlias, this.ParameterMarker);
        }
        /// <summary>
        /// 创建Update命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <returns>Update命令生成树</returns>
        protected virtual UpdateTree CreateUpdateTree(string schemaName, string tableName, string tableAlias)
        {
            return new UpdateTree(schemaName, tableName, tableAlias, this.ParameterMarker);
        }
        /// <summary>
        /// 创建TOP查询命令的生成树
        /// </summary>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP查询命令的生成树</returns>
        protected virtual QueryTree CreateTopQueryTree(int topCount)
        {
            return new TopQueryTree(this.ParameterMarker, topCount);
        }
        /// <summary>
        /// 创建With As 查询命令生成树
        /// </summary>
        /// <param name="innerQuerySql">查询sql</param>
        /// <param name="tableAlias">临时表名</param>
        /// <returns>With As 查询命令生成树</returns>
        protected virtual WithAsQueryTree CreateWithAsQueryTree(string innerQuerySql, string tableAlias)
        {
            return new WithAsQueryTree(this.ParameterMarker, innerQuerySql, tableAlias);
        }
        /// <summary>
        /// 加载查询命令生成树
        /// </summary>
        /// <param name="queryTree">查询命令生成树</param>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集</param>
        protected void LoadQueryTree(QueryTree queryTree, IEnumerable<INodeBuilder> queryChildBuilders)
        {
            foreach (INodeBuilder builder in queryChildBuilders)
            {
                switch (builder.ParentNodeType)
                {
                    case SqlType.Select:
                        queryTree.Select.Append(builder);
                        break;
                    case SqlType.From:
                        queryTree.From.Append(builder);
                        break;
                    case SqlType.Where:
                        queryTree.Where.Append(builder);
                        break;
                    case SqlType.GroupBy:
                        queryTree.GroupBy.Append(builder);
                        break;
                    case SqlType.OrderBy:
                        queryTree.OrderBy.Append(builder);
                        break;
                }
            }
        }
        /// <summary>
        /// 加载With As查询命令生成树
        /// </summary>
        /// <param name="queryTree">With As查询命令生成树</param>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集</param>
        protected void LoadQueryTree(WithAsQueryTree queryTree, IEnumerable<INodeBuilder> queryChildBuilders)
        {
            //加载查询条件节点
            foreach (INodeBuilder nodeBuilder in queryChildBuilders)
            {
                switch (nodeBuilder.ParentNodeType)
                {
                    case SqlType.Where:
                        queryTree.Where.Append(nodeBuilder);
                        break;
                    case SqlType.OrderBy:
                        queryTree.OrderBy.Append(nodeBuilder);
                        break;
                }
            }
        }
        #region 获取sql表达式节点
        /// <summary>
        /// 获取sql参数表达式
        /// </summary>
        /// <param name="parameterName">参数名称</param>
        /// <returns>sql参数表达式</returns>
        public ISqlBuilder GetParameterBuilder(string parameterName)
        {
            return new SqlBuilder("{0}{1}", this.ParameterMarker, parameterName);
        }
        /// <summary>
        /// 获取基础Column节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <returns>基础Column节点</returns>
        public virtual ISqlBuilder GetColumnBuilder(string tableAlias, string columnName)
        {
            return new SqlBuilder("{0}.{1}", tableAlias, columnName);
        }
        /// <summary>
        /// 获取sql column节点生成类
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="columnAlias">列别名</param>
        /// <returns>sql column节点生成类</returns>
        public virtual INodeBuilder GetColumnBuilder(string tableAlias, string columnName, string columnAlias)
        {
            //若列的别名为空，则不使用别名
            if (string.IsNullOrEmpty(columnAlias))
                return new ColumnBuilder(columnName, $"{tableAlias}.{columnName}");
            //若列的别名不为空, 则使用别名
            return new ColumnBuilder(columnAlias, $"{tableAlias}.{columnName} {columnAlias}");
        }
        /// <summary>
        /// 获取Sql函数表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="functionName">sql函数名</param>
        /// <returns>Sql函数表达式节点</returns>
        public virtual INodeBuilder GetFunctionNodeBuilder(string tableAlias, string columnName, string functionName)
        {
            return new NodeBuilder(SqlType.Select, "{0}({1}.{2})", functionName, tableAlias, columnName);
        }
        /// <summary>
        /// 获取UPDATE SET节点的子sql表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="parameterName">参数名</param>
        /// <returns>UPDATE SET节点的子sql表达式节点</returns>
        public virtual INodeBuilder GetUpdateSetChildBuilder(string tableAlias, string columnName, string parameterName)
        {
            return new NodeBuilder(SqlType.UpdateSet, $"{tableAlias}.{columnName} = {this.ParameterMarker}{parameterName}");
        }
        /// <summary>
        /// 获取Table表达式节点
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">表别名</param>
        /// <param name="schemaName">数据库架构名</param>
        /// <returns>Table表达式节点</returns>
        public virtual INodeBuilder GetTableBuilder(string tableName, string tableAlias, string schemaName)
        {
            //若架构名为空，则获取空架构名的Table节点
            if (string.IsNullOrEmpty(schemaName))
                return new TableBuilder(tableAlias, $"{tableName} {tableAlias}");
            //若架构名不为空，则获取有架构名的Table节点
            return new TableBuilder(tableAlias, $"{schemaName}.{tableName} {tableAlias}");
        }
        /// <summary>
        /// 获取Where节点的子表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="rightSqlExpression">右边的sql条件表达式</param>
        /// <returns>Where节点的子表达式节点</returns>
        public virtual INodeBuilder GetWhereChildBuilder(string tableAlias, string columnName, string rightSqlExpression)
        {
            return new NodeBuilder(SqlType.Where, $"{tableAlias}.{columnName} {rightSqlExpression}");
        }
        /// <summary>
        /// 获取等于条件表达式节点
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="parameterName">参数名</param>
        /// <returns>等于条件表达式节点</returns>
        public virtual INodeBuilder GetEqualsBuilder(string tableAlias, string columnName, string parameterName)
        {
            return new NodeBuilder(SqlType.Where, $"{tableAlias}.{columnName} = {this.ParameterMarker}{parameterName}");
        }
        /// <summary>
        /// 获取OrderBy节点的子节点表达式
        /// </summary>
        /// <param name="tableAlias">表别名</param>
        /// <param name="columnName">列名</param>
        /// <param name="isDesc">是否为降序[true:降序 false:升序]</param>
        /// <returns>OrderBy节点的子节点表达式</returns>
        public virtual INodeBuilder GetOrderByChildBuilder(string tableAlias, string columnName, bool isDesc)
        {
            return new NodeBuilder(SqlType.OrderBy, "{0}.{1} {2}", tableAlias, columnName, isDesc ? "DESC" : string.Empty);
        }
        #endregion
        #region 获取sql命令生成树
        /// <summary>
        /// 获取建表语句生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="columnNodes">列节点迭代器</param>
        /// <returns>建表语句生成树</returns>
        public ICommandTree GetBuildTableTree(string schemaName, string tableName, IEnumerable<IColumnNode> columnNodes)
        {
            //创建建表语句生成树
            BuildTableTree buildTableTree = this.CreateBuildTableTree(schemaName, tableName);
            //为建表语句生成树添加列节点
            foreach (IColumnNode columnNode in columnNodes)
                buildTableTree.Add(columnNode);
            //返回建表语句生成树
            return buildTableTree;
        }
        /// <summary>
        /// 获取为Table添加列的语句生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="columnNodes">列节点迭代器</param>
        /// <returns>为Table添加列的语句生成树</returns>
        public ICommandTree GetAlterTableAddColumnsTree(string schemaName, string tableName, IEnumerable<IColumnNode> columnNodes)
        {
            //创建为Table添加列的语句生成树
            AlterTableAddColumnsTree alterTableAddColumnsTree = this.CreateAlterTableAddColumnsTree(schemaName, tableName);
            //为命令生成树添加列节点
            foreach (IColumnNode columnNode in columnNodes)
                alterTableAddColumnsTree.Add(columnNode);
            //返回为Table添加列的语句生成树
            return alterTableAddColumnsTree;
        }
        /// <summary>
        /// 获取Insert命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">完整表名</param>
        /// <param name="insertNodes">Insert命令生成树子节点</param>
        /// <returns>Insert命令生成树</returns>
        public ICommandTree GetInsertTree(string schemaName, string tableName, IEnumerable<KeyValuePair<string, string>> insertNodes)
        {
            //创建Insert命令生成树
            InsertTree insertTree = this.CreateInsertTree(schemaName, tableName);
            //为Insert命令生成树添加节点
            foreach (KeyValuePair<string, string> insertNodePair in insertNodes)
                insertTree.Append(insertNodePair.Key, insertNodePair.Value);
            //返回Insert命令生成树
            return insertTree;
        }
        /// <summary>
        /// 获取Delete命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="whereChildBuilders">Where语句段子节点集合</param>
        /// <returns>Delete命令生成树</returns>
        public ICommandTree GetDeleteTree(string schemaName, string tableName, string tableAlias, IEnumerable<ISqlBuilder> whereChildBuilders)
        {
            //创建Delete命令生成树
            DeleteTree deleteTree = this.CreateDeleteTree(schemaName, tableName, tableAlias);
            //为Delete命令生成树的Where节点添加子节点
            foreach (ISqlBuilder sqlBuilder in whereChildBuilders)
                deleteTree.Where.Append(sqlBuilder);
            //返回Delete命令生成树
            return deleteTree;
        }
        /// <summary>
        /// 获取Update命令生成树
        /// </summary>
        /// <param name="schemaName">数据库架构名</param>
        /// <param name="tableName">表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="updateChildBuilders">Update命令生成树的子节点集合</param>
        /// <returns>Update命令生成树</returns>
        public ICommandTree GetUpdateTree(string schemaName, string tableName, string tableAlias, IEnumerable<INodeBuilder> updateChildBuilders)
        {
            //创建Update命令生成树
            UpdateTree updateTree = this.CreateUpdateTree(schemaName, tableName, tableAlias);
            //为Update命令生成树添加子节点
            foreach (INodeBuilder nodeBuilder in updateChildBuilders)
            {
                switch (nodeBuilder.ParentNodeType)
                {
                    //为UpdateTree的Set节点添加子节点
                    case SqlType.UpdateSet:
                        updateTree.Set.Append(nodeBuilder);
                        break;
                    //为UpdateTree的Where节点添加子节点
                    case SqlType.Where:
                        updateTree.Where.Append(nodeBuilder);
                        break;
                }
            }
            //返回Update命令生成树
            return updateTree;
        }
        /// <summary>
        /// 获取查询命令生成树
        /// Create sql query tree.
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点</param>
        /// <returns>sql查询命令生成树</returns>
        public ICommandTree GetQueryTree(IEnumerable<INodeBuilder> queryChildBuilders)
        {
            //创建查询命令生成树
            QueryTree queryTree = new QueryTree(this.ParameterMarker);
            //填充查询命令生成树各个节点
            this.LoadQueryTree(queryTree, queryChildBuilders);
            //返回查询命令生成树
            return queryTree;
        }
        /// <summary>
        /// 获取TOP查询sql的生成树
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP查询sql的生成树</returns>
        public ICommandTree GetTopQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, int topCount)
        {
            //创建top查询命令生成树
            QueryTree queryTree = this.CreateTopQueryTree(topCount);
            //填充查询命令生成树各个节点
            this.LoadQueryTree(queryTree, queryChildBuilders);
            //返回查询命令生成树
            return queryTree;
        }
        /// <summary>
        /// 获取生成Distinct查询sql的生成树
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <returns>Distinct查询sql的生成树</returns>
        public ICommandTree GetDistinctQueryTree(IEnumerable<INodeBuilder> queryChildBuilders)
        {
            //创建查询命令生成树
            QueryTree queryTree = new DistinctQueryTree(this.ParameterMarker);
            //填充查询命令生成树各个节点
            this.LoadQueryTree(queryTree, queryChildBuilders);
            //返回查询命令生成树
            return queryTree;
        }
        /// <summary>
        /// 获取分页查询命令生成树
        /// </summary>
        /// <param name="queryChildBuilders">分页查询命令生成树的子节点集</param>
        /// <returns>分页查询命令生成树</returns>
        public abstract ICommandTree GetPagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders);
        /// <summary>
        /// 获取With As 查询命令生成树
        /// </summary>
        /// <param name="innerQuerySql">查询sql</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="queryChildBuilders">查询条件表达式节点集合</param>
        /// <returns>With As 查询命令生成树</returns>
        public ICommandTree GetWithAsQueryTree(string innerQuerySql, string tableAlias, IEnumerable<INodeBuilder> queryChildBuilders)
        {
            //创建with as查询命令生成树
            WithAsQueryTree withAsQueryTree = this.CreateWithAsQueryTree(innerQuerySql, tableAlias);
            //加载查询条件节点
            this.LoadQueryTree(withAsQueryTree, queryChildBuilders);
            //返回with as查询命令生成树
            return withAsQueryTree;
        }
        #endregion
    }
}