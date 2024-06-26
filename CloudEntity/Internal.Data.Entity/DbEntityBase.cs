using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 实体查询数据源基类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbEntityBase<TEntity> : DbSortBase
        where TEntity : class
    {
        /// <summary>
        /// 当前对象的关联对象属性链接列表
        /// </summary>
        private readonly IList<PropertyLinker> _propertyLinkers;

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
                    yield return base.CommandFactory.GetColumnBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, columnMapper.ColumnAlias);
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
                    yield return base.CommandFactory.GetColumnBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, columnMapper.ColumnAlias);
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
            return base.CommandFactory.GetOrderByChildBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, isDesc);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbEntityBase(IMapperContainer mapperContainer, ICommandFactory commandFactory, IDbHelper dbHelper)
            : base(commandFactory, dbHelper)
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