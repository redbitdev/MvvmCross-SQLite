using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cirrious.MvvmCross.Community.Plugins.Sqlite
{
    /// <summary>
    /// Result set contains a List of rows which contains a list of column data
    /// </summary>
    public class SQLiteDataResultSet : List<List<object>>
    {
        /// <summary>
        /// Names of the columns available
        /// </summary>
        private List<string> _columnNames;

        public SQLiteDataResultSet(List<string> columns)
        {
            this.Columns = columns;
        }

        ///// <summary>
        ///// Gets the data of the column
        ///// </summary>
        ///// <param name="columnName">the name of the column</param>
        ///// <returns></returns>
        //public object this[string columnName]
        //{
        //    get
        //    {
        //        var index = _columnNames.FindIndex(x => x.Equals(columnName));
        //        if (index != -1)
        //            return this[index];
        //        else
        //            return null;
        //    }
        //}

        /// <summary>
        /// The columns available in the data results
        /// </summary>
        public List<string> Columns
        {
            get { return _columnNames; }
            private set { _columnNames = value; }
        }

        /// <summary>
        /// The total fields available
        /// </summary>
        public int FieldCount
        {
            get { return Columns.Count; }
        }
    }
}
