using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace StoredProcedureEFCore
{
    internal class StoredProcBuilder : IStoredProcBuilder
    {
        private const string RetParamName = "_retParam";
        private readonly DbCommand _cmd;

        public StoredProcBuilder(DbContext ctx, string name)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand();
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandText = name;
            cmd.Transaction = ctx.Database.CurrentTransaction?.GetDbTransaction();
            cmd.CommandTimeout = ctx.Database.GetCommandTimeout().GetValueOrDefault(cmd.CommandTimeout);

            _cmd = cmd;
        }

        public IStoredProcBuilder AddParam<T>(string name, T val)
        {
            AddParamInner(name, val, ParameterDirection.Input);
            return this;
        }

        public IStoredProcBuilder AddParam<T>(string name, out IOutParam<T> outParam)
        {
            outParam = AddOutputParamInner(name, default(T), ParameterDirection.Output);
            return this;
        }

        public IStoredProcBuilder AddParam<T>(string name, T val, out IOutParam<T> outParam, int size = 0, byte precision = 0, byte scale = 0)
        {
            outParam = AddOutputParamInner(name, val, ParameterDirection.InputOutput, size, precision, scale);
            return this;
        }

        public IStoredProcBuilder AddParam<T>(string name, out IOutParam<T> outParam, int size = 0, byte precision = 0, byte scale = 0)
        {
            outParam = AddOutputParamInner(name, default(T), ParameterDirection.Output, size, precision, scale);
            return this;
        }

        public IStoredProcBuilder AddParam<T>(string name, T val, out IOutParam<T> outParam)
        {
            outParam = AddOutputParamInner(name, val, ParameterDirection.InputOutput);
            return this;
        }

        public IStoredProcBuilder AddParam(DbParameter parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            _cmd.Parameters.Add(parameter);
            return this;
        }

        public IStoredProcBuilder ReturnValue<T>(out IOutParam<T> retParam)
        {
            retParam = AddOutputParamInner(RetParamName, default(T), ParameterDirection.ReturnValue);
            return this;
        }

        public IStoredProcBuilder SetTimeout(int timeout)
        {
            _cmd.CommandTimeout = timeout;
            return this;
        }

        public void Exec(Action<DbDataReader> action)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            bool ownsConnection = false;
            try
            {
                ownsConnection = OpenConnection();
                using (DbDataReader r = _cmd.ExecuteReader())
                {
                    action(r);
                }
            }
            finally
            {
                if (ownsConnection)
                    CloseConnection();
                Dispose();
            }
        }

        public Task ExecAsync(Func<DbDataReader, Task> action)
        {
            return ExecAsync(action, CancellationToken.None);
        }

        public async Task ExecAsync(Func<DbDataReader, Task> action, CancellationToken cancellationToken)
        {
            if (action is null)
                throw new ArgumentNullException(nameof(action));

            bool ownsConnection = false;
            try
            {
                ownsConnection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                using (DbDataReader r = await _cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    try
                    {
                        await action(r).ConfigureAwait(false);
                    }
                    catch (Exception)
                    {
                        // In case the action bombs out, cancel the command and rethrow to propagate the actual action
                        // exception. If we don't cancel the command, we will be stuck on disposing of the reader until
                        // the sproc completes, even though the action has already thrown an exception. This is also the
                        // case when the cancellation token is cancelled after the action exception but before the sproc
                        // completes: we will still be stuck on disposing of the reader until the sproc completes. This
                        // is caused by the fact that DbDataReader.Dispose does not react to cancellations and simply
                        // waits for the sproc to complete. // The only way to cancel the execution when the reader has
                        // been engaged and the action has thrown, is to cancel the command.
                        _cmd.Cancel();
                        throw;
                    }
                }
            }
            finally
            {
                if (ownsConnection)
                    CloseConnection();
                Dispose();
            }
        }

        public int ExecNonQuery()
        {
            bool ownsConnection = false;
            try
            {
                ownsConnection = OpenConnection();
                return _cmd.ExecuteNonQuery();
            }
            finally
            {
                if (ownsConnection)
                    CloseConnection();
                Dispose();
            }
        }

        public Task<int> ExecNonQueryAsync()
        {
            return ExecNonQueryAsync(CancellationToken.None);
        }


        public async Task<int> ExecNonQueryAsync(CancellationToken cancellationToken)
        {
            bool ownsConnection = false;
            try
            {
                ownsConnection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                return await _cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (ownsConnection)
                    CloseConnection();
                Dispose();
            }
        }

        public void ExecScalar<T>(out T val)
        {
            bool ownsConnection = false;
            try
            {
                ownsConnection = OpenConnection();
                object scalar = _cmd.ExecuteScalar();
                val = DefaultIfDBNull<T>(scalar);
            }
            finally
            {
                if (ownsConnection)
                    CloseConnection();
                Dispose();
            }
        }

        public Task ExecScalarAsync<T>(Action<T> action)
        {
            return ExecScalarAsync(action, CancellationToken.None);
        }

        public async Task ExecScalarAsync<T>(Action<T> action, CancellationToken cancellationToken)
        {
            bool ownsConnection = false;
            try
            {
                ownsConnection = await OpenConnectionAsync(cancellationToken).ConfigureAwait(false);
                object scalar = await _cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                T val = DefaultIfDBNull<T>(scalar);
                action(val);
            }
            finally
            {
                if (ownsConnection)
                    CloseConnection();
                Dispose();
            }
        }

        public void Dispose()
        {
            _cmd.Dispose();
        }

        private OutputParam<T> AddOutputParamInner<T>(string name, T val, ParameterDirection direction, int size = 0, byte precision = 0, byte scale = 0)
        {
            DbParameter param = AddParamInner(name, val, direction, size, precision, scale);
            return new OutputParam<T>(param);
        }

        private DbParameter AddParamInner<T>(string name, T val, ParameterDirection direction, int size = 0, byte precision = 0, byte scale = 0)
        {
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            DbParameter param = _cmd.CreateParameter();
            param.ParameterName = name;
            param.Value = (object) val ?? DBNull.Value;
            param.Direction = direction;
            param.DbType = DbTypeConverter.ConvertToDbType<T>();
            param.Size = size;
            param.Precision = precision;
            param.Scale = scale;

            _cmd.Parameters.Add(param);
            return param;
        }

        private bool OpenConnection()
        {
            if (_cmd.Connection.State == ConnectionState.Closed)
            {
                _cmd.Connection.Open();

                return true;
            }

            return false;
        }

        private async Task<bool> OpenConnectionAsync(CancellationToken cancellationToken)
        {
            if (_cmd.Connection.State == ConnectionState.Closed)
            {
                await _cmd.Connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                return true;
            }

            return false;
        }

        private void CloseConnection()
        {
            _cmd.Connection.Close();
        }

        private static T DefaultIfDBNull<T>(object o)
        {
            return o == DBNull.Value ? default(T) : (T) o;
        }
    }
}
