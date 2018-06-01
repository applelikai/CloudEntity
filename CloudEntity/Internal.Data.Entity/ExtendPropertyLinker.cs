using CloudEntity.Data.Entity;
using CloudEntity.Mapping;
using System.Linq;

namespace CloudEntity.Internal.Data.Entity
{
    /// <summary>
    /// PropertyLinker扩展类
    /// </summary>
    internal static class ExtendPropertyLinker
    {
        /// <summary>
        /// Extendable method: 获取对象访问关联对象
        /// </summary>
        /// <param name="propertyLinker">属性关联对象</param>
        /// <param name="mapperContainer">Mapper容器</param>
        /// <returns>对象访问关联对象</returns>
        internal static AccessorLinker ToAccessorLinker(this PropertyLinker propertyLinker, IMapperContainer mapperContainer)
        {
            //获取子联访问链接集合
            AccessorLinker[] accessorLinkers = propertyLinker.RelationalLinkers.Select(p => p.ToAccessorLinker(mapperContainer)).ToArray();
            //创建并返回关联访问链接
            return new AccessorLinker(propertyLinker.Property, mapperContainer, accessorLinkers);
        }
    }
}
