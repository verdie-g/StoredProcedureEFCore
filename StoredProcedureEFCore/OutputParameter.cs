using System;
using System.Data.Common;

namespace StoredProcedureEFCore
{
    internal class OutputParam<T> : IOutParam<T>
    {
        public OutputParam(DbParameter param)
        {
            _dbParam = param;
        }

        public T Value
        {
            get
            {
                if (_dbParam.Value is System.DBNull)
                {
                    if (Nullable.GetUnderlyingType(typeof(T)) != null)
                    {
                        return default(T);
                    }
                }

                return (T)Convert.ChangeType(_dbParam.Value, typeof(T));
            }
        }

        public override string ToString() => _dbParam.Value.ToString();

        private DbParameter _dbParam;
    }
}
