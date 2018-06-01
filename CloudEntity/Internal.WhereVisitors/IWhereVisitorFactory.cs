using System.Linq.Expressions;

namespace CloudEntity.Internal.WhereVisitors
{
    /// <summary>
    /// 创建WhereVisitor的工厂
    /// 李凯 Apple_Li
    /// </summary>
    public interface IWhereVisitorFactory
    {
        /// <summary>
        /// 创建表达式解析器
        /// </summary>
        /// <param name="expressionType">表达式类型</param>
        /// <returns>表达式解析器</returns>
        WhereVisitor GetVisitor(ExpressionType expressionType);
    }
}
