using System;
using System.Collections.Generic;

namespace Test
{
    public class DataAccessBase
    {
        /// <summary>
        /// Stored procedure that list, with a limit, the Table1's rows
        /// </summary>
        /// <param name="limit">Rows limit</param>
        /// <returns></returns>
        public IEnumerable<Dbo.ResultProc> ListRowsFromTable1(long limit)
        {
            using (DataAccess.TestContext ctx = new DataAccess.TestContext())
            {
                return ctx.ExecuteStoredProcedure<Dbo.ResultProc>("[dbo].[ListAll]", new StoredProcedureParameter("limit", limit));
            }
        }

        /// <summary>
        /// Stored procedure that return the parameter
        /// </summary>
        /// <param name="boolToReturn"></param>
        /// <returns></returns>
        public bool IsSomething(bool boolToReturn)
        {
            using (DataAccess.TestContext ctx = new DataAccess.TestContext())
            {
                return ctx.ExecuteStoredProcedure("[dbo].[ReturnBoolean]", new StoredProcedureParameter("boolean_to_return", boolToReturn));
            }
        }
    }
}
