using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace CloudEntity.Internal.CommandTrees
{
    /// <summary>
    /// 方法表达式解析类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    internal class MethodCallExpressionParser : PredicateParser
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
                    parameterValue = string.Format("%{0}%", parameterValue);
                    return "LIKE";
                case "StartsWith":
                    parameterValue = string.Concat(parameterValue, "%");
                    return "LIKE";
                case "EndsWith":
                    parameterValue = string.Concat("%", parameterValue);
                    return "LIKE";
                default:
                    return null;
            }
        }
        /// <summary>
        /// 转换获取IList类型的参数值列表
        /// </summary>
        /// <param name="value">值</param>
        /// <returns>IList类型参数值列表</returns>
        private IList GetList(object value)
        {
            // 若值为IList，则直接as 成IList并获取
            if (value is IList)
                return value as IList;
            // 若值也不是可迭代类型，则抛出异常
            if (!(value is IEnumerable))
            {
                // 抛出异常
                throw new Exception(string.Format("解析sql参数值列表出错: {0}", value.ToString()));
            }
            // 转为可迭代类型
            IEnumerable values = value as IEnumerable;
            // 初始化列表
            IList list = new ArrayList();
            // 遍历可迭代类型并加载列表
            foreach (object item in values)
                list.Add(item);
            // 获取列表
            return list;
        }
        /// <summary>
        /// 解析普通类型方法调用的表达式获取sql表达式
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="methodCallExpression">方法调用表达式</param>
        /// <param name="parameterSetter">sql参数设置对象</param>
        /// <returns>sql表达式</returns>
        private INodeBuilder GetNodeBuilder(ParameterExpression parameterExpression, MethodCallExpression methodCallExpression, IParameterSetter parameterSetter)
        {
            // 初始化参数名和参数值
            string parameterName = string.Empty;
            object parameterValue = null;
            // 获取二叉树sql表达式节点
            BinaryBuilder binaryBuilder = new BinaryBuilder()
            {
                LeftBuilder = base.GetSqlBuilder(parameterExpression, methodCallExpression.Object, ref parameterName, ref parameterValue),
                RightBuilder = base.GetSqlBuilder(parameterExpression, methodCallExpression.Arguments[0], ref parameterName, ref parameterValue)
            };
            binaryBuilder.SqlOperator = this.GetSqlOperator(methodCallExpression.Method.Name, ref parameterValue);
            // 确定sql参数节点前，先确定正式的sql参数名称
            parameterName = parameterSetter.GetParameterName(parameterName);
            // 确定二叉树sql表达式节点的sql参数节点
            if (binaryBuilder.LeftBuilder == null)
                binaryBuilder.LeftBuilder = base.GetParameterBuilder(parameterName);
            if (binaryBuilder.RightBuilder == null)
                binaryBuilder.RightBuilder = base.GetParameterBuilder(parameterName);
            // 检查解析表达式是否正确
            if (string.IsNullOrEmpty(binaryBuilder.SqlOperator))
                throw new Exception(string.Format("Unknow Expression: {0}", methodCallExpression.ToString()));
            // 添加sql参数
            parameterSetter.AddSqlParameter(parameterName, parameterValue);
            // 获取sql表达式节点及其附属参数
            return binaryBuilder;
        }
        /// <summary>
        /// 解析Contains方法表达式,获取sql IN语句表达式并设置sql参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="methodCallExpression">方法调用表达式</param>
        /// <param name="parameterSetter">sql参数设置对象</param>
        /// <returns>sql IN语句表达式</returns>
        private INodeBuilder GetSqlInBuilder(ParameterExpression parameterExpression, MethodCallExpression methodCallExpression, IParameterSetter parameterSetter)
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
            //初始化sql表达式
            StringBuilder sqlBuilder = new StringBuilder();
            //获取参数名的开头使用次数
            int times = parameterSetter.GetStartWithTimes(parameterName);
            //获取参数值列表并遍历
            IList sqlParameterValues = this.GetList(parameterValues);
            for (int i = 0; i < sqlParameterValues.Count; i++)
            {
                // 获取新建参数名
                string newParameterName = $"{parameterName}{(i + times).ToString()}";
                // 拼接sql表达式
                sqlBuilder.Append("\n       ");
                sqlBuilder.Append(i == 0 ? '(' : ' ');
                sqlBuilder.AppendFormat("${0}", newParameterName);
                sqlBuilder.Append((i + 1 == sqlParameterValues.Count) ? ")" : ",");
                // 添加sql参数
                parameterSetter.AddSqlParameter(newParameterName, sqlParameterValues[i]);
            }
            //获取右节点
            binaryBuilder.RightBuilder = new SqlBuilder(sqlBuilder.ToString());
            //返回sql表达式
            return binaryBuilder;
        }

        /// <summary>
        /// 创建Lambda表达式解析对象
        /// </summary>
        /// <param name="commandFactory">Sql命令工厂</param>
        /// <param name="mapperContainer">Mapper对象容器</param>
        public MethodCallExpressionParser(ICommandFactory commandFactory, IMapperContainer mapperContainer)
            : base(commandFactory, mapperContainer) { }
        /// <summary>
        /// 解析查询条件表达式，生成并获取sql条件表达式节点列表，附带设置sql参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="bodyExpression">Lambda表达式的主体(或主体的一部分)</param>
        /// <param name="parameterSetter">sql参数设置对象</param>
        /// <returns>sql条件表达式节点</returns>
        public override INodeBuilder Parse(ParameterExpression parameterExpression, Expression bodyExpression, IParameterSetter parameterSetter)
        {
            // 获取方法调用表达式主体
            MethodCallExpression methodCallExpression = bodyExpression as MethodCallExpression;
            // 解析普通类型的方法表达式
            if (methodCallExpression.Object != null)
            {
                // 若方法调用对象为泛型 并且 方法为Contains,
                if (methodCallExpression.Object.Type.GetTypeInfo().IsGenericType && methodCallExpression.Method.Name.Equals("Contains"))
                {
                    // 则解析获取 sql IN表达式
                    return this.GetSqlInBuilder(parameterExpression, methodCallExpression, parameterSetter);
                }
                // 否则解析普通表达式获取sql表达式及其附属参数
                return this.GetNodeBuilder(parameterExpression, methodCallExpression, parameterSetter);
            }
            else if (methodCallExpression.Method.Name.Equals("Contains"))
            {
                // 否则解析获取 sql IN表达式及其附属参数
                return this.GetSqlInBuilder(parameterExpression, methodCallExpression, parameterSetter);
            }
            // 若表达式不满足条件,则异常提示
            throw new Exception(string.Format("Unknow Expression: {0}", bodyExpression.ToString()));
        }
    }
}
