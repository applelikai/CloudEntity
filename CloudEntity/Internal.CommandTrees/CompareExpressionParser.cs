using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Linq.Expressions;

namespace CloudEntity.Internal.CommandTrees
{
    /// <summary>
    /// 比较类型的Lambda表达式解析类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    internal class CompareExpressionParser : PredicateParser
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
                    return base.GetWhereChildBuilder(expression.GetMemberExpression(), "IS NULL");
                case ExpressionType.NotEqual:
                    return base.GetWhereChildBuilder(expression.GetMemberExpression(), "IS NOT NULL");
            };
            return null;
        }

        /// <summary>
        /// 创建比较类型表达式解析对象
        /// </summary>
        /// <param name="commandFactory">Sql命令工厂</param>
        /// <param name="mapperContainer">Mapper对象容器</param>
        public CompareExpressionParser(ICommandFactory commandFactory, IMapperContainer mapperContainer)
            : base(commandFactory, mapperContainer) { }
        /// <summary>
        /// 解析查询条件表达式, 生成sql条件表达式节点以及设置sql参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="bodyExpression">Lambda表达式的主体(或主体的一部分)</param>
        /// <param name="parameterSetter">sql参数设置对象</param>
        /// <returns>sql条件表达式节点</returns>
        public override INodeBuilder Parse(ParameterExpression parameterExpression, Expression bodyExpression, IParameterSetter parameterSetter)
        {
            // 获取二叉树表达式主体
            BinaryExpression binaryExpression = bodyExpression as BinaryExpression;
            // 若该表达式中包含( == null) 或 ( != null)
            if (this.IsContainsNullExpression(binaryExpression))
            {
                // 直接返回sql NULL表达式节点及其空的附属参数
                return this.GetNullBuilder(parameterExpression, binaryExpression);
            }
            // 初始化的参数名及参数值
            string parameterName = string.Empty;
            object parameterValue = null;
            //获取sql表达式节点
            BinaryBuilder binaryBuilder = new BinaryBuilder()
            {
                LeftBuilder = base.GetSqlBuilder(parameterExpression, binaryExpression.Left, ref parameterName, ref parameterValue),
                NodeType = binaryExpression.NodeType,
                RightBuilder = base.GetSqlBuilder(parameterExpression, binaryExpression.Right, ref parameterName, ref parameterValue)
            };
            // 确定sql参数节点前，先确定正式的sql参数名称，并设置参数
            parameterName = parameterSetter.GetParameterName(parameterName);
            parameterSetter.AddSqlParameter(parameterName, parameterValue);
            // 确定二叉树sql表达式节点的sql参数节点
            if (binaryBuilder.LeftBuilder == null)
                binaryBuilder.LeftBuilder = base.GetParameterBuilder(parameterName);
            if (binaryBuilder.RightBuilder == null)
                binaryBuilder.RightBuilder = base.GetParameterBuilder(parameterName);
            // 获取最终的二叉树sql表达式节点
            return binaryBuilder;
        }
    }
}
