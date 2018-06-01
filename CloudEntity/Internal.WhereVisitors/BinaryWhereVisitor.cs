using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Linq.Expressions;

namespace CloudEntity.Internal.WhereVisitors
{
    /// <summary>
    /// 二叉树表达式解析类
    /// 李凯 Apple_Li
    /// </summary>
    internal class BinaryWhereVisitor : WhereVisitor
    {
        //创建表达式解析器的工厂
        private IWhereVisitorFactory factory;

        /// <summary>
        /// 创建二叉树表达式解析对象
        /// </summary>
        /// <param name="parameterFactory">sql参数创建对象</param>
        /// <param name="columnGetter">列名获取器</param>
        /// <param name="factory">创建表达式解析器的工厂</param>
        public BinaryWhereVisitor(IParameterFactory parameterFactory, IColumnGetter columnGetter, IWhereVisitorFactory factory)
            : base(parameterFactory, columnGetter)
        {
            this.factory = factory;
        }
        /// <summary>
        /// 解析查询条件表达式,生成sql条件表达式节点及其附属的sql参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="bodyExpression">Lambda表达式的主体(或主体的一部分)</param>
        /// <returns>sql条件表达式节点及其附属的sql参数</returns>
        public override KeyValuePair<INodeBuilder, IDbDataParameter[]> Visit(ParameterExpression parameterExpression, Expression bodyExpression)
        {
            //获取二叉树表达式
            BinaryExpression binaryExpression = bodyExpression as BinaryExpression;
            //获取左sql表达式及其附属参数
            WhereVisitor leftVisitor = this.factory.GetVisitor(binaryExpression.Left.NodeType);
            KeyValuePair<INodeBuilder, IDbDataParameter[]> leftPair = leftVisitor.Visit(parameterExpression, binaryExpression.Left);
            //获取右sql表达式及其附属参数
            WhereVisitor rightVisitor = this.factory.GetVisitor(binaryExpression.Right.NodeType);
            KeyValuePair<INodeBuilder, IDbDataParameter[]> rightPair = rightVisitor.Visit(parameterExpression, binaryExpression.Right);
            //获取最终的sql条件表达式节点
            INodeBuilder nodeBuilder = new BinaryBuilder()
            {
                LeftBuilder = leftPair.Key,
                NodeType = binaryExpression.NodeType,
                RightBuilder = rightPair.Key
            };
            //获取其附属参数
            IDbDataParameter[] sqlParameters = leftPair.Value.Concat(rightPair.Value).ToArray();
            //返回最终的sql表达式及其附属参数
            return new KeyValuePair<INodeBuilder, IDbDataParameter[]>(nodeBuilder, sqlParameters);
        }
    }
}
