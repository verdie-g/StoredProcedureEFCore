using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace StoredProcedure.Extensions
{
  public static class DbContextExtension
  {
    /// <summary>
    /// Execute a stored procedure
    /// </summary>
    /// <typeparam name="T">Type of the result object</typeparam>
    /// <param name="ctx"></param>
    /// <param name="name">Procedure's name</param>
    /// <param name="parameters">Procedure's parameters</param>
    /// <returns></returns>
    public static List<T> Exec<T>(this DbContext ctx, string name, params (string, object)[] parameters) where T : class
    {
      using (DbCommand cmd = CreateDbCommand(ctx, name, parameters))
      {
        using (IDataReader reader = cmd.ExecuteReader())
        {
          return reader.ToList<T>();
        }
      }
    }

    /// <summary>
    /// Execute a stored procedure and return the first row of the first column
    /// </summary>
    /// <typeparam name="T">Type of the result object</typeparam>
    /// <param name="ctx"></param>
    /// <param name="name">Procedure's name</param>
    /// <param name="parameters">Procedure's parameters</param>
    /// <returns></returns>
    public static T ExecScalar<T>(this DbContext ctx, string name, params (string, object)[] parameters) where T : IComparable
    {
      using (DbCommand cmd = CreateDbCommand(ctx, name, parameters))
      {
        return (T)cmd.ExecuteScalar();
      }
    }

    /// <summary>
    /// Execute a stored procedure and return the first row
    /// </summary>
    /// <typeparam name="T">Type of the result object</typeparam>
    /// <param name="ctx"></param>
    /// <param name="name">Procedure's name</param>
    /// <param name="parameters">Procedure's parameters</param>
    /// <returns></returns>
    public static T ExecFirst<T>(this DbContext ctx, string name, params (string, object)[] parameters) where T : class
    {
      using (DbCommand cmd = CreateDbCommand(ctx, name, parameters))
      {
        using (IDataReader reader = cmd.ExecuteReader())
        {
          return reader.First<T>();
        }
      }
    }


    /// <summary>
    /// Execute a stored procedure and return the first row or default value if the result set is empty
    /// </summary>
    /// <typeparam name="T">Type of the result object</typeparam>
    /// <param name="ctx"></param>
    /// <param name="name">Procedure's name</param>
    /// <param name="parameters">Procedure's parameters</param>
    /// <returns></returns>
    public static T ExecFirstOrDefault<T>(this DbContext ctx, string name, params (string, object)[] parameters) where T : class
    {
      using (DbCommand cmd = CreateDbCommand(ctx, name, parameters))
      {
        using (IDataReader reader = cmd.ExecuteReader())
        {
          return reader.FirstOrDefault<T>();
        }
      }
    }

    /// <summary>
    /// Execute a stored procedure and return the first row or throw an exception if result set contains more than one row
    /// </summary>
    /// <typeparam name="T">Type of the result object</typeparam>
    /// <param name="ctx"></param>
    /// <param name="name">Procedure's name</param>
    /// <param name="parameters">Procedure's parameters</param>
    /// <returns></returns>
    public static T ExecSingle<T>(this DbContext ctx, string name, params (string, object)[] parameters) where T : class
    {
      using (DbCommand cmd = CreateDbCommand(ctx, name, parameters))
      {
        using (IDataReader reader = cmd.ExecuteReader())
        {
          return reader.Single<T>();
        }
      }
    }

    /// <summary>
    /// Execute a stored procedure that only return a boolean
    /// </summary>
    /// <param name="ctx"></param>
    /// <param name="name">Procedure's name</param>
    /// <param name="parameters">Procedure's parameters</param>
    /// <returns></returns>
    public static bool Exec(this DbContext ctx, string name, params (string, object)[] parameters)
    {
      using (DbCommand cmd = CreateDbCommand(ctx, name, parameters))
      {
        DbParameter returnParameter = cmd.CreateParameter();
        returnParameter.ParameterName = "@out";
        returnParameter.DbType = DbType.Boolean;
        returnParameter.Direction = ParameterDirection.ReturnValue;
        cmd.Parameters.Add(returnParameter);
        cmd.ExecuteNonQuery();
        return Convert.ToBoolean(returnParameter.Value);
      }
    }

    private static DbCommand CreateDbCommand(DbContext ctx, string name, params (string name, object value)[] parameters)
    {
      DbCommand cmd = ctx.Database.GetDbConnection().CreateCommand();
      cmd.CommandType = CommandType.StoredProcedure;
      cmd.CommandText = name;
      foreach (var parameter in parameters)
      {
        DbParameter param = cmd.CreateParameter();
        param.ParameterName = '@' + parameter.name;
        param.Value = parameter.value;
        cmd.Parameters.Add(param);
      }
      ctx.Database.OpenConnection();
      return cmd;
    }
  }
}
