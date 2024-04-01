using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity
{
    /// <summary>
    /// 用于创建对象，并给对象属性赋值的实体访问类
    /// 李凯 Apple_Li 15150598493
    /// 最后修改时间：2024/03/24
    /// </summary>
    public sealed class ObjectAccessor
    {
        /// <summary>
        /// 创建对象的委托
        /// </summary>
        private readonly Delegate _creator;
        /// <summary>
        /// creators线程锁
        /// </summary>
        private readonly object _creatorLocker;
        /// <summary>
        /// 创建对象的匿名函数字典
        /// </summary>
        private readonly IDictionary<ConstructorInfo, Delegate> _creators;
        /// <summary>
        /// 属性访问器字典
        /// </summary>
        private readonly IDictionary<string, PropertyAccessor> _propertyAccessors;
        /// <summary>
        /// 访问器字典线程锁
        /// </summary>
        private readonly static object _accessorsLocker;
        /// <summary>
        /// 对象访问器字典
        /// </summary>
        private readonly static IDictionary<string, ObjectAccessor> _accessors;

        /// <summary>
        /// Object's type
        /// 对象类型
        /// </summary>
        public TypeInfo ObjectType { get; private set; }

        /// <summary>
        /// 对象访问器静态初始化
        /// </summary>
        static ObjectAccessor()
        {
            _accessorsLocker = new object();
            _accessors = new Dictionary<string, ObjectAccessor>();
        }
        /// <summary>
        /// 创建对象访问器
        /// </summary>
        /// <param name="objectType">对象类型</param>
        private ObjectAccessor(Type objectType)
        {
            // 赋值
            this.ObjectType = objectType.GetTypeInfo();
            // 初始化不带参数的创建对象的匿名函数
            NewExpression newExpression = Expression.New(objectType);
            _creator = Expression.Lambda(newExpression).Compile();
            // 初始化创建对象的匿名函数字典
            _creators = new Dictionary<ConstructorInfo, Delegate>();
            _creatorLocker = new object();
            // 初始化属性访问器字典
            _propertyAccessors = this.GetPropertyAccessors();
        }
        /// <summary>
        /// 获取当前对象所有可读可写的属性
        /// </summary>
        /// <returns>当前对象所有可读可写的属性</returns>
        private IEnumerable<PropertyInfo> GetProperties()
        {
            //遍历属性,获取可读可写的基本类型的属性
            foreach (PropertyInfo property in this.ObjectType.GetProperties())
            {
                //排除不可读属性
                if (!property.CanRead)
                    continue;
                //排除不可写属性
                if (!property.CanWrite)
                    continue;
                //返回属性
                yield return property;
            }
        }
        /// <summary>
        /// 获取属性访问器字典
        /// </summary>
        /// <returns>属性访问器字典</returns>
        private IDictionary<string, PropertyAccessor> GetPropertyAccessors()
        {
            // 创建属性访问器字典
            IDictionary<string, PropertyAccessor> propertyAccessors = new Dictionary<string, PropertyAccessor>();
            // 遍历属性,创建并注册属性访问器
            foreach (PropertyInfo property in this.GetProperties())
                propertyAccessors.Add(property.Name, new PropertyAccessor(property));
            // 返回属性访问器字典
            return propertyAccessors;
        }
        /// <summary>
        /// 获取创建对象的委托
        /// </summary>
        /// <param name="constructor">构造函数</param>
        /// <returns>创建对象的委托</returns>
        private Delegate GetCreator(ConstructorInfo constructor)
        {
            Start:
            // 若creators中包含当前构造函数的委托，直接返回
            if (_creators.ContainsKey(constructor))
                return _creators[constructor];
            // 进入单线程模式
            lock (_creatorLocker)
            {
                // 委托不存在则创建委托
                if (!_creators.ContainsKey(constructor))
                {
                    // 获取ParameterExpressions
                    ParameterInfo[] parameterInfos = constructor.GetParameters();
                    ParameterExpression[] parameterExpressions = new ParameterExpression[parameterInfos.Length];
                    for (int i = 0; i < parameterInfos.Length; i++)
                        parameterExpressions[i] = Expression.Parameter(parameterInfos[i].ParameterType, parameterInfos[i].Name);
                    // 创建委托
                    NewExpression newExpression = Expression.New(constructor, parameterExpressions);
                    Delegate creator = Expression.Lambda(newExpression, parameterExpressions).Compile();
                    // 添加委托
                    _creators.Add(constructor, creator);
                }
            }
            // 回到开始
            goto Start;
        }

        /// <summary>
        /// 给对象某属性赋值
        /// </summary>
        /// <param name="propertyName">对象某属性的名称</param>
        /// <param name="instance">对象</param>
        /// <param name="value">值</param>
        public void SetValue(string propertyName, object instance, object value)
        {
            // 为对象当前属性赋值
            _propertyAccessors[propertyName].SetValue(instance, value);
        }
        /// <summary>
        /// 给对象某属性赋转换值(值类型不一致则先转换)
        /// </summary>
        /// <param name="propertyName">对象某属性的名称</param>
        /// <param name="instance">对象</param>
        /// <param name="value">值</param>
        public void SetConvertValue(string propertyName, object instance, object value)
        {
            // 为对象当前属性赋值(若值类型与属性类型不一致，则转换值类型再赋给当前对象属性)
            _propertyAccessors[propertyName].SetConvertValue(instance, value);
        }
        /// <summary>
        /// 给对象某属性赋值
        /// </summary>
        /// <param name="propertyName">对象某属性的名称</param>
        /// <param name="instance">对象</param>
        /// <param name="value">值</param>
        public void TrySetValue(string propertyName, object instance, object value)
        {
            // 若不包含当前属性,退出
            if (!_propertyAccessors.ContainsKey(propertyName))
                return;
            // 若对象实例为空,退出
            if (instance == null)
                return;
            // 为对象实例当前属性赋值
            _propertyAccessors[propertyName].SetValue(instance, value);
        }
        /// <summary>
        /// 获取对象某属性的值
        /// </summary>
        /// <param name="propertyName">对象某属性的属性名</param>
        /// <param name="instance">对象</param>
        /// <returns>对象某属性的值</returns>
        public object GetValue(string propertyName, object instance)
        {
            // 获取对象实例当前属性值
            return _propertyAccessors[propertyName].GetValue(instance);
        }
        /// <summary>
        /// 获取对象某属性的值
        /// </summary>
        /// <param name="propertyName">对象某属性的属性名</param>
        /// <param name="instance">对象</param>
        /// <returns>对象某属性的值</returns>
        public object TryGetValue(string propertyName, object instance)
        {
            // 若不包含当前属性，返回null
            if (!_propertyAccessors.ContainsKey(propertyName))
                return null;
            // 若对象实例为空,返回null
            if (instance == null)
                return null;
            // 获取对象实例当前属性值
            return _propertyAccessors[propertyName].GetValue(instance);
        }
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <returns>实体对象</returns>
        public object CreateInstance()
        {
            return _creator.DynamicInvoke();
        }
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="arguments">构造函数的参数集合</param>
        /// <returns>实体对象</returns>
        public object CreateInstance(object[] arguments)
        {
            // 获取类型数组
            Type[] argumentTypes = new Type[arguments.Length];
            for (int i = 0; i < argumentTypes.Length; i++)
            {
                Check.ArgumentNull(arguments[i], "arguments", string.Format("arguments[{0}] is null", i));
                argumentTypes[i] = arguments[i].GetType();
            }
            // 获取构造函数
            ConstructorInfo constructor = this.ObjectType.GetConstructor(argumentTypes);
            if (constructor == null)
                return null;
            // 创建对象
            return this.GetCreator(constructor).DynamicInvoke(arguments);
        }
        /// <summary>
        /// 获取所有可读可写的属性名
        /// </summary>
        /// <returns>所有可读可写的属性名</returns>
        public IEnumerable<string> GetPropertyNames()
        {
            return _propertyAccessors.Keys;
        }
        /// <summary>
        /// 获取对象访问器
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>对象访问器</returns>
        public static ObjectAccessor GetAccessor(Type objectType)
        {
            // 非空检查
            Check.ArgumentNull(objectType, nameof(objectType));
            // 若字典中存在当前类型访问器,直接返回
            Start:
            if (ObjectAccessor._accessors.ContainsKey(objectType.FullName))
                return ObjectAccessor._accessors[objectType.FullName];
            // 进入线程安全模式
            lock (ObjectAccessor._accessorsLocker)
            {
                // 若字典中不存在当前类型访问器,创建并添加到字典中
                if (!ObjectAccessor._accessors.ContainsKey(objectType.FullName))
                    ObjectAccessor._accessors.Add(objectType.FullName, new ObjectAccessor(objectType));
                // 回到Start,重新再字典中获取当前类型的访问器
                goto Start;
            }
        }
    }
}
