
namespace Han.DbLight
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Han.Cache;
    using Han.Infrastructure;
    using Han.DbLight.TableMetadata;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public sealed class Table
    {
        private List<IColumn> sqlGenColumns=new List<IColumn>();

        private List<IColumn> primaryColumns = new List<IColumn>();

        private List<IColumn> columns = new List<IColumn>();
        internal Table()
       {
           
       }
       internal Table(List<IColumn> columns)
       {
           this.columns = columns;
       }


        public string TableName { get; set; }
      
       

        /// <summary>
        /// 标记ColumnAttribute的属性列，并不一定在数据库真实存在
        /// </summary>
        public List<IColumn> Columns
        {
            get { return this.columns; }
        }

        /// <summary>
        /// Gets the list of primary key columns.
        /// </summary>
        public List<IColumn> KeyColumns
        {
            get
            {
               return Columns.Where(c => c.IsPrimaryKey).ToList();
            }
        }
        /// <summary>
        /// 真实数据库列
        /// </summary>
        public List<IColumn> DbColumns
        {
            get
            {
                return Columns.Where(c => !c.IsAliasColumn).ToList();
            }
        }

        /// <summary>
        /// Gets a column with given name.
        /// </summary>
        /// <param name="name">name of a column</param>
        /// <returns>column with given name</returns>
        public IColumn GetColumn(string name)
        {
            foreach (IColumn column in this.columns)
            {
                if (column.ColumnName.Equals(name))
                    return column;
            }
            return null;
        }
    }
}
