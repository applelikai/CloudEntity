using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity
{
    /// <summary>
    /// 属性值存取类
    /// 李凯 Apple_Li
    /// </summary>
    internal class PropertyAccessor
    {
        /// <summary>
        /// 属性
        /// </summary>
        private PropertyInfo property;
        /// <summary>
        /// 用于获取对象某属性值的委托
        /// </summary>
        private Delegate getValue;
        /// <summary>
        /// 用于给对象某属性赋值的委托
        /// </summary>
        private Delegate setValue;

        /// <summary>
        /// 创建属性存取器
        /// </summary>
        /// <param name="property">属性</param>
        public PropertyAccessor(PropertyInfo property)
        {
            //非空检查
            Check.ArgumentNull(property, nameof(property));
            //给属性赋值
            this.property = property;
            //创建参数表达式
            ParameterExpression entityExpression = Expression.Parameter(property.DeclaringType);//创建对象参数表达式
            //初始化获取对象属性值的委托
            MemberExpression getPropertyValue = Expression.Property(entityExpression, property);//创建指定对象某属性的表达式
            this.getValue = Expression.Lambda(getPropertyValue, entityExpression).Compile();
            //初始化给对象某熟悉赋值的委托
            ParameterExpression valueExpression = Expression.Parameter(property.PropertyType);//创建属性值参数表达式
            MethodCallExpression setPropertyValue = Expression.Call(entityExpression, property.GetSetMethod(), valueExpression);
            this.setValue = Expression.Lambda(setPropertyValue, entityExpression, valueExpression).Compile();
        }
        /// <summary>
        /// Get entity's property's value
        /// 获取对象某熟悉的值
        /// </summary>
        /// <param name="entity">对象</param>
        /// <returns>The entity's property's value</returns>
        public object GetValue(object entity)
        {
            //获取entity当前属性的值
            return this.getValue.DynamicInvoke(entity);
        }
        /// <summary>
        /// Set entity's property's value
        /// 给对象某属性赋值
        /// </summary>
        /// <param name="entity">对象</param>
        /// <param name="value">对象的属性值</param>
        public void SetValue(object entity, object value)
        {
            //若属性类型和值类型不一致，退出
            if (!this.property.PropertyType.GetTypeInfo().IsInstanceOfType(value))
                return;
            //为entity赋值
            this.setValue.DynamicInvoke(entity, value);
        }
    }
}
