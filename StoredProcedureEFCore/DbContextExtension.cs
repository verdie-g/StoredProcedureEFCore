using Microsoft.EntityFrameworkCore;

namespace StoredProcedureEFCore
{
  public static class DbContextExtension
  {
    /// <summary>
    /// Load a stored procedure
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="name">Procedure's name</param>
    /// <returns></returns>
    public static IStoredProcBuilder LoadStoredProc(this DbContext ctx, string name)
    {
      return new StoredProcBuilder(ctx, name);
    }
  }
}
