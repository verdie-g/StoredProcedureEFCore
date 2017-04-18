using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public class StoredProcedureParameter
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public StoredProcedureParameter(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}
