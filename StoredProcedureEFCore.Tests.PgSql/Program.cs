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

      ctx.LoadStoredProc("public.listall")
         .AddParam("llimit", 300L)
         // .AddOutputParam("limitOut", out IOutputParam<long> limitOut)
         .Exec(r => rows = r.ToList<Model>());

      ctx.LoadStoredProc("public.ReturnBoolean")
         .AddParam("boolean_to_return", true)
         .ReturnValue(out IOutParam<bool> retParam)
         .ExecNonQuery();

      bool b = retParam.Value;

      ctx.LoadStoredProc("public.ListAll")
         .AddParam("limit", 1L)
         .ExecScalar(out long l);

      Console.WriteLine(l);
    }
  }
}
