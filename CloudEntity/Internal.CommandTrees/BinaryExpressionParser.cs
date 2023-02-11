using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Linq.Expressions;

namespace CloudEntity.Internal.CommandTrees
{
    /// <summary>
    /// 二叉树表达式解析类
    /// 李凯 Apple_Li 15150598493
    /// 最后修改时间：2023/02/10 23:14
    /// </summary>
    internal class BinaryExpressionParser : PredicateParser
    {
        /// <summary>
        /// 创建表达式解析器的工厂
        /// </summary>
        private IPredicateParserFactory _factory;

        /// <summary>
        /// 创建二叉树表达式解析对象
        /// </summary>
        /// <param name="commandTreeFactory">创建Sql命令生成树的工厂</param>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="factory">创建表达式解析器的工厂</param>
        public BinaryExpressionParser(ICommandTreeFactory commandTreeFactory, IMapperContainer mapperContainer, IPredicateParserFactory factory)
            : base(commandTreeFactory, mapperContainer)
        {
            _factory = factory;
        }
        /// <summary>
        /// 解析查询条件表达式，生成并获取sql条件表达式节点，附带设置sql参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="bodyExpression">Lambda表达式的主体(或主体的一部分)</param>
        /// <param name="parameterSetter">sql参数设置对象</param>
        /// <returns>sql条件表达式节点</returns>
        public override INodeBuilder Parse(ParameterExpression parameterExpression, Expression bodyExpression, IParameterSetter parameterSetter)
        {
            // 获取二叉树表达式
            BinaryExpression binaryExpression = bodyExpression as BinaryExpression;
            // 获取左sql表达式
            PredicateParser leftParser = _factory.GetPredicateParser(binaryExpression.Left.NodeType);
            INodeBuilder leftBuilder = leftParser.Parse(parameterExpression, binaryExpression.Left, parameterSetter);
            // 获取右sql表达式
            PredicateParser rightParser = _factory.GetPredicateParser(binaryExpression.Right.NodeType);
            INodeBuilder rightBuilder = rightParser.Parse(parameterExpression, binaryExpression.Right, parameterSetter);
            // 获取最终的sql条件表达式节点
            return new BinaryBuilder()
            {
                LeftBuilder = leftBuilder,
                NodeType = binaryExpression.NodeType,
                RightBuilder = rightBuilder
            };
        }
    }
}
