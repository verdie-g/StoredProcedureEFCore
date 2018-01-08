using Microsoft.EntityFrameworkCore;
using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace StoredProcedureEFCore
{
  internal class StoredProcBuilder : IStoredProcBuilder
  {
    private DbCommand _cmd;

    public StoredProcBuilder(DbContext ctx, string name)
    {
      if (name is null)
        throw new ArgumentNullException(nameof(name));

      DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand();
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.CommandText = name;

      _cmd = cmd;
    }

    public IStoredProcBuilder AddParam<T>(string name, T val)
    {
      AddParamInner<T>(name, val, ParameterDirection.Input);
      return this;
    }

    public IStoredProcBuilder AddParam<T>(string name, out IOutParam<T> outParam)
    {
      outParam = AddOutputParamInner<T>(name, default(T), ParameterDirection.Output);
      return this;
    }

    public IStoredProcBuilder AddParam<T>(string name, T val, out IOutParam<T> outParam)
    {
      outParam = AddOutputParamInner<T>(name, val, ParameterDirection.InputOutput);
      return this;
    }

    public IStoredProcBuilder ReturnValue<T>(out IOutParam<T> retParam)
    {
      retParam = AddOutputParamInner<T>("_retParam", default(T), ParameterDirection.ReturnValue);
      return this;
    }

    public void Exec(Action<DbDataReader> action)
    {
      if (action is null)
        throw new ArgumentNullException(nameof(action));

      try
      {
        OpenConnection();
        using (DbDataReader r = _cmd.ExecuteReader())
        {
          action(r);
        }
      }
      finally
      {
        Dispose();
      }
    }

    public async Task ExecAsync(Func<DbDataReader, Task> action)
    {
      if (action is null)
        throw new ArgumentNullException(nameof(action));

      try
      {
        await OpenConnectionAsync();
        using (DbDataReader r = await _cmd.ExecuteReaderAsync())
        {
          await action(r);
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
        OpenConnection();
        _cmd.ExecuteNonQuery();
      }
      finally
      {
        Dispose();
      }
    }

    public async Task ExecNonQueryAsync()
    {
      try
      {
        await OpenConnectionAsync();
        await _cmd.ExecuteNonQueryAsync();
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
        OpenConnection();
        val = (T)_cmd.ExecuteScalar();
      }
      finally
      {
        Dispose();
      }
    }

    public async Task ExecScalarAsync<T>(Action<T> action)
    {
      try
      {
        await OpenConnectionAsync();
        T val = (T)await _cmd.ExecuteScalarAsync();
        action(val);
      }
      finally
      {
        Dispose();
      }
    }

    public void Dispose()
    {
      _cmd.Connection.Close();
      _cmd.Dispose();
    }

    private OutputParam<T> AddOutputParamInner<T>(string name, T val, ParameterDirection direction)
    {
      DbParameter param = AddParamInner(name, val, direction);
      return new OutputParam<T>(param);
    }

    private DbParameter AddParamInner<T>(string name, T val, ParameterDirection direction)
    {
      if (name is null)
        throw new ArgumentNullException(nameof(name));

      DbParameter param = _cmd.CreateParameter();
      param.ParameterName = name;
      param.Value = val;
      param.Direction = direction;
      param.DbType = DbTypeConverter.ConvertToDbType<T>();

      _cmd.Parameters.Add(param);
      return param;
    }

    private void OpenConnection()
    {
      _cmd.Connection.Open();
    }

    private async Task OpenConnectionAsync()
    {
      await _cmd.Connection.OpenAsync();
    }
  }
}
