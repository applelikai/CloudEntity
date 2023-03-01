using CloudEntity.Internal.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CloudEntity.Mapping.Common
{
    /// <summary>
    /// 实体和表的映射基类
    /// </summary>
    public abstract class TableMapper : ITableMapper
    {
        /// <summary>
        /// 线程锁
        /// </summary>
        private object locker;
        /// <summary>
        /// 表的基本信息
        /// </summary>
        private ITableHeader tableHeader;
        /// <summary>
        /// 主键列与属性的映射对象
        /// </summary>
        private IColumnMapper keyMapper;
        /// <summary>
        /// 列与属性的映射对象字典(Key:属性名, Value:ColumnMapper)
        /// </summary>
        private IDictionary<string, IColumnMapper> _columnMappers;

        /// <summary>
        /// 实体类型
        /// </summary>
        public Type EntityType { get; private set; }
        /// <summary>
        /// 对应的表的基本信息
        /// </summary>
        public ITableHeader Header
        {
            get
            {
                Start:
                //若tableInfo不为空直接返回
                if (this.tableHeader != null)
                    return this.tableHeader;
                //进入临界区(只能一个线程进入)
                lock (this.locker)
                {
                    //若tableInfo为空则创建
                    if (this.tableHeader == null)
                        this.tableHeader = this.GetHeader();
                    //回到开始
                    goto Start;
                }
            }
        }
        /// <summary>
        /// 主键列与属性的映射对象
        /// </summary>
        public IColumnMapper KeyMapper
        {
            get
            {
                Start:
                //若keyMapper不为空，直接返回
                if (this.keyMapper != null)
                    return this.keyMapper;
                //进入临界区
                lock (this.locker)
                {
                    //若keyMapper为空，则查询获取
                    if (this.keyMapper == null)
                        this.keyMapper = _columnMappers.Values.Single(c => c.ColumnAction.ToString().StartsWith("Primary"));
                    //回到Start
                    goto Start;
                }
            }
        }

        /// <summary>
        /// 创建表的基本信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="schemaName">架构名</param>
        /// <param name="tableAlias">表的别名</param>
        /// <returns>表的基本信息</returns>
        protected ITableHeader GetHeader(string tableName, string schemaName = null, string tableAlias = null)
        {
            return new TableHeader()
            {
                SchemaName = schemaName,
                TableName = tableName,
                TableAlias = tableAlias ?? this.EntityType.Name.ToLower()
            };
        }
        /// <summary>
        /// 获取表的基本信息
        /// </summary>
        /// <returns>表的基本信息</returns>
        protected abstract ITableHeader GetHeader();
        /// <summary>
        /// 加载列与属性的映射对象字典
        /// </summary>
        /// <param name="columnMappers">列与属性的映射对象字典</param>
        protected abstract void LoadColumnMappers(IDictionary<string, IColumnMapper> columnMappers);

        /// <summary>
        /// 创建TableMapper类型
        /// </summary>
        /// <param name="entityType">实体类型</param>
        public TableMapper(Type entityType)
        {
            // 非空检查
            Check.ArgumentNull(entityType, nameof(entityType));
            // 赋值
            this.EntityType = entityType;
            this.locker = new object();
            // 初始化列与属性的映射对象字典
            _columnMappers = new Dictionary<string, IColumnMapper>();
            // 加载列与属性的映射对象字典
            this.LoadColumnMappers(_columnMappers);
        }
        /// <summary>
        /// 获取当前属性对应的ColumnMapper
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns>当前属性对应的ColumnMapper</returns>
        public IColumnMapper GetColumnMapper(string propertyName)
        {
            // 若存在ColumnMapper则获取
            if (_columnMappers.ContainsKey(propertyName))
                return _columnMappers[propertyName];
            // 若不存在则扔出异常
            throw new Exception(string.Format("Can not find columnMapper about property named {0}, Please Check this property.", propertyName));
        }
        /// <summary>
        /// 获取所有的列与属性的映射对象
        /// </summary>
        /// <returns>所有的列与属性的映射对象</returns>
        public IEnumerable<IColumnMapper> GetColumnMappers()
        {
            return _columnMappers.Values;
        }
    }
    /// <summary>
    /// 实体和表的映射基类
    /// </summary>
    public abstract class TableMapper<TEntity> : TableMapper
        where TEntity : class
    {
        /// <summary>
        /// 加载ColumnMapper字典，用null为ColumnMapper占位
        /// </summary>
        /// <param name="columnMappers">ColumnMapper字典</param>
        private void LoadNullColumnMappers(IDictionary<string, IColumnMapper> columnMappers)
        {
            //遍历所有属性,加载ColumnMapper字典
            foreach (PropertyInfo property in base.EntityType.GetRuntimeProperties())
            {
                //若当前属性不满足Mapping条件，本次循环
                if (!Check.IsCanMapping(property))
                    continue;
                //添加初始化项目，为之后的ColumnMapper占位
                columnMappers.Add(property.Name, null);
            }
        }
        /// <summary>
        /// 加载ColumnMapper字典，彻底填充ColumnMapper
        /// </summary>
        /// <param name="columnMappers">ColumnMapper字典</param>
        private void FullColumnMappers(IDictionary<string, IColumnMapper> columnMappers)
        {
            //获取所有空项的key数组
            string[] keys = columnMappers.Where(pair => pair.Value == null).Select(pair => pair.Key).ToArray();
            //检查ColumnMapper字典中的所有空项
            foreach (string key in keys)
            {
                //获取属性
                PropertyInfo property = base.EntityType.GetProperty(key);
                //指定列Mapping对象
                columnMappers[key] = new ColumnMapper(property)
                {
                    ColumnName = property.Name
                };
            }
        }

        /// <summary>
        /// 加载列与属性的映射对象字典
        /// </summary>
        /// <param name="columnMappers">列与属性的映射对象字典</param>
        protected override void LoadColumnMappers(IDictionary<string, IColumnMapper> columnMappers)
        {
            // 初步加载Mapper字典，为其占位
            this.LoadNullColumnMappers(columnMappers);

            // 获取Column映射设置对象
            ColumnMapSetter<TEntity> columnMapSetter = new ColumnMapSetter<TEntity>(columnMappers);
            // 设置列与属性的映射关系，加载columnMapper字典
            this.SetColumnMappers(columnMapSetter);

            // 再次加载ColumnMapper字典，彻底填充ColumnMapper
            this.FullColumnMappers(columnMappers);
        }
        /// <summary>
        /// 设置列与属性的映射关系
        /// </summary>
        /// <param name="setter">列与属性的关系的设置器</param>
        protected abstract void SetColumnMappers(IColumnMapSetter<TEntity> setter);

        /// <summary>
        /// 创建TableMapper对象
        /// </summary>
        public TableMapper()
            : base(typeof(TEntity)) { }
    }
}
