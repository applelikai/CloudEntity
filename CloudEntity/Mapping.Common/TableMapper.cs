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
        private IDictionary<string, IColumnMapper> columnMappers;

        /// <summary>
        /// 列与属性的映射对象字典(Key:属性名, Value:ColumnMapper)
        /// </summary>
        protected IDictionary<string, IColumnMapper> ColumnMappers
        {
            get
            {
                Start:
                //若columnMappers不为空，直接返回
                if (this.columnMappers != null)
                    return this.columnMappers;
                //进入临界区
                lock (this.locker)
                {
                    //若columnMappers为空，则创建
                    if (this.columnMappers == null)
                    {
                        this.columnMappers = new Dictionary<string, IColumnMapper>();
                        this.LoadColumnMappers(columnMappers);
                    }
                    //回到开始
                    goto Start;
                }
            }
        }

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
                        this.keyMapper = this.ColumnMappers.Values.Single(c => c.ColumnAction.ToString().StartsWith("Primary"));
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
        /// 创建ColumnMapper对象
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns>ColumnMapper对象</returns>
        protected abstract IColumnMapper CreateColumnMapper(PropertyInfo property);
        /// <summary>
        /// 设置列与属性的映射
        /// </summary>
        /// <param name="columnMappers">列与属性的映射对象字典</param>
        protected virtual void LoadColumnMappers(IDictionary<string, IColumnMapper> columnMappers)
        {
            //遍历所有属性
            foreach (PropertyInfo property in this.EntityType.GetRuntimeProperties())
            {
                //若当前属性不满足Mapping条件，本次循环
                if (!Check.IsCanMapping(property))
                    continue;
                //若当前属性已注册，退出本次循环
                if (columnMappers.ContainsKey(property.Name))
                    continue;
                //创建ColumnMapper并添加
                columnMappers.Add(property.Name, this.CreateColumnMapper(property));
            }
        }

        /// <summary>
        /// 创建TableMapper类型
        /// </summary>
        /// <param name="entityType">实体类型</param>
        public TableMapper(Type entityType)
        {
            //非空检查
            Check.ArgumentNull(entityType, nameof(entityType));
            //赋值
            this.locker = new object();
            this.EntityType = entityType;
        }
        /// <summary>
        /// 获取当前属性对应的ColumnMapper
        /// </summary>
        /// <param name="propertyName">属性名</param>
        /// <returns>当前属性对应的ColumnMapper</returns>
        public IColumnMapper GetColumnMapper(string propertyName)
        {
            if (this.ColumnMappers.ContainsKey(propertyName))
                return this.ColumnMappers[propertyName];
            throw new Exception(string.Format("Can not find columnMapper about property named {0}, Please Check this property."));
        }
        /// <summary>
        /// 获取所有的列与属性的映射对象
        /// </summary>
        /// <returns>所有的列与属性的映射对象</returns>
        public IEnumerable<IColumnMapper> GetColumnMappers()
        {
            return this.ColumnMappers.Values;
        }
    }
    /// <summary>
    /// 实体和表的映射基类
    /// </summary>
    public abstract class TableMapper<TEntity> : TableMapper
        where TEntity : class
    {
        /// <summary>
        /// 创建ColumnMapper对象
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns>ColumnMapper对象</returns>
        protected override IColumnMapper CreateColumnMapper(PropertyInfo property)
        {
            return new ColumnMapper(property)
            {
                ColumnFullName = string.Format("{0}.{1}", base.Header.TableAlias, property.Name)
            };
        }
        /// <summary>
        /// 设置列与属性的映射关系
        /// </summary>
        /// <param name="setter">列与属性的关系的设置器</param>
        protected abstract void SetColumnMappers(IColumnMapSetter<TEntity> setter);
        /// <summary>
        /// 设置列与属性的映射
        /// </summary>
        /// <param name="columnMappers">列与属性的映射对象字典</param>
        protected override void LoadColumnMappers(IDictionary<string, IColumnMapper> columnMappers)
        {
            this.SetColumnMappers(new ColumnMapSetter<TEntity>(base.Header.TableAlias, columnMappers));
            base.LoadColumnMappers(columnMappers);
        }

        /// <summary>
        /// 创建TableMapper对象
        /// </summary>
        public TableMapper()
            : base(typeof(TEntity)) { }
    }
}
