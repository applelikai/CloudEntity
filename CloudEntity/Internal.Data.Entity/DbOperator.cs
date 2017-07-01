using CloudEntity.CommandTrees;
using CloudEntity.CommandTrees.Commom;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Linq;

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
            get { return ObjectAccessor.GetAccessor(typeof(TEntity).GetTypeInfo()); }
        }
        /// <summary>
        /// Table元数据解析器
        /// </summary>
        protected ITableMapper TableMapper { get; private set; }
        /// <summary>
        /// 创建查询数据源的工厂
        /// </summary>
        public IDbFactory Factory { get; private set; }

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
                        string columnName = columnMapper.ColumnFullName;    //获取列名
                        string parameterName = columnMapper.Property.Name;  //获取(属性名所为)参数名
                        yield return new NodeBuilder(SqlType.Where, "{0} = ${1}", columnName, parameterName);
                        continue;
                }
            }
        }
        /// <summary>
        /// 获取所有非空Property对应的Column的Update子sql表达式节点(并加载sql参数)
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="parameters">sql参数集合</param>
        /// <returns>所有非空Property对应的Column的Update子sql表达式节点</returns>
        private IEnumerable<INodeBuilder> GetNotNullUpdateChildNodes(TEntity entity, IList<IDbDataParameter> parameters)
        {
            //遍历所有的ColumnMapper对象
            foreach (IColumnMapper columnMapper in this.TableMapper.GetColumnMappers())
            {
                switch (columnMapper.ColumnAction)
                {
                    //解析主键列，获取对应sql表达式节点添加至Where节点
                    case ColumnAction.PrimaryAndInsert:
                    case ColumnAction.PrimaryAndIdentity:
                        yield return new NodeBuilder(SqlType.Where, "{0} = ${1}", columnMapper.ColumnFullName, columnMapper.Property.Name);
                        break;
                    //解析所有可编辑的列元数据，获取对应sql表达式节点添加至Set节点
                    default:
                        //获取当前Column对应的Property的值
                        object value = this.EntityAccessor.GetValue(columnMapper.Property.Name, entity);
                        if (value == null)
                            continue;
                        //返回Set表达式节点
                        yield return new NodeBuilder(SqlType.UpdateSet, "{0} = ${1}", columnMapper.ColumnFullName, columnMapper.Property.Name);
                        //添加sql参数
                        parameters.Add(this.DbHelper.Parameter(columnMapper.Property.Name, value));
                        break;
                }
            }
        }
        /// <summary>
        /// 获取所有非空Property对应的Column的Insert节点(并加载Sql参数)
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <param name="parameters">sql参数集合</param>
        /// <returns>所有非空Property对应的Column的Insert节点</returns>
        private IEnumerable<KeyValuePair<string, string>> GetNotNullInsertChildNodes(TEntity entity, IList<IDbDataParameter> parameters)
        {
            //便利所有的ColumnMapper
            foreach (IColumnMapper columnMapper in this.TableMapper.GetColumnMappers())
            {
                //排除主键自增列
                if (columnMapper.ColumnAction == ColumnAction.PrimaryAndIdentity)
                    continue;
                //获取Column对应的Property的值
                object value = this.EntityAccessor.GetValue(columnMapper.Property.Name, entity);
                //若值为空，跳过
                if (value == null)
                    continue;
                //返回Column的Insert节点
                yield return new KeyValuePair<string, string>(columnMapper.ColumnName, columnMapper.Property.Name);
                //加载sql参数
                parameters.Add(this.DbHelper.Parameter(columnMapper.Property.Name, value));
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
            foreach (KeyValuePair<string, object> setPair in setParameters)
            {
                IColumnMapper columnMapper = this.TableMapper.GetColumnMapper(setPair.Key);
                string parameterName = string.Concat("New", columnMapper.Property.Name);
                INodeBuilder setBuilder = new NodeBuilder(SqlType.UpdateSet, "{0} = ${1}", columnMapper.ColumnFullName, parameterName);
                nodeBuilders.Add(setBuilder);
                sqlParameters.Add(this.DbHelper.Parameter(parameterName, setPair.Value));
            }
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
                    object value = this.EntityAccessor.GetValue(columnMapper.Property.Name, entity);
                    yield return this.DbHelper.Parameter(columnMapper.Property.Name, value);
                }
            }
        }
        /// <summary>
        /// 获取Insert命令生成树的子节点
        /// </summary>
        /// <returns>Insert命令生成树的子节点</returns>
        protected IEnumerable<KeyValuePair<string, string>> GetInsertChildNodes()
        {
            foreach (IColumnMapper columnMapper in this.TableMapper.GetColumnMappers())
            {
                switch (columnMapper.ColumnAction)
                {
                    case ColumnAction.Insert:
                    case ColumnAction.InsertAndEdit:
                    case ColumnAction.PrimaryAndInsert:
                        yield return new KeyValuePair<string, string>(columnMapper.ColumnName, columnMapper.Property.Name);
                        continue;
                }
            }
        }
        /// <summary>
        /// 获取Update命令生成树的所有子节点
        /// </summary>
        /// <returns>Update命令生成树的所有子节点</returns>
        protected IEnumerable<INodeBuilder> GetUpdateChildNodes()
        {
            //遍历所有的columnMapper对象
            foreach (IColumnMapper columnMapper in this.TableMapper.GetColumnMappers())
            {
                string columnName = columnMapper.ColumnFullName;  //获取列名
                string parameterName = columnMapper.Property.Name; //获取(属性名所为)参数名

                switch (columnMapper.ColumnAction)
                {
                    //解析可编辑的列元数据，获取对应sql表达式节点添加至Set节点
                    case ColumnAction.InsertAndEdit:
                    case ColumnAction.Edit:
                    case ColumnAction.EditAndDefault:
                        yield return new NodeBuilder(SqlType.UpdateSet, "{0} = ${1}", columnName, parameterName);
                        continue;
                    //解析主键列，获取对应sql表达式节点添加至Where节点
                    case ColumnAction.PrimaryAndIdentity:
                    case ColumnAction.PrimaryAndInsert:
                        yield return new NodeBuilder(SqlType.Where, "{0} = ${1}", columnName, parameterName);
                        continue;
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
            //生成Insert sql命令
            ICommandTree insertTree = this.CommandTreeFactory.CreateInsertTree(
                this.TableMapper.Header.TableFullName, 
                this.GetInsertChildNodes());
            string commandText = insertTree.Compile();
            //获取sql参数数组
            IDbDataParameter[] parameters = this.GetParameters(commandText, entity).ToArray();
            //执行insert
            return this.ExecuteUpdate(commandText, parameters);
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
            //初始化sql参数集合
            IList<IDbDataParameter> parameters = new List<IDbDataParameter>();
            //获取Insert命令生成树
            ICommandTree insertTree = this.CommandTreeFactory.CreateInsertTree(
                this.TableMapper.Header.TableFullName,
                this.GetNotNullInsertChildNodes(entity, parameters));
            //执行insert
            return this.ExecuteUpdate(insertTree.Compile(), parameters.ToArray());
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
            //生成Delete命令
            ICommandTree deleteTree = this.CommandTreeFactory.CreateDeleteTree(
                this.TableMapper.Header.TableFullName,
                this.TableMapper.Header.TableAlias,
                this.GetDeleteChildNodes());
            string commandText = deleteTree.Compile();
            //获取sql参数数组
            IDbDataParameter[] parameters = this.GetParameters(commandText, entity).ToArray();
            //执行并返回DB受影响行数
            return this.ExecuteUpdate(commandText, parameters);
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
                this.TableMapper.Header.TableFullName,
                this.TableMapper.Header.TableAlias,
                entities.NodeBuilders.Where(b => b.ParentNodeType == SqlType.Where));
            //执行并返回DB受影响行数
            return this.ExecuteUpdate(deleteTree.Compile(), entities.Parameters.ToArray());
        }
        /// <summary>
        /// 将实体对象的所有的非标识属性的值至数据库
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>修改的实体对象的数量</returns>
        public int Modify(TEntity entity)
        {
            //非空检查
            Check.ArgumentNull(entity, nameof(entity));
            //初始化sql参数集合
            IList<IDbDataParameter> parameters = new List<IDbDataParameter>();
            //获取UPDATE命令生成树
            ICommandTree updateTree = this.CommandTreeFactory.CreateUpdateTree(
                this.TableMapper.Header.TableFullName, 
                this.TableMapper.Header.TableAlias,
                this.GetNotNullUpdateChildNodes(entity, parameters));
            //执行并返回DB受影响行数
            return this.ExecuteUpdate(updateTree.Compile(), parameters.ToArray());
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
            //生成Update命令
            ICommandTree updateTree = this.CommandTreeFactory.CreateUpdateTree(
                this.TableMapper.Header.TableFullName,
                this.TableMapper.Header.TableAlias,
                this.GetUpdateChildNodes());
            string commandText = updateTree.Compile();
            //获取sql参数数组
            IDbDataParameter[] parameters = this.GetParameters(commandText, entity).ToArray();
            //执行并返回DB受影响行数
            return this.ExecuteUpdate(commandText, parameters);
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
                this.TableMapper.Header.TableFullName,
                this.TableMapper.Header.TableAlias,
                nodeBuilders);
            //执行并返回DB受影响行数
            return this.ExecuteUpdate(updateTree.Compile(), sqlParameters.ToArray());
        }
    }
}
