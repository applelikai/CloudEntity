﻿using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity.Internal.WhereVisitors
{
    /// <summary>
    /// 查询条件Lambda表达式解析类
    /// 李凯 Apple_Li
    /// </summary>
    public abstract class WhereVisitor
    {
        /// <summary>
        /// sql参数创建对象
        /// </summary>
        private IParameterFactory parameterFactory;
        /// <summary>
        /// 列获取器
        /// </summary>
        private IColumnGetter columnGetter;

        /// <summary>
        /// 获取当前指定成员对应的完整列名
        /// </summary>
        /// <param name="memberExpression">指定对象成员的表达式</param>
        /// <returns>当前指定成员对应的完整列名</returns>
        protected string GetColumnFullName(MemberExpression memberExpression)
        {
            return this.columnGetter.GetColumnFullName(memberExpression);
        }
        /// <summary>
        /// 创建sql参数
        /// </summary>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns>sql参数</returns>
        protected IDbDataParameter CreateParameter(string name, object value)
        {
            return this.parameterFactory.Parameter(name, value);
        }
        /// <summary>
        /// 获取sql参数列表
        /// </summary>
        /// <param name="parameterNames">记录不允许重复的sql参数名称</param>
        /// <param name="name">参数名</param>
        /// <param name="value">参数值</param>
        /// <returns>sql参数列表</returns>
        protected IEnumerable<IDbDataParameter> GetParameters(HashSet<string> parameterNames, string name, object value)
        {
            if (parameterNames.Add(name))
                yield return this.parameterFactory.Parameter(name, value);
        }
        /// <summary>
        /// 解析表达式 获取基本类型的sql表达式节点 或 (参数名和参数值)
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="expression">Lambda表达式的主体(或主体的一部分)</param>
        /// <param name="parameterName">参数名</param>
        /// <param name="parameterValue">参数值</param>
        /// <returns></returns>
        protected ISqlBuilder GetSqlBuilder(ParameterExpression parameterExpression, Expression expression, ref string parameterName, ref object parameterValue)
        {
            //若当前表达式节点包含参数表达式
            if (expression.ContainsParameterExpression(parameterExpression))
            {
                //获取成员表达式
                MemberExpression memberExpression = expression.GetMemberExpression();
                //获取参数名
                parameterName = string.Format("{0}_{1}", memberExpression.Expression.Type.Name, memberExpression.Member.Name);
                //获取sql表达式节点(指定列名)
                return new SqlBuilder(this.GetColumnFullName(memberExpression));
            }
            //若当前表达式节点不包含参数表达式
            //1.获取参数名
            if (expression.NodeType == ExpressionType.MemberAccess)
                parameterName = (expression as MemberExpression).Member.Name;
            //2.获取参数值
            parameterValue = Expression.Lambda(expression).Compile().DynamicInvoke();
            //返回空
            return null;
        }

        /// <summary>
        /// 创建Lambda表达式解析对象
        /// </summary>
        /// <param name="parameterFactory">sql参数创建对象</param>
        /// <param name="columnGetter">列名获取器</param>
        public WhereVisitor(IParameterFactory parameterFactory, IColumnGetter columnGetter)
        {
            //非空检查
            Check.ArgumentNull(parameterFactory, nameof(parameterFactory));
            Check.ArgumentNull(columnGetter, nameof(columnGetter));
            //赋值
            this.parameterFactory = parameterFactory;
            this.columnGetter = columnGetter;
        }
        /// <summary>
        /// 解析查询条件表达式,生成sql条件表达式节点及其附属的sql参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="bodyExpression">Lambda表达式的主体(或主体的一部分)</param>
        /// <param name="parameterNames">记录不允许重复的sql参数名称</param>
        /// <returns>sql条件表达式节点及其附属的sql参数</returns>
        public abstract KeyValuePair<INodeBuilder, IDbDataParameter[]> Visit(ParameterExpression parameterExpression, Expression bodyExpression, HashSet<string> parameterNames);
    }
}
