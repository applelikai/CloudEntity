using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity
{
    /// <summary>
    /// 用于创建对象，并给对象属性赋值的实体访问类
    /// 李凯 Apple_Li
    /// </summary>
    public sealed class ObjectAccessor
    {
        private IDictionary<string, PropertyAccessor> propertyAccessors;    //属性访问器字典
        private IDictionary<ConstructorInfo, Delegate> creators;            //创建对象的匿名函数字典
        private object creatorLocker;                                       //creators线程锁
        private static object accessorsLocker;                              //访问器字典线程锁
        private static IDictionary<string, ObjectAccessor> accessors;       //对象访问器字典

        /// <summary>
        /// 属性访问器字典
        /// </summary>
        private IDictionary<string, PropertyAccessor> PropertyAccessors
        {
            get { return this.propertyAccessors ?? (this.propertyAccessors = this.GetPropertyAccessors()); }
        }
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
            ObjectAccessor.accessorsLocker = new object();
            ObjectAccessor.accessors = new Dictionary<string, ObjectAccessor>();
        }
        /// <summary>
        /// 创建对象访问器
        /// </summary>
        /// <param name="objectType">对象类型</param>
        private ObjectAccessor(TypeInfo objectType)
        {
            //赋值
            this.ObjectType = objectType;
            //初始化创建对象的匿名函数字典
            this.creators = new Dictionary<ConstructorInfo, Delegate>();
            this.creatorLocker = new object();
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
            //创建属性访问器字典
            IDictionary<string, PropertyAccessor> propertyAccessors = new Dictionary<string, PropertyAccessor>();
            //遍历属性,创建并注册属性访问器
            foreach (PropertyInfo property in this.GetProperties())
                propertyAccessors.Add(property.Name, new PropertyAccessor(property));
            //返回属性访问器字典
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
            //若creators中包含当前构造函数的委托，直接返回
            if (this.creators.ContainsKey(constructor))
                return this.creators[constructor];
            //进入单线程模式
            lock (this.creatorLocker)
            {
                //委托不存在则创建委托
                if (!this.creators.ContainsKey(constructor))
                {
                    //获取ParameterExpressions
                    ParameterInfo[] parameterInfos = constructor.GetParameters();
                    ParameterExpression[] parameterExpressions = new ParameterExpression[parameterInfos.Length];
                    for (int i = 0; i < parameterInfos.Length; i++)
                        parameterExpressions[i] = Expression.Parameter(parameterInfos[i].ParameterType, parameterInfos[i].Name);
                    //创建委托
                    NewExpression newExpression = Expression.New(constructor, parameterExpressions);
                    Delegate creator = Expression.Lambda(newExpression, parameterExpressions).Compile();
                    //添加委托
                    this.creators.Add(constructor, creator);
                }
                goto Start;
            }
        }

        /// <summary>
        /// 给对象某属性赋值
        /// </summary>
        /// <param name="propertyName">对象某属性的名称</param>
        /// <param name="instance">对象</param>
        /// <param name="value">值</param>
        public void SetValue(string propertyName, object instance, object value)
        {
            //为entity当前属性赋值
            this.PropertyAccessors[propertyName].SetValue(instance, value);
        }
        /// <summary>
        /// 给对象某属性赋值
        /// </summary>
        /// <param name="propertyName">对象某属性的名称</param>
        /// <param name="instance">对象</param>
        /// <param name="value">值</param>
        public void TrySetValue(string propertyName, object instance, object value)
        {
            //若不包含当前属性,退出
            if (!this.PropertyAccessors.ContainsKey(propertyName))
                return;
            //若entity为空,退出
            if (instance == null)
                return;
            //为entity当前属性赋值
            this.PropertyAccessors[propertyName].SetValue(instance, value);
        }
        /// <summary>
        /// 获取对象某属性的值
        /// </summary>
        /// <param name="propertyName">对象某属性的属性名</param>
        /// <param name="instance">对象</param>
        /// <returns>对象某属性的值</returns>
        public object GetValue(string propertyName, object instance)
        {
            //获取entity当前属性值
            return this.PropertyAccessors[propertyName].GetValue(instance);
        }
        /// <summary>
        /// 获取对象某属性的值
        /// </summary>
        /// <param name="propertyName">对象某属性的属性名</param>
        /// <param name="instance">对象</param>
        /// <returns>对象某属性的值</returns>
        public object TryGetValue(string propertyName, object instance)
        {
            //若不包含当前属性，返回null
            if (!this.PropertyAccessors.ContainsKey(propertyName))
                return null;
            //若entity为空,返回null
            if (instance == null)
                return null;
            //获取entity当前属性值
            return this.PropertyAccessors[propertyName].GetValue(instance);
        }
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="arguments">构造函数的参数集合</param>
        /// <returns>实体对象</returns>
        public object CreateInstance(params object[] arguments)
        {
            //获取类型数组
            Type[] argumentTypes = new Type[arguments.Length];
            for (int i = 0; i < argumentTypes.Length; i++)
            {
                Check.ArgumentNull(arguments[i], "arguments", string.Format("arguments[{0}] is null", i));
                argumentTypes[i] = arguments[i].GetType();
            }
            //获取构造函数
            ConstructorInfo constructor = this.ObjectType.GetConstructor(argumentTypes);
            if (constructor == null)
                return null;
            //创建对象
            return this.GetCreator(constructor).DynamicInvoke(arguments);
        }
        /// <summary>
        /// 获取所有可读可写的属性名
        /// </summary>
        /// <returns>所有可读可写的属性名</returns>
        public IEnumerable<string> GetPropertyNames()
        {
            return this.PropertyAccessors.Keys;
        }
        /// <summary>
        /// 获取对象访问器
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <returns>对象访问器</returns>
        public static ObjectAccessor GetAccessor(TypeInfo objectType)
        {
            //检查
            Check.ArgumentNull(objectType, nameof(objectType));
            //若字典中存在当前类型访问器,直接返回
            Start:
            if (ObjectAccessor.accessors.ContainsKey(objectType.FullName))
                return ObjectAccessor.accessors[objectType.FullName];
            //进入线程安全模式
            lock (ObjectAccessor.accessorsLocker)
            {
                //若字典中不存在当前类型访问器,创建并添加到字典中
                if (!ObjectAccessor.accessors.ContainsKey(objectType.FullName))
                    ObjectAccessor.accessors.Add(objectType.FullName, new ObjectAccessor(objectType));
                //回到Start,重新再字典中获取当前类型的访问器
                goto Start;
            }
        }
    }
}
