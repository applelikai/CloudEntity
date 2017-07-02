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
    /// 李凯 Apple_Li
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbList<TEntity> : DbOperator<TEntity>, IDbList<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 操作数据库某Table的线程锁(避免同时刻执行增删改)
        /// </summary>
        private object modifyLocker;

        /// <summary>
        /// 获取当前实体对象
        /// </summary>
        /// <param name="id">实体的ID</param>
        /// <returns>实体对象</returns>
        public TEntity this[object id]
        {
            get
            {
                //非空检查
                Check.ArgumentNull(id, nameof(id));
                //获取查询命令生成树
                ICommandTree queryTree = base.CommandTreeFactory.CreateQueryTree(this.GetQueryByIdChildBuilders());
                //获取Sql参数数组
                IDbDataParameter[] parameters = { base.DbHelper.Parameter(base.TableMapper.KeyMapper.Property.Name, id) };
                //执行查询
                return base.DbHelper.GetResults(this.CreateEntity, queryTree.Compile(), parameters: parameters).SingleOrDefault();
            }
        }
        /// <summary>
        /// CommandTree子节点集合
        /// </summary>
        public IEnumerable<INodeBuilder> NodeBuilders
        {
            get
            {
                //获取Select节点的子节点集合
                foreach (IColumnMapper columnMapper in base.TableMapper.GetColumnMappers())
                {
                    //若列的别名为空，则不使用别名
                    if (string.IsNullOrEmpty(columnMapper.ColumnAlias))
                        yield return new ColumnBuilder(columnMapper.ColumnName, columnMapper.ColumnFullName);
                    //若别名不为空, 则使用别名
                    else
                        yield return new ColumnBuilder(columnMapper.ColumnAlias, string.Format("{0} {1}", columnMapper.ColumnFullName, columnMapper.ColumnAlias));
                }
                //获取From节点的子节点集合
                string tableFullName = string.Format("{0} {1}", base.TableMapper.Header.TableFullName, base.TableMapper.Header.TableAlias);
                yield return new TableBuilder(base.TableMapper.Header.TableName, tableFullName);
            }
        }
        /// <summary>
        /// sql参数集合
        /// </summary>
        public IEnumerable<IDbDataParameter> Parameters
        {
            get { return new IDbDataParameter[0]; }
        }
        /// <summary>
        /// 当前对象的关联对象属性链接数组
        /// </summary>
        public PropertyLinker[] PropertyLinkers { get; private set; }
        /// <summary>
        /// 创建sql参数的工厂
        /// </summary>
        public IParameterFactory ParameterFactory => base.DbHelper;

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
            yield return new NodeBuilder(SqlType.Where, "{0} = ${1}", keyColumnMapper.ColumnFullName, keyColumnMapper.Property.Name);
            
        }
        /// <summary>
        /// 获取根据ID统计单条记录是否存在命令生成树的子节点
        /// </summary>
        /// <returns>根据ID统计单条记录是否存在命令生成树的子节点</returns>
        private IEnumerable<INodeBuilder> GetQueryCountByIdChildBuilders()
        {
            //获取Select节点的子节点集合
            yield return new NodeBuilder(SqlType.Select, "COUNT(*)");
            //获取From节点的子节点集合
            string tableFullName = string.Format("{0} {1}", base.TableMapper.Header.TableFullName, base.TableMapper.Header.TableAlias);
            yield return new TableBuilder(base.TableMapper.Header.TableName, tableFullName);
            //获取Where节点的子节点集合
            IColumnMapper keyColumnMapper = base.TableMapper.KeyMapper;
            yield return new NodeBuilder(SqlType.Where, "{0} = ${1}", keyColumnMapper.ColumnFullName, keyColumnMapper.Property.Name);
        }
        /// <summary>
        /// 创建实体对象
        /// </summary>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询的列</param>
        /// <returns>实体对象</returns>
        private TEntity CreateEntity(IDataReader reader, string[] columnNames)
        {
            return EntityAccessor.CreateEntity(base.TableMapper, reader, columnNames) as TEntity;
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>受影响的行数</returns>
        protected override int ExecuteUpdate(string commandText, IDbDataParameter[] parameters)
        {
            lock (this.modifyLocker)
            {
                return base.DbHelper.ExecuteUpdate(commandText, parameters: parameters);
            }
        }

        /// <summary>
        /// 创建DbList对象
        /// </summary>
        /// <param name="factory">创建数据源的工厂</param>
        /// <param name="dbHelper">DbHelper</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="mapperContainer">mapper容器</param>
        public DbList(IDbFactory factory, DbHelper dbHelper, ICommandTreeFactory commandTreeFactory, IMapperContainer mapperContainer)
            : base(factory, dbHelper, commandTreeFactory, mapperContainer)
        {
            this.modifyLocker = new object();
            this.PropertyLinkers = new PropertyLinker[0];
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
            ICommandTree queryTree = this.CommandTreeFactory.CreateQueryTree(this.GetQueryCountByIdChildBuilders());
            //获取Sql参数数组
            IDbDataParameter[] parameters = { base.DbHelper.Parameter(base.TableMapper.KeyMapper.Property.Name, id) };
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
            //生成Insert sql命令
            ICommandTree insertTree = base.CommandTreeFactory.CreateInsertTree(base.TableMapper.Header.TableFullName, this.GetInsertChildNodes());
            string commandText = insertTree.Compile();
            //遍历对象，每次添加一个对象计算一次DB受影响行数
            foreach (TEntity entity in entities)
            {
                //获取sql参数数组
                IDbDataParameter[] parameters = this.GetParameters(commandText, entity).ToArray();
                //执行Insert
                result += base.DbHelper.ExecuteUpdate(commandText, parameters: parameters);
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
            //生成Update命令
            ICommandTree updateTree = base.CommandTreeFactory.CreateUpdateTree(
                base.TableMapper.Header.TableFullName,
                base.TableMapper.Header.TableAlias,
                this.GetUpdateChildNodes());
            string commandText = updateTree.Compile();
            //遍历对象，每次添加一个对象计算一次DB受影响行数
            foreach (TEntity entity in entities)
            {
                //获取sql参数数组
                IDbDataParameter[] parameters = this.GetParameters(commandText, entity).ToArray();
                //执行并返回DB受影响行数
                result += base.DbHelper.ExecuteUpdate(commandText, parameters: parameters);
            }
            //返回DB受影响函数
            return result;
        }
        /// <summary>
        /// 获取TEntity类型的枚举器
        /// </summary>
        /// <returns>TEntity类型的枚举器</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            //获取查询命令生成树
            ICommandTree queryTree = base.CommandTreeFactory.CreateQueryTree(this.NodeBuilders);
            //执行查询获取结果
            foreach (TEntity entity in base.DbHelper.GetResults(this.CreateEntity, queryTree.Compile()))
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