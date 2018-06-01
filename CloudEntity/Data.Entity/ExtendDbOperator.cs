using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace CloudEntity.Data.Entity
{
    /// <summary>
    /// 数据操作扩展类
    /// </summary>
    public static class ExtendDbOperator
    {
        /// <summary>
        /// 获取属性名及属性值的字典
        /// </summary>
        /// <param name="model">对象</param>
        /// <returns>属性名及属性值的字典</returns>
        private static IDictionary<string, object> GetSetParameters(object model)
        {
            IDictionary<string, object> setParameters = new Dictionary<string, object>();
            //遍历所有属性
            foreach (PropertyInfo property in model.GetType().GetTypeInfo().GetProperties())
            {
                //排除不符合条件的property
                if (!property.PropertyType.FullName.StartsWith("System."))
                    continue;
                if (property.PropertyType.IsArray)
                    continue;
                //获取并添加Update Set表达式子节点及其附属参数
                setParameters.Add(property.Name, property.GetValue(model, null));
            }
            return setParameters;
        }

        /// <summary>
        /// ExtendMethod: 删除数据源中所有符合条件的实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="dbOperator">实体数据操作对象</param>
        /// <param name="predicates">删除条件表达式数组</param>
        /// <returns>删除的实体对象的数量</returns>
        public static int RemoveAll<TEntity>(this IDbOperator<TEntity> dbOperator, params Expression<Func<TEntity, bool>>[] predicates)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(dbOperator, nameof(dbOperator));
            //获取待删除的实体对象的数据源
            IDbQuery<TEntity> entities = dbOperator.Factory.CreateQuery<TEntity>().Filter(predicates);
            //执行删除数据源中所有的实体
            return dbOperator.RemoveAll(entities);
        }
        /// <summary>
        /// ExtendMethod: 删除某属性值匹配的实体对象
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="dbOperator">实体数据操作对象</param>
        /// <param name="selector">执行实体某属性的表达式</param>
        /// <param name="value">属性值</param>
        /// <param name="isLike">LIKE 或 NOT LIKE</param>
        /// <returns>删除的实体对象的数量</returns>
        public static int RemoveLike<TEntity>(this IDbOperator<TEntity> dbOperator, Expression<Func<TEntity, string>> selector, string value, bool isLike = true)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(dbOperator, nameof(dbOperator));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(value, nameof(value));
            //获取待删除的实体对象的数据源
            IDbQuery<TEntity> entities = dbOperator.Factory.CreateQuery<TEntity>().Like(selector, value, isLike);
            //执行删除数据源中所有的实体
            return dbOperator.RemoveAll(entities);
        }
        /// <summary>
        /// ExtendMethod: 删除某属性值在一定区间内的实体
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">实体属性类型</typeparam>
        /// <param name="dbOperator">实体数据操作对象</param>
        /// <param name="selector">执行实体某属性的表达式</param>
        /// <param name="leftValue">最小值</param>
        /// <param name="rightValue">最大值</param>
        /// <returns>删除的实体对象的数量</returns>
        public static int RemoveBetween<TEntity, TProperty>(this IDbOperator<TEntity> dbOperator, Expression<Func<TEntity, TProperty>> selector, TProperty leftValue, TProperty rightValue)
            where TEntity : class
            where TProperty : struct
        {
            //非空检查
            Check.ArgumentNull(dbOperator, nameof(dbOperator));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(leftValue, nameof(leftValue));
            Check.ArgumentNull(rightValue, nameof(rightValue));
            //获取待删除的实体对象的数据源
            IDbQuery<TEntity> entities = dbOperator.Factory.CreateQuery<TEntity>().Between(selector, leftValue, rightValue);
            //执行删除数据源中所有的实体
            return dbOperator.RemoveAll(entities);
        }

        /// <summary>
        /// ExtendMethod: 批量修改符合条件的实体对象的某些属性值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <param name="dbOperator">实体数据操作对象</param>
        /// <param name="model">存储所有待修改的实体统一修改的属性值</param>
        /// <param name="predicates">修改条件表达式数组</param>
        /// <returns>被修改值的实体元素数量</returns>
        public static int SetAll<TEntity>(this IDbOperator<TEntity> dbOperator, object model, params Expression<Func<TEntity, bool>>[] predicates)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(dbOperator, nameof(dbOperator));
            Check.ArgumentNull(model, nameof(model));
            //获取待修改的实体数据源
            IDbQuery<TEntity> entities = dbOperator.Factory.CreateQuery<TEntity>().Filter(predicates);
            //执行批量修改
            return dbOperator.SaveAll(ExtendDbOperator.GetSetParameters(model), entities);
        }
        /// <summary>
        /// ExtendMethod: 批量修改当前数据源中符合条件的实体某属性的值
        /// </summary>
        /// <typeparam name="TEntity">实体类型</typeparam>
        /// <typeparam name="TProperty">对象属性类型</typeparam>
        /// <param name="dbOperator">实体数据操作对象</param>
        /// <param name="selector">指定对象某属性的表达式</param>
        /// <param name="value">属性值</param>
        /// <param name="predicates">修改条件表达式数组</param>
        /// <returns>被修改值的实体元素数量</returns>
        public static int SetAll<TEntity, TProperty>(this IDbOperator<TEntity> dbOperator, Expression<Func<TEntity, TProperty>> selector, TProperty value, params Expression<Func<TEntity, bool>>[] predicates)
            where TEntity : class
        {
            //非空检查
            Check.ArgumentNull(dbOperator, nameof(dbOperator));
            Check.ArgumentNull(selector, nameof(selector));
            Check.ArgumentNull(value, nameof(value));
            //获取属性参数字典
            IDictionary<string, object> setParameters = new Dictionary<string, object>()
            {
                {selector.Body.GetMemberName(), value}
            };
            //获取待修改的实体数据源
            IDbQuery<TEntity> entities = dbOperator.Factory.CreateQuery<TEntity>().Filter(predicates);
            //执行批量修改
            return dbOperator.SaveAll(setParameters, entities);
        }
    }
}
