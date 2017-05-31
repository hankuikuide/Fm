using Han.DbLight.TableMetadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Han.DbLight.Oracle
{
    public class OracleSqlBuilder
    {
        private const string insertTemplate = "INSERT INTO {0} ({1}) \r\n VALUES ({2})";
        private const string updateTemplate = "UPDATE {0} SET {1} WHERE {2}";
        private const string deleteTemplate = "DELETE FROM {0} WHERE {1}";

        private static Dictionary<string, ColumnAttribute> GetColumnProMap(Type _type)
        {
            Dictionary<string, ColumnAttribute> dict = new Dictionary<string, ColumnAttribute>();
            foreach (var propertyInfo in _type.GetProperties())
            {
                var attribute = propertyInfo.GetCustomAttributes(true).OfType<ColumnAttribute>().FirstOrDefault();
                if (attribute != null)
                {
                    dict.Add(propertyInfo.Name.ToLower(), attribute);
                }
                else
                {
                    dict.Add(propertyInfo.Name.ToLower(), null);
                }
            }

            return dict;
        }

        /// <summary>
        /// 构建插入SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="usedProperies"></param>
        /// <returns></returns>
        public static string InsertBuilder<T>(List<string> usedProperies) where T : class
        {
            string strColumns = "", strValues = "";
            StringBuilder columns = new StringBuilder();
            StringBuilder values = new StringBuilder();

            //获取表名
            TableAttribute table = typeof(T).GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
            //获取属性名与数据库字段的对象关系
            var proMap = GetColumnProMap(typeof(T));

            //获取主键,如果没有指定插入，则自动添加
            var columnAttributes = proMap.Values.ToList();
            var primaryCols = columnAttributes.FindAll(c => c.IsPrimaryKey);
            foreach (var priCols in primaryCols)
            {
                if (!usedProperies.Contains(priCols.ColumnName, StringComparer.OrdinalIgnoreCase))
                {
                    usedProperies.Add(priCols.ColumnName);
                }
            }
            //构造插入列和值，判断是不是是数据库自动插入和自动生成列
            foreach (var item in usedProperies)
            {
                if (proMap.ContainsKey(item.ToLower()))
                {
                    IColumn col = proMap[item.ToLower()];
                    if (col.IsAutoInsert)
                    {
                        continue;
                    }

                    columns.AppendFormat("{0},", col.ColumnName);

                    if (!col.IsSqlGenColumn)
                    {
                        values.AppendFormat(":{0},", item);
                    }
                    else
                    {
                        values.AppendFormat("{0},", col.GetSql());
                    }
                }
            }

            if (columns.Length > 0)
            {
                strColumns = columns.ToString().Substring(0, columns.Length - 1);
                strValues = values.ToString().Substring(0, values.Length - 1);
            }

            return string.Format(insertTemplate, table.Name, strColumns, strValues);
        }

        /// <summary>
        /// 构建更新SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="where"></param>
        /// <param name="usedProperies"></param>
        /// <returns></returns>
        public static string UpateBuilder<T>(string where, List<string> usedProperies) where T : class
        {
            List<string> cols = new List<string>();

            TableAttribute table = typeof(T).GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();

            var proMap = GetColumnProMap(typeof(T));

            var whereCount = Regex.Matches(where, @":\S+[\s\)]?");
            var count = whereCount.Count;
            if (count < 1)
            {
                throw new ArgumentException("update 没有 where 条件.");
            }

            for (int i = 0; i < usedProperies.Count - count; i++)
            {
                var item = usedProperies[i];
                if (proMap.ContainsKey(item.ToLower()))
                {
                    var temp = proMap[item.ToLower()].ColumnName + "=:" + item;
                    if (!cols.Contains(temp))
                    {
                        cols.Add(temp);
                    }
                }
            }

            return string.Format(updateTemplate, table.Name, string.Join(",", cols), where);
        }

        /// <summary>
        /// 构建删除SQL
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="usedProperies"></param>
        /// <returns></returns>
        public static string DeleteBuilder<T>(string where) where T : class
        {
            TableAttribute table = typeof(T).GetCustomAttributes(true).OfType<TableAttribute>().FirstOrDefault();
            return string.Format(deleteTemplate, table.Name, where);

        }
    }
}
