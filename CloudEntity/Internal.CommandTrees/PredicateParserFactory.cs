using CloudEntity.CommandTrees;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CloudEntity.Internal.CommandTrees
{
    /// <summary>
    /// 创建WhereVisitor的工厂
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    public class PredicateParserFactory : IPredicateParserFactory
    {
        /// <summary>
        /// Sql命令工厂
        /// </summary>
        private ICommandFactory _commandFactory;
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
        private IDictionary<ExpressionType, PredicateParser> _whereVisitors;

        /// <summary>
        /// 创建表达式解析器
        /// </summary>
        /// <param name="expressionType">表达式类型</param>
        /// <returns>表达式解析器</returns>
        protected virtual PredicateParser CreatePredicateParser(ExpressionType expressionType)
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
                    return new CompareExpressionParser(_commandFactory, _mapperContainer);
                //创建方法调用表达式解析器
                case ExpressionType.Call:
                    return new MethodCallExpressionParser(_commandFactory, _mapperContainer);
                //创建NOT一元表达式解析器
                case ExpressionType.Not:
                    return new UnaryNotExpressionParser(_commandFactory, _mapperContainer, this);
                //创建二叉树表达式解析器
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                case ExpressionType.Or:
                case ExpressionType.OrElse:
                    return new BinaryExpressionParser(_commandFactory, _mapperContainer, this);
                //其他类型的表达式不支持解析
                default:
                    throw new Exception(string.Format("unsupported expression's type:{0}", expressionType));
            }
        }

        /// <summary>
        /// 创建获取表达式解析器的工厂
        /// </summary>
        /// <param name="commandFactory">Sql命令工厂</param>
        /// <param name="mapperContainer">Mapper容器</param>
        public PredicateParserFactory(ICommandFactory commandFactory, IMapperContainer mapperContainer = null)
        {
            //非空检查
            Check.ArgumentNull(commandFactory, nameof(commandFactory));
            //赋值
            _commandFactory = commandFactory;
            _mapperContainer = mapperContainer;
            _locaker = new object();
            _whereVisitors = new Dictionary<ExpressionType, PredicateParser>();
        }
        /// <summary>
        /// 创建表达式解析器
        /// </summary>
        /// <param name="expressionType">表达式类型</param>
        /// <returns>表达式解析器</returns>
        public PredicateParser GetPredicateParser(ExpressionType expressionType)
        {
            Start:
            //若字典中存在当前类型的表达式解析器，则直接获取
            if (_whereVisitors.ContainsKey(expressionType))
                return _whereVisitors[expressionType];
            //进入单线程模式
            lock (_locaker)
            {
                //若字典中不存在当前类型的表达式解析器则注册
                if (!_whereVisitors.ContainsKey(expressionType))
                    _whereVisitors.Add(expressionType, this.CreatePredicateParser(expressionType));
                //回到Start,重新获取当前类型的表达式解析器
                goto Start;
            }
        }
    }
}
