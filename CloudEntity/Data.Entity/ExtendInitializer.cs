using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 初始化类扩展
    /// </summary>
    public static class ExtendInitializer
    {
        /// <summary>
        /// ExtendMethod: 初始化并生成某实体类所Mapping的Table
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="container">数据容器</param>
        public static void InitTable<TEntity>(this IDbContainer container)
            where TEntity : class
        {
            container.InitTable(typeof(TEntity));
        }
        /// <summary>
        /// ExtendMethod: 从旧表重命名获取所Mapping的Table
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="container">数据容器</param>
        /// <param name="oldTableName">旧的表名</param>
        public static void RenameTable<TEntity>(this IDbContainer container, string oldTableName)
            where TEntity : class
        {
            container.RenameTable(typeof(TEntity), oldTableName);
        }
        /// <summary>
        /// ExtendMethod: 删除实体类Mapping的表
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="container">数据容器</param>
        public static void DropTable<TEntity>(this IDbContainer container)
            where TEntity : class
        {
            container.DropTable(typeof(TEntity));
        }
    }
}
