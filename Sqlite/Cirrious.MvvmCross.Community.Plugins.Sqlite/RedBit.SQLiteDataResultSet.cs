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

        /// <summary>
        /// Gets the data value as a string
        /// </summary>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <returns></returns>
        public string GetData(int row, string column)
        {
            var col = this.Columns.IndexOf(column);
            if (col == -1)
                return null;
            else
            {
                var ret = this[row][col];
                if (ret == null)
                    return null;
                else
                    return ret.ToString();
            }
        }

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
