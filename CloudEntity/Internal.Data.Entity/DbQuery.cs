using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Internal.CommandTrees;
using CloudEntity.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 查询数据源
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbQuery<TEntity> : DbQueryBase<TEntity>, IDbQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 对象访问器
        /// </summary>
        private ObjectAccessor _objectAccessor;
        /// <summary>
        /// 创建表达式解析器的工厂
        /// </summary>
        private IPredicateParserFactory _predicateParserFactory;
        /// <summary>
        /// 当前对象的关联对象属性链接列表
        /// </summary>
        private IList<PropertyLinker> _propertyLinkers;

        /// <summary>
        /// 创建数据操作对象的工厂
        /// </summary>
        public IDbFactory Factory { get; private set; }
        /// <summary>
        /// 当前对象的关联对象属性链接列表
        /// </summary>
        public IEnumerable<PropertyLinker> PropertyLinkers 
        {
            get { return _propertyLinkers; }
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
            ITableMapper tableMapper = base.MapperContainer.GetTableMapper(memberExpression.Expression.Type);
            // 获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            // 获取查询条件表达式节点
            return base.CommandTreeFactory.GetWhereChildBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, rightSqlExpression);
        }
        /// <summary>
        /// 创建实体对象
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询的列</param>
        /// <returns>实体对象</returns>
        private object CreateEntity(IDataReader reader, string[] columnNames)
        {
            ITableMapper tableMapper = base.MapperContainer.GetTableMapper(typeof(TEntity));
            AccessorLinker[] accessorLinkers = this.PropertyLinkers.Select(l => l.ToAccessorLinker(base.MapperContainer)).ToArray();
            return _objectAccessor.CreateEntity(tableMapper, reader, columnNames, accessorLinkers);
        }

        /// <summary>
        /// 获取所有的sql表达式节点
        /// </summary>
        /// <returns>所有的sql表达式节点</returns>
        protected virtual IEnumerable<INodeBuilder> GetNodeBuilders()
        {
            return base.NodeBuilders;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="dbFactory">创建数据操作对象的工厂</param>
        /// <param name="predicateParserFactory">创建表达式解析器的工厂</param>
        public DbQuery(IMapperContainer mapperContainer, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper, IDbFactory dbFactory, IPredicateParserFactory predicateParserFactory)
            : base(mapperContainer, commandTreeFactory, dbHelper)
        {
            // 初始化
            _objectAccessor = ObjectAccessor.GetAccessor(typeof(TEntity));
            _propertyLinkers = new List<PropertyLinker>();
            // 赋值
            _predicateParserFactory = predicateParserFactory;
            this.Factory = dbFactory;
        }
        /// <summary>
        /// 添加关联的对象属性链接
        /// </summary>
        /// <param name="propertyLinker">关联的对象属性链接</param>
        public void AddPropertyLinker(PropertyLinker propertyLinker)
        {
            _propertyLinkers.Add(propertyLinker);
        }
        /// <summary>
        /// 添加关联的对象属性链接列表
        /// </summary>
        /// <param name="propertyLinkers">关联的对象属性链接列表</param>
        public void AddPropertyLinkers(IEnumerable<PropertyLinker> propertyLinkers)
        {
            foreach (PropertyLinker propertyLinker in propertyLinkers)
                _propertyLinkers.Add(propertyLinker);
        }
        /// <summary>
        /// 获取sql字符串
        /// </summary>
        /// <returns>sql字符串</returns>
        public string ToSqlString()
        {
            //获取sql命令
            return base.CommandTreeFactory.GetQueryTree(this.GetNodeBuilders()).Compile();
        }
        /// <summary>
        /// 设置数据源检索条件
        /// </summary>
        /// <param name="predicate">检索条件表达式</param>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        public IDbQuery<TEntity> SetWhere(Expression<Func<TEntity, bool>> predicate)
        {
            // 解析Lambda表达式，获取并添加Sql表达式节点，并添加附带的sql参数
            foreach (INodeBuilder sqlBuilder in _predicateParserFactory.GetWhereChildBuilders(this, predicate.Parameters[0], predicate.Body))
            {
                // （解析时，已自动为当前数据源添加解析得到的sql参数，只需要）添加sql表达式节点
                base.AddNodeBuilder(sqlBuilder);
            }
            // 再次获取下当前查询数据源方面链式操作
            return this;
        }
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <typeparam name="TProperty">实体属性类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        public IDbQuery<TEntity> SetWhere<TProperty>(Expression<Func<TEntity, TProperty>> selector, string sqlPredicate)
        {
            // 获取成员表达式
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            // 获取查询条件表达式节点
            INodeBuilder nodeBuilder = this.GetWhereChildBuilder(memberExpression, sqlPredicate);
            // 添加sql表达式节点
            base.AddNodeBuilder(nodeBuilder);
            // 再次获取下当前查询数据源方面链式操作
            return this;
        }
        /// <summary>
        /// 将此数据源的查询结果映射为TModel对象数据源
        /// </summary>
        /// <typeparam name="TModel">TModel对象（只要是有无参构造函数的类就可以）</typeparam>
        /// <returns>TModel对象数据源</returns>
        public IEnumerable<TModel> Cast<TModel>()
            where TModel : class, new()
        {
            // 获取sql命令
            string commandText = base.CommandTreeFactory.GetQueryTree(this.GetNodeBuilders()).Compile();
            // 执行查询
            return base.DbHelper.GetModels<TModel>(commandText, base.Parameters.ToArray());
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            // 获取sql命令
            string commandText = base.CommandTreeFactory.GetQueryTree(this.GetNodeBuilders()).Compile();
            // 执行查询
            foreach (TEntity entity in base.DbHelper.GetResults(this.CreateEntity, commandText, parameters: base.Parameters.ToArray()))
                yield return entity;
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
