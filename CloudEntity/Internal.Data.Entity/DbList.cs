using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 数据列表
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbList<TEntity> : DbOperator<TEntity>, IDbList<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 操作数据库某Table的线程锁(避免同时刻执行增删改)
        /// </summary>
        private readonly object _modifyLocker;
        /// <summary>
        /// sql表达式节点集合
        /// </summary>
        private readonly IList<INodeBuilder> _nodebuilders;
        /// <summary>
        /// sql参数集合
        /// </summary>
        private readonly IList<IDbDataParameter> _sqlParameters;

        /// <summary>
        /// 获取当前实体对象
        /// </summary>
        /// <param name="id">实体的ID</param>
        /// <returns>实体对象</returns>
        public TEntity this[object id]
        {
            get
            {
                // 非空检查
                Check.ArgumentNull(id, nameof(id));

                // 获取查询列名数组
                string[] columnNames = this.GetSelectNames().ToArray();
                // 获取从DataReader到实体的转换器
                ReaderEntityConverter converter = new ReaderEntityConverter(columnNames, typeof(TEntity), base.TableMapper);

                // 获取查询命令生成树
                ICommandTree queryTree = base.CommandFactory.GetQueryTree(this.GetQueryByIdChildBuilders());
                // 获取Sql参数数组
                IDbDataParameter[] parameters = { base.DbHelper.CreateParameter(base.TableMapper.KeyMapper.Property.Name, id) };
                // 执行查询
                return base.DbHelper.GetResults(converter.Convert, queryTree.Compile(), parameters: parameters).SingleOrDefault() as TEntity;
            }
        }
        /// <summary>
        /// CommandTree子节点集合
        /// </summary>
        public IEnumerable<INodeBuilder> NodeBuilders
        {
            get { return _nodebuilders; }
        }
        /// <summary>
        /// sql参数集合
        /// </summary>
        public IEnumerable<IDbDataParameter> Parameters
        {
            get { return _sqlParameters; }
        }

        /// <summary>
        /// 获取基本sql查询命令生成树的子节点列表
        /// </summary>
        /// <returns>基本sql查询命令生成树的子节点列表</returns>
        private IEnumerable<INodeBuilder> GetQueryNodeBuilders()
        {
            // 获取表别名
            string tableAlias = base.TableMapper.Header.TableAlias;
            // 获取Select节点的子节点集合
            foreach (IColumnMapper columnMapper in base.TableMapper.GetColumnMappers())
            {
                // 依次获取列节点
                yield return base.CommandFactory.GetColumnBuilder(tableAlias, columnMapper.ColumnName, columnMapper.ColumnAlias);
            }
            // 获取表名
            string tableName = base.TableMapper.Header.TableName;
            // 获取数据库架构名
            string schemaName = base.TableMapper.Header.SchemaName ?? base.DbHelper.DefaultSchemaName;
            // 获取From节点的子Table表达式节点
            yield return base.CommandFactory.GetTableBuilder(tableName, tableAlias, schemaName);
        }
        /// <summary>
        /// 获取根据ID查询单条记录命令生成树的子节点
        /// </summary>
        /// <returns>根据ID查询单条记录命令生成树的子节点</returns>
        private IEnumerable<INodeBuilder> GetQueryByIdChildBuilders()
        {
            //获取Select节点的子节点集合 和 From节点的子节点集合
            foreach (INodeBuilder nodeBuilder in this.NodeBuilders)
                yield return nodeBuilder;
            //获取Where节点的子节点集合
            IColumnMapper keyColumnMapper = base.TableMapper.KeyMapper;
            yield return base.CommandFactory.GetEqualsBuilder(this.TableMapper.Header.TableAlias, keyColumnMapper.ColumnName, keyColumnMapper.Property.Name);
        }
        /// <summary>
        /// 获取根据ID统计单条记录是否存在命令生成树的子节点
        /// </summary>
        /// <returns>根据ID统计单条记录是否存在命令生成树的子节点</returns>
        private IEnumerable<INodeBuilder> GetQueryCountByIdChildBuilders()
        {
            //获取Select节点的子节点集合
            yield return new NodeBuilder(SqlType.Select, "COUNT(*)");
            //获取表名
            string tableName = base.TableMapper.Header.TableName;
            //获取表别名
            string tableAlias = base.TableMapper.Header.TableAlias;
            //获取数据库架构名
            string schemaName = base.TableMapper.Header.SchemaName ?? base.DbHelper.DefaultSchemaName;
            //获取From节点的子Table表达式节点
            ITableHeader tableHeader = base.TableMapper.Header;
            yield return base.CommandFactory.GetTableBuilder(tableName, tableAlias, schemaName);
            //获取Where节点的子节点集合
            IColumnMapper keyColumnMapper = base.TableMapper.KeyMapper;
            yield return base.CommandFactory.GetEqualsBuilder(tableAlias, keyColumnMapper.ColumnName, keyColumnMapper.Property.Name);
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>受影响的行数</returns>
        protected override int ExecuteUpdate(string commandText, IDbDataParameter[] parameters)
        {
            lock (_modifyLocker)
            {
                return base.DbHelper.ExecuteUpdate(commandText, parameters: parameters);
            }
        }

        /// <summary>
        /// 创建DbList对象
        /// </summary>
        /// <param name="factory">创建数据源的工厂</param>
        /// <param name="dbHelper">DbHelper</param>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <param name="mapperContainer">mapper容器</param>
        public DbList(IDbFactory factory, IDbHelper dbHelper, ICommandFactory commandFactory, IMapperContainer mapperContainer)
            : base(factory, dbHelper, commandFactory, mapperContainer)
        {
            _modifyLocker = new object();
            _nodebuilders = this.GetQueryNodeBuilders().ToList();
            _sqlParameters = new List<IDbDataParameter>();
        }
        /// <summary>
        /// 判断当前ID的实体是否存在
        /// </summary>
        /// <param name="id">实体的ID</param>
        /// <returns>1:存在 0:不存在</returns>
        public int Exist(object id)
        {
            //非空检查
            Check.ArgumentNull(id, nameof(id));
            //获取CommandTree
            ICommandTree queryTree = this.CommandFactory.GetQueryTree(this.GetQueryCountByIdChildBuilders());
            //获取Sql参数数组
            IDbDataParameter[] parameters = { base.DbHelper.CreateParameter(base.TableMapper.KeyMapper.Property.Name, id) };
            //执行查询获取第一行第一列的数据
            return TypeHelper.ConvertTo<int>(base.DbHelper.GetScalar(queryTree.Compile(), parameters: parameters));
        }
        /// <summary>
        /// Add many entities to databse
        /// </summary>
        /// <param name="entities">entities</param>
        /// <returns>How many rows changed</returns>
        public int AddRange(IEnumerable<TEntity> entities)
        {
            //非空检查
            Check.ArgumentNull(entities, nameof(entities));
            //初始化Db受影响函数
            int result = 0;
            //遍历对象，每次添加一个对象计算一次DB受影响行数
            foreach (TEntity entity in entities)
            {
                //获取sql参数数组
                IDbDataParameter[] parameters = this.GetParameters(base.InsertCommandText, entity).ToArray();
                //执行Insert
                result += base.DbHelper.ExecuteUpdate(base.InsertCommandText, parameters: parameters);
            }
            //返回DB受影响函数
            return result;
        }
        /// <summary>
        /// 批量保存实体信息
        /// </summary>
        /// <param name="entities">实体对象集合</param>
        /// <returns>保存的实体数量</returns>
        public int SaveAll(IEnumerable<TEntity> entities)
        {
            //非空检查
            Check.ArgumentNull(entities, nameof(entities));
            //初始化Db受影响函数
            int result = 0;
            //遍历对象，每次添加一个对象计算一次DB受影响行数
            foreach (TEntity entity in entities)
            {
                //获取sql参数数组
                IDbDataParameter[] parameters = this.GetParameters(base.UpdateCommandText, entity).ToArray();
                //执行并返回DB受影响行数
                result += base.DbHelper.ExecuteUpdate(base.UpdateCommandText, parameters: parameters);
            }
            //返回DB受影响函数
            return result;
        }
        /// <summary>
        /// 获取查询sql字符串
        /// </summary>
        /// <returns>查询sql字符串</returns>
        public string ToSqlString()
        {
            // 获取查询命令生成树
            ICommandTree queryTree = base.CommandFactory.GetQueryTree(this.NodeBuilders);
            // 获取生成的查询sql字符串
            return queryTree.Compile();
        }
        /// <summary>
        /// 获取TEntity类型的枚举器
        /// </summary>
        /// <returns>TEntity类型的枚举器</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            // 获取查询列名数组
            string[] columnNames = this.GetSelectNames().ToArray();
            // 获取从DataReader到实体的转换器
            ReaderEntityConverter converter = new ReaderEntityConverter(columnNames, typeof(TEntity), base.TableMapper);

            // 获取查询sql命令
            string commandText = this.ToSqlString();
            // 执行查询获取结果
            foreach (TEntity entity in base.DbHelper.GetResults(converter.Convert, commandText))
                yield return entity;
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}