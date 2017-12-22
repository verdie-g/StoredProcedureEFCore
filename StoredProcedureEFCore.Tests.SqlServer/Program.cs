using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace StoredProcedureEFCore.Tests.SqlServer
{
  class Program
  {
    static void Main(string[] args)
    {
      DbContext ctx = new TestContext();

      List<Table1> rows = null;

      ctx.LoadStoredProc("dbo.ListAll")
         .AddParam("limit", 100)
         .Exec(r => rows = r.ToList<Table1>());

      ctx.LoadStoredProc("dbo.ReturnBoolean")
         .AddParam("boolean_to_return", true)
         .ReturnValue(out IReturnParameter<bool> retParam)
         .ExecNonQuery();

      bool b = retParam.Value;

      ctx.LoadStoredProc("dbo.ListAll")
         .AddParam("limit", 1)
         .ExecScalar(out long l);

      Console.WriteLine(l);
    }
  }
}
