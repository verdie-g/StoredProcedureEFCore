using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace StoredProcedure
{
  public static class DbTools
  {
    /// <summary>
    /// Execute a stored procedure
    /// </summary>
    /// <typeparam name="T">Type of the result object</typeparam>
    /// <param name="ctx"></param>
    /// <param name="name">Procedure's name</param>
    /// <param name="parameters">Procedure's parameters</param>
    /// <returns></returns>
    public static List<T> Exec<T>(this DbContext ctx, string name, params (string, object)[] parameters)
    {
      using (IDataReader reader = CreateDbCommand(ctx, name, parameters).ExecuteReader())
      {
        return reader.ToList<T>();
      }
    }

    /// <summary>
    /// Execute a stored procedure and the first row of the first column
    /// </summary>
    /// <typeparam name="T">Type of the result object</typeparam>
    /// <param name="ctx"></param>
    /// <param name="name">Procedure's name</param>
    /// <param name="parameters">Procedure's parameters</param>
    /// <returns></returns>
    public static T ExecScalar<T>(this DbContext ctx, string name, params (string, object)[] parameters)
    {
      DbCommand cmd = CreateDbCommand(ctx, name, parameters);
      return (T)cmd.ExecuteScalar();
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
      DbCommand cmd = CreateDbCommand(ctx, name, parameters);
      DbParameter returnParameter = cmd.CreateParameter();
      returnParameter.ParameterName = "@out";
      returnParameter.DbType = DbType.Boolean;
      returnParameter.Direction = ParameterDirection.ReturnValue;
      cmd.Parameters.Add(returnParameter);
      cmd.ExecuteNonQuery();
      return Convert.ToBoolean(returnParameter.Value);
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

    /// <summary>
    /// Map reader to a model list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    private static List<T> ToList<T>(this IDataReader reader)
    {
      var res = new List<T>();
      Dictionary<string, PropertyInfo> props = GetDataReaderColumns<T>(reader);
      while (reader.Read())
      {
        T row = MapNextRow<T>(reader, props);
        res.Add(row);
      }
      return res;
    }

    private static T MapNextRow<T>(IDataReader reader, Dictionary<string, PropertyInfo> props)
    {
      T row = Activator.CreateInstance<T>();
      for (int i = 0; i < reader.FieldCount; i++)
      {
        string name = reader.GetName(i);
        if (props.TryGetValue(name, out PropertyInfo prop))
        {
          object value = reader.GetValue(i);
          prop.SetValue(row, value == DBNull.Value ? null : value);
        }
      }
      return row;
    }

    private static Dictionary<string, PropertyInfo> GetDataReaderColumns<T>(IDataReader reader)
    {
      var res = new Dictionary<string, PropertyInfo>(reader.FieldCount);
      Type modelType = typeof(T);
      for (int i = 0; i < reader.FieldCount; i++)
      {
        string name = reader.GetName(i);
        string nameNoUnderscore = name.Replace("_", "").Replace("-", "");
        PropertyInfo prop = modelType.GetProperty(nameNoUnderscore, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop != null)
        {
          res[name] = prop;
        }
      }
      return res;
    }

  }
}
