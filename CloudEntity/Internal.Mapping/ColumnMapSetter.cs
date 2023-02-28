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
    internal class ColumnMapSetter<TEntity> : IColumnMapSetter<TEntity>, IColumnSetter
        where TEntity : class
    {
        /// <summary>
        /// 实体类型
        /// </summary>
        private Type _entityType;
        /// <summary>
        /// 当前的列与属性的映射对象
        /// </summary>
        private ColumnMapper _currentColumnMapper;
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
        public ColumnMapSetter()
        {
            //初始化值
            _entityType = typeof(TEntity);
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
            //创建ColumnMapper
            _currentColumnMapper = new ColumnMapper(property)
            {
                ColumnAction = action,
                ColumnName = columnName ?? property.Name,
                AllowNull = allowNull != null ? allowNull.Value : this.GetAllowNull(action)
            };
            //指定columnMapper
            _columnMappers[property.Name] = _currentColumnMapper;
            //饭hi列属性设置器
            return this;
        }
        /// <summary>
        /// 设置当前列的别名
        /// </summary>
        /// <param name="columnAlias">列的别名</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter As(string columnAlias = null)
        {
            _currentColumnMapper.ColumnAlias = columnAlias ?? _currentColumnMapper.Property.Name;
            return this;
        }
        /// <summary>
        /// 设置类型长度
        /// </summary>
        /// <param name="length">类型长度</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter Length(int length)
        {
            //设置类型长度
            _currentColumnMapper.Length = length;
            //获取当前设置对象
            return this;
        }
        /// <summary>
        /// 设置类型精度(长度和小数点位数)
        /// </summary>
        /// <param name="length">类型长度</param>
        /// <param name="decimals">小数点位数</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter Round(int length, int decimals)
        {
            //设置类型长度
            _currentColumnMapper.Length = length;
            //设置小数点位数
            _currentColumnMapper.Decimals = decimals;
            //获取当前设置对象
            return this;
        }
        /// <summary>
        /// 设置数据类型
        /// </summary>
        /// <param name="dataType">数据类型,可以不设置</param>
        /// <param name="length">类型长度</param>
        /// <param name="decimals">小数点位数</param>
        /// <returns>列信息设置器</returns>
        public IColumnSetter DataType(DataType? dataType = null, int? length = null, int? decimals = null)
        {
            //设置数据类型
            if (dataType != null)
                _currentColumnMapper.DataType = dataType.Value;
            //设置类型长度
            _currentColumnMapper.Length = length;
            //设置小数点位数
            _currentColumnMapper.Decimals = decimals;
            //获取当前设置对象
            return this;
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
                //指定列Mapping对象
                _columnMappers[key] = new ColumnMapper(property)
                {
                    ColumnName = property.Name
                };
            }
            //获取ColumnMapper字典
            return _columnMappers;
        }
    }
}
