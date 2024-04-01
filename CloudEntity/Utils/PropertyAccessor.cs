using System;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity
{
    /// <summary>
    /// 属性值存取类
    /// 李凯 Apple_Li 15150598493
    /// </summary>
    internal class PropertyAccessor
    {
        /// <summary>
        /// 属性
        /// </summary>
        private readonly PropertyInfo _property;
        /// <summary>
        /// 属性原类型(如Nullable<int>的原类型为int)
        /// </summary>
        private readonly Type _propertySourceType;
        /// <summary>
        /// 用于获取对象某属性值的委托
        /// </summary>
        private readonly Delegate _getValue;
        /// <summary>
        /// 用于给对象某属性赋值的委托
        /// </summary>
        private readonly Delegate _setValue;

        /// <summary>
        /// 创建属性存取器
        /// </summary>
        /// <param name="property">属性</param>
        public PropertyAccessor(PropertyInfo property)
        {
            // 非空检查
            Check.ArgumentNull(property, nameof(property));
            // 指定当前属性
            _property = property;
            // 指定当前属性的原属性类型
            _propertySourceType = property.PropertyType.SourceType();
            // 创建参数表达式
            ParameterExpression entityExpression = Expression.Parameter(property.DeclaringType);//创建对象参数表达式
            // 初始化获取对象属性值的委托
            MemberExpression getPropertyValue = Expression.Property(entityExpression, property);//创建指定对象某属性的表达式
            _getValue = Expression.Lambda(getPropertyValue, entityExpression).Compile();
            // 初始化给对象某熟悉赋值的委托
            ParameterExpression valueExpression = Expression.Parameter(property.PropertyType);//创建属性值参数表达式
            MethodCallExpression setPropertyValue = Expression.Call(entityExpression, property.GetSetMethod(), valueExpression);
            _setValue = Expression.Lambda(setPropertyValue, entityExpression, valueExpression).Compile();
        }
        /// <summary>
        /// Get entity's property's value
        /// 获取对象某熟悉的值
        /// </summary>
        /// <param name="instance">对象</param>
        /// <returns>The entity's property's value</returns>
        public object GetValue(object instance)
        {
            // 获取entity当前属性的值
            return _getValue.DynamicInvoke(instance);
        }
        /// <summary>
        /// Set entity's property's value
        /// 给对象某属性赋值
        /// </summary>
        /// <param name="instance">对象</param>
        /// <param name="value">对象的属性值</param>
        public void SetValue(object instance, object value)
        {
            // 若属性类型和值类型不一致，退出
            if (!_propertySourceType.IsInstanceOfType(value))
                return;
            // 为entity赋值
            _setValue.DynamicInvoke(instance, value);
        }
        /// <summary>
        /// 给对象某属性赋值(若类型不一致则转换)
        /// </summary>
        /// <param name="instance">对象</param>
        /// <param name="value">对象的属性值</param>
        public void SetConvertValue(object instance, object value)
        {
            // 若属性类型和值类型不一致
            if (!_propertySourceType.IsInstanceOfType(value))
            {
                // 转换获取属性目标值
                object targetValue = Convert.ChangeType(value, _propertySourceType);
                // 转换值的类型并赋值
                _setValue.DynamicInvoke(instance, targetValue);
                // 退出
                return;
            }
            // 为entity赋值
            _setValue.DynamicInvoke(instance, value);
        }
    }
}
