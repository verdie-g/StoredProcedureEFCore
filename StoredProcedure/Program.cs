using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace StoredProcedure
{
  public class Program
  {
    public static void Main(string[] args)
    {
      int count = 500;
      var da = new DataAccessBase();

      List<Dbo.ResultProc> rows = da.ListRowsFromTable1(count).ToList();
      Debug.Assert(rows.Count == count);
      Debug.Assert(rows[0].ExtraProperty == null);
      Debug.Assert(rows[0].Name != null);
      Debug.Assert(rows[0].Date != null);
      Debug.Assert(da.IsSomething(true));
      Debug.Assert(!da.IsSomething(false));

      long l = da.FirstId();
      Debug.Assert(l != 0);
    }
  }
}
