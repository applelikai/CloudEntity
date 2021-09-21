using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity.Internal.Mapping
{
    /// <summary>
    /// 设置当前对象列的映射关系的类
    /// </summary>
    /// <typeparam name="TEntity">实体</typeparam>
    internal class ColumnMapSetter<TEntity> : IColumnMapSetter<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 表的别名
        /// </summary>
        private string _tableAlias;
        /// <summary>
        /// 实体类型
        /// </summary>
        private Type _entityType;
        /// <summary>
        /// 列名获取对象
        /// </summary>
        private IColumnNameGetter _columnNameGetter;
        /// <summary>
        /// 列和属性的映射对象字典
        /// </summary>
        private IDictionary<string, IColumnMapper> _columnMappers;

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
        /// 获取初始化的列和属性的映射对象字典
        /// </summary>
        /// <returns>列和属性的映射对象字典</returns>
        private IDictionary<string, IColumnMapper> GetInitColumnMappers()
        {
            //初始化ColumnMapper字典
            IDictionary<string, IColumnMapper> columnMappers = new Dictionary<string, IColumnMapper>();
            //遍历所有属性,加载ColumnMapper字典
            foreach (PropertyInfo property in _entityType.GetRuntimeProperties())
            {
                //若当前属性不满足Mapping条件，本次循环
                if (!Check.IsCanMapping(property))
                    continue;
                //添加初始化项目，为之后的ColumnMapper占位
                columnMappers.Add(property.Name, null);
            }
            //获取ColumnMapper字典
            return columnMappers;
        }

        /// <summary>
        /// 创建设置列与属性映射关系的对象
        /// </summary>
        /// <param name="tableAlias">表的别名</param>
        /// <param name="columnNameGetter">列名获取对象</param>
        public ColumnMapSetter(string tableAlias, IColumnNameGetter columnNameGetter)
        {
            //初始化值
            _tableAlias = tableAlias;
            _entityType = typeof(TEntity);
            _columnNameGetter = columnNameGetter;
            //初始化ColumnMapper字典
            _columnMappers = this.GetInitColumnMappers();
            
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
            //获取列名
            if (string.IsNullOrEmpty(columnName))
                columnName = _columnNameGetter.GetColumnName(property.Name);
            //创建ColumnMapper
            ColumnMapper columnMapper = new ColumnMapper(property)
            {
                ColumnAction = action,
                ColumnName = columnName,
                AllowNull = allowNull != null ? allowNull.Value : this.GetAllowNull(action)
            };
            //指定columnMapper
            this._columnMappers[property.Name] = columnMapper;
            //饭hi列属性设置器
            return new ColumnSetter(columnMapper);
        }
        /// <summary>
        /// 获取列和属性的映射对象字典
        /// </summary>
        /// <returns>列和属性的映射对象字典</returns>
        public IDictionary<string, IColumnMapper> GetColumnMappers()
        {
            //获取所有空项的key数组
            string[] keys = _columnMappers.Where(pair => pair.Value == null).Select(pair => pair.Key).ToArray();
            //检查ColumnMapper字典中的所有空项
            foreach (string key in keys)
            {
                //获取属性
                PropertyInfo property = _entityType.GetProperty(key);
                //获取列名
                string columnName = _columnNameGetter.GetColumnName(property.Name);
                //指定列Mapping对象
                _columnMappers[key] = new ColumnMapper(property)
                {
                    ColumnName = columnName
                };
            }
            //获取ColumnMapper字典
            return _columnMappers;
        }
    }
}
