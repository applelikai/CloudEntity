﻿using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Mapping;
using System.Linq.Expressions;

namespace CloudEntity.Internal.CommandTrees
{
    /// <summary>
    /// 查询条件Lambda表达式解析类
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    public abstract class PredicateParser
    {
        /// <summary>
        /// Sql命令工厂
        /// </summary>
        private ICommandFactory _commandFactory;
        /// <summary>
        /// Mapper容器
        /// </summary>
        private IMapperContainer _mapperContainer;

        #region sql表达式生成
        /// <summary>
        /// 获取sql参数表达式
        /// </summary>
        /// <param name="parameterName">参数名</param>
        /// <returns>sql参数表达式</returns>
        protected ISqlBuilder GetParameterBuilder(string parameterName)
        {
            //获取sql参数表达式
            return _commandFactory.GetParameterBuilder(parameterName);
        }
        /// <summary>
        /// 获取基本类型的sql列节点
        /// </summary>
        /// <param name="memberExpression">成员表达式</param>
        /// <returns>基本类型的sql表达式节点</returns>
        protected ISqlBuilder GetColumnBuilder(MemberExpression memberExpression)
        {
            //若Mapper容器为空，则当前查询为视图查询
            if (_mapperContainer == null)
            {
                // 获取目标类型名称作为临时表名
                string tableAlias = memberExpression.Member.DeclaringType.Name.ToLower();
                // 获取成员名称为列名
                string columnName = memberExpression.Member.Name;
                // 获取sql列节点生成器
                return _commandFactory.GetColumnBuilder(tableAlias, columnName);
            }
            //若不为空,则获取针对表的sql表达式
            // 获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(memberExpression.Expression.Type);
            // 获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            // 获取sql列节点生成器
            return _commandFactory.GetColumnBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName);
        }
        /// <summary>
        /// 解析表达式 获取基本类型的sql表达式节点 或 (参数名和参数值)
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="expression">Lambda表达式的主体(或主体的一部分)</param>
        /// <param name="parameterName">参数名</param>
        /// <param name="parameterValue">参数值</param>
        /// <returns>sql表达式节点</returns>
        protected ISqlBuilder GetSqlBuilder(ParameterExpression parameterExpression, Expression expression, ref string parameterName, ref object parameterValue)
        {
            //若当前表达式节点包含参数表达式
            if (expression.ContainsParameterExpression(parameterExpression))
            {
                //获取成员表达式
                MemberExpression memberExpression = expression.GetMemberExpression();
                //获取参数名
                //parameterName = string.Format("{0}_{1}", memberExpression.Expression.Type.Name, memberExpression.Member.Name);
                parameterName = memberExpression.Member.Name;
                //获取sql表达式节点(指定列名)
                return this.GetColumnBuilder(memberExpression);
            }
            //若当前表达式节点不包含参数表达式
            //1.获取参数名
            if (expression.NodeType == ExpressionType.MemberAccess)
                parameterName = (expression as MemberExpression).Member.Name;
            //2.获取参数值
            parameterValue = Expression.Lambda(expression).Compile().DynamicInvoke();
            //返回空
            return null;
        }
        /// <summary>
        /// 获取Where节点的子sql表达式节点
        /// </summary>
        /// <param name="memberExpression">成员表达式</param>
        /// <param name="rightSqlExpression">右边的sql条件表达式</param>
        /// <returns>Where节点的子sql表达式节点</returns>
        protected INodeBuilder GetWhereChildBuilder(MemberExpression memberExpression, string rightSqlExpression)
        {
            // 若Mapper容器为空，则当前查询为视图查询
            if (_mapperContainer == null)
            {
                // 获取目标类型名称作为临时表名
                string tableAlias = memberExpression.Member.DeclaringType.Name.ToLower();
                // 获取成员名称为列名
                string columnName = memberExpression.Member.Name;
                // 获取Where节点的子sql表达式节点
                return _commandFactory.GetWhereChildBuilder(tableAlias, columnName, rightSqlExpression);
            }
            //若不为空,则获取针对实体类对应的表的sql表达式
            // 获取当前实体类型的Table元数据解析器
            ITableMapper tableMapper = _mapperContainer.GetTableMapper(memberExpression.Expression.Type);
            // 获取columnMapper
            IColumnMapper columnMapper = tableMapper.GetColumnMapper(memberExpression.Member.Name);
            // 获取Where节点的子sql表达式节点
            return _commandFactory.GetWhereChildBuilder(tableMapper.Header.TableAlias, columnMapper.ColumnName, rightSqlExpression);
        }
        #endregion

        /// <summary>
        /// 创建Lambda表达式解析对象
        /// </summary>
        /// <param name="commandFactory">Sql命令工厂</param>
        /// <param name="mapperContainer">Mapper对象容器</param>
        public PredicateParser(ICommandFactory commandFactory, IMapperContainer mapperContainer)
        {
            //非空检查
            Check.ArgumentNull(commandFactory, nameof(commandFactory));
            //赋值
            _commandFactory = commandFactory;
            _mapperContainer = mapperContainer;
        }
        /// <summary>
        /// 解析查询条件表达式，生成并获取sql条件表达式节点，附带设置sql参数
        /// </summary>
        /// <param name="parameterExpression">Lambda表达式的参数</param>
        /// <param name="bodyExpression">Lambda表达式的主体(或主体的一部分)</param>
        /// <param name="parameterSetter">sql参数设置对象</param>
        /// <returns>sql条件表达式节点</returns>
        public abstract INodeBuilder Parse(ParameterExpression parameterExpression, Expression bodyExpression, IParameterSetter parameterSetter);
    }
}
