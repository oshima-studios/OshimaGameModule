using System.Data;

namespace Oshima.FunGame.OshimaServers.Service
{
    public class Utility
    {
        public static class DataSetConverter
        {
            /// <summary>
            /// 将DataSet转换为Dictionary列表
            /// </summary>
            /// <param name="dataSet">输入的DataSet</param>
            /// <returns>Dictionary列表，每个Dictionary代表一行数据</returns>
            public static List<Dictionary<string, object>> ConvertDataSetToDictionary(DataSet dataSet)
            {
                List<Dictionary<string, object>> result = [];

                if (dataSet == null || dataSet.Tables.Count == 0)
                    return result;

                foreach (DataTable table in dataSet.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        Dictionary<string, object> rowDict = [];

                        foreach (DataColumn column in table.Columns)
                        {
                            // 处理DBNull值
                            if (row[column] != DBNull.Value)
                            {
                                rowDict[column.ColumnName] = row[column];
                            }
                        }

                        result.Add(rowDict);
                    }
                }

                return result;
            }

            /// <summary>
            /// 将DataSet的第一张表转换为Dictionary列表
            /// </summary>
            /// <param name="dataSet">输入的DataSet</param>
            /// <returns>Dictionary列表，每个Dictionary代表一行数据</returns>
            public static List<Dictionary<string, object>> ConvertFirstTableToDictionary(DataSet dataSet)
            {
                List<Dictionary<string, object>> result = [];

                if (dataSet == null || dataSet.Tables.Count == 0)
                    return result;

                DataTable table = dataSet.Tables[0];

                foreach (DataRow row in table.Rows)
                {
                    Dictionary<string, object> rowDict = [];

                    foreach (DataColumn column in table.Columns)
                    {
                        // 处理DBNull值
                        if (row[column] != DBNull.Value)
                        {
                            rowDict[column.ColumnName] = row[column];
                        }
                    }

                    result.Add(rowDict);
                }

                return result;
            }
        }
    }
}
