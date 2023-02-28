using System.Linq.Expressions;
using System.Text;

namespace CloudEntity.CommandTrees.Commom
{
    /// <summary>
    /// sql条件表达式节点
    /// </summary>
    public class BinaryBuilder : INodeBuilder
    {
        /// <summary>
        /// 父节点类型
        /// </summary>
        private SqlType parentNodeType = SqlType.Where;
        /// <summary>
        /// sql操作符
        /// </summary>
        private string sqlOperator;

        /// <summary>
        /// 父节点的sql节点类型
        /// </summary>
        public SqlType ParentNodeType
        {
            get { return this.parentNodeType; }
        }
        /// <summary>
        /// 表达式类型(大于 小于 等)
        /// </summary>
        public ExpressionType NodeType
        {
            set
            {
                switch (value)
                {
                    //大于
                    case ExpressionType.GreaterThan:
                        this.sqlOperator = ">";
                        break;
                    //大于等于
                    case ExpressionType.GreaterThanOrEqual:
                        this.sqlOperator = ">=";
                        break;
                    //小于
                    case ExpressionType.LessThan:
                        this.sqlOperator = "<";
                        break;
                    //小于等于
                    case ExpressionType.LessThanOrEqual:
                        this.sqlOperator = "<=";
                        break;
                    //等于
                    case ExpressionType.Equal:
                        this.sqlOperator = "=";
                        break;
                    //不等于
                    case ExpressionType.NotEqual:
                        this.sqlOperator = "!=";
                        break;

                    //或者
                    case ExpressionType.Or:
                    case ExpressionType.OrElse:
                        this.sqlOperator = "OR";
                        break;
                    //并且
                    case ExpressionType.And:
                    case ExpressionType.AndAlso:
                        this.sqlOperator = "AND";
                        break;
                }
            }
        }
        /// <summary>
        /// sql操作符
        /// </summary>
        public string SqlOperator
        {
            get { return this.sqlOperator; }
            set { this.sqlOperator = value; }
        }
        /// <summary>
        /// 左表达式节点
        /// </summary>
        public ISqlBuilder LeftBuilder { get; set; }
        /// <summary>
        /// 右表达式节点
        /// </summary>
        public ISqlBuilder RightBuilder { get; set; }
        /// <summary>
        /// 当前节点类型
        /// </summary>
        public BuilderType BuilderType
        {
            get { return BuilderType.Binary; }
        }

        /// <summary>
        /// 拼接sql
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            switch (this.sqlOperator)
            {
                case "AND":
                case "OR":
                    commandText.Append("(");
                    this.LeftBuilder.Build(commandText);
                    commandText.AppendFormat(" {0} ", this.sqlOperator);
                    this.RightBuilder.Build(commandText);
                    commandText.Append(")");
                    break;
                default:
                    this.LeftBuilder.Build(commandText);
                    commandText.AppendFormat(" {0} ", this.sqlOperator);
                    this.RightBuilder.Build(commandText);
                    break;
            }
        }
    }
}
