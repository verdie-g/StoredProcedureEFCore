using System.Collections.Generic;

namespace StoredProcedure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var da = new DataAccessBase();
            IEnumerable<Dbo.ResultProc> rows = da.ListRowsFromTable1(500);
            bool isSomething = da.IsSomething(true); 
        }
    }
}
