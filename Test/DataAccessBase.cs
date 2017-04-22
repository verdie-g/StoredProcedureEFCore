using System;
using System.Collections.Generic;

namespace Test
{
    public class DataAccessBase
    {
        public IEnumerable<Dbo.ResultProc> ListRowsFromTable1(long limit)
        {
            using (DataAccess.TestContext ctx = new DataAccess.TestContext())
            {
                return ctx.ExecuteStoredProcedure<Dbo.ResultProc>("[dbo].[ListAll]", new StoredProcedureParameter("limit", limit));
            }
        }
    }
}
