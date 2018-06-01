using System.Data;

namespace CloudEntity.Data
{
    /// <summary>
    /// DataReader扩展类
    /// Apple_Li 李凯
    /// </summary>
    public static class ExtendDataReader
    {
        /// <summary>
        /// 扩展方法:获取当前查询所包含的列的数组
        /// </summary>
        /// <param name="reader">DataReader</param>
        /// <returns>当前查询所包含的列的数组</returns>
        public static string[] GetColumns(this IDataReader reader)
        {
            string[] columnNames = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
                columnNames[i] = reader.GetName(i);
            return columnNames;
        }
    }
}
