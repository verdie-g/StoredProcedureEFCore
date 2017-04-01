using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Dbo
{
    public class ResultProc
    {
        [Field("id")]
        public long Id { get; set; }
        [Field("name")]
        public string Name { get; set; }
        [Field("date")]
        public DateTime Date { get; set; }
        [Field("active")]
        public bool Active { get; set; }
    }
}
