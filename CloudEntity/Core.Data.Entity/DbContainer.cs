using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Internal.Data.Entity;
using CloudEntity.Internal.WhereVisitors;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity.Core.Data.Entity
{
    /// <summary>
    /// 数据容器类
    /// 李凯 Apple_Li
    /// </summary>
    public sealed class DbContainer : IDbContainer, IDbFactory
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private object _locker;
        /// <summary>
        /// 访问DbList字典的线程锁
        /// </summary>
        private object _dbListsLocker;
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string _connectionString;
        /// <summary>
        /// 操作数据库的DbHelper
        /// </summary>
        private DbHelper _dbHelper;
        /// <summary>
        /// Table初始化器
        /// </summary>
        private TableInitializer _tableInitializer;
        /// <summary>
        /// 列初始化器
        /// </summary>
        private ColumnInitializer _columnInitializer;
        /// <summary>
        /// 创建Sql命令生成树的工厂
        /// </summary>
        private ICommandTreeFactory _commandTreeFactory;
        /// <summary>
        /// Mapper容器
        /// </summary>
        private IMapperContainer _mapperContainer;
        /// <summary>
        /// 列获取器
        /// </summary>
        private IColumnGetter _columnGetter;
        /// <summary>
        /// mpping列获取器
        /// </summary>
        private IColumnGetter _mapperColumnGetter;
        /// <summary>
        /// 创建表达式解析器的工厂
        /// </summary>
        private IWhereVisitorFactory _whereVisitorFactory;
        /// <summary>
        /// 创建查询条件解析器的工厂
        /// </summary>
        private IWhereVisitorFactory _mapperWhereVisitorFactory;
        /// <summary>
        /// 数据列表字典
        /// </summary>
        private IDictionary<Type, object> _dbLists;
        /// <summary>
        /// 控制字典缓存的线程锁
        /// </summary>
        private static object _containersLocker;
        /// <summary>
        /// 容器集字典
        /// </summary>
        private static IDictionary<Type, ContainerSet> _containerSets;

        /// <summary>
        /// 操作数据库的DbHelper
        /// </summary>
        public DbHelper DbHelper
        {
            get { return this._dbHelper; }
        }
        
        /// <summary>
        /// 获取sql成员表达式节点
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>sql成员表达式节点</returns>
        private ISqlBuilder GetSqlBuilder(Expression expression)
        {
            IColumnMapper columnMapper = this._mapperContainer.GetColumnMapper(expression.GetProperty());
            return new SqlBuilder(columnMapper.ColumnFullName);
        }
        /// <summary>
        /// 获取sql表达式节点
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>sql表达式节点</returns>
        private INodeBuilder GetNodeBuilder(Expression expression)
        {
            switch (expression.NodeType)
            {
                //解析普通二叉树表达式节点
                case ExpressionType.GreaterThan:            //大于     >
                case ExpressionType.GreaterThanOrEqual:     //大于等于  >=
                case ExpressionType.LessThan:               //小于     <
                case ExpressionType.LessThanOrEqual:        //小于等于  <=
                case ExpressionType.Equal:                  //等于     ==
                case ExpressionType.NotEqual:               //不等于   !=
                    BinaryExpression binaryExpression = expression as BinaryExpression;
                    return new BinaryBuilder()
                    {
                        LeftBuilder = this.GetSqlBuilder(binaryExpression.Left),
                        NodeType = binaryExpression.NodeType,
                        RightBuilder = this.GetSqlBuilder(binaryExpression.Right)
                    };
                //解析 AND OR表达式节点
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    BinaryExpression andOrExpression = expression as BinaryExpression;
                    return new BinaryBuilder()
                    {
                        LeftBuilder = this.GetNodeBuilder(andOrExpression.Left),
                        NodeType = andOrExpression.NodeType,
                        RightBuilder = this.GetNodeBuilder(andOrExpression.Right)
                    };
                default:
                    throw new Exception(string.Format("unknow expression:{0]", expression));
            }
        }
        /// <summary>
        /// 获取Column节点迭代器
        /// </summary>
        /// <param name="tableMapper">Table元数据解析器</param>
        /// <param name="propertyNames">属性名数组</param>
        /// <returns>Column节点迭代器</returns>
        private IEnumerable<ColumnBuilder> GetColumnBuilders(ITableMapper tableMapper, IEnumerable<string> propertyNames)
        {
            //遍历所有的Column元数据解析器
            foreach (IColumnMapper columnMapper in tableMapper.GetColumnMappers())
            {
                //若propertyNames中包含当前Property元数据解析器的Property名称
                if (propertyNames.Contains(columnMapper.Property.Name))
                {
                    //若列的别名为空，则不使用别名
                    if (string.IsNullOrEmpty(columnMapper.ColumnAlias))
                        yield return new ColumnBuilder(columnMapper.ColumnName, columnMapper.ColumnFullName);
                    //若别名不为空, 则使用别名
                    else
                        yield return new ColumnBuilder(columnMapper.ColumnAlias, string.Format("{0} {1}", columnMapper.ColumnFullName, columnMapper.ColumnAlias));
                }
            }
        }
        /// <summary>
        /// 过滤获取统计查询Sql表达式节点集合
        /// </summary>
        /// <param name="nodeBuilders">源Sql表达式节点集合</param>
        /// <param name="functionName">执行统计的函数名</param>
        /// <returns>统计查询Sql表达式节点集合</returns>
        private IEnumerable<INodeBuilder> GetFunctionNodeBuilders(IEnumerable<INodeBuilder> nodeBuilders, string functionName)
        {
            //获取Sql函数表达式节点
            yield return new NodeBuilder(SqlType.Select, "{0}(*)", functionName);
            //获取其他的sql表达式节点
            foreach (INodeBuilder nodeBuilder in nodeBuilders)
            {
                switch (nodeBuilder.ParentNodeType)
                {
                    case SqlType.From:
                    case SqlType.Where:
                        yield return nodeBuilder;
                        break;
                }
            }
        }
        /// <summary>
        /// 过滤获取统计查询Sql表达式节点集合
        /// </summary>
        /// <param name="nodeBuilders">源Sql表达式节点集合</param>
        /// <param name="functionName">执行统计的函数名</param>
        /// <param name="lambdaExpression">指定对象某属性的表达式</param>
        /// <returns>统计查询Sql表达式节点集合</returns>
        private IEnumerable<INodeBuilder> GetFunctionNodeBuilders(IEnumerable<INodeBuilder> nodeBuilders, string functionName, LambdaExpression lambdaExpression)
        {
            //获取Sql函数表达式节点
            IColumnMapper columnMapper = this._mapperContainer.GetColumnMapper(lambdaExpression.Body.GetProperty());
            yield return new NodeBuilder(SqlType.Select, "{0}({1})", functionName, columnMapper.ColumnFullName);
            //获取其他的sql表达式节点
            foreach (INodeBuilder nodeBuilder in nodeBuilders)
            {
                switch (nodeBuilder.ParentNodeType)
                {
                    case SqlType.From:
                    case SqlType.Where:
                        yield return nodeBuilder;
                        break;
                }
            }
        }
        /// <summary>
        /// 获取条件查询Sql表达式节点集合
        /// </summary>
        /// <param name="nodeBuilders">源Sql表达式节点集合</param>
        /// <param name="columnGetter">列获取器</param>
        /// <param name="property">属性</param>
        /// <param name="whereTemplate">sql条件表达式</param>
        /// <returns>条件查询Sql表达式节点集合</returns>
        private IEnumerable<INodeBuilder> GetQueryNodeBuilders(IEnumerable<INodeBuilder> nodeBuilders, IColumnGetter columnGetter, PropertyInfo property, string whereTemplate)
        {
            //返回原先的Sql表达式节点
            foreach (INodeBuilder nodeBuilder in nodeBuilders)
                yield return nodeBuilder;
            //返回新增的查询条件表达式节点
            yield return new NodeBuilder(SqlType.Where, "{0} {1}", columnGetter.GetColumnFullName(property), whereTemplate);
        }
        /// <summary>
        /// 获取排序查询Sql表达式节点集合
        /// </summary>
        /// <param name="nodeBuilders">源Sql表达式节点集合</param>
        /// <param name="columnGetter">列获取器</param>
        /// <param name="selector">指定对象某属性的表达式</param>
        /// <param name="isAsc">true:升序 false:降序</param>
        /// <returns>排序查询Sql表达式节点集合</returns>
        private IEnumerable<INodeBuilder> GetSortedQueryNodeBuilders(IEnumerable<INodeBuilder> nodeBuilders, IColumnGetter columnGetter, LambdaExpression selector, bool isAsc)
        {
            //返回原先的Sql表达式节点
            foreach (INodeBuilder nodeBuilder in nodeBuilders)
                yield return nodeBuilder;
            //生成并返回OrderBy节点的子节点
            string columnFullName = columnGetter.GetColumnFullName(selector.Body.GetProperty());
            yield return new NodeBuilder(SqlType.OrderBy, "{0} {1}", columnFullName, isAsc ? "ASC" : "DESC");
        }
        /// <summary>
        /// 解析关联表达式获取sql表达式节点迭代器
        /// </summary>
        /// <param name="expression">条件表达式主体</param>
        /// <returns>sql表达式节点迭代器</returns>
        private IEnumerable<INodeBuilder> GetJoinedOnNodeBuilders(Expression expression)
        {
            switch (expression.NodeType)
            {
                //解析普通二叉树表达式节点
                case ExpressionType.GreaterThan:            //大于     >
                case ExpressionType.GreaterThanOrEqual:     //大于等于  >=
                case ExpressionType.LessThan:               //小于     <
                case ExpressionType.LessThanOrEqual:        //小于等于  <=
                case ExpressionType.Equal:                  //等于     ==
                case ExpressionType.NotEqual:               //不等于   !=
                //解析Or节点
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    yield return this.GetNodeBuilder(expression);
                    break;
                //解析And节点
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    //获取AND二叉树表达式主体
                    BinaryExpression andExpression = expression as BinaryExpression;
                    //解析表达式主体的左节点，后去sql表达式生成器集合
                    foreach (INodeBuilder nodeBuilder in this.GetJoinedOnNodeBuilders(andExpression.Left))
                        yield return nodeBuilder;
                    //解析表达式主体的右节点，后去sql表达式生成器集合
                    foreach (INodeBuilder nodeBuilder in this.GetJoinedOnNodeBuilders(andExpression.Right))
                        yield return nodeBuilder;
                    break;
                //不可解析的表达式直接扔出异常
                default:
                    throw new Exception(string.Format("unknow expression: {0}", expression));
            }
        }
        /// <summary>
        /// 解析关联表达式获取sql表达式节点迭代器
        /// </summary>
        /// <param name="expression">条件表达式主体</param>
        /// <param name="nodeBuilders">主表sql表达式节点集合</param>
        /// <param name="otherSourceNodeBuilders">从表sql表达式节点集合</param>
        /// <param name="getJoinBuilder">传入tableBuilder获取joinBuilder的函数</param>
        /// <returns>关联查询的sql表达式节点集合</returns>
        private IEnumerable<INodeBuilder> GetJoinedQueryNodeBuilders(Expression expression, IEnumerable<INodeBuilder> nodeBuilders, IEnumerable<INodeBuilder> otherSourceNodeBuilders, Func<TableBuilder, JoinBuilder> getJoinBuilder)
        {
            //记录列名
            IList<string> columnNames = new List<string>();
            //返回主表sql表达式节点集合
            foreach (INodeBuilder nodeBuilder in nodeBuilders)
            {
                //去除相同列名的列
                if (nodeBuilder.BuilderType == BuilderType.Column)
                {
                    ColumnBuilder columnBuilder = nodeBuilder as ColumnBuilder;
                    if (columnNames.Contains(columnBuilder.ColumnName))
                        continue;
                    columnNames.Add(columnBuilder.ColumnName);
                }
                //返回主表表达式节点
                yield return nodeBuilder;
            }
            //返回从表sql表达式节点集合
            foreach (INodeBuilder nodeBuilder in otherSourceNodeBuilders)
            {
                //去除相同列名的列
                if (nodeBuilder.BuilderType == BuilderType.Column)
                {
                    ColumnBuilder columnBuilder = nodeBuilder as ColumnBuilder;
                    if (columnNames.Contains(columnBuilder.ColumnName))
                        continue;
                    columnNames.Add(columnBuilder.ColumnName);
                }
                //获取从表Table表达式节点并生成JOIN表达式节点
                if (nodeBuilder.BuilderType == BuilderType.Table)
                {
                    //获取joinBuilder节点
                    TableBuilder tableBuilder = nodeBuilder as TableBuilder;
                    JoinBuilder joinBuilder = getJoinBuilder(tableBuilder);
                    //拼接join on表达式节点
                    foreach (ISqlBuilder sqlBuilder in this.GetJoinedOnNodeBuilders(expression))
                        joinBuilder.OnBuilders.Append(sqlBuilder);
                    //返回joinBuilder节点并跳过
                    yield return joinBuilder;
                    continue;
                }
                //返回其他类型的表达式节点
                yield return nodeBuilder;
            }
        }
        /// <summary>
        /// 获取选定项查询命令生成树子节点集合
        /// </summary>
        /// <param name="nodeBuilders">源Sql表达式节点集合</param>
        /// <param name="selector">选定查询项表达式</param>
        /// <param name="tableMapper">Table元数据解析器</param>
        /// <returns>选定项查询命令生成树子节点集合</returns>
        private IEnumerable<INodeBuilder> GetSelectedQueryNodeBuilders(IEnumerable<INodeBuilder> nodeBuilders, LambdaExpression selector, ITableMapper tableMapper)
        {
            //解析Lambda Select表达式为nodeBuilders添加父类型为Select的子sql表达式节点
            switch (selector.Body.NodeType)
            {
                //解析成员表达式(e => e.Property1)
                case ExpressionType.MemberAccess:
                    IColumnMapper columnMapper = this._mapperContainer.GetColumnMapper(selector.Body.GetProperty());
                    if (string.IsNullOrEmpty(columnMapper.ColumnAlias))
                        yield return new ColumnBuilder(columnMapper.ColumnAlias, string.Format("{0} {1}", columnMapper.ColumnFullName, columnMapper.ColumnAlias));
                    else
                        yield return new ColumnBuilder(columnMapper.ColumnName, columnMapper.ColumnFullName);
                    break;
                //解析MemberInitExpression(e => new { PropertyA = e.Property1, PropertyB = a.Property2})
                case ExpressionType.MemberInit:
                    MemberInitExpression memberInitExpression = selector.Body as MemberInitExpression;
                    //获取表达式中包含的TEntity的所有属性的名称
                    string[] propertyNames = new string[memberInitExpression.Bindings.Count];
                    for (int i = 0; i < memberInitExpression.Bindings.Count; i++)
                    {
                        MemberBinding memberBing = memberInitExpression.Bindings[i];
                        propertyNames[i] = memberBing.ToString().Split('.').Last();
                    }
                    //为nodeBuilders添加父类型为Select的子sql表达式节点
                    foreach (INodeBuilder columnBuilder in this.GetColumnBuilders(tableMapper, propertyNames))
                        yield return columnBuilder;
                    break;
                //解析NewExpression(e => new Model(e.Property1, e.Property2))
                case ExpressionType.New:
                    NewExpression newExpression = selector.Body as NewExpression;
                    IEnumerable<string> memberNames = newExpression.Arguments.OfType<MemberExpression>().Select(m => m.Member.Name);
                    //为nodeBuilders添加父类型为Select的子sql表达式节点
                    foreach (INodeBuilder columnBuilder in this.GetColumnBuilders(tableMapper, memberNames))
                        yield return columnBuilder;
                    break;
                default:
                    throw new Exception(string.Format("Unknow Expression: {0}", selector));
            }
            //返回源源Sql表达式节点集合非Select父节点的所有子节点集合
            foreach (INodeBuilder nodeBuilder in nodeBuilders)
            {
                if (nodeBuilder.ParentNodeType != SqlType.Select)
                    yield return nodeBuilder;
            }
        }
        /// <summary>
        /// 解析表达式，获取sql表达式集合以及其附属参数集合
        /// </summary>
        /// <param name="whereVisitorFactory">查询条件解析器工厂</param>
        /// <param name="parameterExpression">lambda表达式参数节点</param>
        /// <param name="expression">表达式</param>
        /// <returns></returns>
        private IEnumerable<KeyValuePair<INodeBuilder, IDbDataParameter[]>> GetNodeBuilderPairs(IWhereVisitorFactory whereVisitorFactory, ParameterExpression parameterExpression, Expression expression)
        {
            switch (expression.NodeType)
            {
                //解析 && &表达式节点
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    BinaryExpression binaryExpression = expression as BinaryExpression;
                    //解析返回左表达式节点集合
                    foreach (KeyValuePair<INodeBuilder, IDbDataParameter[]> nodeBuilderPair in this.GetNodeBuilderPairs(whereVisitorFactory, parameterExpression, binaryExpression.Left))
                        yield return nodeBuilderPair;
                    //解析返回有表达式节点集合
                    foreach (KeyValuePair<INodeBuilder, IDbDataParameter[]> nodeBuilderPair in this.GetNodeBuilderPairs(whereVisitorFactory, parameterExpression, binaryExpression.Right))
                        yield return nodeBuilderPair;
                    break;
                //解析其他类型的表达式
                default:
                    WhereVisitor whereVisitor = whereVisitorFactory.GetVisitor(expression.NodeType);
                    yield return whereVisitor.Visit(parameterExpression, expression);
                    break;
            }
        }
        
        /// <summary>
        /// 静态初始化
        /// </summary>
        static DbContainer()
        {
            DbContainer._containersLocker = new object();
            DbContainer._containerSets = new Dictionary<Type, ContainerSet>();
        }
        /// <summary>
        /// 获取数据容器
        /// </summary>
        /// <typeparam name="TInitializer">初始化器类型</typeparam>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>数据容器</returns>
        public static IDbContainer GetContainer<TInitializer>(string connectionString)
            where TInitializer : DbInitializer, new()
        {
            Start:
            //若当前初始化器类型的容器集存在,直接获取数据容器
            if (DbContainer._containerSets.ContainsKey(typeof(TInitializer)))
                return DbContainer._containerSets[typeof(TInitializer)].GetContainer(connectionString);
            //进入临界区
            lock (DbContainer._containersLocker)
            {
                //若当前初始化器类型的容器集不存在，则创建并添加
                if (!DbContainer._containerSets.ContainsKey(typeof(TInitializer)))
                    DbContainer._containerSets.Add(typeof(TInitializer), new ContainerSet(new TInitializer()));
                //回到Start
                goto Start;
            }
        }
        /// <summary>
        /// 获取数据容器
        /// </summary>
        /// <param name="initializerType">初始化器类型</param>
        /// <param name="connectionString">连接字符串</param>
        /// <returns>数据容器</returns>
        public static IDbContainer GetContainer(Type initializerType, string connectionString)
        {
            Start:
            //若当前初始化器类型的容器集存在,直接获取数据容器
            if (DbContainer._containerSets.ContainsKey(initializerType))
                return DbContainer._containerSets[initializerType].GetContainer(connectionString);
            //进入临界区
            lock (DbContainer._containersLocker)
            {
                //若当前初始化器类型的容器集不存在，则创建并添加
                if (!DbContainer._containerSets.ContainsKey(initializerType))
                {
                    DbInitializer initializer = Activator.CreateInstance(initializerType) as DbInitializer;
                    DbContainer._containerSets.Add(initializerType, new ContainerSet(initializer));
                }
                //回到Start
                goto Start;
            }
        }

        /// <summary>
        /// 创建数据容器
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="initializer">初始化器</param>
        internal DbContainer(string connectionString, DbInitializer initializer)
        {
            //非空检查
            Check.ArgumentNull(connectionString, nameof(connectionString));
            Check.ArgumentNull(initializer, nameof(initializer));
            //初始化线程锁
            this._locker = new object();
            this._dbListsLocker = new object();
            //赋值
            this._dbHelper = initializer.CreateDbHelper(connectionString);
            this._commandTreeFactory = initializer.CreateCommandTreeFactory();
            this._mapperContainer = initializer.CreateMapperContainer();
            this._tableInitializer = initializer.CreateTableInitializer();  //获取Table初始化器
            this._columnInitializer = initializer.CreateColumnInitializer();//获取列初始化器
            this._dbLists = new Dictionary<Type, object>();                 //初始化DbList字典
            this._columnGetter = new ColumnGetter();
            this._mapperColumnGetter = new MapperColumnGetter(this._mapperContainer);
            this._whereVisitorFactory = new WhereVisitorFactory(this._dbHelper, this._columnGetter);
            this._mapperWhereVisitorFactory = new WhereVisitorFactory(this._dbHelper, this._mapperColumnGetter);
        }

        /// <summary>
        /// 初始化某实体类所Mapping的Table
        /// </summary>
        /// <param name="entityType">某实体类的类型</param>
        public void InitTable(Type entityType)
        {
            //检查Table初始化器是否为空,直接退出
            if (this._tableInitializer == null)
                return;
            //获取TableMapper
            ITableMapper tableMapper = this._mapperContainer.GetTableMapper(entityType);
            //若Table不存在,则创建Table
            if (!this._tableInitializer.IsExist(this.DbHelper, tableMapper.Header))
                this._tableInitializer.CreateTable(this.DbHelper, this._commandTreeFactory, tableMapper);
            //若Table存在，则检查并添加EntityMapper中某些属性未注册到目标Table的列
            else
            {
                //若列初始化器不为空
                if (this._columnInitializer != null)
                    //检查并添加EntityMapper中某些属性未注册到目标Table的列
                    this._columnInitializer.AlterTableAddColumns(this.DbHelper, this._commandTreeFactory, tableMapper);
            }
        }
        /// <summary>
        /// 重命名当前实体所Mapping的表
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <param name="oldTableName">旧的表名</param>
        public void RenameTable(Type entityType, string oldTableName)
        {
            //检查Table初始化器是否为空,直接退出
            if (this._tableInitializer == null)
                return;
            //获取TableMapper
            ITableMapper tableMapper = this._mapperContainer.GetTableMapper(entityType);
            //若Table未生成,则从旧表修改
            if (!this._tableInitializer.IsExist(this.DbHelper, tableMapper.Header))
                this._tableInitializer.RenameTable(this.DbHelper, tableMapper.Header, oldTableName);
        }
        /// <summary>
        /// 删除实体类Mapping的表
        /// </summary>
        /// <param name="entityType">实体类型</param>
        public void DropTable(Type entityType)
        {
            //检查Table初始化器是否为空,直接退出
            if (this._tableInitializer == null)
                return;
            //获取TableMapper
            ITableMapper tableMapper = this._mapperContainer.GetTableMapper(entityType);
            //若Table存在,则删除Table
            if (this._tableInitializer.IsExist(this.DbHelper, tableMapper.Header))
                this._tableInitializer.DropTable(this.DbHelper, tableMapper.Header);
        }
        /// <summary>
        /// 创建事故执行器
        /// </summary>
        /// <returns>事故执行器</returns>
        public IDbExecutor CreateExecutor()
        {
            return new DbExecutor(this);
        }
        /// <summary>
        /// 创建统计查询对象
        /// </summary>
        /// <param name="dbBase">操作数据库的基础对象</param>
        /// <param name="functionName">统计函数名</param>
        /// <returns>统计查询对象</returns>
        public IDbScalar CreateScalar(IDbBase dbBase, string functionName)
        {
            //返回新的DbScaler对象
            return new DbScalar(this._mapperContainer, this._commandTreeFactory, this.DbHelper)
            {
                NodeBuilders = this.GetFunctionNodeBuilders(dbBase.NodeBuilders, functionName),
                Parameters = dbBase.Parameters
            };
        }
        /// <summary>
        /// 创建统计查询对象
        /// </summary>
        /// <param name="dbBase">操作数据库的基础对象</param>
        /// <param name="functionName">统计函数名</param>
        /// <param name="lambdaExpression">指定对象某属性的表达式</param>
        /// <returns>统计查询对象</returns>
        public IDbScalar CreateScalar(IDbBase dbBase, string functionName, LambdaExpression lambdaExpression)
        {
            //返回新的DbScaler对象
            return new DbScalar(this._mapperContainer, this._commandTreeFactory, this.DbHelper)
            {
                NodeBuilders = this.GetFunctionNodeBuilders(dbBase.NodeBuilders, functionName, lambdaExpression),
                Parameters = dbBase.Parameters
            };
        }
        /// <summary>
        /// 创建新的查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>新的查询数据源</returns>
        public IDbQuery<TEntity> CreateQuery<TEntity>()
            where TEntity : class
        {
            return this.List<TEntity>();
        }
        /// <summary>
        /// 创建新的查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicates">查询条件表达式数组</param>
        /// <returns>新的查询数据源</returns>
        public IDbQuery<TEntity> CreateQuery<TEntity>(IDbQuery<TEntity> source, params Expression<Func<TEntity, bool>>[] predicates)
            where TEntity : class
        {
            //初始化Sql表达式节点集合 和 Sql参数节点集合
            List<INodeBuilder> nodeBuilders = new List<INodeBuilder>();
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();
            //加载Sql表达式节点集合 和 Sql参数节点集合
            foreach (Expression<Func<TEntity, bool>> predicate in predicates)
            {
                foreach (KeyValuePair<INodeBuilder, IDbDataParameter[]> nodeBuilderPair in this.GetNodeBuilderPairs(this._mapperWhereVisitorFactory, predicate.Parameters[0], predicate.Body))
                {
                    nodeBuilders.Add(nodeBuilderPair.Key);
                    parameters.AddRange(nodeBuilderPair.Value);
                }
            }
            //返回新的查询对象
            return new DbQuery<TEntity>(this._mapperContainer, this._commandTreeFactory, this.DbHelper)
            {
                Factory = this,
                NodeBuilders = source.NodeBuilders.Concat(nodeBuilders),
                Parameters = source.Parameters.Concat(parameters),
                PropertyLinkers = source.PropertyLinkers
            };
        }
        /// <summary>
        /// 创建新的查询对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="property">属性</param>
        /// <param name="whereTemplate">sql条件表达式</param>
        /// <param name="parameters">sql参数</param>
        /// <returns>新的查询对象</returns>
        public IDbQuery<TEntity> CreateQuery<TEntity>(IDbQuery<TEntity> source, PropertyInfo property, string whereTemplate, params IDbDataParameter[] parameters)
            where TEntity : class
        {
            //返回新的查询对象
            return new DbQuery<TEntity>(this._mapperContainer, this._commandTreeFactory, this.DbHelper)
            {
                Factory = this,
                NodeBuilders = this.GetQueryNodeBuilders(source.NodeBuilders, this._mapperColumnGetter, property, whereTemplate),
                Parameters = source.Parameters.Concat(parameters),
                PropertyLinkers = source.PropertyLinkers
            };
        }
        /// <summary>
        /// 创建根据某属性排好序的查询对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体某属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="keySelector">指定实体对象某属性的表达式</param>
        /// <param name="isAsc">true:升序 false:降序</param>
        /// <returns>排好序的查询对象</returns>
        public IDbQuery<TEntity> CreateSortedQuery<TEntity, TKey>(IDbQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector, bool isAsc = true)
            where TEntity : class
        {
            //返回新的查询对象
            return new DbQuery<TEntity>(this._mapperContainer, this._commandTreeFactory, this.DbHelper)
            {
                Factory = this,
                NodeBuilders = this.GetSortedQueryNodeBuilders(source.NodeBuilders, this._mapperColumnGetter, keySelector, isAsc),
                Parameters = source.Parameters,
                PropertyLinkers = source.PropertyLinkers
            };
        }
        /// <summary>
        /// 创建连接查询对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联对象的数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>连接查询对象</returns>
        public IDbQuery<TEntity> CreateJoinedQuery<TEntity, TOther>(IDbQuery<TEntity> source, IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class
        {
            //获取PropertyLinkers
            IList<PropertyLinker> propertyLinkers = source.PropertyLinkers.ToList();
            propertyLinkers.Add(new PropertyLinker(selector.Body.GetProperty(), otherSource.PropertyLinkers));
            //返回新的查询对象
            return new DbQuery<TEntity>(this._mapperContainer, this._commandTreeFactory, this.DbHelper)
            {
                Factory = this,
                NodeBuilders = this.GetJoinedQueryNodeBuilders(predicate.Body, source.NodeBuilders, otherSource.NodeBuilders, JoinBuilder.GetInnerJoinBuilder),
                Parameters = source.Parameters.Concat(otherSource.Parameters),
                PropertyLinkers = propertyLinkers.ToArray()
            };
        }
        /// <summary>
        /// 创建左连接查询对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联对象的数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>左连接查询对象</returns>
        public IDbQuery<TEntity> CreateLeftJoinedQuery<TEntity, TOther>(IDbQuery<TEntity> source, IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class
        {
            //获取PropertyLinkers
            IList<PropertyLinker> propertyLinkers = source.PropertyLinkers.ToList();
            propertyLinkers.Add(new PropertyLinker(selector.Body.GetProperty(), otherSource.PropertyLinkers));
            //返回新的查询对象
            return new DbQuery<TEntity>(this._mapperContainer, this._commandTreeFactory, this.DbHelper)
            {
                Factory = this,
                NodeBuilders = this.GetJoinedQueryNodeBuilders(predicate.Body, source.NodeBuilders, otherSource.NodeBuilders, JoinBuilder.GetLeftJoinBuilder),
                Parameters = source.Parameters.Concat(otherSource.Parameters),
                PropertyLinkers = propertyLinkers.ToArray()
            };
        }
        /// <summary>
        /// 创建分页查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderBySelector">指定排序属性的表达式</param>
        /// <param name="pageSize">每页元素数量</param>
        /// <param name="pageIndex">当前第几页</param>
        /// <param name="isAsc">true:升序 false:降序</param>
        /// <returns>分页查询数据源</returns>
        public IDbPagedQuery<TEntity> CreatePagedQuery<TEntity>(IDbQuery<TEntity> source, Expression<Func<TEntity, object>> orderBySelector, int pageSize, int pageIndex = 1, bool isAsc = true)
            where TEntity : class
        {
            //获取排序属性
            IColumnMapper columnMapper = this._mapperContainer.GetColumnMapper(orderBySelector.Body.GetProperty());
            //创建分页查询数据源
            return new DbPagedQuery<TEntity>(this._mapperContainer, this._commandTreeFactory, this.DbHelper)
            {
                PropertyLinkers = source.PropertyLinkers,
                OrderByColumn = columnMapper.ColumnFullName,
                IsAsc = isAsc,
                NodeBuilders = source.NodeBuilders,
                Parameters = source.Parameters,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
        }
        /// <summary>
        /// 创建选定项查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TElement">结果类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">转换实体对象到结果对象的表达式</param>
        /// <returns>选定项查询数据源</returns>
        public IDbSelectedQuery<TElement> CreateSelectedQuery<TEntity, TElement>(IDbQuery<TEntity> source, Expression<Func<TEntity, TElement>> selector)
            where TEntity : class
        {
            //获取Table元数据解析器
            ITableMapper tableMapper = this._mapperContainer.GetTableMapper(typeof(TEntity));
            //返回新的查询对象
            return new DbSelectedQuery<TElement, TEntity>(this._mapperContainer, this._commandTreeFactory, this.DbHelper)
            {
                NodeBuilders = this.GetSelectedQueryNodeBuilders(source.NodeBuilders, selector, tableMapper),
                Parameters = source.Parameters,
                PropertyLinkers = source.PropertyLinkers,
                Convert = selector.Compile()
            };
        }
        /// <summary>
        /// 创建去除重复项查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TElement">结果类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">转换实体对象到结果对象的表达式</param>
        /// <returns>去除重复项查询数据源</returns>
        public IDbSelectedQuery<TElement> CreateDistinctQuery<TEntity, TElement>(IDbQuery<TEntity> source, Expression<Func<TEntity, TElement>> selector)
            where TEntity : class
        {
            //获取Table元数据解析器
            ITableMapper tableMapper = this._mapperContainer.GetTableMapper(typeof(TEntity));
            //返回新的查询对象
            return new DbDistinctQuery<TElement, TEntity>(this._mapperContainer, this._commandTreeFactory, this.DbHelper)
            {
                NodeBuilders = this.GetSelectedQueryNodeBuilders(source.NodeBuilders, selector, tableMapper),
                Parameters = source.Parameters,
                PropertyLinkers = source.PropertyLinkers,
                Convert = selector.Compile()
            };
        }
        /// <summary>
        /// 创建实体操作器
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="transaction">事物出来对象</param>
        /// <returns>实体操作器</returns>
        public IDbOperator<TEntity> CreateOperator<TEntity>(IDbTransaction transaction)
            where TEntity : class
        {
            return new DbTransactionOperator<TEntity>(this, this.DbHelper, this._commandTreeFactory, this._mapperContainer, transaction);
        }
        /// <summary>
        /// 创建自动连接Table的增删改集合
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>自动连接Table的增删改集合</returns>
        public IDbList<TEntity> List<TEntity>()
            where TEntity : class
        {
            Start:
            //若DbList字典中包含当前类型的集合,直接返回
            if (this._dbLists.ContainsKey(typeof(TEntity)))
                return this._dbLists[typeof(TEntity)] as IDbList<TEntity>;
            lock (this._dbListsLocker)
            {
                //若DbList字典中不包含当前类型的集合，创建当前类型的集合
                if (!this._dbLists.ContainsKey(typeof(TEntity)))
                {
                    //注册DbList
                    IDbList<TEntity> entities = new DbList<TEntity>(this, this.DbHelper, this._commandTreeFactory, this._mapperContainer);
                    this._dbLists.Add(typeof(TEntity), entities);
                }
                //重新从字典中获取当前类型的集合
                goto Start;
            }
        }
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">对象类型</typeparam>
        /// <param name="querySql">查询sql</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>视图查询数据源</returns>
        public IDbView<TModel> View<TModel>(string querySql, params IDbDataParameter[] parameters)
            where TModel : class, new()
        {
            return new DbView<TModel>(this, this.DbHelper, this._commandTreeFactory, querySql)
            {
                NodeBuilders = new INodeBuilder[0],
                Parameters = parameters
            };
        }
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图对象</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="predicates">查询条件表达式数组</param>
        /// <returns>新的视图查询数据源</returns>
        public IDbView<TModel> CreateView<TModel>(IDbView<TModel> source, params Expression<Func<TModel, bool>>[] predicates)
            where TModel : class, new()
        {
            //初始化Sql表达式节点集合 和 Sql参数节点集合
            List<INodeBuilder> nodeBuilders = new List<INodeBuilder>();
            List<IDbDataParameter> parameters = new List<IDbDataParameter>();
            //加载Sql表达式节点集合 和 Sql参数节点集合
            foreach (Expression<Func<TModel, bool>> predicate in predicates)
            {
                foreach (KeyValuePair<INodeBuilder, IDbDataParameter[]> nodeBuilderPair in this.GetNodeBuilderPairs(this._whereVisitorFactory, predicate.Parameters[0], predicate.Body))
                {
                    nodeBuilders.Add(nodeBuilderPair.Key);
                    parameters.AddRange(nodeBuilderPair.Value);
                }
            }
            //创建并返回视图查询数据源
            return new DbView<TModel>(this, this._dbHelper, this._commandTreeFactory, source.InnerQuerySql)
            {
                NodeBuilders = source.NodeBuilders.Concat(nodeBuilders),
                Parameters = source.Parameters.Concat(parameters),
            };
        }
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图模型对象</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="property">属性</param>
        /// <param name="whereTemplate">sql条件表达式模板</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>新的视图查询数据源</returns>
        public IDbView<TModel> CreateView<TModel>(IDbView<TModel> source, PropertyInfo property, string whereTemplate, params IDbDataParameter[] parameters)
            where TModel : class, new()
        {
            //创建并返回视图查询数据源
            return new DbView<TModel>(this, this._dbHelper, this._commandTreeFactory, source.InnerQuerySql)
            {
                NodeBuilders = this.GetQueryNodeBuilders(source.NodeBuilders, this._columnGetter, property, whereTemplate),
                Parameters = source.Parameters.Concat(parameters),
            };
        }
        /// <summary>
        /// 创建根据某属性排好序的视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图模型对象</typeparam>
        /// <typeparam name="TKey">对象某属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="keySelector">指定视图模型对象某属性的表达式</param>
        /// <param name="isAsc">true:升序 false:降序</param>
        /// <returns>排好序的视图查询数据源</returns>
        public IDbView<TModel> CreateSortedView<TModel, TKey>(IDbView<TModel> source, Expression<Func<TModel, TKey>> keySelector, bool isAsc = true)
            where TModel : class, new()
        {
            //创建并返回视图查询数据源
            return new DbView<TModel>(this, this._dbHelper, this._commandTreeFactory, source.InnerQuerySql)
            {
                NodeBuilders = this.GetSortedQueryNodeBuilders(source.NodeBuilders, this._columnGetter, keySelector, isAsc),
                Parameters = source.Parameters,
            };
        }
    }
}
