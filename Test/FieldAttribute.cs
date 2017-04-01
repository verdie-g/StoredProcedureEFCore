using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    /// <summary>
    /// Specify the name of the field in the database
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class FieldAttribute : Attribute
    {
        public string DbName { get; private set; }
        public FieldAttribute(string dbName)
        {
            DbName = dbName;
        }
    }
}
