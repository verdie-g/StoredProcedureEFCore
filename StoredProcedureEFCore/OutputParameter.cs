using System;
using System.Data.Common;

namespace StoredProcedureEFCore
{
  internal class OutputParam<T> : IOutputParam<T>
  {
    public OutputParam(DbParameter param)
    {
      _dbParam = param;
    }

    public T Value => (T)Convert.ChangeType(_dbParam.Value, typeof(T));

    public override string ToString() => _dbParam.Value.ToString();

    private DbParameter _dbParam;
  }
}
