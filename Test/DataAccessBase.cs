using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Test
{
    public class DataAccessBase
    {

        [Benchmark]
        public void LoadStoredProcedureHardCoded()
        {
            using (DataAccess.TestContext ctx = new DataAccess.TestContext())
            {
                var res = ctx.ExecuteStoredProcedure("[dbo].[ListAll]", r => new Dbo.ResultProc
                {
                    Id = long.Parse(r["id"].ToString()),
                    Name = r["name"].ToString(),
                    Date = Convert.ToDateTime(r["date"].ToString()),
                    Active = Convert.ToBoolean(r["active"].ToString())
                });
            }
        }

        [Benchmark]
        public void LoadStoredProcedureReflective()
        {
            using (DataAccess.TestContext ctx = new DataAccess.TestContext())
            {
                var res = ctx.ExecuteStoredProcedure2<Dbo.ResultProc>("[dbo].[ListAll]");
            }
        }

        [Benchmark]
        public void LoadStoredProcedureReflectiveWithOptimisation()
        {
            using (DataAccess.TestContext ctx = new DataAccess.TestContext())
            {
                var res = ctx.ExecuteStoredProcedure3<Dbo.ResultProc>("[dbo].[ListAll]");
            }
        }
    }
}
