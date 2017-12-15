using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

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
      Dictionary<int, PropertyInfo> props = GetDataReaderColumns<T>(reader);
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
      Dictionary<int, PropertyInfo> props = GetDataReaderColumns<T>(reader);
      reader.Read();
      return MapNextRow<T>(reader, props);
    }

    private static T MapNextRow<T>(IDataReader reader, Dictionary<int, PropertyInfo> props)
    {
      T row = Activator.CreateInstance<T>();
      for (int i = 0; i < reader.FieldCount; i++)
      {
        if (props.TryGetValue(i, out PropertyInfo prop))
        {
          object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
          prop.SetValue(row, value);
        }
      }
      return row;
    }

    private static Dictionary<int, PropertyInfo> GetDataReaderColumns<T>(IDataReader reader)
    {
      var res = new Dictionary<int, PropertyInfo>(reader.FieldCount);
      Type modelType = typeof(T);
      for (int i = 0; i < reader.FieldCount; i++)
      {
        string name = reader.GetName(i);
        string nameNoUnderscore = Regex.Replace(name, "[_-]", "");
        PropertyInfo prop = modelType.GetProperty(nameNoUnderscore, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop != null)
        {
          res[i] = prop;
        }
      }
      return res;
    }
  }
}
