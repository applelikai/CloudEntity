using System;
using System.Reflection;

namespace CloudEntity
{
    /// <summary>
    /// 异常检查类
    /// Apple_Li 2017/05/17
    /// </summary>
    public static class Check
    {
        /// <summary>
        /// 检查当前类型是否为基本类型
        /// </summary>
        /// <param name="type">类型</param>
        internal static void IsBaseType(Type type)
        {
            if (type.Namespace != "System")
                throw new Exception(string.Format("{0} not start with System", type.FullName));
        }
        /// <summary>
        /// 检查当前属性是否满足Mapping条件
        /// </summary>
        /// <param name="property">属性</param>
        /// <returns>当前属性是否满足Mapping条件</returns>
        internal static bool IsCanMapping(PropertyInfo property)
        {
            //检查是否可读
            if (!property.CanRead)
                return false;
            //是否可写
            if (!property.CanWrite)
                return false;
            //是否为基本类型
            if (!property.PropertyType.Namespace.Equals("System"))
                return false;
            //是否为抽象类型
            if (property.PropertyType.GetTypeInfo().IsAbstract)
                return false;
            //若都通过了,则当前属性满足Mapping条件
            return true;
        }

        /// <summary>
        /// 检查参数是否为空
        /// </summary>
        /// <param name="argument">参数</param>
        /// <param name="paramName">参数名</param>
        /// <param name="message">异常信息</param>
        public static void ArgumentNull(object argument, string paramName, string message = null)
        {
            if (argument != null)
                return;
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(paramName);
            else
                throw new ArgumentNullException(paramName, message);
        }
        /// <summary>
        /// 检查参数是否为空
        /// </summary>
        /// <param name="argument">参数</param>
        /// <param name="paramName">参数名</param>
        /// <param name="message">异常信息</param>
        public static void ArgumentNull(string argument, string paramName, string message = null)
        {
            if (!string.IsNullOrEmpty(argument))
                return;
            if (string.IsNullOrEmpty(message))
                throw new ArgumentNullException(paramName);
            else
                throw new ArgumentNullException(paramName, message);
        }
    }
}
