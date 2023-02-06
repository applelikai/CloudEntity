using System;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 查询扩展类
    /// 李凯 Apple_Li 15150598493
    /// 最后修改时间：2023/02/05
    /// </summary>
    public static class ExtendQuery
    {
        /// <summary>
        /// Extendable method: 过滤数据源中属性包含(或不包含)某些值的实体(生成Sql IN表达式)
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <typeparam name="TProperty">对象属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性的表达式</param>
        /// <param name="values">对象属性所包含的值</param>
        /// <param name="isIn">IN 或 NOT IN</param>
        /// <returns>新的数据源</returns>
        public static IDbQuery<TEntity> In<TEntity, TProperty>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TProperty>> selector, TProperty[] values, bool isIn = true)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(values, nameof(values));
            //获取sql表达式模板
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            StringBuilder sqlTemplate = new StringBuilder();
            sqlTemplate.AppendLine(isIn ? "IN " : "NOT IN ");
            for (int i = 0; i < values.Length; i++)
            {
                sqlTemplate.AppendFormat("\t{0}", i == 0 ? '(' : ' ');
                sqlTemplate.AppendFormat("${0}", memberExpression.Member.Name);
                sqlTemplate.AppendFormat("{0}{1}", i, (i + 1 == values.Length) ? ")" : ",\n");
            }
            //获取原始参数集合
            IDbDataParameter[] parameters = new IDbDataParameter[values.Length];
            for (int i = 0; i < values.Length; i++)
                parameters[i] = source.ParameterFactory.Parameter(string.Concat(memberExpression.Member.Name, i), values[i]);
            //过滤获取新数据源
            return source.Factory.CreateQuery(source, memberExpression, sqlTemplate.ToString(), parameters.ToArray());
        }
        /// <summary>
        /// Extendable method: 过滤数据源中属性包含(或不包含)某些值的实体(生成Sql IN表达式)
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <typeparam name="TProperty">对象属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象某属性的表达式</param>
        /// <param name="selectorSource">子查询数据源</param>
        /// <param name="isIn">IN 或 NOT IN</param>
        /// <returns>新的数据源</returns>
        public static IDbQuery<TEntity> In<TEntity, TProperty>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TProperty>> selector, IDbSelectedQuery<TProperty> selectorSource, bool isIn = true)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(selectorSource, nameof(selectorSource));
            //获取sql表达式模板
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            StringBuilder sqlTemplate = new StringBuilder();
            sqlTemplate.AppendLine(isIn ? "IN " : "NOT IN ");
            sqlTemplate.Append("\t(");
            sqlTemplate.Append(selectorSource.ToWhereSqlString().Trim());
            sqlTemplate.Append(")");
            sqlTemplate.Replace("\n ", "\n\t");
            //过滤获取新数据源
            return source.Factory.CreateQuery(source, memberExpression, sqlTemplate.ToString(), selectorSource.Parameters.ToArray());
        }
        /// <summary>
        /// Extendable method: 过滤数据源中属性在一定范围内的实体
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象属性表达式</param>
        /// <param name="left">最小值</param>
        /// <param name="right">最大值</param>
        public static IDbQuery<TEntity> Between<TEntity, TProperty>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TProperty>> selector, TProperty left, TProperty right)
            where TEntity : class
            where TProperty : struct
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(left, nameof(left));
            Check.ArgumentNull(right, nameof(right));
            //获取sql表达式模板
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            string sqlTemplate = string.Format("BETWEEN ${0}Left AND ${0}Right", memberExpression.Member.Name);
            //获取sql参数
            IDbDataParameter[] parameters = new IDbDataParameter[]
            {
                source.ParameterFactory.Parameter(string.Concat(memberExpression.Member.Name, "Left"), left),
                source.ParameterFactory.Parameter(string.Concat(memberExpression.Member.Name, "Right"), right)
            };
            //创建新数据源
            return source.Factory.CreateQuery(source, memberExpression, sqlTemplate, parameters);
        }
        /// <summary>
        /// Extendable method: 过滤数据源中属性在一定范围内的实体
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定对象可空类型属性表达式</param>
        /// <param name="left">最小值</param>
        /// <param name="right">最大值</param>
        public static IDbQuery<TEntity> Between<TEntity, TProperty>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TProperty?>> selector, TProperty left, TProperty right)
            where TEntity : class
            where TProperty : struct
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(left, nameof(left));
            Check.ArgumentNull(right, nameof(right));
            //获取sql表达式模板
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            string sqlTemplate = string.Format("BETWEEN ${0}Left AND ${0}Right", memberExpression.Member.Name);
            //获取sql参数
            IDbDataParameter[] parameters = new IDbDataParameter[]
            {
                source.ParameterFactory.Parameter(string.Concat(memberExpression.Member.Name, "Left"), left),
                source.ParameterFactory.Parameter(string.Concat(memberExpression.Member.Name, "Right"), right)
            };
            //创建新数据源
            return source.Factory.CreateQuery(source, memberExpression, sqlTemplate, parameters);
        }
        /// <summary>
        /// Extendable method: 解析表达式为 {Column} LIKE ${Property} as ParameterName
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <param name="source">数据源过滤器</param>
        /// <param name="selector">指定对象某属性的表达式</param>
        /// <param name="value">参数值</param>
        /// <param name="isLike">LIKE 或 NOT LIKE</param>
        public static IDbQuery<TEntity> Like<TEntity>(this IDbQuery<TEntity> source, Expression<Func<TEntity, string>> selector, string value, bool isLike = true)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(value, "value");
            //获取sql条件表达式
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            string sqlTemplate = string.Format("{0} ${1}_{2}", isLike ? "LIKE" : "NOT LIKE", memberExpression.Expression.Type.Name, memberExpression.Member.Name);
            //获取参数
            IDbDataParameter parameter = source.ParameterFactory.Parameter(string.Format("{0}_{1}", memberExpression.Expression.Type.Name, memberExpression.Member.Name), value);
            //创建新的数据源
            return source.Factory.CreateQuery(source, memberExpression, sqlTemplate, parameter);
        }
        /// <summary>
        /// Extendable method: 过滤获取包含数据源中某属性值为空(或非空)的元素的新数据源
        /// </summary>
        /// <typeparam name="TEntity">对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">数据源过滤器</param>
        /// <param name="selector">指定对象属性表达式</param>
        /// <param name="isNull">IS NULL 或 IS NOT NULL</param>
        public static IDbQuery<TEntity> IsNull<TEntity, TProperty>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TProperty>> selector, bool isNull = true)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            //拼接sql
            string sqlTemplate = isNull ? "IS NULL" : "IS NOT NULL";
            //创建新的数据源
            return source.Factory.CreateQuery(source, selector.Body.GetMemberExpression(), sqlTemplate);
        }
        /// <summary>
        /// Extendable method: 筛选数据源符合条件的对象
        /// </summary>
        /// <typeparam name="TEntity">The entity's type.</typeparam>
        /// <param name="source">基础数据源</param>
        /// <param name="predicate">筛选表达式</param>
        /// <returns>被筛选后的数据源</returns>
        public static IDbQuery<TEntity> Where<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(predicate, nameof(predicate));
            //创建新的查询对象
            return source.Factory.CreateQuery(source, predicate);
        }
        /// <summary>
        /// Extendable method: 筛选数据源符合条件的对象
        /// </summary>
        /// <typeparam name="TEntity">The entity's type.</typeparam>
        /// <param name="source">查询数据源</param>
        /// <param name="predicate">筛选表达式</param>
        /// <returns>被筛选后的数据源</returns>
        public static IDbQuery<TEntity> Where<TEntity>(this IDbQuery<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(predicate, nameof(predicate));
            //创建新的查询对象
            return source.Factory.CreateQuery(source, predicate);
        }
        /// <summary>
        /// Extendable method: 选定属性查询
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TElement">包含项类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">指定查询项表达式</param>
        /// <returns>查询选定属性的数据源</returns>
        public static IDbQuery<TEntity> IncludeBy<TEntity, TElement>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TElement>> selector)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            //创建新的查询数据源
            return source.Factory.CreateIncludedQuery(source, selector);
        }
        /// <summary>
        /// Extendable method: 获取关联查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>关联查询数据源</returns>
        public static IDbQuery<TEntity> Join<TEntity, TOther>(this IDbQuery<TEntity> source, IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(otherSource, nameof(otherSource));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            //创建关联查询
            return source.Factory.CreateJoinedQuery(source, otherSource, selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取关联查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联对象的选择性查询数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>关联查询数据源</returns>
        public static IDbQuery<TEntity> Join<TEntity, TOther>(this IDbQuery<TEntity> source, IDbSelectedQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(otherSource, nameof(otherSource));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            //创建关联查询
            return source.Factory.CreateJoinedQuery(source, otherSource, selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取左关联查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>左关联查询数据源</returns>
        public static IDbQuery<TEntity> LeftJoin<TEntity, TOther>(this IDbQuery<TEntity> source, IDbQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(otherSource, nameof(otherSource));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            //创建关联查询
            return source.Factory.CreateLeftJoinedQuery(source, otherSource, selector, predicate);
        }
        /// <summary>
        /// Extendable method: 获取左关联查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TOther">关联的实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="otherSource">关联对象的选择性查询数据源</param>
        /// <param name="selector">指定关联实体类型的属性表达式</param>
        /// <param name="predicate">TEntity 与 TOther关系表达式</param>
        /// <returns>左关联查询数据源</returns>
        public static IDbQuery<TEntity> LeftJoin<TEntity, TOther>(this IDbQuery<TEntity> source, IDbSelectedQuery<TOther> otherSource, Expression<Func<TEntity, TOther>> selector, Expression<Func<TEntity, TOther, bool>> predicate)
            where TEntity : class
            where TOther : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(otherSource, nameof(otherSource));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(predicate, nameof(predicate));
            //创建关联查询
            return source.Factory.CreateLeftJoinedQuery(source, otherSource, selector, predicate);
        }

        /// <summary>
        /// Extendable method: 过滤数据源中属性包含(或不包含)某些值的实体(生成Sql IN表达式)
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">对象属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="selector">指定对象某属性的表达式</param>
        /// <param name="values">对象属性所包含的值</param>
        /// <param name="isIn">IN 或 NOT IN</param>
        /// <returns>新的视图查询数据源</returns>
        public static IDbView<TModel> In<TModel, TProperty>(this IDbView<TModel> source, Expression<Func<TModel, TProperty>> selector, TProperty[] values, bool isIn = true)
            where TModel : class, new()
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(values, nameof(values));
            //获取sql表达式模板
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            StringBuilder sqlTemplate = new StringBuilder();
            sqlTemplate.AppendLine(isIn ? "IN " : "NOT IN ");
            for (int i = 0; i < values.Length; i++)
            {
                sqlTemplate.AppendFormat("\t {0}", i == 0 ? '(' : ' ');
                sqlTemplate.AppendFormat("${0}", memberExpression.Member.Name);
                sqlTemplate.AppendFormat("{0}{1}", i, (i + 1 == values.Length) ? ")" : ",\n");
            }
            //获取原始参数集合
            IDbDataParameter[] parameters = new IDbDataParameter[values.Length];
            for (int i = 0; i < values.Length; i++)
                parameters[i] = source.ParameterFactory.Parameter(string.Concat(memberExpression.Member.Name, i), values[i]);
            //过滤获取新数据源
            return source.Factory.CreateView(source, memberExpression, sqlTemplate.ToString(), parameters.ToArray());
        }
        /// <summary>
        /// Extendable method: 过滤数据源中属性包含(或不包含)某些值的实体(生成Sql IN表达式)
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">对象属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="selector">指定对象某属性的表达式</param>
        /// <param name="selectorSource">子查询数据源</param>
        /// <param name="isIn">IN 或 NOT IN</param>
        /// <returns>新的视图查询数据源</returns>
        public static IDbView<TModel> In<TModel, TProperty>(this IDbView<TModel> source, Expression<Func<TModel, TProperty>> selector, IDbSelectedQuery<TProperty> selectorSource, bool isIn = true)
            where TModel : class, new()
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(selectorSource, nameof(selectorSource));
            //获取sql表达式模板
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            StringBuilder sqlTemplate = new StringBuilder();
            sqlTemplate.AppendLine(isIn ? "IN " : "NOT IN ");
            sqlTemplate.Append("\t(");
            sqlTemplate.Append(selectorSource.ToWhereSqlString().Trim());
            sqlTemplate.Append(")");
            sqlTemplate.Replace("\n ", "\n      ");
            //过滤获取新数据源
            return source.Factory.CreateView(source, memberExpression, sqlTemplate.ToString(), selectorSource.Parameters.ToArray());
        }
        /// <summary>
        /// Extendable method: 执行区间查询获取视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="selector">指定对象某属性的表达式</param>
        /// <param name="left">最小值</param>
        /// <param name="right">最大值</param>
        /// <returns>视图查询数据源</returns>
        public static IDbView<TModel> Between<TModel, TProperty>(this IDbView<TModel> source, Expression<Func<TModel, TProperty>> selector, TProperty left, TProperty right)
            where TModel : class, new()
            where TProperty : struct
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(left, nameof(left));
            Check.ArgumentNull(right, nameof(right));
            //获取sql表达式模板
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            string sqlTemplate = string.Format("BETWEEN ${0}Left AND ${0}Right", memberExpression.Member.Name);
            //获取sql参数
            IDbDataParameter[] parameters = new IDbDataParameter[]
            {
                source.ParameterFactory.Parameter(string.Concat(memberExpression.Member.Name, "Left"), left),
                source.ParameterFactory.Parameter(string.Concat(memberExpression.Member.Name, "Right"), right)
            };
            //创建新的视图查询数据源
            return source.Factory.CreateView(source, memberExpression, sqlTemplate, parameters);
        }
        /// <summary>
        /// Extendable method: 执行模糊查询获取视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="selector">指定对象某属性的表达式</param>
        /// <param name="value">参数值</param>
        /// <param name="isLike">LIKE 或 NOT LIKE</param>
        /// <returns>模糊筛选后的视图查询数据源</returns>
        public static IDbView<TModel> Like<TModel>(this IDbView<TModel> source, Expression<Func<TModel, string>> selector, string value, bool isLike = true)
            where TModel : class, new()
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(value, nameof(value));
            //获取sql条件表达式
            MemberExpression memberExpression = selector.Body.GetMemberExpression();
            string sqlTemplate = string.Format("{0} ${1}", isLike ? "LIKE" : "NOT LIKE", memberExpression.Member.Name);
            //获取参数
            IDbDataParameter parameter = source.ParameterFactory.Parameter(memberExpression.Member.Name, value);
            //创建新的视图查询数据源
            return source.Factory.CreateView(source, memberExpression, sqlTemplate, parameter);
        }
        /// <summary>
        /// Extendable method: 执行非空或可空查询获取视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <typeparam name="TProperty">属性类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="selector">指定对象某属性的表达式</param>
        /// <param name="isNull">IS NULL 或 IS NOT NULL</param>
        /// <returns>视图查询数据源</returns>
        public static IDbView<TModel> Null<TModel, TProperty>(this IDbView<TModel> source, Expression<Func<TModel, TProperty>> selector, bool isNull = true)
            where TModel : class, new()
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            //拼接sql
            string sqlTemplate = isNull ? "IS NULL" : "IS NOT NULL";
            //创建新的视图查询数据源
            return source.Factory.CreateView(source, selector.Body.GetMemberExpression(), sqlTemplate);
        }
        /// <summary>
        /// Extendable method: 筛选并获取筛选后的视图查询数据源
        /// </summary>
        /// <typeparam name="TModel">视图对象类型</typeparam>
        /// <param name="source">视图查询数据源</param>
        /// <param name="predicate">筛选表达式</param>
        /// <returns>被筛选后的数据源</returns>
        public static IDbView<TModel> Where<TModel>(this IDbView<TModel> source, Expression<Func<TModel, bool>> predicate)
            where TModel : class, new()
        {
            //非空验证
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(predicate, nameof(predicate));
            //创建新的视图查询数据源
            return source.Factory.CreateView(source, predicate);
        }

        /// <summary>
        /// Extendable method: 获取分页查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">排好序的数据源</param>
        /// <param name="pageSize">每页的元素数量</param>
        /// <param name="pageIndex">当前第几页</param>
        /// <returns>分页查询数据源</returns>
        public static IDbPagedQuery<TEntity> Paging<TEntity>(this IDbQuery<TEntity> source, int pageSize, int pageIndex = 1)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            //执行分页查询
            return source.Factory.CreatePagedQuery(source, pageSize, pageIndex);
        }
        /// <summary>
        /// Extendable method: 获取按某属性升序排序的分页查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderBySelector">指定排序属性</param>
        /// <param name="pageSize">每页的元素数量</param>
        /// <param name="pageIndex">当前第几页</param>
        /// <returns>分页查询数据源</returns>
        public static IDbPagedQuery<TEntity> PagingBy<TEntity>(this IDbQuery<TEntity> source, Expression<Func<TEntity, object>> orderBySelector, int pageSize, int pageIndex = 1)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            //执行分页查询
            return source.Factory.CreatePagedQuery(source, orderBySelector, pageSize, pageIndex);
        }
        /// <summary>
        /// Extendable method: 获取按某属性降序排序的分页查询数据源
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="orderBySelector">指定排序属性</param>
        /// <param name="pageSize">每页的元素数量</param>
        /// <param name="pageIndex">当前第几页</param>
        /// <returns>TEntity类型的元素迭代器</returns>
        public static IDbPagedQuery<TEntity> PagingByDescending<TEntity>(this IDbQuery<TEntity> source, Expression<Func<TEntity, object>> orderBySelector, int pageSize, int pageIndex = 1)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            //执行分页查询
            return source.Factory.CreatePagedQuery(source, orderBySelector, pageSize, pageIndex, true);
        }

        /// <summary>
        /// Extendable method: 查询前几条的元素
        /// </summary>
        /// <typeparam name="TEntity">object's type</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <returns>TOP查询数据源</returns>
        public static IDbSelectedQuery<TEntity> Top<TEntity>(this IDbQuery<TEntity> source, int topCount)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(topCount, nameof(topCount));
            //获取TOP查询数据源
            return source.Factory.CreateTopSelectedQuery(source, topCount);
        }
        /// <summary>
        /// Extendable method: 查询前几条选定项
        /// </summary>
        /// <typeparam name="TEntity">object's type</typeparam>
        /// <typeparam name="TResult">result's type</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        /// <param name="selector">指定选定项的表达式</param>
        /// <returns>TOP选定项查询数据源</returns>
        public static IDbSelectedQuery<TResult> Top<TEntity, TResult>(this IDbQuery<TEntity> source, int topCount, Expression<Func<TEntity, TResult>> selector)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(topCount, nameof(topCount));
            Check.ArgumentNull(selector, nameof(selector));
            //获取TOP查询数据源
            return source.Factory.CreateTopSelectedQuery(source, topCount, selector);
        }
        /// <summary>
        /// Extendable method: 转换数据源中所有的元素获取TResult类型迭代器
        /// </summary>
        /// <typeparam name="TEntity">object's type</typeparam>
        /// <typeparam name="TResult">result's type</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">转换匿名函数</param>
        /// <returns>TResult类型迭代器</returns>
        public static IDbSelectedQuery<TResult> Select<TEntity, TResult>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TResult>> selector)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            //执行获取结果
            return source.Factory.CreateSelectedQuery(source, selector);
        }
        /// <summary>
        /// Extendable method: 转换数据源中所有的元素获取TResult类型迭代器并去除重复
        /// </summary>
        /// <typeparam name="TEntity">object's type</typeparam>
        /// <typeparam name="TResult">result's type</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="selector">转换匿名函数</param>
        /// <returns>去除重复的TResult类型迭代器</returns>
        public static IDbSelectedQuery<TResult> Distinct<TEntity, TResult>(this IDbQuery<TEntity> source, Expression<Func<TEntity, TResult>> selector)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(source, nameof(source));
            Check.ArgumentNull(selector, nameof(selector));
            //执行获取去除重复的数据源并装换成TResult类型
            return source.Factory.CreateDistinctQuery(source, selector);
        }

        /// <summary>
        /// Extendable method: 获取数据源中唯一满足条件的对象
        /// 1.没有满足条件的对象会有异常
        /// 2.有多个满足条件的对象也会有异常
        /// </summary>
        /// <typeparam name="TEntity">The entity's type.</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">筛选表达式</param>
        /// <returns>数据源中唯一满足条件的对象</returns>
        public static TEntity Single<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            //获取数据源中唯一符合条件的对象
            return source.Where(predicate).Single();
        }
        /// <summary>
        /// Extendable method: 获取数据源中唯一满足条件的对象
        /// 1.没有满足条件的对象会有异常
        /// 2.有多个满足条件的对象也会有异常
        /// </summary>
        /// <typeparam name="TEntity">The entity's type.</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">筛选表达式</param>
        /// <returns>数据源中唯一满足条件的对象</returns>
        public static TEntity Single<TEntity>(this IDbQuery<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            //获取数据源中唯一符合条件的对象
            return source.Where(predicate).Single();
        }
        /// <summary>
        /// Extendable method: 获取数据源中唯一满足条件的对象
        /// 1.没有满足条件的对象会有异常
        /// 2.有多个满足条件的对象也会有异常
        /// </summary>
        /// <typeparam name="TEntity">The entity's type.</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">筛选表达式</param>
        /// <returns>数据源中唯一满足条件的对象</returns>
        public static TEntity SingleOrDefault<TEntity>(this IDbSource<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            //获取数据源中唯一符合条件的对象
            return source.Where(predicate).SingleOrDefault();
        }
        /// <summary>
        /// Extendable method: 获取数据源中唯一满足条件的对象
        /// 1.没有满足条件的对象会有异常
        /// 2.有多个满足条件的对象也会有异常
        /// </summary>
        /// <typeparam name="TEntity">The entity's type.</typeparam>
        /// <param name="source">数据源</param>
        /// <param name="predicate">筛选表达式</param>
        /// <returns>数据源中唯一满足条件的对象</returns>
        public static TEntity SingleOrDefault<TEntity>(this IDbQuery<TEntity> source, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class
        {
            //获取数据源中唯一符合条件的对象
            return source.Where(predicate).SingleOrDefault();
        }
    }
}