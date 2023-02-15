using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Internal.CommandTrees;
using CloudEntity.Internal.Data.Entity;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace CloudEntity.Core.Data.Entity
{
    /// <summary>
    /// 数据容器类
    /// 李凯 Apple_Li 15150598493
    /// 最后修改时间：2023/02/08 23:55
    /// </summary>
    public sealed class DbContainer : IDbContainer, IDbFactory
    {
        /// <summary>
        /// 访问DbList字典的线程锁
        /// </summary>
        private object _dbListsLocker;
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
        /// 创建表达式解析器的工厂
        /// </summary>
        private IPredicateParserFactory _predicateParserFactory;
        /// <summary>
        /// 创建查询条件解析器的工厂
        /// </summary>
        private IPredicateParserFactory _mapperPredicateParserFactory;
        /// <summary>
        /// 数据列表字典
        /// </summary>
        private IDictionary<Type, object> _dbLists;
        /// <summary>
        /// 控制字典缓存的线程锁
        /// </summary>
        private static object _containersLocker;
        /// <summary>
        /// 数据容器字典
        /// </summary>
        private static IDictionary<string, IDbContainer> _containers;

        /// <summary>
        /// 操作数据库的DbHelper
        /// </summary>
        public DbHelper DbHelper
        {
            get { return _dbHelper; }
        }
        
        /// <summary>
        /// 获取sql成员表达式节点
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>sql成员表达式节点</returns>
        private ISqlBuilder GetSqlBuilder(Expression expression)
        {
            //获取成员表达式
            MemberExpression memberExpression = expression.GetMemberExpression();
            //获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(memberExpression.Expression.Type);
            //获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            //获取sql列节点生成器
            return _commandTreeFactory.GetColumnBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName);
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
        /// 获取条件查询sql表达式节点
        /// </summary>
        /// <param name="memberExpression">左边指定对象成员表达式</param>
        /// <param name="rightSqlExpression">右边的sql表达式</param>
        /// <returns>sql表达式节点</returns>
        private INodeBuilder GetWhereChildBuilder(MemberExpression memberExpression, string rightSqlExpression)
        {
            // 获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(memberExpression.Expression.Type);
            // 获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            // 获取查询条件表达式节点
            return _commandTreeFactory.GetWhereChildBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, rightSqlExpression);
        }
        /// <summary>
        /// 获取Column节点列表
        /// </summary>
        /// <param name="tableMapper">Table元数据解析器</param>
        /// <param name="propertyNames">属性名数组</param>
        /// <returns>Column节点列表</returns>
        private IEnumerable<INodeBuilder> GetColumnBuilders(ITableMapper tableMapper, IEnumerable<string> propertyNames)
        {
            //遍历所有的Column元数据解析器
            foreach (IColumnMapper columnMapper in tableMapper.GetColumnMappers())
            {
                //若propertyNames中包含当前Property元数据解析器的Property名称
                if (propertyNames.Contains(columnMapper.Property.Name))
                {
                    //依次获取column节点生成器
                    yield return _commandTreeFactory.GetColumnBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, columnMapper.ColumnAlias);
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
            //获取成员表达式
            MemberExpression memberExpression = lambdaExpression.Body.GetMemberExpression();
            //获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(memberExpression.Expression.Type);
            //获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            //获取Sql函数表达式节点
            yield return _commandTreeFactory.GetFunctionNodeBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, functionName);
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
                //解析转换表达式及其成员表达式(e => e.Property1)
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                    //获取成员表达式
                    MemberExpression memberExpression = selector.Body.GetMemberExpression();
                    //获取当前实体类型的Table元数据解析器
                    ITableMapper currentTableMapper = _mapperContainer.GetTableMapper(memberExpression.Expression.Type);
                    //获取columnMapper
                    IColumnMapper columnMapper = currentTableMapper.GetColumnMapper(memberExpression.Member.Name);
                    //依次获取column节点生成器
                    yield return _commandTreeFactory.GetColumnBuilder(currentTableMapper.Header.TableAlias, columnMapper.ColumnName, columnMapper.ColumnAlias);
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
        #region 获取OrderBy的子节点集合
        /// <summary>
        /// 获取orderby节点的子节点
        /// </summary>
        /// <param name="memberExpression">成员表达式</param>
        /// <param name="isDesc">是否降序[true:降序 false:升序]</param>
        /// <returns>orderby节点的子节点</returns>
        private INodeBuilder GetOrderbyNodeBuilder(MemberExpression memberExpression, bool isDesc)
        {
            //获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(memberExpression.Expression.Type);
            //获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            //获取不使用别名的OrderBy节点的子表达式(排序时，禁止使用别名)
            return _commandTreeFactory.GetOrderByChildBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, isDesc);
        }
        /// <summary>
        /// 获取orderby节点的子节点集合
        /// </summary>
        /// <param name="memberExpressions">成员表达式列表</param>
        /// <param name="isDesc">是否降序[true:降序 false:升序]</param>
        /// <returns>orderby节点的子节点集合</returns>
        private IEnumerable<INodeBuilder> GetOrderbyNodeBuilders(IEnumerable<MemberExpression> memberExpressions, bool isDesc)
        {
            //遍历所有的成员表达式
            foreach (MemberExpression memberExpression in memberExpressions)
            {
                //依次获取orderby节点的子表达式节点
                yield return this.GetOrderbyNodeBuilder(memberExpression, isDesc);
            }
        }
        /// <summary>
        /// 获取orderby节点的子节点集合
        /// </summary>
        /// <param name="selector">选定排序项表达式</param>
        /// <param name="isDesc">true:降序 false:升序</param>
        /// <returns>orderby节点的子节点集合</returns>
        private IEnumerable<INodeBuilder> GetOrderbyNodeBuilders(LambdaExpression selector, bool isDesc)
        {
            //解析Lambda Select表达式为nodeBuilders添加父类型为Select的子sql表达式节点
            switch (selector.Body.NodeType)
            {
                //解析转换表达式及其成员表达式(e => e.Property1)
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                    //获取成员表达式
                    MemberExpression memberExpression = selector.Body.GetMemberExpression();
                    //生成并返回OrderBy节点的子节点
                    yield return this.GetOrderbyNodeBuilder(memberExpression, isDesc);
                    break;
                //解析NewExpression(e => new Model(e.Property1, e.Property2))
                case ExpressionType.New:
                    //获取成员数组
                    NewExpression newExpression = selector.Body as NewExpression;
                    //获取成员表达式列表
                    IEnumerable<MemberExpression> memberExpressions = newExpression.Arguments.OfType<MemberExpression>();
                    //为nodeBuilders添加父类型为Select的子sql表达式节点
                    foreach (INodeBuilder columnBuilder in this.GetOrderbyNodeBuilders(memberExpressions, isDesc))
                        yield return columnBuilder;
                    break;
                //遇到未知类型的表达式直接异常
                default:
                    throw new Exception(string.Format("Unknow Expression: {0}", selector));
            }
        }
        #endregion
        #region 获取数据源操作对象
        /// <summary>
        /// 获取复制的查询数据源
        /// </summary>
        /// <param name="source">基础数据源</param>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>查询数据源</returns>
        private DbQuery<TEntity> CloneToQuery<TEntity>(IDbSource<TEntity> source)
            where TEntity : class
        {
            // 创建查询数据源
            DbQuery<TEntity> cloned = new DbQuery<TEntity>(_mapperContainer, _commandTreeFactory, _dbHelper, this, _mapperPredicateParserFactory);
            // 复制sql表达式节点列表
            cloned.AddNodeBuilders(source.NodeBuilders);
            // 复制sql参数列表
            cloned.AddSqlParameters(source.Parameters);
            // 获取查询数据源
            return cloned;
        }
        /// <summary>
        /// 获取复制的查询数据源
        /// </summary>
        /// <param name="source">源数据源</param>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>查询数据源</returns>
        private DbQuery<TEntity> CloneToQuery<TEntity>(IDbQuery<TEntity> source)
            where TEntity : class
        {
            // 创建查询数据源
            DbQuery<TEntity> cloned = new DbQuery<TEntity>(_mapperContainer, _commandTreeFactory, _dbHelper, this, _mapperPredicateParserFactory);
            // 复制sql表达式节点列表
            cloned.AddNodeBuilders(source.NodeBuilders);
            // 复制sql参数列表
            cloned.AddSqlParameters(source.Parameters);
            // 复制关联的对象链接列表
            cloned.AddPropertyLinkers(source.PropertyLinkers);
            // 获取查询数据源
            return cloned;
        }
        /// <summary>
        /// 复制视图查询数据源
        /// </summary>
        /// <param name="source">视图查询数据源</param>
        /// <typeparam name="TModel">视图结果类型</typeparam>
        /// <returns>视图查询数据源</returns>
        private DbView<TModel> CloneView<TModel>(IDbView<TModel> source)
            where TModel : class, new()
        {
            // 创建新的视图数据源
            DbView<TModel> cloned = new DbView<TModel>(this, _commandTreeFactory, this.DbHelper, _predicateParserFactory, source.InnerQuerySql);
            // 复制sql表达式节点列表
            cloned.AddNodeBuilders(source.NodeBuilders);
            // 复制sql参数列表
            cloned.AddSqlParameters(source.Parameters);
            // 获取复制的数据源
            return cloned;
        }
        #endregion

        /// <summary>
        /// 静态初始化
        /// </summary>
        static DbContainer()
        {
            _containersLocker = new object();
            _containers = new Dictionary<string, IDbContainer>();
        }
        /// <summary>
        /// 初始化数据容器池中的数据容器
        /// </summary>
        /// <typeparam name="TInitializer">数据库初始化器类型</typeparam>
        /// <param name="connectionString">连接字符串</param>
        public static void Init<TInitializer>(string connectionString)
            where TInitializer : DbInitializer, new()
        {
            //非空检查
            Check.ArgumentNull(connectionString, nameof(connectionString));
            //若字典中包含当前连接字符串的数据容器则直接退出
            if (_containers.ContainsKey(connectionString))
                return;
            //进入临界区(单线程模式)
            lock (_containersLocker)
            {
                //若当前数据容器不存在，则创建并添加至字典中
                if (!_containers.ContainsKey(connectionString))
                {
                    //创建数据库初始化器
                    DbInitializer initializer = new TInitializer();
                    //创建并添加数据容器
                    _containers.Add(connectionString, new DbContainer(connectionString, initializer));
                }
            }
        }
        /// <summary>
        /// 初始化数据容器池中的数据容器
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="initializer">数据库初始化器</param>
        public static void Init(string connectionString, DbInitializer initializer)
        {
            //非空检查
            Check.ArgumentNull(connectionString, nameof(connectionString));
            Check.ArgumentNull(initializer, nameof(initializer));
            //若字典中包含当前连接字符串的数据容器则直接退出
            if (_containers.ContainsKey(connectionString))
                return;
            //进入临界区(单线程模式)
            lock (_containersLocker)
            {
                //若当前数据容器不存在，则创建并添加至字典中
                if (!_containers.ContainsKey(connectionString))
                {
                    //创建并添加数据容器
                    _containers.Add(connectionString, new DbContainer(connectionString, initializer));
                }
            }
        }
        /// <summary>
        /// 获取(若不存在则创建)数据容器
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="initializer">初始化器(第一次获取创建数据容器时需要)</param>
        /// <returns>数据容器</returns>
        public static IDbContainer Get(string connectionString, DbInitializer initializer = null)
        {
            //非空检查
            Check.ArgumentNull(connectionString, nameof(connectionString));
            //开始
            Start:
            //若字典中包含当前连接字符串的数据容器则直接获取
            if (_containers.ContainsKey(connectionString))
                return _containers[connectionString];
            //若需要创建数据容器,则检查初始化器是否为空
            Check.ArgumentNull(initializer, nameof(initializer));
            //进入临界区(单线程模式)
            lock (_containersLocker)
            {
                //若当前数据容器不存在，则创建并添加至字典中
                if (!_containers.ContainsKey(connectionString))
                    _containers.Add(connectionString, new DbContainer(connectionString, initializer));
            }
            //回到Start
            goto Start;
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
            _dbListsLocker = new object();
            //赋值
            _dbHelper = initializer.CreateDbHelper(connectionString);
            _commandTreeFactory = initializer.CreateCommandTreeFactory();
            _mapperContainer = initializer.CreateMapperContainer();
            _tableInitializer = initializer.CreateTableInitializer();  //获取Table初始化器
            _columnInitializer = initializer.CreateColumnInitializer();//获取列初始化器
            _dbLists = new Dictionary<Type, object>();                 //初始化DbList字典
            _predicateParserFactory = new PredicateParserFactory(_commandTreeFactory);
            _mapperPredicateParserFactory = new PredicateParserFactory(_commandTreeFactory, _mapperContainer);
        }

        /// <summary>
        /// 初始化某实体类所Mapping的Table
        /// </summary>
        /// <param name="entityType">某实体类的类型</param>
        public void InitTable(Type entityType)
        {
            //检查Table初始化器是否为空,直接退出
            if (_tableInitializer == null)
                return;
            //获取TableMapper
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(entityType);
            //若Table不存在,则创建Table
            if (!_tableInitializer.IsExist(DbHelper, tableMapper.Header))
                _tableInitializer.CreateTable(DbHelper, _commandTreeFactory, tableMapper);
            //若Table存在，则检查并添加EntityMapper中某些属性未注册到目标Table的列
            else
            {
                //若列初始化器不为空
                if (_columnInitializer != null)
                    //检查并添加EntityMapper中某些属性未注册到目标Table的列
                    _columnInitializer.AlterTableAddColumns(this.DbHelper, _commandTreeFactory, tableMapper);
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
        /// 创建实体操作器
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="transaction">事物出来对象</param>
        /// <returns>实体操作器</returns>
        public IDbOperator<TEntity> CreateOperator<TEntity>(IDbTransaction transaction)
            where TEntity : class
        {
            return new DbTransactionOperator<TEntity>(this, this.DbHelper, _commandTreeFactory, _mapperContainer, transaction);
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
            if (_dbLists.ContainsKey(typeof(TEntity)))
                return _dbLists[typeof(TEntity)] as IDbList<TEntity>;
            lock (_dbListsLocker)
            {
                //若DbList字典中不包含当前类型的集合，创建当前类型的集合
                if (!_dbLists.ContainsKey(typeof(TEntity)))
                {
                    //注册DbList
                    IDbList<TEntity> entities = new DbList<TEntity>(this, this.DbHelper, _commandTreeFactory, _mapperContainer);
                    _dbLists.Add(typeof(TEntity), entities);
                }
                //重新从字典中获取当前类型的集合
                goto Start;
            }
        }
        #region 创建统计查询数据源
        /// <summary>
        /// 创建统计查询数据源
        /// </summary>
        /// <param name="dbBase">操作数据库的基础对象</param>
        /// <param name="functionName">统计函数名</param>
        /// <returns>统计查询数据源</returns>
        public IDbScalar CreateScalar(IDbBase dbBase, string functionName)
        {
            // 创建DbScaler对象
            DbScalar dbScalar = new DbScalar(this._mapperContainer, this._commandTreeFactory, this.DbHelper);
            // 加载sql表达式节点列表
            foreach (INodeBuilder nodeBuilder in this.GetFunctionNodeBuilders(dbBase.NodeBuilders, functionName))
                dbScalar.AddNodeBuilder(nodeBuilder);
            // 加载sql参数列表
            foreach (IDbDataParameter sqlParameter in dbBase.Parameters)
                dbScalar.AddSqlParameter(sqlParameter);
            // 获取DbScaler对象
            return dbScalar;
        }
        /// <summary>
        /// 创建统计查询数据源
        /// </summary>
        /// <param name="dbBase">操作数据库的基础对象</param>
        /// <param name="functionName">统计函数名</param>
        /// <param name="lambdaExpression">指定对象某属性的表达式</param>
        /// <returns>统计查询数据源</returns>
        public IDbScalar CreateScalar(IDbBase dbBase, string functionName, LambdaExpression lambdaExpression)
        {
            // 创建DbScaler对象
            DbScalar dbScalar = new DbScalar(this._mapperContainer, this._commandTreeFactory, this.DbHelper);
            // 加载sql表达式节点列表
            foreach (INodeBuilder nodeBuilder in this.GetFunctionNodeBuilders(dbBase.NodeBuilders, functionName, lambdaExpression))
                dbScalar.AddNodeBuilder(nodeBuilder);
            // 加载sql参数列表
            foreach (IDbDataParameter sqlParameter in dbBase.Parameters)
                dbScalar.AddSqlParameter(sqlParameter);
            // 获取DbScaler对象
            return dbScalar;
        }
        #endregion
        #region 创建查询数据源
        /// <summary>
        /// 创建新的查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>新的查询数据源</returns>
        public IDbQuery<TEntity> CreateQuery<TEntity>()
            where TEntity : class
        {
            // 获取当前实体对象的唯一数据源
            IDbSource<TEntity> source = this.List<TEntity>();
            // 从唯一数据源中复制获取查询数据源
            return this.CloneToQuery(source);
        }
        /// <summary>
        /// 复制新的查询数据源
        /// </summary>
        /// <param name="source">来源数据源</param>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <returns>新的查询数据源</returns>
        public IDbQuery<TEntity> CreateQuery<TEntity>(IDbSource<TEntity> source)
            where TEntity : class
        {
            // 复制新的查询数据源
            DbQuery<TEntity> cloned = this.CloneToQuery(source);
            // 获取新的查询数据源
            return cloned;
        }
        /// <summary>
        /// 创建新的查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">查询数据源</param>
        /// <returns>新的查询数据源</returns>
        public IDbQuery<TEntity> CreateQuery<TEntity>(IDbQuery<TEntity> source)
            where TEntity : class
        {
            // 复制新的查询数据源
            DbQuery<TEntity> cloned = this.CloneToQuery(source);
            // 获取新的查询数据源
            return cloned;
        }
        /// <summary>
        /// 创建新的查询数据源
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <param name="parameters">sql参数数组</param>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">实体属性类型</typeparam>
        /// <returns>新的查询数据源</returns>
        public IDbQuery<TEntity> CreateQuery<TEntity, TProperty>(IDbQuery<TEntity> source, Expression<Func<TEntity, TProperty>> selector, string sqlPredicate, params IDbDataParameter[] parameters)
            where TEntity : class
        {
            // 获取成员表达式
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            // 获取查询条件表达式节点
            INodeBuilder nodeBuilder = this.GetWhereChildBuilder(memberExpression, sqlPredicate);

            // 复制查询数据源
            DbQuery<TEntity> cloned = this.CloneToQuery(source);
            // 添加sql表达式节点
            cloned.AddNodeBuilder(nodeBuilder);
            // 添加sql参数数组
            cloned.AddSqlParameters(parameters);
            // 最终获取复制的数据源
            return cloned;
        }
        /// <summary>
        /// 创建新的查询数据源
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlFormat">sql条件格式化字符串</param>
        /// <param name="values">sql参数值数组</param>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">实体属性类型</typeparam>
        /// <returns>新的查询数据源</returns>
        public IDbQuery<TEntity> CreateQuery<TEntity, TProperty>(IDbQuery<TEntity> source, Expression<Func<TEntity, TProperty>> selector, string sqlFormat, params TProperty[] values)
            where TEntity : class
        {
            // 获取成员表达式
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            // 获取属性名称
            string memberName = memberExpression.Member.Name;
            // 获取此属性名称开头的使用次数
            int times = source.Parameters.Count(p => p.ParameterName.StartsWith(memberName));
            // 初始化参数名称数组
            string[] parameterNames = new string[values.Length];
            // 遍历参数值列表，加载sql参数名称数组
            for (int i = 0; i < values.Length; i++)
                parameterNames[i] = $"{memberName}{(i + times).ToString()}";
            // 获取右边的sql表达式
            string rightSqlExpression = string.Format(sqlFormat, parameterNames);
            // 获取查询条件表达式节点
            INodeBuilder nodeBuilder = this.GetWhereChildBuilder(memberExpression, rightSqlExpression);

            // 复制查询数据源
            DbQuery<TEntity> cloned = this.CloneToQuery(source);
            // 添加sql表达式节点
            cloned.AddNodeBuilder(nodeBuilder);
            // 添加sql参数列表
            for (int i = 0; i < parameterNames.Length; i++)
                cloned.AddSqlParameter(parameterNames[i], values[i]);
            // 最终获取复制的数据源
            return cloned;
        }
        /// <summary>
        /// 创建根据某属性排好序的查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TKey">实体某属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="keySelector">指定实体对象某属性的表达式</param>
        /// <param name="isDesc">true:降序 false:升序</param>
        /// <returns>排好序的查询数据源</returns>
        public IDbQuery<TEntity> CreateSortedQuery<TEntity, TKey>(IDbQuery<TEntity> source, Expression<Func<TEntity, TKey>> keySelector, bool isDesc = false)
            where TEntity : class
        {
            // 复制查询数据源
            DbQuery<TEntity> cloned = this.CloneToQuery(source);
            // 加载sql表达式节点列表
            cloned.AddNodeBuilders(this.GetOrderbyNodeBuilders(keySelector, isDesc));
            // 最终获取复制的数据源
            return cloned;
        }
        #endregion
        #region 创建关联查询数据源
        /// <summary>
        /// 创建连接查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联对象的数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>连接查询数据源</returns>
        public IDbQuery<TEntity> CreateJoinedQuery<TEntity, TOther>(IDbQuery<TEntity> source, IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class
        {
            // 创建新的查询数据源
            DbQuery<TEntity> query = new DbQuery<TEntity>(_mapperContainer, _commandTreeFactory, _dbHelper, this, _mapperPredicateParserFactory);
            // 加载sql表达式节点列表
            foreach (INodeBuilder nodeBuilder in this.GetJoinedQueryNodeBuilders(predicate.Body, source.NodeBuilders, otherSource.NodeBuilders, JoinBuilder.GetInnerJoinBuilder))
                query.AddNodeBuilder(nodeBuilder);
            // 加载去除重复参数名的sql参数列表
            query.AddSqlParameters(source.Parameters.Concat(otherSource.Parameters).DistinctBy(p => p.ParameterName));
            // 加载关联对象属性链接列表
            query.AddPropertyLinkers(source.PropertyLinkers);
            query.AddPropertyLinker(new PropertyLinker(selector.Body.GetProperty(), otherSource.PropertyLinkers.ToArray()));
            // 返回新的查询数据源
            return query;
        }
        /// <summary>
        /// 创建连接查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联对象的选择性查询数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>连接查询数据源</returns>
        public IDbQuery<TEntity> CreateJoinedQuery<TEntity, TOther>(IDbQuery<TEntity> source, IDbSelectedQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class
        {
            // 创建新的查询数据源
            DbQuery<TEntity> query = new DbQuery<TEntity>(_mapperContainer, _commandTreeFactory, _dbHelper, this, _mapperPredicateParserFactory);
            // 加载sql表达式节点列表
            foreach (INodeBuilder nodeBuilder in this.GetJoinedQueryNodeBuilders(predicate.Body, source.NodeBuilders, otherSource.NodeBuilders, JoinBuilder.GetInnerJoinBuilder))
                query.AddNodeBuilder(nodeBuilder);
            // 加载去除重复参数名的sql参数列表
            query.AddSqlParameters(source.Parameters.Concat(otherSource.Parameters).DistinctBy(p => p.ParameterName));
            // 加载关联对象属性链接
            query.AddPropertyLinkers(source.PropertyLinkers);
            query.AddPropertyLinker(new PropertyLinker(selector.Body.GetProperty()));
            // 获取新的查询数据源
            return query;
        }
        /// <summary>
        /// 创建左连接查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联对象的数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>左连接查询数据源</returns>
        public IDbQuery<TEntity> CreateLeftJoinedQuery<TEntity, TOther>(IDbQuery<TEntity> source, IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class
        {
            // 创建新的查询数据源
            DbQuery<TEntity> query = new DbQuery<TEntity>(_mapperContainer, _commandTreeFactory, _dbHelper, this, _mapperPredicateParserFactory);
            // 加载sql表达式节点列表
            foreach (INodeBuilder nodeBuilder in this.GetJoinedQueryNodeBuilders(predicate.Body, source.NodeBuilders, otherSource.NodeBuilders, JoinBuilder.GetLeftJoinBuilder))
                query.AddNodeBuilder(nodeBuilder);
            // 加载去除重复参数名的sql参数列表
            query.AddSqlParameters(source.Parameters.Concat(otherSource.Parameters).DistinctBy(p => p.ParameterName));
            // 加载关联对象属性链接
            query.AddPropertyLinkers(source.PropertyLinkers);
            query.AddPropertyLinker(new PropertyLinker(selector.Body.GetProperty(), otherSource.PropertyLinkers.ToArray()));
            // 返回新的查询数据源
            return query;
        }
        /// <summary>
        /// 创建左连接查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联对象的选择性查询数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>左连接查询数据源</returns>
        public IDbQuery<TEntity> CreateLeftJoinedQuery<TEntity, TOther>(IDbQuery<TEntity> source, IDbSelectedQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class
        {
            // 创建新的查询数据源
            DbQuery<TEntity> query = new DbQuery<TEntity>(_mapperContainer, _commandTreeFactory, _dbHelper, this, _mapperPredicateParserFactory);
            // 加载sql表达式节点列表
            foreach (INodeBuilder nodeBuilder in this.GetJoinedQueryNodeBuilders(predicate.Body, source.NodeBuilders, otherSource.NodeBuilders, JoinBuilder.GetLeftJoinBuilder))
                query.AddNodeBuilder(nodeBuilder);
            // 加载去除重复参数名的sql参数列表
            query.AddSqlParameters(source.Parameters.Concat(otherSource.Parameters).DistinctBy(p => p.ParameterName));
            // 加载关联对象属性链接列表
            query.AddPropertyLinkers(source.PropertyLinkers);
            query.AddPropertyLinker(new PropertyLinker(selector.Body.GetProperty()));
            // 返回新的查询数据源
            return query;
        }
        #endregion
        #region 创建分页查询数据源
        /// <summary>
        /// 创建分页查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="pageSize">每页元素数量</param>
        /// <param name="pageIndex">当前第几页</param>
        /// <returns>分页查询数据源</returns>
        public IDbPagedQuery<TEntity> CreatePagedQuery<TEntity>(IDbQuery<TEntity> source, int pageSize, int pageIndex)
            where TEntity : class
        {
            // 创建分页查询数据源
            DbPagedQuery<TEntity> query = new DbPagedQuery<TEntity>(_mapperContainer, _commandTreeFactory, this.DbHelper)
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            // 加载sql表达式节点列表
            query.AddNodeBuilders(source.NodeBuilders);
            // 加载sql参数列表
            query.AddSqlParameters(source.Parameters);
            // 加载关联对象属性链接列表
            query.AddPropertyLinkers(source.PropertyLinkers);
            // 返回新的查询数据源
            return query;
        }
        /// <summary>
        /// 创建分页查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderBySelector">指定排序属性的表达式</param>
        /// <param name="pageSize">每页元素数量</param>
        /// <param name="pageIndex">当前第几页</param>
        /// <param name="isDesc">是否降序</param>
        /// <returns>分页查询数据源</returns>
        public IDbPagedQuery<TEntity> CreatePagedQuery<TEntity>(IDbQuery<TEntity> source, Expression<Func<TEntity, object>> orderBySelector, int pageSize, int pageIndex, bool isDesc = false)
            where TEntity : class
        {
            //创建分页查询数据源
            DbPagedQuery<TEntity> query = new DbPagedQuery<TEntity>(_mapperContainer, _commandTreeFactory, this.DbHelper)
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            // 加载sql表达式节点列表
            query.AddNodeBuilders(source.NodeBuilders);
            query.AddNodeBuilders(this.GetOrderbyNodeBuilders(orderBySelector, isDesc));
            // 加载sql参数列表
            query.AddSqlParameters(source.Parameters);
            // 加载关联对象属性链接列表
            query.AddPropertyLinkers(source.PropertyLinkers);
            // 返回新的查询数据源
            return query;
        }
        #endregion
        #region 创建包含项或选定项查询数据源
        /// <summary>
        /// 创建查询部分字段的数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TElement">映射类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定查询项表达式</param>
        /// <returns>查询部分字段的数据源</returns>
        public IDbQuery<TEntity> CreateIncludedQuery<TEntity, TElement>(IDbQuery<TEntity> source, Expression<Func<TEntity, TElement>> selector)
            where TEntity : class
        {
            // 获取Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(typeof(TEntity));
            // 创建新的查询数据源
            DbQuery<TEntity> query = new DbQuery<TEntity>(_mapperContainer, _commandTreeFactory, this.DbHelper, this, _mapperPredicateParserFactory);
            // 加载sql表达式节点列表
            query.AddNodeBuilders(this.GetSelectedQueryNodeBuilders(source.NodeBuilders, selector, tableMapper));
            // 加载sql参数列表
            query.AddSqlParameters(source.Parameters);
            // 加载关联对象属性链接列表
            query.AddPropertyLinkers(source.PropertyLinkers);
            // 获取查询数据源
            return query;
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
            // 获取Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(typeof(TEntity));
            // 创建新的查询数据源
            DbSelectedQuery<TElement, TEntity> query = new DbSelectedQuery<TElement, TEntity>(_mapperContainer, _commandTreeFactory, this.DbHelper)
            {
                Convert = selector.Compile()
            };
            // 加载sql表达式节点列表
            query.AddNodeBuilders(this.GetSelectedQueryNodeBuilders(source.NodeBuilders, selector, tableMapper));
            // 加载sql参数列表
            query.AddSqlParameters(source.Parameters);
            // 加载关联对象属性链接列表
            query.AddPropertyLinkers(source.PropertyLinkers);
            // 获取查询数据源
            return query;
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
            // 获取Table元数据解析器
            ITableMapper tableMapper = this._mapperContainer.GetTableMapper(typeof(TEntity));
            // 创建新的查询数据源
            DbDistinctQuery<TElement, TEntity> query = new DbDistinctQuery<TElement, TEntity>(_mapperContainer, _commandTreeFactory, this.DbHelper)
            {
                Convert = selector.Compile()
            };
            // 加载sql表达式节点列表
            query.AddNodeBuilders(this.GetSelectedQueryNodeBuilders(source.NodeBuilders, selector, tableMapper));
            // 加载sql参数列表
            query.AddSqlParameters(source.Parameters);
            // 加载关联对象属性链接列表
            query.AddPropertyLinkers(source.PropertyLinkers);
            // 获取查询数据源
            return query;
        }
        /// <summary>
        /// 创建TOP选定项查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP选定项查询数据源</returns>
        public IDbSelectedQuery<TEntity> CreateTopSelectedQuery<TEntity>(IDbQuery<TEntity> source, int topCount)
            where TEntity : class
        {
            // 获取Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(typeof(TEntity));
            // 创建新的查询数据源
            DbTopSelectedQuery<TEntity, TEntity> query = new DbTopSelectedQuery<TEntity, TEntity>(_mapperContainer, _commandTreeFactory, _dbHelper, topCount)
            {
                Convert = e => e
            };
            // 加载sql表达式节点列表
            query.AddNodeBuilders(source.NodeBuilders);
            // 加载sql参数列表
            query.AddSqlParameters(source.Parameters);
            // 加载关联对象属性链接列表
            query.AddPropertyLinkers(source.PropertyLinkers);
            // 获取查询数据源
            return query;
        }
        /// <summary>
        /// 创建TOP选定项查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TElement">结果类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <param name="selector">转换实体对象到结果对象的表达式</param>
        /// <returns>TOP选定项查询数据源</returns>
        public IDbSelectedQuery<TElement> CreateTopSelectedQuery<TEntity, TElement>(IDbQuery<TEntity> source, int topCount, Expression<Func<TEntity, TElement>> selector)
            where TEntity : class
        {
            // 获取Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(typeof(TEntity));
            // 创建新的查询数据源
            DbTopSelectedQuery<TElement, TEntity> query = new DbTopSelectedQuery<TElement, TEntity>(_mapperContainer, this._commandTreeFactory, this._dbHelper, topCount)
            {
                Convert = selector.Compile()
            };
            // 加载sql表达式节点列表
            query.AddNodeBuilders(this.GetSelectedQueryNodeBuilders(source.NodeBuilders, selector, tableMapper));
            // 加载sql参数列表
            query.AddSqlParameters(source.Parameters);
            // 加载关联对象属性链接列表
            query.AddPropertyLinkers(source.PropertyLinkers);
            // 获取查询数据源
            return query;
        }
        #endregion
        #region 创建sql视图查询数据源
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">对象类型</typeparam>
        /// <param name="querySql">查询sql</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>视图查询数据源</returns>
        public IDbView<TModel> CreateView<TModel>(string querySql, params IDbDataParameter[] parameters)
            where TModel : class, new()
        {
            // 创建数据源
            DbView<TModel> source = new DbView<TModel>(this, _commandTreeFactory, this.DbHelper, _predicateParserFactory, querySql);
            // 添加sql参数
            source.AddSqlParameters(parameters);
            // 获取数据源
            return source;
        }
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图对象</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <returns>新的视图查询数据源</returns>
        public IDbView<TModel> CreateView<TModel>(IDbView<TModel> source)
            where TModel : class, new()
        {
            // 复制数据源
            DbView<TModel> cloned = this.CloneView(source);
            // 获取复制的数据源
            return cloned;
        }
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <param name="source">视图查询数据源</param>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <param name="parameters">sql参数数组</param>
        /// <typeparam name="TModel">视图模型对象</typeparam>
        /// <typeparam name="TProperty">模型属性类型</typeparam>
        /// <returns>新的视图查询数据源</returns>
        public IDbView<TModel> CreateView<TModel, TProperty>(IDbView<TModel> source, Expression<Func<TModel, TProperty>> selector, string sqlPredicate, params IDbDataParameter[] parameters)
            where TModel : class, new()
        {
            // 获取视图查询临时表名
            string tableAlias = typeof(TModel).Name.ToLower();
            // 获取指定的对象成员名称为视图查询映射列名
            string columnName = selector.Body.GetMemberExpression().Member.Name;
            // 获取查询条件表达式节点
            INodeBuilder nodeBuilder = _commandTreeFactory.GetWhereChildBuilder(tableAlias, columnName, sqlPredicate);

            // 复制数据源
            DbView<TModel> cloned = this.CloneView(source);
            // 添加获取查询条件表达式节点
            cloned.AddNodeBuilder(nodeBuilder);
            // 添加sql参数数组
            cloned.AddSqlParameters(parameters);
            // 创建并返回视图查询数据源
            return cloned;
        }
        /// <summary>
        /// 创建视图查询数据源
        /// </summary>
        /// <param name="source">视图查询数据源</param>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlFormat">sql条件格式化字符串</param>
        /// <param name="values">sql参数值数组</param>
        /// <typeparam name="TModel">视图模型对象</typeparam>
        /// <typeparam name="TProperty">模型属性类型</typeparam>
        /// <returns>新的视图查询数据源</returns>
        public IDbView<TModel> CreateView<TModel, TProperty>(IDbView<TModel> source, Expression<Func<TModel, TProperty>> selector, string sqlFormat, params TProperty[] values)
            where TModel : class, new()
        {
            // 获取成员表达式
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            // 获取属性名称为视图查询映射列名
            string columnName = memberExpression.Member.Name;
            // 获取此属性名称开头的参数名的个数
            int count = source.Parameters.Count(p => p.ParameterName.StartsWith(columnName));
            // 初始化参数名称数组
            string[] parameterNames = new string[values.Length];
            // 遍历参数值列表，加载sql参数名称数组
            for (int i = 0; i < values.Length; i++)
                parameterNames[i] = $"{columnName}{(i + count).ToString()}";
            // 获取视图查询临时表名
            string tableAlias = typeof(TModel).Name.ToLower();
            // 获取右边的sql表达式
            string rightSqlExpression = string.Format(sqlFormat, parameterNames);
            // 获取查询条件表达式节点
            INodeBuilder nodeBuilder = _commandTreeFactory.GetWhereChildBuilder(tableAlias, columnName, rightSqlExpression);

            // 复制数据源
            DbView<TModel> cloned = this.CloneView(source);
            // 添加获取查询条件表达式节点
            cloned.AddNodeBuilder(nodeBuilder);
            // 添加sql参数列表
            for (int i = 0; i < parameterNames.Length; i++)
                cloned.AddSqlParameter(parameterNames[i], values[i]);
            // 创建并返回视图查询数据源
            return cloned;
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
            // 复制数据源
            DbView<TModel> cloned = this.CloneView(source);
            // 添加获取查询条件表达式节点
            INodeBuilder nodeBuilder = new NodeBuilder(SqlType.OrderBy, "t.{0} {1}", keySelector.Body.GetMemberExpression().Member.Name, isAsc ? "ASC" : "DESC");
            cloned.AddNodeBuilder(nodeBuilder);
            // 创建并返回视图查询数据源
            return cloned;
        }
        #endregion
    }
}