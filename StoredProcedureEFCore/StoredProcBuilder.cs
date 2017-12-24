using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;

namespace StoredProcedureEFCore
{
  internal class StoredProcBuilder : IStoredProcBuilder
  {
    private DbCommand _cmd;

    public StoredProcBuilder(DbContext ctx, string name)
    {
      DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand();
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.CommandText = name;
      ctx.Database.OpenConnection();

      _cmd = cmd;
    }

    public IStoredProcBuilder AddParam(string name, object val)
    {
      AddParamInner(name, val);
      return this;
    }

    public IStoredProcBuilder AddOutputParam<T>(string name, out IOutParam<T> outParam)
    {
      outParam = AddOutputParamInner<T>(name, null, ParameterDirection.Output);
      return this;
    }

    public IStoredProcBuilder AddInputOutputParam<T>(string name, T val, out IOutParam<T> outParam)
    {
      outParam = AddOutputParamInner<T>(name, val, ParameterDirection.InputOutput);
      return this;
    }

    public IStoredProcBuilder ReturnValue<T>(out IOutParam<T> retParam)
    {
      retParam = AddOutputParamInner<T>("_retParam", null, ParameterDirection.ReturnValue);
      return this;
    }

    public void Exec(Action<IDataReader> action)
    {
      try
      {
        using (IDataReader r = _cmd.ExecuteReader())
        {
          action(r);
        }
      }
      finally
      {
        Dispose();
      }
    }

    public void ExecNonQuery()
    {
      try
      {
        _cmd.ExecuteNonQuery();
      }
      finally
      {
        Dispose();
      }
    }

    public void ExecScalar<T>(out T val)
    {
      try
      {
        val = (T)_cmd.ExecuteScalar();
      }
      finally
      {
        Dispose();
      }
    }

    public void Dispose()
    {
      _cmd.Dispose();
    }

    private OutputParam<T> AddOutputParamInner<T>(string name, object val, ParameterDirection direction)
    {
      DbParameter param = AddParamInner(name, val, p =>
      {
        p.Direction = direction;
        p.DbType = DbTypeConverter.ConvertToDbType<T>();
      });

      return new OutputParam<T>(param);
    }

    private DbParameter AddParamInner(string name, object val = null, Action<DbParameter> action = null)
    {
      DbParameter param = _cmd.CreateParameter();
      param.ParameterName = '@' + name;
      param.Value = val;
      action?.Invoke(param);
      _cmd.Parameters.Add(param);
      return param;
    }
  }
}
