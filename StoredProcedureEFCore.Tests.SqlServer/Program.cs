using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace StoredProcedureEFCore.Tests.SqlServer
{
  class Program
  {
    static void Main(string[] args)
    {
      DbContext ctx = new TestContext();
      List<Table1> rows = ctx.Exec<Table1>("dbo.ListAll", ("limit", 100));
      Debug.Assert(rows.Count == 100);
      Debug.Assert(rows[0].Date != default(DateTime));
    }
  }
}
