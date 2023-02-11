using CloudEntity.CommandTrees;
using CloudEntity.Data;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace CloudEntity.Internal.CommandTrees
{
    /// <summary>
    /// 表达式解析对象工厂扩展类
    /// </summary>
    public static class PredicateParserFactoryExtender
    {
        /// <summary>
        /// 解析Lambda表达式内容，获取Where节点下的子sql生成器节点集合
        /// </summary>
        /// <param name="factory">表达式解析对象工厂</param>
        /// <param name="parameterSetter">sql参数设置对象</param>
        /// <param name="parameterExpression">lambda表达式中的参数表达式</param>
        /// <param name="expression">lambda表达式的主体表达式（或其主体的左表达式 或 右表达式）</param>
        /// <returns>Where节点下的子sql生成器节点集合</returns>
        public static IEnumerable<INodeBuilder> GetWhereChildBuilders(this IPredicateParserFactory factory, IParameterSetter parameterSetter, ParameterExpression parameterExpression, Expression expression)
        {
            switch (expression.NodeType)
            {
                //解析 && &表达式节点
                case ExpressionType.And:
                case ExpressionType.AndAlso:
                    BinaryExpression binaryExpression = expression as BinaryExpression;
                    // 依次解析获取左表达式节点集合
                    foreach (INodeBuilder nodeBuilder in factory.GetWhereChildBuilders(parameterSetter, parameterExpression, binaryExpression.Left))
                        yield return nodeBuilder;
                    // 依次解析获取右表达式节点集合
                    foreach (INodeBuilder nodeBuilder in factory.GetWhereChildBuilders(parameterSetter, parameterExpression, binaryExpression.Right))
                        yield return nodeBuilder;
                    break;
                //解析其他类型的表达式
                default:
                    PredicateParser predicateParser = factory.GetPredicateParser(expression.NodeType);
                    yield return predicateParser.Parse(parameterExpression, expression, parameterSetter);
                    break;
            }
        }
    }
}