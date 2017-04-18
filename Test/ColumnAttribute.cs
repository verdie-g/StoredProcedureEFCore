using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    /// <summary>
    /// Specify the column name in the database of the property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class ColumnAttribute : Attribute
    {
        public string ColumnName { get; private set; }
        public ColumnAttribute(string columnName)
        {
            ColumnName = columnName;
        }
    }
}
