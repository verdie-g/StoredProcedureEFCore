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
        .ExecAsync(async r => rows = await r.ToListAsync<Model>());

      long limitOutValue = limitOut.Value;

      List<Model2> rows2 = null;

      // EXEC dbo.ListNotAll @limit = 200
      await ctx.LoadStoredProc("dbo.ListNotAll")
        .AddParam("limit", 200L)
        .ExecAsync(async r => rows2 = await r.ToListAsync<Model2>());

      // EXEC dbo.ListNotAll @limit = 400
      await ctx.LoadStoredProc("dbo.ListNotAll")
        .AddParam("limit", 400L)
        .ExecAsync(async r => rows2 = await r.ToListAsync<Model2>());

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

      // EXEC dbo.SelectParam @n = NULL
      await ctx.LoadStoredProc("dbo.SelectParam")
        .AddParam<int?>("n", null)
        .ExecScalarAsync<int?>(i => Console.WriteLine(i));

      await ctx.LoadStoredProc("dbo.OutputFixedSize")
        .AddParam("fixed_size", out IOutParam<string> fixedSizeParam, new ParamExtra { Size = 255 })
        .ExecNonQueryAsync();

      string s = fixedSizeParam.Value;
    }
  }
}
