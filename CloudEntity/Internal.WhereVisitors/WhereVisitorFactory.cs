using CloudEntity.CommandTrees;
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
        /// <summary>
        /// 创建参数的工厂
        /// </summary>
        private IParameterFactory _parameterFactory;
        /// <summary>
        /// 创建Sql命令生成树的工厂
        /// </summary>
        private ICommandTreeFactory _commandTreeFactory;
        /// <summary>
        /// Mapper容器
        /// </summary>
        private IMapperContainer _mapperContainer;
        /// <summary>
        /// 线程锁
        /// </summary>
        private object _locaker;
        /// <summary>
        /// 表达式解析器字典
        /// </summary>
        private IDictionary<ExpressionType, WhereVisitor> _whereVisitors;

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
                    return new CompareWhereVisitor(_parameterFactory, _commandTreeFactory, _mapperContainer);
                //创建方法调用表达式解析器
                case ExpressionType.Call:
                    return new MethodCallWhereVisitor(_parameterFactory, _commandTreeFactory, _mapperContainer);
                //创建NOT一元表达式解析器
                case ExpressionType.Not:
                    return new UnaryNotWhereVisitor(_parameterFactory, _commandTreeFactory, _mapperContainer, this);
                //创建二叉树表达式解析器
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return new BinaryWhereVisitor(_parameterFactory, _commandTreeFactory, _mapperContainer, this);
                //其他类型的表达式不支持解析
                default:
                    throw new Exception(string.Format("unsupported expression's type:{0}", expressionType));
            }
        }

        /// <summary>
        /// 创建获取表达式解析器的工厂
        /// </summary>
        /// <param name="parameterFactory">参数创建器</param>
        /// <param name="commandTreeFactory">创建Sql命令生成树的工厂</param>
        /// <param name="mapperContainer">Mapper容器</param>
        public WhereVisitorFactory(IParameterFactory parameterFactory, ICommandTreeFactory commandTreeFactory, IMapperContainer mapperContainer = null)
        {
            //非空检查
            Check.ArgumentNull(parameterFactory, nameof(parameterFactory));
            Check.ArgumentNull(commandTreeFactory, nameof(commandTreeFactory));
            //赋值
            _parameterFactory = parameterFactory;
            _commandTreeFactory = commandTreeFactory;
            _mapperContainer = mapperContainer;
            _locaker = new object();
            _whereVisitors = new Dictionary<ExpressionType, WhereVisitor>();
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
            if (this._whereVisitors.ContainsKey(expressionType))
                return this._whereVisitors[expressionType];
            //进入单线程模式
            lock (this._locaker)
            {
                //若字典中不存在当前类型的表达式解析器则注册
                if (!this._whereVisitors.ContainsKey(expressionType))
                    this._whereVisitors.Add(expressionType, this.CreateWhereVisitor(expressionType));
                //回到Start,重新获取当前类型的表达式解析器
                goto Start;
            }
        }
    }
}
