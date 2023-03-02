using CloudEntity.CommandTrees;
using CloudEntity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 排序查询数据源基类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    internal abstract class DbSortBase : DbQueryBase
    {
        /// <summary>
        /// 获取orderby节点的子节点
        /// </summary>
        /// <param name="memberExpression">成员表达式</param>
        /// <param name="isDesc">是否降序[true:降序 false:升序]</param>
        /// <returns>orderby节点的子节点</returns>
        protected abstract INodeBuilder GetOrderbyNodeBuilder(MemberExpression memberExpression, bool isDesc);
        /// <summary>
        /// 获取orderby节点的子节点集合
        /// </summary>
        /// <param name="memberExpressions">成员表达式列表</param>
        /// <param name="isDesc">是否降序[true:降序 false:升序]</param>
        /// <returns>orderby节点的子节点集合</returns>
        protected IEnumerable<INodeBuilder> GetOrderbyNodeBuilders(IEnumerable<MemberExpression> memberExpressions, bool isDesc)
        {
            //遍历所有的成员表达式
            foreach (MemberExpression memberExpression in memberExpressions)
            {
                //依次获取orderby节点的子表达式节点
                yield return this.GetOrderbyNodeBuilder(memberExpression, isDesc);
            }
        }
        /// <summary>
        /// 获取orderby节点的子节点集合
        /// </summary>
        /// <param name="selector">选定排序项表达式</param>
        /// <param name="isDesc">true:降序 false:升序</param>
        /// <returns>orderby节点的子节点集合</returns>
        protected IEnumerable<INodeBuilder> GetOrderbyNodeBuilders(LambdaExpression selector, bool isDesc)
        {
            //解析Lambda Select表达式为nodeBuilders添加父类型为Select的子sql表达式节点
            switch (selector.Body.NodeType)
            {
                //解析转换表达式及其成员表达式(e => e.Property1)
                case ExpressionType.Convert:
                case ExpressionType.MemberAccess:
                    //获取成员表达式
                    MemberExpression memberExpression = selector.Body.GetMemberExpression();
                    //生成并返回OrderBy节点的子节点
                    yield return this.GetOrderbyNodeBuilder(memberExpression, isDesc);
                    break;
                //解析NewExpression(e => new Model(e.Property1, e.Property2))
                case ExpressionType.New:
                    //获取成员数组
                    NewExpression newExpression = selector.Body as NewExpression;
                    //获取成员表达式列表
                    IEnumerable<MemberExpression> memberExpressions = newExpression.Arguments.OfType<MemberExpression>();
                    //为nodeBuilders添加父类型为Select的子sql表达式节点
                    foreach (INodeBuilder columnBuilder in this.GetOrderbyNodeBuilders(memberExpressions, isDesc))
                        yield return columnBuilder;
                    break;
                //遇到未知类型的表达式直接异常
                default:
                    throw new Exception(string.Format("Unknow Expression: {0}", selector));
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        public DbSortBase(ICommandFactory commandFactory, IDbHelper dbHelper)
         : base(commandFactory, dbHelper) { }
    }
}