using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
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
    /// Apple_Li 李凯 15150598493
    /// 最后修改时间：2023/02/15 22:27
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbQuery<TEntity> : DbEntityBase<TEntity>, IDbQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 创建表达式解析器的工厂
        /// </summary>
        private IPredicateParserFactory _predicateParserFactory;

        /// <summary>
        /// 创建数据操作对象的工厂
        /// </summary>
        public IDbFactory Factory { get; private set; }
        
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
            ITableMapper tableMapper = base.MapperContainer.GetTableMapper(memberExpression.Expression.Type);
            //获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            //获取sql列节点生成器
            return base.CommandTreeFactory.GetColumnBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName);
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
            ITableMapper tableMapper = base.MapperContainer.GetTableMapper(memberExpression.Expression.Type);
            // 获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            // 获取查询条件表达式节点
            return base.CommandTreeFactory.GetWhereChildBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, rightSqlExpression);
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
        /// 解析获取可以添加的关联查询的sql表达式节点列表
        /// </summary>
        /// <param name="nodeBuilders">关联数据源的sql表达式节点列表</param>
        /// <param name="expression">条件表达式主体</param>
        /// <param name="createJoinBuilder">创建joinBuilder的函数</param>
        /// <returns>sql表达式节点列表</returns>
        private IEnumerable<INodeBuilder> GetJoinQueryBuilders(IEnumerable<INodeBuilder> nodeBuilders, Expression expression, Func<TableBuilder, JoinBuilder> createJoinBuilder)
        {
            // 获取现有查询的所有的列名数组
            string[] columnNames = base.NodeBuilders.Where(n => n.BuilderType == BuilderType.Column)
                .Select(n => (n as ColumnBuilder).ColumnName).ToArray();
            // 遍历所有的sql表达式节点
            foreach (INodeBuilder nodeBuilder in nodeBuilders)
            {
                // 若为Column节点
                if (nodeBuilder.BuilderType == BuilderType.Column)
                {
                    // 获取列节点
                    ColumnBuilder columnBuilder = nodeBuilder as ColumnBuilder;
                    // 若列名不重复则获取
                    if (!columnNames.Contains(columnBuilder.ColumnName))
                        yield return columnBuilder;
                }
                // 若为From节点下的子节点
                if (nodeBuilder.ParentNodeType == SqlType.From)
                {
                    // 若不为Table节点
                    if (nodeBuilder.BuilderType != BuilderType.Table)
                    {
                        // 则直接获取
                        yield return nodeBuilder;
                        // 跳过本次循环
                        continue;
                    }
                    // 到这里，确定当前节点为Table节点，先获取Table节点
                    TableBuilder tableBuilder = nodeBuilder as TableBuilder;
                    // 创建Join sql表达式节点
                    JoinBuilder joinBuilder = createJoinBuilder(tableBuilder);
                    // 解析获取join on表达式节点列表
                    foreach (ISqlBuilder sqlBuilder in this.GetJoinedOnNodeBuilders(expression))
                    {
                        // 并添加
                        joinBuilder.OnBuilders.Append(sqlBuilder);
                    }
                    // 获取Join sql表达式节点
                    yield return joinBuilder;
                }
                // 若为Where节点下的子节点,则直接获取
                if (nodeBuilder.ParentNodeType == SqlType.Where)
                    yield return nodeBuilder;
            }
        }
        /// <summary>
        /// 过滤获取可以被添加的sql参数列表
        /// </summary>
        /// <param name="sqlParameters">原sql参数列表</param>
        /// <returns>sql参数列表</returns>
        private IEnumerable<IDbDataParameter> GetFilteredSqlParameters(IEnumerable<IDbDataParameter> sqlParameters)
        {
            // 获取现有查询的sql参数名数组
            string[] parameterNames = base.Parameters.Select(p => p.ParameterName).ToArray();
            // 遍历所有的sql参数列表
            foreach (IDbDataParameter sqlParameter in sqlParameters)
            {
                // 若不重复则获取
                if (!parameterNames.Contains(sqlParameter.ParameterName))
                    yield return sqlParameter;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="dbFactory">创建数据操作对象的工厂</param>
        /// <param name="predicateParserFactory">创建表达式解析器的工厂</param>
        public DbQuery(IMapperContainer mapperContainer, ICommandTreeFactory commandTreeFactory, IDbHelper dbHelper, IDbFactory dbFactory, IPredicateParserFactory predicateParserFactory)
            : base(mapperContainer, commandTreeFactory, dbHelper)
        {
            // 赋值
            _predicateParserFactory = predicateParserFactory;
            this.Factory = dbFactory;
        }
        /// <summary>
        /// 获取sql字符串
        /// </summary>
        /// <returns>sql字符串</returns>
        public string ToSqlString()
        {
            // 获取查询命令生成树
            ICommandTree commandTree = base.CommandTreeFactory.GetQueryTree(base.NodeBuilders);
            // 获取sql命令
            return commandTree.Compile();
        }
        /// <summary>
        /// 为数据源指定需要查询的项（不指定则查询所有项）
        /// </summary>
        /// <param name="selector">指定查询项表达式</param>
        /// <typeparam name="TElement">查询项类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        public IDbQuery<TEntity> SetIncludeBy<TElement>(Expression<Func<TEntity, TElement>> selector)
        {
            // 非空检查
            Check.ArgumentNull(selector, nameof(selector));
            // 为数据源指定需要查询的项（不指定则查询所有项）
            base.SetSelectBy(selector);
            // 再次获取下当前查询数据源方面链式操作
            return this;
        }
        /// <summary>
        /// 为数据源指定需要关联查询的其他数据源
        /// </summary>
        /// <param name="otherSource">关联的数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">关联条件表达式</param>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        public IDbQuery<TEntity> SetJoin<TOther>(IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TOther : class
        {
            // 非空检查
            Check.ArgumentNull(otherSource, nameof(otherSource));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            // 解析获取可以添加的关联查询的sql表达式节点列表
            IEnumerable<INodeBuilder> nodeBuilders = this.GetJoinQueryBuilders(otherSource.NodeBuilders, predicate.Body, JoinBuilder.GetInnerJoinBuilder);
            // 添加sql表达式节点列表
            base.AddNodeBuilders(nodeBuilders);
            // 添加过滤后不重复的sql参数列表
            base.AddSqlParameters(this.GetFilteredSqlParameters(otherSource.Parameters));
            // 添加关联对象属性链接列表
            base.AddPropertyLinker(new PropertyLinker(selector.Body.GetProperty(), otherSource.PropertyLinkers.ToArray()));
            // 再次获取下当前查询数据源方面链式操作
            return this;
        }
        /// <summary>
        /// 为数据源左连接关联其他实体查询数据源
        /// </summary>
        /// <param name="otherSource">关联的数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">关联条件表达式</param>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        public IDbQuery<TEntity> SetLeftJoin<TOther>(IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TOther : class
        {
            // 非空检查
            Check.ArgumentNull(otherSource, nameof(otherSource));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            // 解析获取可以添加的关联查询的sql表达式节点列表
            IEnumerable<INodeBuilder> nodeBuilders = this.GetJoinQueryBuilders(otherSource.NodeBuilders, predicate.Body, JoinBuilder.GetLeftJoinBuilder);
            // 添加sql表达式节点列表
            base.AddNodeBuilders(nodeBuilders);
            // 添加过滤后不重复的sql参数列表
            base.AddSqlParameters(this.GetFilteredSqlParameters(otherSource.Parameters));
            // 添加关联对象属性链接列表
            base.AddPropertyLinker(new PropertyLinker(selector.Body.GetProperty(), otherSource.PropertyLinkers.ToArray()));
            // 再次获取下当前查询数据源方面链式操作
            return this;
        }
        /// <summary>
        /// 设置数据源检索条件
        /// </summary>
        /// <param name="predicate">检索条件表达式</param>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        public IDbQuery<TEntity> SetWhere(Expression<Func<TEntity, bool>> predicate)
        {
            // 非空检查
            Check.ArgumentNull(predicate, nameof(predicate));
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
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlPredicate">sql条件</param>
        /// <param name="sqlParameters">sql参数数组</param>
        /// <typeparam name="TProperty">实体属性类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        public IDbQuery<TEntity> SetWhere<TProperty>(Expression<Func<TEntity, TProperty>> selector, string sqlPredicate, params IDbDataParameter[] sqlParameters)
        {
            // 获取成员表达式
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            // 获取查询条件表达式节点
            INodeBuilder nodeBuilder = this.GetWhereChildBuilder(memberExpression, sqlPredicate);
            // 添加sql表达式节点
            base.AddNodeBuilder(nodeBuilder);
            // 添加sql参数数组
            base.AddSqlParameters(sqlParameters);
            // 再次获取下当前查询数据源方面链式操作
            return this;
        }
        /// <summary>
        /// 设置数据源数据检索条件
        /// </summary>
        /// <param name="selector">指定对象成员表达式</param>
        /// <param name="sqlFormat">sql条件格式化字符串</param>
        /// <param name="values">sql参数值数组</param>
        /// <typeparam name="TProperty">实体属性类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        public IDbQuery<TEntity> SetWhere<TProperty>(Expression<Func<TEntity, TProperty>> selector, string sqlFormat, params TProperty[] values)
        {
            // 获取成员表达式
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            // 获取属性名称
            string memberName = memberExpression.Member.Name;
            // 获取此属性名称开头的使用次数
            int times = base.Parameters.Count(p => p.ParameterName.StartsWith(memberName));
            // 初始化参数名称数组
            string[] parameterNames = new string[values.Length];
            // 遍历参数值列表，加载sql参数名称数组
            for (int i = 0; i < values.Length; i++)
                parameterNames[i] = $"{memberName}{(i + times).ToString()}";
            // 获取右边的sql表达式
            string rightSqlExpression = string.Format(sqlFormat, parameterNames);
            // 获取查询条件表达式节点
            INodeBuilder nodeBuilder = this.GetWhereChildBuilder(memberExpression, rightSqlExpression);
            
            // 添加sql表达式节点
            base.AddNodeBuilder(nodeBuilder);
            // 添加sql参数列表
            for (int i = 0; i < parameterNames.Length; i++)
                base.AddSqlParameter(parameterNames[i], values[i]);
            // 再次获取下当前查询数据源方面链式操作
            return this;
        }
        /// <summary>
        /// 为数据源设置排序条件
        /// </summary>
        /// <param name="keySelector">指定排序项的表达式</param>
        /// <param name="isDesc">是否为降序</param>
        /// <typeparam name="TKey">排序项类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        public IDbQuery<TEntity> SetSort<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool isDesc = false)
        {
            // 设置排序条件
            base.SetOrderBy(keySelector, isDesc);
            // 再次获取下当前查询数据源方面链式操作
            return this;
        }
        /// <summary>
        /// 为数据源重新设置排序条件（之前的排序条件会被清空）
        /// </summary>
        /// <param name="keySelector">指定排序项的表达式</param>
        /// <param name="isDesc">是否为降序</param>
        /// <typeparam name="TKey">排序项类型</typeparam>
        /// <returns>数据源（还是原来的数据源并未复制）</returns>
        public IDbQuery<TEntity> SetSortBy<TKey>(Expression<Func<TEntity, TKey>> keySelector, bool isDesc = false)
        {
            // 清空之前的sql排序表达式节点
            base.RemoveNodeBuilders(SqlType.OrderBy);
            // 设置排序条件
            base.SetOrderBy(keySelector, isDesc);
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
            string commandText = this.ToSqlString();
            // 执行查询获取映射对象迭代器
            return base.DbHelper.GetResults(base.GetModels<TModel>, commandText, parameters: base.Parameters.ToArray());
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            // 获取sql命令
            string commandText = this.ToSqlString();
            // 执行查询获取实体对象列表并遍历
            foreach (TEntity entity in base.DbHelper.GetResults(base.GetEntities, commandText, parameters: base.Parameters.ToArray()))
            {
                // 依次获取实体对象
                yield return entity;
            }
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
