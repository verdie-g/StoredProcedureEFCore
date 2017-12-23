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

      List<Model> rows = null;

      ctx.LoadStoredProc("dbo.ListAll")
         .AddParam("limit", 300)
         .AddOutputParam("limitOut", out IOutputParam<long> limitOut)
         .Exec(r => rows = r.ToList<Model>());

      long limitOutValue = limitOut.Value;

      ctx.LoadStoredProc("dbo.ReturnBoolean")
         .AddParam("boolean_to_return", true)
         .ReturnValue(out IOutputParam<bool> retParam)
         .ExecNonQuery();

      bool b = retParam.Value;

      ctx.LoadStoredProc("dbo.ListAll")
         .AddParam("limit", 1)
         .ExecScalar(out long l);

      Console.WriteLine(l);
    }
  }
}
