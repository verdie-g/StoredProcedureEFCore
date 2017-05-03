using System;

namespace Test.Dbo
{
    public class ResultProc
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public bool Active { get; set; }
        public int NameWithUnderscore { get; set; }
        public string ExtraProperty { get; set; }
    }
}
