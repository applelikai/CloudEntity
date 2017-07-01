using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;

namespace CloudEntity.Internal.WhereVisitors
{
    /// <summary>
    /// 比较类型的Lambda表达式解析类
    /// 李凯 Apple_Li
    /// </summary>
    internal class CompareWhereVisitor : WhereVisitor
    {
        /// <summary>
        /// 判断当前表达式内容是否为null
        /// </summary>
        /// <param name="expression">表达式</param>
        /// <returns>当前表达式内容是否为null</returns>
        private bool IsNullExpression(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Constant:
                    ConstantExpression constantExpression = expression as ConstantExpression;
                    return constantExpression.ToString().Equals("null");
                default:
                    return false;
            }
        }
        /// <summary>
        /// 判断当前表达式是否包含 null表达式
        /// </summary>
        /// <param name="binaryExpression">二叉树表达式</param>
        /// <returns>当前表达式是否包含 null表达式</returns>
        private bool IsContainsNullExpression(BinaryExpression binaryExpression)
        {
            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                    if (this.IsNullExpression(binaryExpression.Left))
                        return true;
                    if (this.IsNullExpression(binaryExpression.Right))
                        return true;
                    return false;
                default:
                    return false;
            }
        }
        /// <summary>
        /// 获取sql null表达式节点
        /// </summary>
        /// <param name="parameterExpression">实体参数表达式</param>
        /// <param name="binaryExpression">二叉树表达式</param>
        /// <returns>sql null表达式节点</returns>
        private INodeBuilder GetNullBuilder(ParameterExpression parameterExpression, BinaryExpression binaryExpression)
        {
            //获取包含参数表达式的表达式
            Expression expression = null;
            if (binaryExpression.Left.ContainsParameterExpression(parameterExpression))
                expression = binaryExpression.Left;
            else if (binaryExpression.Right.ContainsParameterExpression(parameterExpression))
                expression = binaryExpression.Right;
            else
                return null;
            //返回sql表达式节点
            switch (binaryExpression.NodeType)
            {
                case ExpressionType.Equal:
                    return new NodeBuilder(SqlType.Where, "{0} IS NULL", base.GetColumnFullName(expression.GetProperty()));
                case ExpressionType.NotEqual:
                    return new NodeBuilder(SqlType.Where, "{0} IS NOT NULL", base.GetColumnFullName(expression.GetProperty()));
            };
            return null;
        }

        /// <summary>
        /// 创建比较类型表达式解析对象
        /// </summary>
        /// <param name="parameterFactory">sql参数创建对象</param>
        /// <param name="mapperContainer">mapper容器</param>
        public CompareWhereVisitor(IParameterFactory parameterFactory, IMapperContainer mapperContainer)
            : base(parameterFactory, mapperContainer)
        {
        }
        /// <summary>
        /// 解析查询条件表达式,生成sql条件表达式节点及其附属的sql参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="bodyExpression">Lambda表达式的主体(或主体的一部分)</param>
        /// <returns>sql条件表达式节点及其附属的sql参数</returns>
        public override KeyValuePair<INodeBuilder, IDbDataParameter[]> Visit(ParameterExpression parameterExpression, Expression bodyExpression)
        {
            //获取二叉树表达式主体
            BinaryExpression binaryExpression = bodyExpression as BinaryExpression;
            //若该表达式中包含( == null) 或 ( != null),直接返回sql NULL表达式节点及其空的附属参数
            if (this.IsContainsNullExpression(binaryExpression))
            {
                INodeBuilder nodeBuilder = this.GetNullBuilder(parameterExpression, binaryExpression);
                return new KeyValuePair<INodeBuilder, IDbDataParameter[]>(nodeBuilder, new IDbDataParameter[0]);
            }
            //初始化的参数名及参数值
            string parameterName = string.Empty;    //初始化参数名
            object parameterValue = null;           //初始化参数值
            //获取sql表达式节点
            BinaryBuilder binaryBuilder = new BinaryBuilder()
            {
                LeftBuilder = base.GetSqlBuilder(parameterExpression, binaryExpression.Left, ref parameterName, ref parameterValue),
                NodeType = binaryExpression.NodeType,
                RightBuilder = base.GetSqlBuilder(parameterExpression, binaryExpression.Right, ref parameterName, ref parameterValue)
            };
            //确定二叉树sql表达式节点的sql参数节点
            if (binaryBuilder.LeftBuilder == null)
                binaryBuilder.LeftBuilder = new SqlBuilder("${0}", parameterName);
            if (binaryBuilder.RightBuilder == null)
                binaryBuilder.RightBuilder = new SqlBuilder("${0}", parameterName);
            //获取其附属参数数组
            IDbDataParameter[] sqlParameters = new IDbDataParameter[]
            {
                base.CreateParameter(parameterName, parameterValue)
            };
            //返回最终的sql参数表达式及其附属参数
            return new KeyValuePair<INodeBuilder, IDbDataParameter[]>(binaryBuilder, sqlParameters);
        }
    }
}
