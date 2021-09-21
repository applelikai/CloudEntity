using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// 实体数据操作类
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal abstract class DbOperator<TEntity> : IDbOperator<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// Insert sql
        /// </summary>
        private string _insertSql;
        /// <summary>
        /// insert所有非自增列的sql
        /// </summary>
        private string _insertAllSql;
        /// <summary>
        /// delete sql
        /// </summary>
        private string _deleteSql;
        /// <summary>
        /// update sql
        /// </summary>
        private string _updateSql;
        /// <summary>
        /// update所有非主键列的sql
        /// </summary>
        private string _updateAllSql;

        /// <summary>
        /// 操作数据库的Helper
        /// </summary>
        protected DbHelper DbHelper { get; private set; }
        /// <summary>
        /// 创建CommandTree的工厂
        /// </summary>
        protected ICommandTreeFactory CommandTreeFactory { get; private set; }
        /// <summary>
        /// 实体访问器
        /// </summary>
        protected ObjectAccessor EntityAccessor
        {
            get { return ObjectAccessor.GetAccessor(typeof(TEntity)); }
        }
        /// <summary>
        /// Table元数据解析器
        /// </summary>
        protected ITableMapper TableMapper { get; private set; }
        /// <summary>
        /// Insert sql
        /// </summary>
        protected string InsertSql
        {
            get { return this._insertSql; }
        }
        /// <summary>
        /// Update Sql
        /// </summary>
        protected string UpdateSql
        {
            get { return this._updateSql; }
        }

        /// <summary>
        /// 创建查询数据源的工厂
        /// </summary>
        public IDbFactory Factory { get; private set; }

        /// <summary>
        /// 获取Insert命令生成树的子节点
        /// </summary>
        /// <returns>Insert命令生成树的子节点</returns>
        private IEnumerable<KeyValuePair<string, string>> GetInsertChildNodes()
        {
            foreach (IColumnMapper columnMapper in this.TableMapper.GetColumnMappers())
            {
                switch (columnMapper.ColumnAction)
                {
                    case ColumnAction.Insert:
                    case ColumnAction.InsertAndEdit:
                    case ColumnAction.PrimaryAndInsert:
                        yield return new KeyValuePair<string, string>(columnMapper.ColumnName, columnMapper.Property.Name);
                        break;
                }
            }
        }
        /// <summary>
        /// 获取Insert所有非自增列命令生成树的子节点
        /// </summary>
        /// <returns>Insert命令生成树的子节点</returns>
        private IEnumerable<KeyValuePair<string, string>> GetInsertAllChildNodes()
        {
            foreach (IColumnMapper columnMapper in this.TableMapper.GetColumnMappers())
            {
                switch (columnMapper.ColumnAction)
                {
                    //排除主键自增列
                    case ColumnAction.PrimaryAndIdentity:
                        break;
                    //返回Column的Insert节点
                    default:
                        yield return new KeyValuePair<string, string>(columnMapper.ColumnName, columnMapper.Property.Name);
                        break;
                }
            }
        }
        /// <summary>
        /// 获取Delete命令生成树的所有子节点
        /// </summary>
        /// <returns>Delete命令生成树的所有子节点</returns>
        private IEnumerable<INodeBuilder> GetDeleteChildNodes()
        {
            //遍历所有的columnMapper对象
            foreach (IColumnMapper columnMapper in this.TableMapper.GetColumnMappers())
            {
                switch (columnMapper.ColumnAction)
                {
                    //获取所有的主键列的propertyMapper,添加唯一条件节点至Where节点
                    case ColumnAction.PrimaryAndIdentity:
                    case ColumnAction.PrimaryAndInsert:
                        //获取table别名
                        string tableAlias = this.TableMapper.Header.TableAlias;
                        //依次获取等于sql条件表达式节点
                        yield return this.CommandTreeFactory.GetEqualsBuilder(tableAlias, columnMapper.ColumnName, columnMapper.Property.Name);
                        continue;
                }
            }
        }
        /// <summary>
        /// 获取Update命令生成树的所有子节点
        /// </summary>
        /// <returns>Update命令生成树的所有子节点</returns>
        private IEnumerable<INodeBuilder> GetUpdateChildNodes()
        {
            //获取table别名
            string tableAlias = this.TableMapper.Header.TableAlias;
            //遍历所有的columnMapper对象
            foreach (IColumnMapper columnMapper in this.TableMapper.GetColumnMappers())
            {
                //依次获取sql表达式节点
                switch (columnMapper.ColumnAction)
                {
                    //解析可编辑的列元数据，获取对应sql表达式节点添加至Set节点
                    case ColumnAction.InsertAndEdit:
                    case ColumnAction.Edit:
                    case ColumnAction.EditAndDefault:
                        yield return this.CommandTreeFactory.GetUpdateSetChildBuilder(tableAlias, columnMapper.ColumnName, columnMapper.Property.Name);
                        break;
                    //解析主键列，获取对应sql表达式节点添加至Where节点
                    case ColumnAction.PrimaryAndIdentity:
                    case ColumnAction.PrimaryAndInsert:
                        yield return this.CommandTreeFactory.GetEqualsBuilder(tableAlias, columnMapper.ColumnName, columnMapper.Property.Name);
                        break;
                }
            }
        }
        /// <summary>
        /// 获取所有Property对应的Column的Update子sql表达式节点
        /// </summary>
        /// <returns>所有Property对应的Column的Update子sql表达式节点</returns>
        private IEnumerable<INodeBuilder> GetUpdateAllChildNodes()
        {
            //获取table别名
            string tableAlias = this.TableMapper.Header.TableAlias;
            //遍历所有的ColumnMapper对象
            foreach (IColumnMapper columnMapper in this.TableMapper.GetColumnMappers())
            {
                //构建sql条件表达式以及参数
                switch (columnMapper.ColumnAction)
                {
                    //解析主键列，获取对应sql表达式节点添加至Where节点
                    case ColumnAction.PrimaryAndInsert:
                    case ColumnAction.PrimaryAndIdentity:
                        //返回Where表达式的子节点
                        yield return this.CommandTreeFactory.GetEqualsBuilder(tableAlias, columnMapper.ColumnName, columnMapper.Property.Name);
                        break;
                    //解析所有可编辑的列元数据，获取对应sql表达式节点添加至Set节点
                    default:
                        //返回Set表达式节点
                        yield return this.CommandTreeFactory.GetUpdateSetChildBuilder(tableAlias, columnMapper.ColumnName, columnMapper.Property.Name);
                        break;
                }
            }
        }
        /// <summary>
        /// 加载Update命令生成树的Set节点子节点及其附属的sql参数
        /// </summary>
        /// <param name="setParameters">属性名及属性值字典</param>
        /// <param name="nodeBuilders">Update命令生成树的Set节点子节点集合</param>
        /// <param name="sqlParameters">附属的sql参数集合</param>
        private void LoadUpdateChildNodesAndParameters(IDictionary<string, object> setParameters, List<INodeBuilder> nodeBuilders, List<IDbDataParameter> sqlParameters)
        {
            //获取table别名
            string tableAlias = this.TableMapper.Header.TableAlias;
            //遍历setParameters
            foreach (KeyValuePair<string, object> setPair in setParameters)
            {
                //获取columnMapper
                IColumnMapper columnMapper = this.TableMapper.GetColumnMapper(setPair.Key);
                //获取参数名
                string parameterName = string.Concat("New", columnMapper.Property.Name);
                //获取Update Set的子sql表达式节点
                INodeBuilder setBuilder = this.CommandTreeFactory.GetUpdateSetChildBuilder(tableAlias, columnMapper.ColumnName, parameterName);
                //添加sql表达式节点
                nodeBuilders.Add(setBuilder);
                //添加sql参数
                sqlParameters.Add(this.DbHelper.Parameter(parameterName, setPair.Value));
            }
        }
        /// <summary>
        /// 获取Insert Sql
        /// </summary>
        /// <returns>Insert Sql</returns>
        private string GetInsertSql()
        {
            //获取TableHeader对象
            ITableHeader tableHeader = this.TableMapper.Header;
            //获取命令生成树
            ICommandTree commandTree = this.CommandTreeFactory.CreateInsertTree(tableHeader.SchemaName, tableHeader.TableName, this.GetInsertChildNodes());
            //获取Insert命令
            return commandTree.Compile();
        }
        /// <summary>
        /// 获取insert所有非自增列的sql
        /// </summary>
        /// <returns>insert所有非自增列的sql</returns>
        private string GetInsertAllSql()
        {
            //获取TableHeader对象
            ITableHeader tableHeader = this.TableMapper.Header;
            //获取命令生成树
            ICommandTree commandTree = this.CommandTreeFactory.CreateInsertTree(tableHeader.SchemaName, tableHeader.TableName, this.GetInsertAllChildNodes());
            //获取Insert命令
            return commandTree.Compile();
        }
        /// <summary>
        /// 获取Delete Sql
        /// </summary>
        /// <returns>Delete Sql</returns>
        private string GetDeleteSql()
        {
            //获取TableHeader对象
            ITableHeader tableHeader = this.TableMapper.Header;
            //获取命令生成树
            ICommandTree commandTree = this.CommandTreeFactory.CreateDeleteTree(tableHeader.SchemaName, tableHeader.TableName, tableHeader.TableAlias, this.GetDeleteChildNodes());
            //获取Insert命令
            return commandTree.Compile();
        }
        /// <summary>
        /// 获取Update Sql
        /// </summary>
        /// <returns>Update Sql</returns>
        private string GetUpdateSql()
        {
            //获取TableHeader对象
            ITableHeader tableHeader = this.TableMapper.Header;
            //获取命令生成树
            ICommandTree commandTree = this.CommandTreeFactory.CreateUpdateTree(tableHeader.SchemaName, tableHeader.TableName, tableHeader.TableAlias, this.GetUpdateChildNodes());
            //获取Insert命令
            return commandTree.Compile();
        }
        /// <summary>
        /// 获取update所有非主键列的sql
        /// </summary>
        /// <returns>update所有非主键列的sql</returns>
        private string GetUpdateAllSql()
        {
            //获取TableHeader对象
            ITableHeader tableHeader = this.TableMapper.Header;
            //获取命令生成树
            ICommandTree commandTree = this.CommandTreeFactory.CreateUpdateTree(tableHeader.SchemaName, tableHeader.TableName, tableHeader.TableAlias, this.GetUpdateAllChildNodes());
            //获取Insert命令
            return commandTree.Compile();
        }

        /// <summary>
        /// 获取sql参数集合
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="entity">实体</param>
        /// <returns>sql参数集合</returns>
        protected IEnumerable<IDbDataParameter> GetParameters(string commandText, object entity)
        {
            //遍历ColumnMapper，依次返回sql参数
            foreach (IColumnMapper columnMapper in this.TableMapper.GetColumnMappers())
            {
                if (commandText.Contains(string.Concat(this.CommandTreeFactory.ParameterMarker, columnMapper.Property.Name)))
                {
                    //获取参数值
                    object value = this.EntityAccessor.GetValue(columnMapper.Property.Name, entity);
                    //依次获取参数
                    yield return this.DbHelper.Parameter(columnMapper.Property.Name, value);
                }
            }
        }
        /// <summary>
        /// 执行更新
        /// </summary>
        /// <param name="commandText">sql命令</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>受影响的行数</returns>
        protected abstract int ExecuteUpdate(string commandText, IDbDataParameter[] parameters);

        /// <summary>
        /// 创建实体数据操作对象
        /// </summary>
        /// <param name="factory">查询数据源创建工厂</param>
        /// <param name="dbHelper">数据库操作对象</param>
        /// <param name="commandTreeFactory">创建CommandTree的工厂</param>
        /// <param name="mapperContainer">mapper容器</param>
        public DbOperator(IDbFactory factory, DbHelper dbHelper, ICommandTreeFactory commandTreeFactory, IMapperContainer mapperContainer)
        {
            //非空检查
            Check.ArgumentNull(factory, nameof(factory));
            Check.ArgumentNull(dbHelper, nameof(dbHelper));
            Check.ArgumentNull(commandTreeFactory, nameof(commandTreeFactory));
            Check.ArgumentNull(mapperContainer, nameof(mapperContainer));
            //赋值
            this.Factory = factory;
            this.DbHelper = dbHelper;
            this.CommandTreeFactory = commandTreeFactory;
            this.TableMapper = mapperContainer.GetTableMapper(typeof(TEntity));
            //初始化sql
            this._insertSql = this.GetInsertSql();
            this._insertAllSql = this.GetInsertAllSql();
            this._deleteSql = this.GetDeleteSql();
            this._updateSql = this.GetUpdateSql();
            this._updateAllSql = this.GetUpdateAllSql();
        }
        /// <summary>
        /// 向数据库插入实体对象指定插入的列的值
        /// </summary>
        /// <param name="entity">待添加的实体对象</param>
        /// <returns>添加的实体对象的数量</returns>
        public int Add(TEntity entity)
        {
            //非空检查
            Check.ArgumentNull(entity, nameof(entity));
            //获取sql参数数组
            IDbDataParameter[] parameters = this.GetParameters(this._insertSql, entity).ToArray();
            //执行insert
            return this.ExecuteUpdate(this._insertSql, parameters);
        }
        /// <summary>
        /// (异步)向数据库插入实体对象指定插入的列的值
        /// </summary>
        /// <param name="entity">待添加的实体对象</param>
        /// <returns>添加的实体对象的数量</returns>
        public Task<int> AddAsync(TEntity entity)
        {
            return Task.Factory.StartNew(() => this.Add(entity));
        }
        /// <summary>
        /// 向数据库插入实体对象所有非自增标识的列的值
        /// </summary>
        /// <param name="entity">待添加的实体对象</param>
        /// <returns>添加的实体对象的数量</returns>
        public int Insert(TEntity entity)
        {
            //非空检查
            Check.ArgumentNull(entity, nameof(entity));
            //获取sql参数数组
            IDbDataParameter[] parameters = this.GetParameters(this._insertAllSql, entity).ToArray();
            //执行insert
            return this.ExecuteUpdate(this._insertAllSql, parameters);
        }
        /// <summary>
        /// (异步)向数据库插入实体对象所有非自增标识的列的值
        /// </summary>
        /// <param name="entity">待添加的实体对象</param>
        /// <returns>添加的实体对象的数量</returns>
        public Task<int> InsertAsync(TEntity entity)
        {
            return Task.Factory.StartNew(() => this.Insert(entity));
        }
        /// <summary>
        /// 删除数据库中的某个实体对象
        /// </summary>
        /// <param name="entity">待删除的实体对象</param>
        /// <returns>删除的实体对象的数量</returns>
        public int Remove(TEntity entity)
        {
            //非空检查
            Check.ArgumentNull(entity, nameof(entity));
            //获取sql参数数组
            IDbDataParameter[] parameters = this.GetParameters(this._deleteSql, entity).ToArray();
            //执行并返回DB受影响行数
            return this.ExecuteUpdate(this._deleteSql, parameters);
        }
        /// <summary>
        /// (异步)删除数据库中的某个实体对象
        /// </summary>
        /// <param name="entity">待删除的实体对象</param>
        /// <returns>删除的实体对象的数量</returns>
        public Task<int> RemoveAsync(TEntity entity)
        {
            return Task.Factory.StartNew(() => this.Remove(entity));
        }
        /// <summary>
        /// 批量删除数据库中的实体对象
        /// </summary>
        /// <param name="entities">待删除的实体对象数据源</param>
        /// <returns>删除的实体对象的数量</returns>
        public int RemoveAll(IDbQuery<TEntity> entities)
        {
            //非空检查
            Check.ArgumentNull(entities, nameof(entities));
            //创建Delete命令生成树
            ICommandTree deleteTree = this.CommandTreeFactory.CreateDeleteTree(
                this.TableMapper.Header.SchemaName,
                this.TableMapper.Header.TableName,
                this.TableMapper.Header.TableAlias,
                entities.NodeBuilders.Where(b => b.ParentNodeType == SqlType.Where));
            //执行并返回DB受影响行数
            return this.ExecuteUpdate(deleteTree.Compile(), entities.Parameters.ToArray());
        }
        /// <summary>
        /// 将实体对象的所有的属性的值更新至数据库
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>修改的实体对象的数量</returns>
        public int Modify(TEntity entity)
        {
            //非空检查
            Check.ArgumentNull(entity, nameof(entity));
            //初始化sql参数集合
            IDbDataParameter[] parameters = this.GetParameters(this._updateAllSql, entity).ToArray();
            //执行并返回DB受影响行数
            return this.ExecuteUpdate(this._updateAllSql, parameters);
        }
        /// <summary>
        /// (异步)将实体对象的所有的属性的值更新至数据库
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>修改的实体对象的数量</returns>
        public Task<int> ModifyAsync(TEntity entity)
        {
            return Task.Factory.StartNew(() => this.Modify(entity));
        }
        /// <summary>
        /// 保存实体对象中所有指定修改的属性的值至数据库
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>修改的实体对象的数量</returns>
        public int Save(TEntity entity)
        {
            //非空检查
            Check.ArgumentNull(entity, nameof(entity));
            //获取sql参数数组
            IDbDataParameter[] parameters = this.GetParameters(this._updateSql, entity).ToArray();
            //执行并返回DB受影响行数
            return this.ExecuteUpdate(this._updateSql, parameters);
        }
        /// <summary>
        /// (异步)保存实体对象中所有指定修改的属性的值至数据库
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>修改的实体对象的数量</returns>
        public Task<int> SaveAsync(TEntity entity)
        {
            return Task.Factory.StartNew(() => this.Save(entity));
        }
        /// <summary>
        /// 批量修改符合条件的实体的信息
        /// </summary>
        /// <param name="setParameters">属性名及属性值字典</param>
        /// <param name="entities">待修改的实体的数据源</param>
        /// <returns>修改的实体对象的数量</returns>
        public int SaveAll(IDictionary<string, object> setParameters, IDbQuery<TEntity> entities)
        {
            //非空检查
            Check.ArgumentNull(setParameters, nameof(setParameters));
            Check.ArgumentNull(entities, nameof(entities));
            //初始化sql条件表达式集合及其sql参数集合
            List<INodeBuilder> nodeBuilders = new List<INodeBuilder>();
            List<IDbDataParameter> sqlParameters = new List<IDbDataParameter>();
            //Set 添加Update Set表达式子节点及其附属参数
            this.LoadUpdateChildNodesAndParameters(setParameters, nodeBuilders, sqlParameters);
            //Where 获取entitis中的Where表达式子节点及其附属的sql参数填充至nodeBuilders 和 sqlParameters
            nodeBuilders.AddRange(entities.NodeBuilders.Where(n => n.ParentNodeType == SqlType.Where));
            sqlParameters.AddRange(entities.Parameters);
            //创建Update命令生成树
            ICommandTree updateTree = this.CommandTreeFactory.CreateUpdateTree(
                this.TableMapper.Header.SchemaName,
                this.TableMapper.Header.TableName,
                this.TableMapper.Header.TableAlias,
                nodeBuilders);
            //执行并返回DB受影响行数
            return this.ExecuteUpdate(updateTree.Compile(), sqlParameters.ToArray());
        }
    }
}
