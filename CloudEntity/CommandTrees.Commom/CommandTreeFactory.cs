using System.Collections.Generic;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// 创建CommandTree的工厂
    /// 李凯 Apple_Li
    /// </summary>
    public abstract class CommandTreeFactory : ICommandTreeFactory
    {
        private ColumnNodeHelper columnNodeHelper;  //获取列节点信息的Helper

        /// <summary>
        /// 获取列节点信息的Helper
        /// </summary>
        private ColumnNodeHelper ColumnNodeHelper
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
        /// 创建Delete命令生成树
        /// </summary>
        /// <param name="tableFullName">完整表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <returns>Delete命令生成树</returns>
        protected virtual DeleteTree CreateDeleteTree(string tableFullName, string tableAlias)
        {
            return new DeleteTree(tableFullName, tableAlias, this.ParameterMarker);
        }
        /// <summary>
        /// 创建Update命令生成树
        /// </summary>
        /// <param name="tableFullName">完整表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <returns>Update命令生成树</returns>
        protected virtual UpdateTree CreateUpdateTree(string tableFullName, string tableAlias)
        {
            return new UpdateTree(tableFullName, tableAlias, this.ParameterMarker);
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
        /// 创建建表语句生成树
        /// </summary>
        /// <param name="tableFullName">table全名</param>
        /// <param name="columnNodes">列节点迭代器</param>
        /// <returns>建表语句生成树</returns>
        public ICommandTree CreateBuildTableTree(string tableFullName, IEnumerable<IColumnNode> columnNodes)
        {
            //创建建表语句生成树
            BuildTableTree buildTableTree = new BuildTableTree(tableFullName, this.ColumnNodeHelper);
            //为建表语句生成树添加列节点
            foreach (IColumnNode columnNode in columnNodes)
                buildTableTree.Add(columnNode);
            //返回建表语句生成树
            return buildTableTree;
        }
        /// <summary>
        /// 创建为Table添加列的语句生成树
        /// </summary>
        /// <param name="tableFullName">table全名</param>
        /// <param name="columnNodes">列节点迭代器</param>
        /// <returns>为Table添加列的语句生成树</returns>
        public ICommandTree CreateAlterTableAddColumnsTree(string tableFullName, IEnumerable<IColumnNode> columnNodes)
        {
            //创建为Table添加列的语句生成树
            AlterTableAddColumnsTree alterTableAddColumnsTree = new AlterTableAddColumnsTree(tableFullName, this.ColumnNodeHelper);
            //为命令生成树添加列节点
            foreach (IColumnNode columnNode in columnNodes)
                alterTableAddColumnsTree.Add(columnNode);
            //返回为Table添加列的语句生成树
            return alterTableAddColumnsTree;
        }
        /// <summary>
        /// 创建Insert命令生成树
        /// </summary>
        /// <param name="tableFullName">完整表名</param>
        /// <param name="insertNodes">Insert命令生成树子节点</param>
        /// <returns>Insert命令生成树</returns>
        public ICommandTree CreateInsertTree(string tableFullName, IEnumerable<KeyValuePair<string, string>> insertNodes)
        {
            //创建Insert命令生成树
            InsertTree insertTree = new InsertTree(tableFullName, this.ParameterMarker);
            //为Insert命令生成树添加节点
            foreach (KeyValuePair<string, string> insertNodePair in insertNodes)
                insertTree.Append(insertNodePair.Key, insertNodePair.Value);
            //返回Insert命令生成树
            return insertTree;
        }
        /// <summary>
        /// 创建Delete命令生成树
        /// </summary>
        /// <param name="tableFullName">完整的表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="whereChildBuilders">Where语句段子节点集合</param>
        /// <returns>Delete命令生成树</returns>
        public ICommandTree CreateDeleteTree(string tableFullName, string tableAlias, IEnumerable<ISqlBuilder> whereChildBuilders)
        {
            //创建Delete命令生成树
            DeleteTree deleteTree = this.CreateDeleteTree(tableFullName, tableAlias);
            //为Delete命令生成树的Where节点添加子节点
            foreach (ISqlBuilder sqlBuilder in whereChildBuilders)
                deleteTree.Where.Append(sqlBuilder);
            //返回Delete命令生成树
            return deleteTree;
        }
        /// <summary>
        /// 创建Update命令生成树
        /// </summary>
        /// <param name="tableFullName">完整表名</param>
        /// <param name="tableAlias">临时表名</param>
        /// <param name="updateChildBuilders">Update命令生成树的子节点集合</param>
        /// <returns>Update命令生成树</returns>
        public ICommandTree CreateUpdateTree(string tableFullName, string tableAlias, IEnumerable<INodeBuilder> updateChildBuilders)
        {
            //创建Update命令生成树
            UpdateTree updateTree = this.CreateUpdateTree(tableFullName, tableAlias);
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
        /// 创建查询命令生成树
        /// Create sql query tree.
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点</param>
        /// <returns>sql查询命令生成树</returns>
        public ICommandTree CreateQueryTree(IEnumerable<INodeBuilder> queryChildBuilders)
        {
            //创建查询命令生成树
            QueryTree queryTree = new QueryTree(this.ParameterMarker);
            //填充查询命令生成树各个节点
            this.LoadQueryTree(queryTree, queryChildBuilders);
            //返回查询命令生成树
            return queryTree;
        }
        /// <summary>
        /// 创建TOP查询sql的生成树
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP查询sql的生成树</returns>
        public ICommandTree CreateTopQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, int topCount)
        {
            //创建top查询命令生成树
            QueryTree queryTree = new TopQueryTree(this.ParameterMarker, topCount);
            //填充查询命令生成树各个节点
            this.LoadQueryTree(queryTree, queryChildBuilders);
            //返回查询命令生成树
            return queryTree;
        }
        /// <summary>
        /// 创建生成Distinct查询sql的生成树
        /// </summary>
        /// <param name="queryChildBuilders">查询命令生成树的子节点集合</param>
        /// <returns>Distinct查询sql的生成树</returns>
        public ICommandTree CreateDistinctQueryTree(IEnumerable<INodeBuilder> queryChildBuilders)
        {
            //创建查询命令生成树
            QueryTree queryTree = new DistinctQueryTree(this.ParameterMarker);
            //填充查询命令生成树各个节点
            this.LoadQueryTree(queryTree, queryChildBuilders);
            //返回查询命令生成树
            return queryTree;
        }
        /// <summary>
        /// 创建分页查询命令生成树
        /// </summary>
        /// <param name="queryChildBuilders">分页查询命令生成树的子节点集</param>
        /// <param name="orderByColumn">排序的列</param>
        /// <param name="isAsc">True:升序(False为降序)</param>
        /// <returns>分页查询命令生成树</returns>
        public abstract ICommandTree CreatePagingQueryTree(IEnumerable<INodeBuilder> queryChildBuilders, string orderByColumn, bool isAsc = true);
        /// <summary>
        /// 创建With As 查询命令生成树
        /// </summary>
        /// <param name="innerQuerySql">查询sql</param>
        /// <param name="queryChildBuilders">查询条件表达式节点集合</param>
        /// <returns>With As 查询命令生成树</returns>
        public ICommandTree CreateWithAsQueryTree(string innerQuerySql, IEnumerable<INodeBuilder> queryChildBuilders)
        {
            //创建with as查询命令生成树
            WithAsQueryTree withAsQueryTree = new WithAsQueryTree(this.ParameterMarker, innerQuerySql);
            //加载查询条件节点
            foreach (INodeBuilder nodeBuilder in queryChildBuilders)
            {
                switch (nodeBuilder.ParentNodeType)
                {
                    case SqlType.Where:
                        withAsQueryTree.Where.Append(nodeBuilder);
                        break;
                    case SqlType.OrderBy:
                        withAsQueryTree.OrderBy.Append(nodeBuilder);
                        break;
                }
            }
            //返回with as查询命令生成树
            return withAsQueryTree;
        }
    }
}