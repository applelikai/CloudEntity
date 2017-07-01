using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CloudEntity.Internal.WhereVisitors
{
    /// <summary>
    /// 方法表达式解析类
    /// 李凯 Apple_Li
    /// </summary>
    internal class MethodCallWhereVisitor : WhereVisitor
    {
        /// <summary>
        /// 获取sql表达式操作符
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="parameterValue">sql参数名</param>
        /// <returns>sql表达式操作符</returns>
        private string GetSqlOperator(string methodName, ref object parameterValue)
        {
            switch (methodName)
            {
                case "Equals":
                    return "=";
                case "Contains":
                    parameterValue = string.Format("%{0}%", parameterValue.ToString());
                    return "LIKE";
                case "StartsWith":
                    parameterValue = string.Concat(parameterValue.ToString(), "%");
                    return "LIKE";
                case "EndsWith":
                    parameterValue = string.Concat("%", parameterValue.ToString());
                    return "LIKE";
                default:
                    return null;
            }
        }
        /// <summary>
        /// 解析普通类型方法调用的表达式获取sql表达式及其附属的参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="methodCallExpression">方法调用表达式</param>
        /// <returns>sql表达式及其附属的参数</returns>
        private KeyValuePair<INodeBuilder, IDbDataParameter[]> GetNodeBuilderPair(ParameterExpression parameterExpression, MethodCallExpression methodCallExpression)
        {
            //初始化参数名和参数值
            string parameterName = string.Empty;
            object parameterValue = null;
            //获取二叉树sql表达式节点
            BinaryBuilder binaryBuilder = new BinaryBuilder()
            {
                LeftBuilder = base.GetSqlBuilder(parameterExpression, methodCallExpression.Object, ref parameterName, ref parameterValue),
                RightBuilder = base.GetSqlBuilder(parameterExpression, methodCallExpression.Arguments[0], ref parameterName, ref parameterValue)
            };
            binaryBuilder.SqlOperator = this.GetSqlOperator(methodCallExpression.Method.Name, ref parameterValue);
            //确定二叉树sql表达式节点的sql参数节点
            if (binaryBuilder.LeftBuilder == null)
                binaryBuilder.LeftBuilder = new SqlBuilder("${0}", parameterName);
            if (binaryBuilder.RightBuilder == null)
                binaryBuilder.RightBuilder = new SqlBuilder("${0}", parameterName);
            //检查解析表达式是否正确
            if (string.IsNullOrEmpty(binaryBuilder.SqlOperator))
                throw new Exception(string.Format("Unknow Expression: {0}", methodCallExpression.ToString()));
            //返回sql表达式节点及其附属参数
            return new KeyValuePair<INodeBuilder, IDbDataParameter[]>(binaryBuilder, new IDbDataParameter[]
            {
                base.CreateParameter(parameterName, parameterValue)
            });
        }
        /// <summary>
        /// 解析Contains方法表达式,获取sql IN语句表达式及其附属参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="methodCallExpression">方法调用表达式</param>
        /// <returns>sql IN语句表达式及其附属参数</returns>
        private KeyValuePair<INodeBuilder, IDbDataParameter[]> GetSqlInBuilderPair(ParameterExpression parameterExpression, MethodCallExpression methodCallExpression)
        {
            //获取sql表达式节点和所属参数数组
            BinaryBuilder binaryBuilder = new BinaryBuilder()
            {
                SqlOperator = "IN"
            };
            //初始化参数名和参数集合
            string parameterName = string.Empty;
            object parameterValues = null;
            //获取左节点
            foreach (Expression argument in methodCallExpression.Arguments)
            {
                binaryBuilder.LeftBuilder = base.GetSqlBuilder(parameterExpression, argument, ref parameterName, ref parameterValues);
                //若左节点不为空，直接退出
                if (binaryBuilder.LeftBuilder != null)
                    break;
            }
            //获取参数集合
            if (parameterValues == null)
                parameterValues = Expression.Lambda(methodCallExpression.Object).Compile().DynamicInvoke();
            //获取IN语句内的sql表达式及其附属sql参数数组
            IList sqlParameterValues = parameterValues as IList;
            StringBuilder sqlTemplate = new StringBuilder();
            IDbDataParameter[] sqlParameters = new IDbDataParameter[sqlParameterValues.Count];
            for (int i = 0; i < sqlParameterValues.Count; i++)
            {
                //拼接sql表达式
                sqlTemplate.Append("\n<######>");
                sqlTemplate.Append(i == 0 ? '(' : ' ');
                sqlTemplate.AppendFormat("${0}", parameterName);
                sqlTemplate.AppendFormat("{0}{1}", i, (i + 1 == sqlParameterValues.Count) ? ")" : ",");
                //拼接参数
                sqlParameters[i] = base.CreateParameter(string.Concat(parameterName, i), sqlParameterValues[i]);
            }
            //获取右节点
            binaryBuilder.RightBuilder = new SqlBuilder(sqlTemplate.ToString());
            //返回sql表达式及其附属参数数组
            return new KeyValuePair<INodeBuilder, IDbDataParameter[]>(binaryBuilder, sqlParameters);
        }

        /// <summary>
        /// 创建Lambda表达式解析对象
        /// </summary>
        /// <param name="parameterFactory">sql参数创建对象</param>
        /// <param name="mapperContainer">mapper容器</param>
        public MethodCallWhereVisitor(IParameterFactory parameterFactory, IMapperContainer mapperContainer)
            : base(parameterFactory, mapperContainer)
        {
        }
        /// <summary>
        /// 方法调用表达式,生成sql条件表达式节点及其附属的sql参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="bodyExpression">Lambda表达式的主体(或主体的一部分)</param>
        /// <returns>sql条件表达式节点及其附属的sql参数</returns>
        public override KeyValuePair<INodeBuilder, IDbDataParameter[]> Visit(ParameterExpression parameterExpression, Expression bodyExpression)
        {
            //获取方法调用表达式主体
            MethodCallExpression methodCallExpression = bodyExpression as MethodCallExpression;
            //解析普通类型的方法表达式
            if (methodCallExpression.Object != null)
            {
                //若方法调用对象为泛型 并且 方法为Contains,则解析获取 sql IN表达式
                if (methodCallExpression.Object.Type.GetTypeInfo().IsGenericType && methodCallExpression.Method.Name.Equals("Contains"))
                    return this.GetSqlInBuilderPair(parameterExpression, methodCallExpression);
                //解析普通表达式获取sql表达式及其附属参数
                return this.GetNodeBuilderPair(parameterExpression, methodCallExpression);
            }
            //解析获取 sql IN表达式及其附属参数
            else if (methodCallExpression.Method.Name.Equals("Contains"))
                return this.GetSqlInBuilderPair(parameterExpression, methodCallExpression);
            //若表达式不满足条件,则异常提示
            throw new Exception(string.Format("Unknow Expression: {0}", bodyExpression.ToString()));
        }
    }
}
