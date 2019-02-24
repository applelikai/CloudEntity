using System;
using System.Collections.Generic;
using System.Text;

namespace CloudEntity.CommandTrees.Commom.SqlClient
{
    /// <summary>
    /// sql row_number函数节点生成类
    /// 李凯 Apple_Li
    /// </summary>
    internal class RowNumberBuilder : ISqlBuilder
    {
        /// <summary>
        /// 列名数组
        /// </summary>
        private string[] _columnNames;
        /// <summary>
        /// True:升序(False为降序)
        /// </summary>
        private bool _isAsc;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="columnNames">列名数组</param>
        /// <param name="isAsc">True:升序(False为降序)</param>
        public RowNumberBuilder(string[] columnNames, bool isAsc)
        {
            this._columnNames = columnNames;
            this._isAsc = isAsc;
        }
        /// <summary>
        /// 拼接sql语句
        /// </summary>
        /// <param name="commandText">待拼接的sql</param>
        public void Build(StringBuilder commandText)
        {
            //拼接ROW_NUMBER函数
            commandText.Append("ROW_NUMBER()");
            commandText.Append(" OVER(ORDER BY");
            //拼接排序列
            for (int i = 0; i < this._columnNames.Length; i++)
            {
                commandText.Append(" ");
                commandText.Append(this._columnNames[i]);
                commandText.Append(this._isAsc ? string.Empty : " DESC");
                if (i + 1 < this._columnNames.Length)
                    commandText.Append(',');
            }
            //拼接结尾
            commandText.Append(")");
            commandText.Append(" RowNumber");
        }
    }
}
