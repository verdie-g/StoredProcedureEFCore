using System;
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

      List<Dbo.ResultProc> rows = da.ListRowsFromTable1(count);
      Debug.Assert(rows.Count == count);
      CheckRow(rows[0]);

      Debug.Assert(da.IsSomething(true));
      Debug.Assert(!da.IsSomething(false));

      long l = da.FirstId();
      Debug.Assert(l != 0);

      Dbo.ResultProc first = da.FirstRow(1);
      CheckRow(first);
      first = da.FirstRow(2);
      CheckRow(first);

      try
      {
        da.FirstRow(0);
        Debug.Assert(false);
      }
      catch (Exception)
      {
      }

      first = da.FirstRow(1);
      CheckRow(first);
      first = da.FirstRow(2);
      CheckRow(first);

      Dbo.ResultProc single;

      try
      {
        single = da.SingleRow(0);
        Debug.Assert(false);
      }
      catch (Exception)
      {
      }

      try
      {
        single = da.SingleRow(2);
        Debug.Assert(false);
      }
      catch (Exception)
      {
      }

      single = da.SingleRow(1);
      CheckRow(single);

      List<long> column = da.Column(0);
      Debug.Assert(column.Count == 0);
      column = da.Column(10);
      Debug.Assert(column.Count == 10);
      Debug.Assert(column[0] != 0);

      Dictionary<long, Dbo.ResultProc> dic = da.Dictionary(100);
      Debug.Assert(dic.Count == 100);
      CheckRow(dic.First().Value);
      Debug.Assert(dic.First().Value.Id != 0);
    }

    private static void CheckRow(Dbo.ResultProc row)
    {
      Debug.Assert(row.ExtraProperty == null);
      Debug.Assert(row.Name != null);
      Debug.Assert(row.Date != null);
    }
  }
}
