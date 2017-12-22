using System;

namespace StoredProcedureEFCore.Tests.SqlServer
{
  internal class Model
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public bool Active { get; set; }
    public TestEnum NameWithUnderscore { get; set; }
  }

  enum TestEnum
  {
    Yes = 0,
    No = 1
  }
}
