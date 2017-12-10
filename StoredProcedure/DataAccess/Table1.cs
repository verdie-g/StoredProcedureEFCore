using System;
using System.Collections.Generic;

namespace StoredProcedure.DataAccess
{
  public partial class Table1
  {
    public long Id { get; set; }
    public string Name { get; set; }
    public DateTime Date { get; set; }
    public bool Active { get; set; }
    public int NameWithUnderscore { get; set; }
  }
}
