using System.Linq.Expressions;

namespace CloudEntity.Internal.CommandTrees
{
    /// <summary>
    /// 创建表达式解析对象的工厂
    /// 李凯 Apple_Li 15150598493
    /// 最后修改时间：2012/02/10 23:22
    /// </summary>
    public interface IPredicateParserFactory
    {
        /// <summary>
        /// 获取表达式解析器
        /// </summary>
        /// <param name="expressionType">表达式类型</param>
        /// <returns>表达式解析器</returns>
        PredicateParser GetPredicateParser(ExpressionType expressionType);
    }
}
