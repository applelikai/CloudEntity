using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Internal.CommandTrees;
using CloudEntity.Internal.Data.Entity;
using CloudEntity.Mapping;
using CloudEntity.Mapping.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace CloudEntity.Core.Data.Entity
{
    /// <summary>
    /// 数据容器类
    /// [Apple_Li 李凯 15150598493]
    /// </summary>
    public sealed class DbContainer : IDbContainer, IDbFactory
    {
        /// <summary>
        /// 访问DbList字典的线程锁
        /// </summary>
        private readonly object _dbListsLocker;
        /// <summary>
        /// 操作数据库的DbHelper
        /// </summary>
        private readonly IDbHelper _dbHelper;
        /// <summary>
        /// Table初始化器
        /// </summary>
        private readonly TableInitializer _tableInitializer;
        /// <summary>
        /// 列初始化器
        /// </summary>
        private readonly ColumnInitializer _columnInitializer;
        /// <summary>
        /// Sql命令工厂
        /// </summary>
        private readonly ICommandFactory _commandFactory;
        /// <summary>
        /// Mapper容器
        /// </summary>
        private readonly IMapperContainer _mapperContainer;
        /// <summary>
        /// 创建表达式解析器的工厂
        /// </summary>
        private readonly IPredicateParserFactory _predicateParserFactory;
        /// <summary>
        /// 创建查询条件解析器的工厂
        /// </summary>
        private readonly IPredicateParserFactory _mapperPredicateParserFactory;
        /// <summary>
        /// 数据列表字典
        /// </summary>
        private readonly IDictionary<Type, object> _dbLists;
        /// <summary>
        /// 控制字典缓存的线程锁
        /// </summary>
        private readonly static object _containersLocker;
        /// <summary>
        /// 数据容器字典
        /// </summary>
        private readonly static IDictionary<string, IDbContainer> _containers;

        /// <summary>
        /// 操作数据库的DbHelper
        /// </summary>
        public IDbHelper DbHelper
        {
            get { return _dbHelper; }
        }
        
        /// <summary>
        /// 复制来源数据源信息到新的实体查询数据源
        /// </summary>
        /// <param name="source">来源数据源</param>
        /// <param name="cloned">新的实体查询数据源</param>
        /// <typeparam name="TEntity">实体类型</typeparam>
        private void Clone<TEntity>(IDbQuery<TEntity> source, DbEntityBase<TEntity> cloned)
            where TEntity : class
        {
            // 复制sql表达式节点列表
            cloned.AddNodeBuilders(source.NodeBuilders);
            // 复制sql参数列表
            cloned.AddSqlParameters(source.Parameters);
            // 复制关联的对象链接列表
            cloned.AddPropertyLinkers(source.PropertyLinkers);
        }
        /// <summary>
        /// 复制来源数据源信息到新的实体查询数据源
        /// </summary>
        /// <param name="source">来源数据源</param>
        /// <param name="cloned">新的实体查询数据源</param>
        private void Clone(IDbBase source, DbScalar cloned)
        {
            // 遍历sql表达式节点列表
            foreach (INodeBuilder nodeBuilder in source.NodeBuilders)
            {
                switch (nodeBuilder.ParentNodeType)
                {
                    // 只复制FORM节点和WHERE节点下的子节点
                    case SqlType.From:
                    case SqlType.Where:
                        cloned.AddNodeBuilder(nodeBuilder);
                        break;
                }
            }
            // 复制sql参数列表
            cloned.AddSqlParameters(source.Parameters);
        }

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
        /// <returns>数据容器</returns>
        public static IDbContainer Get(string connectionString)
        {
            // 非空检查
            Check.ArgumentNull(connectionString, nameof(connectionString));
            // 若字典中包含当前连接字符串的数据容器则直接获取
            if (_containers.ContainsKey(connectionString))
                return _containers[connectionString];
            // 若字典中不包含当前连接字符串的数据容器，则扔出异常
            throw new Exception($"没有找到{connectionString}下的数据容器");
        }

        /// <summary>
        /// 创建数据容器
        /// </summary>
        /// <param name="connectionString">连接字符串</param>
        /// <param name="initializer">初始化器</param>
        private DbContainer(string connectionString, DbInitializer initializer)
        {
            //非空检查
            Check.ArgumentNull(connectionString, nameof(connectionString));
            Check.ArgumentNull(initializer, nameof(initializer));
            //初始化线程锁
            _dbListsLocker = new object();
            //赋值
            _dbHelper = initializer.CreateDbHelper(connectionString);
            _commandFactory = initializer.CreateCommandFactory();
            _mapperContainer = initializer.CreateMapperContainer();
            _tableInitializer = initializer.CreateTableInitializer(_dbHelper, _commandFactory);  //获取Table初始化器
            _columnInitializer = initializer.CreateColumnInitializer(_dbHelper, _commandFactory);//获取列初始化器
            _dbLists = new Dictionary<Type, object>();                 //初始化DbList字典
            _predicateParserFactory = new PredicateParserFactory(_commandFactory);
            _mapperPredicateParserFactory = new PredicateParserFactory(_commandFactory, _mapperContainer);
        }

        /// <summary>
        /// 初始化某实体类所Mapping的Table
        /// </summary>
        /// <param name="entityType">某实体类的类型</param>
        public void InitTable(Type entityType)
        {
            // 检查Table初始化器是否为空,直接退出
            if (_tableInitializer == null)
                return;
            // 获取TableMapper
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(entityType);
            // 若为视图映射，则直接退出
            if (tableMapper is ViewMapper)
                return;
            // 若Table不存在,则创建Table
            if (!_tableInitializer.IsExist(tableMapper.Header))
                _tableInitializer.CreateTable(tableMapper);
            // 若Table存在，则检查并添加EntityMapper中某些属性未注册到目标Table的列
            else
            {
                // 若列初始化器不为空
                if (_columnInitializer != null)
                    // 检查并添加EntityMapper中某些属性未注册到目标Table的列
                    _columnInitializer.AlterTableAddColumns(tableMapper);
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
            if (_tableInitializer == null)
                return;
            //获取TableMapper
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(entityType);
            //若Table未生成,则从旧表修改
            if (!_tableInitializer.IsExist(tableMapper.Header))
                _tableInitializer.RenameTable(tableMapper.Header, oldTableName);
        }
        /// <summary>
        /// 删除实体类Mapping的表
        /// </summary>
        /// <param name="entityType">实体类型</param>
        public void DropTable(Type entityType)
        {
            //检查Table初始化器是否为空,直接退出
            if (_tableInitializer == null)
                return;
            //获取TableMapper
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(entityType);
            //若Table存在,则删除Table
            if (_tableInitializer.IsExist(tableMapper.Header))
                _tableInitializer.DropTable(tableMapper.Header);
        }
        /// <summary>
        /// 创建事故执行器
        /// </summary>
        /// <returns>事故执行器</returns>
        public IDbExecutor CreateExecutor()
        {
            return new DbExecutor(this, this.DbHelper, _commandFactory, _mapperContainer);
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
                    IDbList<TEntity> entities = new DbList<TEntity>(this, this.DbHelper, _commandFactory, _mapperContainer);
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
            // 创建统计查询数据源对象
            DbScalar cloned = new DbScalar(_commandFactory, this.DbHelper);
            // 复制来源数据源信息到新的统计查询数据源数据源
            this.Clone(dbBase, cloned);
            // 添加Sql函数表达式节点
            cloned.AddNodeBuilder(new NodeBuilder(SqlType.Select, "{0}(*)", functionName));
            // 获取复制的统计查询数据源
            return cloned;
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
            // 获取成员表达式
            MemberExpression memberExpression = lambdaExpression.Body.GetMemberExpression();
            // 获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(memberExpression.Expression.Type);
            // 获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            // 获取统计表达式
            INodeBuilder selectBuilder = _commandFactory.GetFunctionNodeBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, functionName);

            // 创建统计查询数据源对象
            DbScalar cloned = new DbScalar(_commandFactory, this.DbHelper);
            // 添加统计表达式
            cloned.AddNodeBuilder(selectBuilder);
            // 复制来源数据源信息到新的统计查询数据源数据源
            this.Clone(dbBase, cloned);
            // 获取复制的统计查询数据源
            return cloned;
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
            return this.CreateQuery(source);
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
            // 创建查询数据源
            DbQuery<TEntity> cloned = new DbQuery<TEntity>(_mapperContainer, _commandFactory, _dbHelper, this, _mapperPredicateParserFactory);
            // 复制sql表达式节点列表
            cloned.AddNodeBuilders(source.NodeBuilders);
            // 复制sql参数列表
            cloned.AddSqlParameters(source.Parameters);
            // 获取查询数据源
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
            // 创建查询数据源
            DbQuery<TEntity> cloned = new DbQuery<TEntity>(_mapperContainer, _commandFactory, _dbHelper, this, _mapperPredicateParserFactory);
            // 复制来源数据源信息到新的实体查询数据源
            this.Clone(source, cloned);
            // 获取查询数据源
            return cloned;
        }
        /// <summary>
        /// 创建TOP实体查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP实体查询数据源</returns>
        public IDbTopQuery<TEntity> CreateTopQuery<TEntity>(IDbQuery<TEntity> source, int topCount)
            where TEntity : class
        {
            // 创建新的查询数据源
            DbTopQuery<TEntity> cloned = new DbTopQuery<TEntity>(_mapperContainer, _commandFactory, this.DbHelper, topCount);
            // 复制来源数据源信息到TOP实体查询数据源
            this.Clone(source, cloned);
            // 最后获取复制的数据源
            return cloned;
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
            // 构建分页查询数据源
            DbPagedQuery<TEntity> cloned = new DbPagedQuery<TEntity>(_mapperContainer, _commandFactory, this.DbHelper)
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            // 复制来源数据源到分页查询数据源
            this.Clone(source, cloned);
            // 获取分页查询数据源
            return cloned;
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
            // 构建分页查询数据源
            DbPagedQuery<TEntity> cloned = new DbPagedQuery<TEntity>(_mapperContainer, _commandFactory, this.DbHelper)
            {
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            // 复制来源数据源到分页查询数据源
            this.Clone(source, cloned);
            // 为分页查询数据源设置排序条件
            cloned.SetOrderBy(orderBySelector, isDesc);
            // 获取分页查询数据源
            return cloned;
        }
        #endregion
        #region 创建包含项或选定项查询数据源
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
            // 构建选定项查询数据源
            DbSelectedQuery<TElement, TEntity> cloned = new DbSelectedQuery<TElement, TEntity>(_mapperContainer, _commandFactory, this.DbHelper);
            // 复制来源数据源信息到选定项查询数据源
            this.Clone(source, cloned);
            // 为复制的数据源指定要查询的选定项
            cloned.SetSelectBy(selector);
            // 指定转换实体对象为TElement类型的委托
            cloned.Convert = selector.Compile();
            // 最后获取复制的数据源
            return cloned;
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
            // 构建去除重复项查询数据源
            DbDistinctQuery<TElement, TEntity> cloned = new DbDistinctQuery<TElement, TEntity>(_mapperContainer, _commandFactory, this.DbHelper);
            // 复制来源数据源信息到去除重复项查询数据源
            this.Clone(source, cloned);
            // 为复制的数据源指定要查询的选定项
            cloned.SetSelectBy(selector);
            // 指定转换实体对象为TElement类型的委托
            cloned.Convert = selector.Compile();
            // 最后获取复制的数据源
            return cloned;
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
            // 创建新的查询数据源
            DbTopSelectedQuery<TElement, TEntity> cloned = new DbTopSelectedQuery<TElement, TEntity>(_mapperContainer, _commandFactory, this.DbHelper, topCount);
            // 复制来源数据源信息到去除重复项查询数据源
            this.Clone(source, cloned);
            // 为复制的数据源指定要查询的选定项
            cloned.SetSelectBy(selector);
            // 指定转换实体对象为TElement类型的委托
            cloned.Convert = selector.Compile();
            // 最后获取复制的数据源
            return cloned;
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
        public IDbAsView<TModel> CreateView<TModel>(string querySql, params IDbDataParameter[] parameters)
            where TModel : class, new()
        {
            // 创建数据源
            DbAsView<TModel> source = new DbAsView<TModel>(this, _commandFactory, this.DbHelper, _predicateParserFactory, querySql);
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
        public IDbAsView<TModel> CreateView<TModel>(IDbAsView<TModel> source)
            where TModel : class, new()
        {
            // 创建新的视图数据源
            DbAsView<TModel> cloned = new DbAsView<TModel>(this, _commandFactory, this.DbHelper, _predicateParserFactory, source.InnerQuerySql);
            // 复制sql表达式节点列表
            cloned.AddNodeBuilders(source.NodeBuilders);
            // 复制sql参数列表
            cloned.AddSqlParameters(source.Parameters);
            // 获取复制的数据源
            return cloned;
        }
        #endregion
    }
}