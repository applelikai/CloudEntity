using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace CloudEntity.Internal.WhereVisitors
{
    /// <summary>
    /// 非表达式解析类
    /// 李凯 Apple_Li
    /// </summary>
    internal class UnaryNotWhereVisitor : WhereVisitor
    {
        //创建表达式解析器的工厂
        private IWhereVisitorFactory factory;

        /// <summary>
        /// 创建非表达式解析对象
        /// </summary>
        /// <param name="parameterFactory">sql参数创建对象</param>
        /// <param name="columnGetter">列名获取器</param>
        /// <param name="factory">创建表达式解析器的工厂</param>
        public UnaryNotWhereVisitor(IParameterFactory parameterFactory, IColumnGetter columnGetter, IWhereVisitorFactory factory)
            : base(parameterFactory, columnGetter)
        {
            this.factory = factory;
        }
        /// <summary>
        /// 解析非表达式,生成sql条件表达式节点及其附属的sql参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="bodyExpression">Lambda表达式的主体(或主体的一部分)</param>
        /// <returns>sql条件表达式节点及其附属的sql参数</returns>
        public override KeyValuePair<INodeBuilder, IDbDataParameter[]> Visit(ParameterExpression parameterExpression, Expression bodyExpression)
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
            WhereVisitor whereVisitor = this.factory.GetVisitor(unaryExpression.Operand.NodeType);
            KeyValuePair<INodeBuilder, IDbDataParameter[]> sqlBuilderPair = whereVisitor.Visit(parameterExpression, unaryExpression.Operand);
            StringBuilder sqlTemplate = new StringBuilder();
            sqlBuilderPair.Key.Build(sqlTemplate);
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
            INodeBuilder nodeBuilder = new NodeBuilder(SqlType.Where, sqlTemplate.ToString());
            //返回sql表达式及其附属的sql参数集合
            return new KeyValuePair<INodeBuilder, IDbDataParameter[]>(nodeBuilder, sqlBuilderPair.Value);
        }
    }
}
