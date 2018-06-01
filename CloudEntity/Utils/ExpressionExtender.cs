using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity
{
    /// <summary>
    /// 表达式扩展类
    /// </summary>
    public static class ExpressionExtender
    {
        /// <summary>
        /// 判断当前表达式是否包含参数表达式
        /// </summary>
        /// <param name="expression">当前表达式</param>
        /// <param name="parameterExpression">参数表达式</param>
        /// <returns>当前表达式是否包含参数表达式</returns>
        internal static bool ContainsParameterExpression(this Expression expression, ParameterExpression parameterExpression)
        {
            switch (expression.NodeType)
            {
                //若当前expression为参数表达式节点
                case ExpressionType.Parameter:
                    return expression.Equals(parameterExpression);
                //若当前expression为成员访问表达式节点
                case ExpressionType.MemberAccess:
                    MemberExpression memberExpression = expression as MemberExpression;
                    if (memberExpression.Expression != null)
                        return memberExpression.Expression.ContainsParameterExpression(parameterExpression);
                    break;
                //若当前expression为一元表达式节点
                case ExpressionType.Convert:
                    UnaryExpression unaryExpression = expression as UnaryExpression;
                    if (unaryExpression.Operand != null)
                        return unaryExpression.Operand.ContainsParameterExpression(parameterExpression);
                    break;
            }
            return false;
        }
        /// <summary>
        /// 判断当前表达式是否包含参数表达式数组中任意一个参数表达式
        /// </summary>
        /// <param name="expression">当前表达式</param>
        /// <param name="parameterExpressions">参数表达式数组</param>
        /// <returns>当前表达式是否包含参数表达式数组中任意一个参数表达式</returns>
        internal static bool ContainsParameterExpression(this Expression expression, params ParameterExpression[] parameterExpressions)
        {
            //若没有任何参数表达式,返回null
            if (parameterExpressions.Length == 0)
                return false;
            //遍历参数表达式，依次判断是否包含当前参数表达式
            foreach (ParameterExpression parameterExpression in parameterExpressions)
            {
                if (expression.ContainsParameterExpression(parameterExpression))
                    return true;
            }
            //若不包含任何参数表达式,返回false
            return false;
        }
        /// <summary>
        /// 获取成员表达式
        /// </summary>
        /// <param name="expression">表达式主体</param>
        /// <returns>成员表达式</returns>
        internal static MemberExpression GetMemberExpression(this Expression expression)
        {
            //获取成员表达式
            MemberExpression memberExpression = expression.GetMemberExpressionOrDefault();
            //若成员表达式不为空，直接返回
            if (memberExpression != null)
                return memberExpression;
            //若成员表达式为空,直接扔出异常
            throw new Exception(string.Format("unknow expression:{0}", expression));
        }
        /// <summary>
        /// 获取成员表达式
        /// </summary>
        /// <param name="expression">表达式主体</param>
        /// <returns>成员表达式</returns>
        internal static MemberExpression GetMemberExpressionOrDefault(this Expression expression)
        {
            switch (expression.NodeType)
            {
                //若当前表达式为成员表达式节点，直接返回
                case ExpressionType.MemberAccess:
                    return expression as MemberExpression;
                //若当前表达式为Convert表达式节点
                case ExpressionType.Convert:
                    UnaryExpression unaryExpression = expression as UnaryExpression;
                    if (unaryExpression.Operand != null)
                        return unaryExpression.Operand.GetMemberExpressionOrDefault();
                    break;
            }
            return null;
        }
        /// <summary>
        /// 获取当前表达式中包含的属性
        /// </summary>
        /// <param name="expression">当前表达式</param>
        /// <returns>当前表达式中包含的属性</returns>
        internal static PropertyInfo GetProperty(this Expression expression)
        {
            PropertyInfo property = expression.GetPropertyOrDefault();
            if (property == null)
                throw new Exception(string.Format("unknow expression: {0}", expression));
            return property;
        }
        /// <summary>
        /// 获取当前表达式中包含的属性
        /// </summary>
        /// <param name="expression">当前表达式</param>
        /// <returns>当前表达式中包含的属性</returns>
        internal static PropertyInfo GetPropertyOrDefault(this Expression expression)
        {
            //获取成员表达式
            MemberExpression memberExpression = expression.GetMemberExpressionOrDefault();
            //若成员表达式为空，返回null
            if (memberExpression == null)
                return null;
            //获取memberInfo
            MemberInfo memberInfo = memberExpression.Member;
            //若memberInfo为PropertyInfo, 直接转换为PropertyInfo并返回
            if (memberInfo is PropertyInfo)
                return memberInfo as PropertyInfo;
            //否则返回null
            return null;
        }

        /// <summary>
        /// 扩展方法:获取表达式树节点中所包含的属性名
        /// </summary>
        /// <param name="expression">表达式树节点</param>
        /// <returns>表达式树节点中所包含的属性名</returns>
        public static string GetMemberName(this Expression expression)
        {
            MemberExpression memberExpression = expression.GetMemberExpression();
            return memberExpression.Member.Name;
        }
    }
}
