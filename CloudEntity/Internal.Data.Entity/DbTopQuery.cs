using CloudEntity.CommandTrees;
using CloudEntity.Data;
using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// TOP实体查询数据源类(查询前几条实体的数据)
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    internal class DbTopQuery<TEntity> : DbEntityBase<TEntity>, IDbTopQuery<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 查询的前几条的元素数量
        /// </summary>
        public int TopCount { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <param name="commandFactory">SQL命令工厂</param>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="topCount">查询的前几条的元素数量</param>
        public DbTopQuery(IMapperContainer mapperContainer, ICommandFactory commandFactory, IDbHelper dbHelper, int topCount)
         : base(mapperContainer, commandFactory, dbHelper) 
        {
            this.TopCount = topCount;
        }
        /// <summary>
        /// 获取sql字符串
        /// </summary>
        /// <returns>sql字符串</returns>
        public string ToSqlString()
        {
            // 获取TOP查询命令生成树
            ICommandTree commandTree = base.CommandFactory.GetTopQueryTree(base.NodeBuilders, this.TopCount);
            // 获取sql命令
            return commandTree.Compile();
        }
        /// <summary>
        /// 将此数据源的查询结果映射为TModel对象数据源
        /// </summary>
        /// <typeparam name="TModel">TModel对象（只要是有无参构造函数的类就可以）</typeparam>
        /// <returns>TModel对象数据源</returns>
        public IEnumerable<TModel> Cast<TModel>()
            where TModel : class, new()
        {
            // 获取查询列名数组
            string[] columnNames = this.GetSelectNames().ToArray();
            // 获取从DataReader到映射类型的转换器
            ReaderModelConverter<TModel> converter = new ReaderModelConverter<TModel>(columnNames, typeof(TEntity), base.MapperContainer, base.PropertyLinkers);

            // 获取查询命令生成树
            ICommandTree commandTree = base.CommandFactory.GetTopQueryTree(base.NodeBuilders, this.TopCount);
            // 获取sql命令
            string commandText = commandTree.Compile();

            // 执行查询获取映射对象迭代器
            return base.DbHelper.GetResults(converter.Convert, commandText, parameters: base.Parameters.ToArray());
        }
        /// <summary>
        /// 将此数据源的查询结果映射为TModel对象数据源
        /// </summary>
        /// <param name="entityModelMaps">实体类型与映射类型部分属性映射字典</param>
        /// <typeparam name="TModel">TModel对象（只要是有无参构造函数的类就可以）</typeparam>
        /// <returns>TModel对象数据源</returns>
        public IEnumerable<TModel> Cast<TModel>(IDictionary<string, string> entityModelMaps)
            where TModel : class, new()
        {
            // 获取查询列名数组
            string[] columnNames = this.GetSelectNames().ToArray();
            // 获取从DataReader到映射类型的转换器
            ReaderModelConverter<TModel> converter = new ReaderModelConverter<TModel>(columnNames, typeof(TEntity), base.MapperContainer, base.PropertyLinkers, entityModelMaps);

            // 获取查询命令生成树
            ICommandTree commandTree = base.CommandFactory.GetTopQueryTree(base.NodeBuilders, this.TopCount);
            // 获取sql命令
            string commandText = commandTree.Compile();

            // 执行查询获取映射对象迭代器
            return base.DbHelper.GetResults(converter.Convert, commandText, parameters: base.Parameters.ToArray());
        }
        /// <summary>
        /// 获取枚举器
        /// </summary>
        /// <returns>枚举器</returns>
        public IEnumerator<TEntity> GetEnumerator()
        {
            // 获取查询列名数组
            string[] columnNames = this.GetSelectNames().ToArray();
            // 获取从DataReader到实体的转换器
            ReaderEntityConverter converter = new ReaderEntityConverter(columnNames, typeof(TEntity), base.MapperContainer, base.PropertyLinkers);

            // 获取SELECT命令生成树
            ICommandTree commandTree = base.CommandFactory.GetTopQueryTree(base.NodeBuilders, this.TopCount);
            // 获取sql命令
            string commandText = commandTree.Compile();

            // 执行查询获取实体对象列表并遍历
            foreach (TEntity entity in base.DbHelper.GetResults(converter.Convert, commandText, parameters: base.Parameters.ToArray()))
            {
                // 依次获取实体对象
                yield return entity;
            }
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