using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 实体查询数据源基类
    /// Apple_Li 李凯 15150598493
    /// 2023/02/17 17:16 最后修改时间：2023/02/18 22:07
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbEntityBase<TEntity> : DbSortBase
        where TEntity : class
    {
        /// <summary>
        /// 当前对象的关联对象属性链接列表
        /// </summary>
        private IList<PropertyLinker> _propertyLinkers;

        /// <summary>
        /// Mapper容器
        /// </summary>
        protected IMapperContainer MapperContainer { get; private set; }

        /// <summary>
        /// 当前对象的关联对象属性链接列表
        /// </summary>
        public IEnumerable<PropertyLinker> PropertyLinkers 
        {
            get { return _propertyLinkers; }
        }

        /// <summary>
        /// 获取获取关联访问链接
        /// </summary>
        /// <param name="propertyLinker">属性关联对象链接</param>
        /// <param name="columnNames">查询的列名数组</param>
        /// <returns>关联访问链接</returns>
        private AccessorLinker GetAccessorLinker(PropertyLinker propertyLinker, string[] columnNames)
        {
            // 获取子联访问链接集合
            AccessorLinker[] accessorLinkers = propertyLinker.RelationalLinkers.Select(p => this.GetAccessorLinker(p, columnNames)).ToArray();
            // 获取查询的列名对应的ColumnMapper对象数组
            IColumnMapper[] columnMappers = this.GetColumnMappers(propertyLinker.Property.PropertyType, columnNames).ToArray();
            // 创建并获取关联访问链接
            return new AccessorLinker(propertyLinker.Property, columnMappers, accessorLinkers);
        }
        /// <summary>
        /// 为实体对象属性赋值
        /// </summary>
        /// <param name="entityAccessor">实体对象创建访问对象</param>
        /// <param name="reader">数据流</param>
        /// <param name="columnMappers">记录对象属性和查询列映射的ColumnMapper对象数组</param>
        /// <param name="entity">实体对象</param>
        private void FillEntity(ObjectAccessor entityAccessor, IDataReader reader, IColumnMapper[] columnMappers, object entity)
        {
            // 遍历属性与列名映射对象数组
            foreach (IColumnMapper columnMapper in columnMappers)
            {
                // 获取查询列的列名
                string selectColumnName = columnMapper.ColumnAlias ?? columnMapper.ColumnName;
                // 获取值
                object value = reader[selectColumnName];
                // 若当前列值为空,则跳过,不赋值
                if (value is DBNull)
                    continue;
                // 为entity当前属性赋值
                entityAccessor.SetValue(columnMapper.Property.Name, entity, value);
            }
        }
        /// <summary>
        /// 创建实体对象并读取DataReader对其填值
        /// </summary>
        /// <param name="entityAccessor">实体对象创建访问对象</param>
        /// <param name="reader">数据流</param>
        /// <param name="columnMappers">记录对象属性和查询列映射的ColumnMapper对象数组</param>
        /// <param name="accessorLinkers">对象访问关联对象数组</param>
        /// <returns>实体对象</returns>
        private object CreateEntity(ObjectAccessor entityAccessor, IDataReader reader, IColumnMapper[] columnMappers, AccessorLinker[] accessorLinkers)
        {
            // 创建当前实体对象
            object entity = entityAccessor.CreateInstance();
            // 为实体对象基本属性赋值
            this.FillEntity(entityAccessor, reader, columnMappers, entity);
            // 遍历对象访问关联对象数组并为关联对象属性赋值
            foreach (AccessorLinker accessorLinker in accessorLinkers)
            {
                // 创建关联实体对象
                object linkEntity = this.CreateEntity(accessorLinker.EntityAccessor, reader, accessorLinker.ColumnMappers, accessorLinker.AccessorLinkers);
                // 为当前实体对象的关联对象属性赋值
                entityAccessor.SetValue(accessorLinker.PropertyName, entity, linkEntity);
            }
            // 获取当前实体对象
            return entity;
        }
        /// <summary>
        /// 创建获取映射对象并读取DataReader对其填值
        /// </summary>
        /// <param name="modelAccessor">映射对象属性访问器</param>
        /// <param name="reader">数据流</param>
        /// <param name="columnMappers">记录查询对象属性名和查询列名的ColumnMapper数组</param>
        /// <typeparam name="TModel">映射的模型对象类型</typeparam>
        /// <returns>映射的模型对象</returns>
        private TModel CreateModel<TModel>(ObjectAccessor modelAccessor, IDataReader reader, IColumnMapper[] columnMappers)
            where TModel : class, new()
        {
            // 创建对象
            TModel model = new TModel();
            // 遍历ColumnMapper对象数组，为对象属性赋值
            foreach (IColumnMapper columnMapper in columnMappers)
            {
                //获取查询列的列名
                string selectColumnName = columnMapper.ColumnAlias ?? columnMapper.ColumnName;
                //获取值
                object value = reader[selectColumnName];
                //若当前列值为空,则跳过,不赋值
                if (value is DBNull)
                    continue;
                //为model当前属性赋值
                modelAccessor.TrySetValue(columnMapper.Property.Name, model, value);
            }
            // 最后获取对象
            return model;
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
                    yield return base.CommandTreeFactory.GetColumnBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, columnMapper.ColumnAlias);
                }
            }
        }
        /// <summary>
        /// 获取查询命令生成树Select节点的子节点集合
        /// </summary>
        /// <param name="selector">选定查询项表达式</param>
        /// <param name="tableMapper">Table元数据解析器</param>
        /// <returns>查询命令生成树Select节点的子节点集合</returns>
        private IEnumerable<INodeBuilder> GetSelectChildBuilders(LambdaExpression selector, ITableMapper tableMapper)
        {
            //解析Lambda Select表达式为nodeBuilders添加父类型为Select的子sql表达式节点
            switch (selector.Body.NodeType)
            {
                //解析转换表达式及其成员表达式(e => e.Property1)
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                    //获取成员表达式
                    MemberExpression memberExpression = selector.Body.GetMemberExpression();
                    //获取columnMapper
                    IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
                    //依次获取column节点生成器
                    yield return base.CommandTreeFactory.GetColumnBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, columnMapper.ColumnAlias);
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
        }
        /// <summary>
        /// 获取查询的列名对应的所有ColumnMapper对象列表
        /// </summary>
        /// <param name="entityType">当前关联的实体类型</param>
        /// <param name="columnNames">查询的列名数组</param>
        /// <returns>查询的列名对应的所有ColumnMapper对象列表</returns>
        private IEnumerable<IColumnMapper> GetColumnMappers(Type entityType, string[] columnNames)
        {
            // 获取当前实体类型的Mapper对象
            ITableMapper tableMapper = this.MapperContainer.GetTableMapper(entityType);
            // 遍历当前实体类型所有ColumnMapper列表
            foreach (IColumnMapper columnMapper in tableMapper.GetColumnMappers())
            {
                // 获取查询列名
                string selectName = columnMapper.ColumnAlias ?? columnMapper.ColumnName;
                // 若查询的列名数组包含当前的查询列名
                if (columnNames.Contains(selectName))
                {
                    // 则直接获取当前的ColumnMapper
                    yield return columnMapper;
                }
            }
        }
        /// <summary>
        /// 获取查询的列名对应的所有ColumnMapper对象列表
        /// </summary>
        /// <param name="entityType">当前关联的实体类型</param>
        /// <param name="propertyLinkers">关联对象属性链接列表</param>
        /// <param name="columnNames">查询的列名数组</param>
        /// <returns>查询的列名对应的所有ColumnMapper对象列表</returns>
        private IEnumerable<IColumnMapper> GetColumnMappers(Type entityType, IEnumerable<PropertyLinker> propertyLinkers, string[] columnNames)
        {
            // 遍历当前实体类型下的所有查询列对应的ColumnMapper列表
            foreach (IColumnMapper columnMapper in this.GetColumnMappers(entityType, columnNames))
            {
                // 并依次获取ColumnMapper对象
                yield return columnMapper;
            }
            // 遍历关联对象属性链接列表
            foreach (PropertyLinker propertyLinker in propertyLinkers)
            {
                // 获取当前关联实体对象的类型
                Type linkedEntityType = propertyLinker.Property.PropertyType;
                // 获取关联对象下的查询的列名对应的所有ColumnMapper对象列表
                foreach (IColumnMapper columnMapper in this.GetColumnMappers(linkedEntityType, propertyLinker.RelationalLinkers, columnNames))
                {
                    // 并依次获取ColumnMapper对象
                    yield return columnMapper;
                }
            }
        }

        /// <summary>
        /// 获取orderby节点的子节点
        /// </summary>
        /// <param name="memberExpression">成员表达式</param>
        /// <param name="isDesc">是否降序[true:降序 false:升序]</param>
        /// <returns>orderby节点的子节点</returns>
        protected override INodeBuilder GetOrderbyNodeBuilder(MemberExpression memberExpression, bool isDesc)
        {
            //获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = this.MapperContainer.GetTableMapper(memberExpression.Expression.Type);
            //获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            //获取不使用别名的OrderBy节点的子表达式(排序时，禁止使用别名)
            return base.CommandTreeFactory.GetOrderByChildBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, isDesc);
        }
        /// <summary>
        /// 读取DataReader获取实体对象列表
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询的列名数组</param>
        /// <returns>实体对象列表</returns>
        protected IEnumerable<TEntity> GetEntities(IDataReader reader, string[] columnNames)
        {
            // 获取实体类型
            Type entityType = typeof(TEntity);
            // 获取对象访问器
            ObjectAccessor entityAccessor = ObjectAccessor.GetAccessor(entityType);
            // 获取当前对象的查询列对应的ColumnMapper数组
            IColumnMapper[] columnMappers = this.GetColumnMappers(entityType, columnNames).ToArray();
            // 获取对象访问关联对象数组
            AccessorLinker[] accessorLinkers = this.PropertyLinkers.Select(l => this.GetAccessorLinker(l, columnNames)).ToArray();
            // 读取DataReader
            while (reader.Read())
            {
                // 创建实体对象并填值
                TEntity entity = this.CreateEntity(entityAccessor, reader, columnMappers, accessorLinkers) as TEntity;
                // 依次获取实体对象
                yield return entity;
            }
        }
        /// <summary>
        /// 读取DataReader获取映射的模型对象列表
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询的列名数组</param>
        /// <typeparam name="TModel">映射的模型对象类型</typeparam>
        /// <returns>映射的模型对象列表</returns>
        protected IEnumerable<TModel> GetModels<TModel>(IDataReader reader, string[] columnNames)
            where TModel : class, new()
        {
            // 获取查询的列名对应的所有ColumnMapper对象数组
            IColumnMapper[] columnMappers = this.GetColumnMappers(typeof(TEntity), this.PropertyLinkers, columnNames).ToArray();
            // 获取对象访问器
            ObjectAccessor modelAccessor = ObjectAccessor.GetAccessor(typeof(TModel));
            // 读取DataReader
            while (reader.Read())
            {
                // 依次获取对象
                yield return this.CreateModel<TModel>(modelAccessor, reader, columnMappers);
            }
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbEntityBase(IMapperContainer mapperContainer, ICommandTreeFactory commandTreeFactory, DbHelper dbHelper)
            : base(commandTreeFactory, dbHelper)
        {
            // 初始化
            _propertyLinkers = new List<PropertyLinker>();
            // 赋值
            this.MapperContainer = mapperContainer;
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
        /// 为数据源指定需要查询的项（不指定则查询所有项）
        /// </summary>
        /// <param name="selector">指定查询项表达式</param>
        public void SetSelectBy<TElement>(Expression<Func<TEntity, TElement>> selector)
        {
            // 获取Table元数据解析器
            ITableMapper tableMapper = this.MapperContainer.GetTableMapper(typeof(TEntity));
            // 获取选定项sql表达式节点列表 （作为查询命令生成树的Select节点的子节点列表）
            IEnumerable<INodeBuilder> selectChildBuilders = this.GetSelectChildBuilders(selector, tableMapper);
            // 清空原先的作为查询命令生成树的Select节点的子节点列表
            base.RemoveNodeBuilders(SqlType.Select);
            // 添加新的选定项sql表达式节点列表 （作为查询命令生成树的Select节点的子节点列表）
            base.AddNodeBuilders(selectChildBuilders);
        }
        /// <summary>
        /// 为数据源指定排序条件
        /// </summary>
        /// <param name="keySelector">指定排序项的表达式</param>
        /// <param name="isDesc">是否为降序</param>
        /// <typeparam name="TKey">排序项类型</typeparam>
        public void SetOrderBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool isDesc = false)
        {
            // 非空检查
            Check.ArgumentNull(keySelector, nameof(keySelector));
            // 获取orderby节点的子节点集合
            IEnumerable<INodeBuilder> nodeBuilders = base.GetOrderbyNodeBuilders(keySelector, isDesc);
            // 添加orderby节点的子节点集合
            base.AddNodeBuilders(nodeBuilders);
        }
    }
}