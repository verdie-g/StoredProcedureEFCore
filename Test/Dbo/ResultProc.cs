using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test.Dbo
{
    public class ResultProc
    {
        [Column("id")]
        public long Id { get; set; }
        [Column("name")]
        public string Name { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("active")]
        public bool Active { get; set; }
    }
}
