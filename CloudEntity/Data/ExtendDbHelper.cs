using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CloudEntity.Data
{
    /// <summary>
    /// DbHelper扩展类
    /// Apple_Li 李凯
    /// </summary>
    public static class ExtendDbHelper
    {
        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="TModel">对象类型</typeparam>
        /// <param name="accessor">对象存取器</param>
        /// <param name="reader">数据流</param>
        /// <param name="columnNames">查询结果列</param>
        /// <returns>对象</returns>
        private static TModel CreateModel<TModel>(this ObjectAccessor accessor, IDataReader reader, string[] columnNames)
            where TModel : class, new()
        {
            //创建对象
            TModel model = new TModel();
            //填值
            for (int i = 0; i < columnNames.Length; i++)
            {
                if (!reader.IsDBNull(i))
                    accessor.TrySetValue(columnNames[i], model, reader.GetValue(i));
            }
            //返回对象
            return model;
        }
        /// <summary>
        /// 获取sql参数流
        /// </summary>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="parameterModel">参数模型对象</param>
        /// <returns>sql参数流</returns>
        private static IEnumerable<IDbDataParameter> GetParameters(this IDbHelper dbHelper, object parameterModel)
        {
            //遍历所有的属性并创建返回sql参数
            foreach (PropertyInfo property in parameterModel.GetType().GetRuntimeProperties())
            {
                yield return dbHelper.CreateParameter(property.Name, property.GetValue(parameterModel));
            }
        }
        /// <summary>
        /// 获取sql参数流
        /// </summary>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="parameterModel">参数模型对象</param>
        /// <param name="commandText">sql命令</param>
        /// <param name="marker">sql参数标识符号</param>
        /// <returns>sql参数流</returns>
        private static IEnumerable<IDbDataParameter> GetParameters(this IDbHelper dbHelper, object parameterModel, string commandText, char marker)
        {
            //遍历所有的属性并创建返回sql参数
            foreach (PropertyInfo property in parameterModel.GetType().GetRuntimeProperties())
            {
                //若sql命令中包含此sql参数名，则创建参数并返回
                if (commandText.Contains(string.Concat(marker, property.Name)))
                    yield return dbHelper.CreateParameter(property.Name, property.GetValue(parameterModel));
            }
        }

        /// <summary>
        /// 扩展方法:执行修改,获取受影响行数
        /// </summary>
        /// <typeparam name="TModel">对象类型</typeparam>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="commandText">sql命令</param>
        /// <param name="model">对象</param>
        /// <returns>受影响行数</returns>
        public static int Modify<TModel>(this IDbHelper dbHelper, string commandText, TModel model)
            where TModel : class, new()
        {
            //执行修改并获取受影响的行数
            return dbHelper.ExecuteUpdate(commandText, parameters: dbHelper.GetParameters(model).ToArray());
        }
        /// <summary>
        /// 扩展方法:执行修改,获取受影响行数
        /// </summary>
        /// <typeparam name="TModel">对象类型</typeparam>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="commandText">sql命令</param>
        /// <param name="model">对象</param>
        /// <param name="marker">sql参数标识符</param>
        /// <returns>受影响行数</returns>
        public static int Modify<TModel>(this IDbHelper dbHelper, string commandText, TModel model, char marker)
            where TModel : class, new()
        {
            //获取sql参数数组
            IDbDataParameter[] parameters = dbHelper.GetParameters(model, commandText, marker).ToArray();
            //执行修改并获取受影响的行数
            return dbHelper.ExecuteUpdate(commandText, parameters: parameters);
        }
        /// <summary>
        /// 扩展方法:执行查询获取对象流
        /// </summary>
        /// <typeparam name="TModel">对象类型</typeparam>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="commandText">sql命令</param>
        /// <param name="parameters">sql参数数组</param>
        /// <returns>对象流</returns>
        public static IEnumerable<TModel> GetModels<TModel>(this IDbHelper dbHelper, string commandText, params IDbDataParameter[] parameters)
            where TModel : class, new()
        {
            //获取TModel类型的对象存取器
            ObjectAccessor accessor = ObjectAccessor.GetAccessor(typeof(TModel));
            //执行查询获取结果
            return dbHelper.GetResults(accessor.CreateModel<TModel>, commandText, parameters: parameters);
        }
        /// <summary>
        /// 扩展方法:执行查询获取对象流
        /// </summary>
        /// <typeparam name="TModel">对象类型</typeparam>
        /// <param name="dbHelper">操作数据库的DbHelper</param>
        /// <param name="commandText">sql命令</param>
        /// <param name="parameterModel">参数对象</param>
        /// <returns>对象流</returns>
        public static IEnumerable<TModel> GetModels<TModel>(this IDbHelper dbHelper, string commandText, object parameterModel)
            where TModel : class, new()
        {
            //获取sql参数数组
            IDbDataParameter[] parameters = dbHelper.GetParameters(parameterModel).ToArray();
            //获取TModel类型的对象存取器
            ObjectAccessor accessor = ObjectAccessor.GetAccessor(typeof(TModel));
            //执行查询获取结果
            return dbHelper.GetResults(accessor.CreateModel<TModel>, commandText, parameters: parameters);
        }
    }
}
