using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace StoredProcedureEFCore.Tests.PgSql
{
  class Program
  {
    static void Main(string[] args)
    {
      DbContext ctx = new TestContext();

      List<Model> rows = null;

      ctx.LoadStoredProc("public.empty").Exec(r => rows = r.ToList<Model>());

      ctx.LoadStoredProc("public.list_all")
        .AddParam("lim", 300L)
        .Exec(r => rows = r.ToList<Model>());

      ctx.LoadStoredProc("public.list_not_all")
        .AddParam("lim", 300L)
        .Exec(r => rows = r.ToList<Model>());

      ctx.LoadStoredProc("public.return_boolean")
         .AddParam("boolean_to_return", true, out IOutParam<bool> retParam)
         .ExecNonQuery();

      Console.WriteLine($"returned boolean: {retParam.Value}");
    }
  }
}
