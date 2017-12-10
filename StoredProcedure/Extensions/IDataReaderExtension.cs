using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace StoredProcedure.Extensions
{
  public static class IDataReaderExtension
  {
    /// <summary>
    /// Map reader to a model list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(this IDataReader reader)
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

    /// <summary>
    /// Map reader's first row to a model
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static T First<T>(this IDataReader reader)
    {
      Dictionary<string, PropertyInfo> props = GetDataReaderColumns<T>(reader);
      reader.Read();
      return MapNextRow<T>(reader, props);
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
