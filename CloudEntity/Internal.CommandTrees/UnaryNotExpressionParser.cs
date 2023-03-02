using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System;
using System.Linq.Expressions;
using System.Text;

namespace CloudEntity.Internal.CommandTrees
{
    /// <summary>
    /// 非表达式解析类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    internal class UnaryNotExpressionParser : PredicateParser
    {
        /// <summary>
        /// 创建表达式解析器的工厂
        /// </summary>
        private IPredicateParserFactory _factory;

        /// <summary>
        /// 创建非表达式解析对象
        /// </summary>
        /// <param name="commandFactory">Sql命令工厂</param>
        /// <param name="mapperContainer">Mapper对象容器</param>
        /// <param name="factory">创建表达式解析器的工厂</param>
        public UnaryNotExpressionParser(ICommandFactory commandFactory, IMapperContainer mapperContainer, IPredicateParserFactory factory)
            : base(commandFactory, mapperContainer)
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
            //获取一元表达式并检查该表达式是否满足解析条件
            UnaryExpression unaryExpression = bodyExpression as UnaryExpression;
            if (unaryExpression.Operand == null)
                throw new Exception(string.Format("Unknow Expression:{0}", unaryExpression.ToString()));
            switch (unaryExpression.Operand.NodeType)
            {
                case ExpressionType.Equal:
                case ExpressionType.NotEqual:
                case ExpressionType.Call:
                    break;
                default:
                    throw new Exception(string.Format("Unknow Expression:{0}", unaryExpression.ToString()));
            }
            //获取sql语句模板
            PredicateParser predicateParser = _factory.GetPredicateParser(unaryExpression.Operand.NodeType);
            INodeBuilder sqlBuilder = predicateParser.Parse(parameterExpression, unaryExpression.Operand, parameterSetter);
            StringBuilder sqlTemplate = new StringBuilder();
            sqlBuilder.Build(sqlTemplate);
            //替换sql语句模板中的操作符为相反类型的操作符(如LIKE 替换为NOT LIKE)
            string baseSqlTemplate = sqlTemplate.ToString();
            if (baseSqlTemplate.Contains("LIKE"))
                sqlTemplate.Replace("LIKE", "NOT LIKE");
            if (baseSqlTemplate.Contains(" = "))
                sqlTemplate.Replace(" = ", " <> ");
            if (baseSqlTemplate.Contains(" != "))
                sqlTemplate.Replace(" != ", " = ");
            if (baseSqlTemplate.Contains("IS NULL"))
                sqlTemplate.Replace("IS NULL", "IS NOT NULL");
            if (baseSqlTemplate.Contains("IS NOT NULL"))
                sqlTemplate.Replace("IS NOT NULL", "IS NULL");
            if (baseSqlTemplate.Contains("IN"))
                sqlTemplate.Replace("IN", "NOT IN");
            //获取sql表达式
            return new NodeBuilder(SqlType.Where, sqlTemplate.ToString());
        }
    }
}
