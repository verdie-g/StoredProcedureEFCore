using System;
using System.Data.Common;

namespace StoredProcedureEFCore
{
  internal class ReturnParameter<T> : IReturnParameter<T>
  {
    public ReturnParameter(DbParameter param)
    {
      _dbParam = param;
    }

    public T Value => (T)Convert.ChangeType(_dbParam.Value, typeof(T));

    private DbParameter _dbParam;
  }
}
