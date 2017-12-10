using System.Collections.Generic;

namespace StoredProcedure
{
  public class DataAccessBase
  {
    /// <summary>
    /// Stored procedure that list, with a limit, the Table1's rows
    /// </summary>
    /// <param name="limit">Rows limit</param>
    /// <returns></returns>
    public List<Dbo.ResultProc> ListRowsFromTable1(long limit)
    {
      using (DataAccess.TestContext ctx = new DataAccess.TestContext())
      {
        return ctx.Exec<Dbo.ResultProc>("[dbo].[ListAll]", ("limit", limit));
      }
    }

    public long FirstId()
    {
      using (DataAccess.TestContext ctx = new DataAccess.TestContext())
      {
        return ctx.ExecScalar<long>("[dbo].[ListAll]");
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
        return ctx.Exec("[dbo].[ReturnBoolean]", ("boolean_to_return", boolToReturn));
      }
    }
  }
}
