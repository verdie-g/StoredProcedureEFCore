using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StoredProcedureEFCore.Tests.SqlServer
{
  class Program
  {
    static async Task Main(string[] args)
    {
      DbContext ctx = new TestContext();

      List<Model> rows = null;

      // EXEC dbo.ListAll @limit = 300, @limitOut OUT
      await ctx.LoadStoredProc("dbo.ListAll")
        .AddParam("limit", 300L)
        .AddParam("limitOut", out IOutParam<long> limitOut)
        .ExecAsync(r => rows = r.ToList<Model>());

      long limitOutValue = limitOut.Value;

      // EXEC @_retParam = dbo.ReturnBoolean @boolean_to_return = true
      await ctx.LoadStoredProc("dbo.ReturnBoolean")
         .AddParam("boolean_to_return", true)
         .ReturnValue(out IOutParam<bool> retParam)
         .ExecNonQueryAsync();

      bool b = retParam.Value;

      // EXEC dbo.ListAll @limit = 1
      await ctx.LoadStoredProc("dbo.ListAll")
         .AddParam("limit", 1L)
         .ExecScalarAsync<long>(l => Console.WriteLine(l));

      // Limit is omitted, it takes default value specified in the stored procedure
      ctx.LoadStoredProc("dbo.ListAll")
         .Exec(r => rows = r.ToList<Model>());
    }
  }
}
