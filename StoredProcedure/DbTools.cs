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
    /// Call a stored procedure
    /// </summary>
    /// <typeparam name="T">Type of the result object</typeparam>
    /// <param name="context"></param>
    /// <param name="name">Procedure's name</param>
    /// <param name="parameters">Procedure's parameters</param>
    /// <returns></returns>
    public static List<T> Exec<T>(this DbContext context, string name, params (string, object)[] parameters)
    {
      using (IDataReader reader = CreateDbCommand(context, name, parameters).ExecuteReader())
      {
        return reader.ToList<T>();
      }
    }

    /// <summary>
    /// Call a stored procedure that only return a boolean
    /// </summary>
    /// <param name="context"></param>
    /// <param name="name">Procedure's name</param>
    /// <param name="parameters">Procedure's parameters</param>
    /// <returns></returns>
    public static bool Exec(this DbContext context, string name, params (string, object)[] parameters)
    {
      DbCommand command = CreateDbCommand(context, name, parameters);
      DbParameter returnParameter = command.CreateParameter();
      returnParameter.ParameterName = "@out";
      returnParameter.DbType = DbType.Boolean;
      returnParameter.Direction = ParameterDirection.ReturnValue;
      command.Parameters.Add(returnParameter);
      command.ExecuteNonQuery();
      return Convert.ToBoolean(returnParameter.Value);
    }

    private static DbCommand CreateDbCommand(DbContext context, string name, params (string name, object value)[] parameters)
    {
      DbCommand command = context.Database.GetDbConnection().CreateCommand();
      command.CommandType = CommandType.StoredProcedure;
      command.CommandText = name;
      foreach (var parameter in parameters)
      {
        DbParameter param = command.CreateParameter();
        param.ParameterName = '@' + parameter.name;
        param.Value = parameter.value;
        command.Parameters.Add(param);
      }
      context.Database.OpenConnection();
      return command;
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
        res.Add(row);
      }
      return res;
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
