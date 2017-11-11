using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity.Internal.Mapping
{
    /// <summary>
    /// 设置当前对象列的映射关系的类
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    internal class ColumnMapSetter <TEntity> : IColumnMapSetter<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 表的别名
        /// </summary>
        private string tableAlias;
        /// <summary>
        /// 列和属性的映射对象字典
        /// </summary>
        private IDictionary<string, IColumnMapper> columnMappers;

        /// <summary>
        /// 获取该列是否允许为空
        /// </summary>
        /// <param name="columnAction">列的操作</param>
        /// <returns>该列是否允许为空</returns>
        private bool GetAllowNull(ColumnAction columnAction)
        {
            switch (columnAction)
            {
                //主键列，以及只允许Insert时赋值的列不允许为空
                case ColumnAction.Insert:
                case ColumnAction.PrimaryAndInsert:
                case ColumnAction.PrimaryAndIdentity:
                    return false;
                //其他类型的列都允许为空
                default:
                    return true;
            }
        }

        /// <summary>
        /// 创建设置列与属性映射关系的对象
        /// </summary>
        /// <param name="tableAlias">表的别名</param>
        /// <param name="columnMappers">列与属性映射对象的字典</param>
        public ColumnMapSetter(string tableAlias, IDictionary<string, IColumnMapper> columnMappers)
        {
            this.tableAlias = tableAlias;
            this.columnMappers = columnMappers;
        }
        /// <summary>
        /// 设置列的映射
        /// </summary>
        /// <param name="selector">执行当前实体某属性</param>
        /// <param name="action">指定对该列的操作</param>
        /// <param name="columnName">列名</param>
        /// <param name="allowNull">是否允许为空</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter Map(Expression<Func<TEntity, object>> selector, ColumnAction action = ColumnAction.InsertAndEdit, string columnName = null, bool? allowNull = null)
        {
            //获取并检查属性
            PropertyInfo property = selector.Body.GetProperty();
            if (!Check.IsCanMapping(property))
                throw new Exception(string.Format("Can not map property {0}", property.Name));
            if (this.columnMappers.ContainsKey(property.Name))
                return null;
            //创建ColumnMapper
            ColumnMapper columnMapper = new ColumnMapper(property)
            {
                ColumnAction = action,
                ColumnName = columnName ?? property.Name,
                ColumnFullName = string.Format("{0}.{1}", this.tableAlias, columnName ?? property.Name),
                AllowNull = allowNull != null ? allowNull.Value : this.GetAllowNull(action)
            };
            //添加columnMapper
            this.columnMappers.Add(property.Name, columnMapper);
            //饭hi列属性设置器
            return new ColumnSetter(columnMapper);
        }
    }
}
