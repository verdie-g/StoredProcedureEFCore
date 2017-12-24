using Microsoft.EntityFrameworkCore;

namespace StoredProcedureEFCore.Tests.PgSql
{
  public partial class TestContext : DbContext
  {
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      if (!optionsBuilder.IsConfigured)
      {
        optionsBuilder.UseNpgsql("Host=localhost;Database=test;Username=postgres;Password=root");
      }
    }
  }
}
