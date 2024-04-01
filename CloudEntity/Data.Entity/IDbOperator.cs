using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 实体操作接口
    /// [作者：Apple_Li 李凯 15150598493]
    /// </summary>
    /// <typeparam name="TEntity">实体类型</typeparam>
    public interface IDbOperator<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 插入元素的Sql命令
        /// </summary>
        string InsertCommandText { get; }
        /// <summary>
        /// 更新元素的Sql命令
        /// </summary>
        string UpdateCommandText { get; }
        /// <summary>
        /// 删除元素的Sql命令
        /// </summary>
        string DeleteCommandText { get; }
        /// <summary>
        /// 创建查询数据源的工厂
        /// </summary>
        IDbFactory Factory { get; }

        /// <summary>
        /// 向数据库插入实体对象指定插入的列的值
        /// </summary>
        /// <param name="entity">待添加的实体对象</param>
        /// <returns>INSERT的行数</returns>
        int Add(TEntity entity);
        /// <summary>
        /// 异步向数据库插入实体对象指定插入的列的值
        /// </summary>
        /// <param name="entity">待添加的实体对象</param>
        /// <returns>INSERT的行数</returns>
        Task<int> AddAsync(TEntity entity);
        /// <summary>
        /// 向数据库插入实体对象所有非自增标识的列的值
        /// </summary>
        /// <param name="entity">待添加的实体对象</param>
        /// <returns>INSERT的行数</returns>
        int Insert(TEntity entity);
        /// <summary>
        /// 异步向数据库插入实体对象所有非自增标识的列的值
        /// </summary>
        /// <param name="entity">待添加的实体对象</param>
        /// <returns>INSERT的行数</returns>
        Task<int> InsertAsync(TEntity entity);

        /// <summary>
        /// 向数据库插入映射对象指定插入的列的值
        /// </summary>
        /// <typeparam name="TModel">映射类型（映射类型的属性名必须与实体类型的属性名一致）</typeparam>
        /// <param name="model">映射对象</param>
        /// <returns>INSERT的行数</returns>
        int AddModel<TModel>(TModel model)
            where TModel : class;
        /// <summary>
        /// 异步向数据库插入映射对象指定插入的列的值
        /// </summary>
        /// <typeparam name="TModel">映射类型（映射类型的属性名必须与实体类型的属性名一致）</typeparam>
        /// <param name="model">映射对象</param>
        /// <returns>INSERT的行数</returns>
        Task<int> AddModelAsync<TModel>(TModel model)
            where TModel : class;
        /// <summary>
        /// 向数据库插入映射对象所有非自增标识的列的值
        /// </summary>
        /// <typeparam name="TModel">映射类型（映射类型的属性名必须与实体类型的属性名一致）</typeparam>
        /// <param name="model">映射对象</param>
        /// <returns>INSERT的行数</returns>
        int InsertModel<TModel>(TModel model)
            where TModel : class;
        /// <summary>
        /// 异步向数据库插入映射对象所有非自增标识的列的值
        /// </summary>
        /// <typeparam name="TModel">映射类型（映射类型的属性名必须与实体类型的属性名一致）</typeparam>
        /// <param name="model">映射对象</param>
        /// <returns>INSERT的行数</returns>
        Task<int> InsertModelAsync<TModel>(TModel model)
            where TModel : class;

        /// <summary>
        /// 删除数据库中的某个实体对象
        /// </summary>
        /// <param name="entity">待删除的实体对象</param>
        /// <returns>删除的实体对象的数量</returns>
        int Remove(TEntity entity);
        /// <summary>
        /// 异步删除数据库中的某个实体对象
        /// </summary>
        /// <param name="entity">待删除的实体对象</param>
        /// <returns>删除的实体对象的数量</returns>
        Task<int> RemoveAsync(TEntity entity);
        /// <summary>
        /// 批量删除数据库中的实体对象
        /// </summary>
        /// <param name="entities">待删除的实体对象数据源</param>
        /// <returns>删除的实体对象的数量</returns>
        int RemoveAll(IDbQuery<TEntity> entities);

        /// <summary>
        /// 向数据库UPDATE实体对象的所有的非标识属性的值
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>UPDATE的行数</returns>
        int Modify(TEntity entity);
        /// <summary>
        /// 异步向数据库UPDATE实体对象的所有的非标识属性的值
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>UPDATE的行数</returns>
        Task<int> ModifyAsync(TEntity entity);
        /// <summary>
        /// 向数据库UPDATE实体对象中所有指定修改的属性的值
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>UPDATE的行数</returns>
        int Save(TEntity entity);
        /// <summary>
        /// 异步向数据库UPDATE实体对象中所有指定修改的属性的值
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns>UPDATE的行数</returns>
        Task<int> SaveAsync(TEntity entity);

        /// <summary>
        /// 向数据库UPDATE映射对象的所有的非标识属性的值
        /// </summary>
        /// <typeparam name="TModel">映射类型（映射类型的属性名必须与实体类型的属性名一致）</typeparam>
        /// <param name="model">映射对象</param>
        /// <returns>UPDATE的行数</returns>
        int ModifyModel<TModel>(TModel model)
            where TModel : class;
        /// <summary>
        /// 异步向数据库UPDATE映射对象的所有的非标识属性的值
        /// </summary>
        /// <typeparam name="TModel">映射类型（映射类型的属性名必须与实体类型的属性名一致）</typeparam>
        /// <param name="model">映射对象</param>
        /// <returns>UPDATE的行数</returns>
        Task<int> ModifyModelAsync<TModel>(TModel model)
            where TModel : class;
        /// <summary>
        /// 向数据库UPDATE映射对象中所有指定修改的属性的值
        /// </summary>
        /// <typeparam name="TModel">映射类型（映射类型的属性名必须与实体类型的属性名一致）</typeparam>
        /// <param name="model">映射对象</param>
        /// <returns>UPDATE的行数</returns>
        int SaveModel<TModel>(TModel model)
            where TModel : class;
        /// <summary>
        /// 异步向数据库UPDATE映射对象中所有指定修改的属性的值
        /// </summary>
        /// <typeparam name="TModel">映射类型（映射类型的属性名必须与实体类型的属性名一致）</typeparam>
        /// <param name="model">映射对象</param>
        /// <returns>UPDATE的行数</returns>
        Task<int> SaveModelAsync<TModel>(TModel model)
            where TModel : class;

        /// <summary>
        /// 批量修改符合条件的实体的信息
        /// </summary>
        /// <param name="setParameters">属性名及属性值字典</param>
        /// <param name="entities">待修改的实体的数据源</param>
        /// <returns>UPDATE的行数</returns>
        int SaveAll(IDictionary<string, object> setParameters, IDbQuery<TEntity> entities);
    }
}
