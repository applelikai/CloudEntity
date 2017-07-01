using CloudEntity.Data;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CloudEntity.Internal.WhereVisitors
{
    /// <summary>
    /// 创建WhereVisitor的工厂
    /// 李凯 Apple_Li
    /// </summary>
    public class WhereVisitorFactory : IWhereVisitorFactory
    {
        private IParameterFactory parameterFactory;                     //参数创建工厂
        private IMapperContainer mapperContainer;                       //Mapper容器
        private object locaker;                                         //线程锁
        private IDictionary<ExpressionType, WhereVisitor> whereVisitors;//表达式解析器字典

        /// <summary>
        /// 创建表达式解析器
        /// </summary>
        /// <param name="expressionType">表达式类型</param>
        /// <returns>表达式解析器</returns>
        protected virtual WhereVisitor CreateWhereVisitor(ExpressionType expressionType)
        {
            switch (expressionType)
            {
                //创建比较类型表达式解析器
                case ExpressionType.GreaterThan:            //大于
                case ExpressionType.GreaterThanOrEqual:     //大于等于
                case ExpressionType.LessThan:               //小于
                case ExpressionType.LessThanOrEqual:        //小于等于
                case ExpressionType.Equal:                  //等于
                case ExpressionType.NotEqual:               //不等于
                    return new CompareWhereVisitor(this.parameterFactory, this.mapperContainer);
                //创建方法调用表达式解析器
                case ExpressionType.Call:
                    return new MethodCallWhereVisitor(this.parameterFactory, this.mapperContainer);
                //创建NOT一元表达式解析器
                case ExpressionType.Not:
                    return new UnaryNotWhereVisitor(this.parameterFactory, this.mapperContainer, this);
                //创建二叉树表达式解析器
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return new BinaryWhereVisitor(this.parameterFactory, this.mapperContainer, this);
                //其他类型的表达式不支持解析
                default:
                    throw new Exception(string.Format("unsupported expression's type:{0}", expressionType));
            }
        }

        /// <summary>
        /// 创建获取表达式解析器的工厂
        /// </summary>
        /// <param name="parameterFactory">参数创建器</param>
        /// <param name="mapperContainer">Mapper容器</param>
        public WhereVisitorFactory(IParameterFactory parameterFactory, IMapperContainer mapperContainer)
        {
            //非空检查
            Check.ArgumentNull(parameterFactory, nameof(parameterFactory));
            Check.ArgumentNull(mapperContainer, nameof(mapperContainer));
            //赋值
            this.parameterFactory = parameterFactory;
            this.mapperContainer = mapperContainer;
            this.locaker = new object();
            this.whereVisitors = new Dictionary<ExpressionType, WhereVisitor>();
        }
        /// <summary>
        /// 创建表达式解析器
        /// </summary>
        /// <param name="expressionType">表达式类型</param>
        /// <returns>表达式解析器</returns>
        public WhereVisitor GetVisitor(ExpressionType expressionType)
        {
        Start:
            //若字典中存在当前类型的表达式解析器，则直接获取
            if (this.whereVisitors.ContainsKey(expressionType))
                return this.whereVisitors[expressionType];
            //进入单线程模式
            lock (this.locaker)
            {
                //若字典中不存在当前类型的表达式解析器则注册
                if (!this.whereVisitors.ContainsKey(expressionType))
                    this.whereVisitors.Add(expressionType, this.CreateWhereVisitor(expressionType));
                //回到Start,重新获取当前类型的表达式解析器
                goto Start;
            }
        }
    }
}
