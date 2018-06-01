using System.Reflection;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 属性链接
    /// 李凯 Apple_Li
    /// </summary>
    public class PropertyLinker
    {
        /// <summary>
        /// 当前对象类型的属性
        /// </summary>
        public PropertyInfo Property { get; private set; }
        /// <summary>
        /// 当前对象的关联对象的属性链接
        /// </summary>
        public PropertyLinker[] RelationalLinkers { get; private set; }

        /// <summary>
        /// 创建属性链接
        /// </summary>
        /// <param name="property">对象类型的属性</param>
        /// <param name="propertyLinkers">对象的关联子属性连接数组</param>
        public PropertyLinker(PropertyInfo property, params PropertyLinker[] propertyLinkers)
        {
            this.Property = property;
            this.RelationalLinkers = propertyLinkers;
        }
    }
}
