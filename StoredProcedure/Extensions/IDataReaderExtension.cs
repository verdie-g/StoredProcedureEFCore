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
    public static List<T> ToList<T>(this IDataReader reader) where T : class
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
    public static T First<T>(this IDataReader reader) where T : class
    {
      return First<T>(reader, false, false);
    }

    /// <summary>
    /// Map reader's first row to a model or return default value if the result set is empty
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static T FirstOrDefault<T>(this IDataReader reader) where T : class
    {
      return First<T>(reader, true, false);
    }

    /// <summary>
    /// Map reader's first row to a model or throw an exception if result set contains more than one row
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static T Single<T>(this IDataReader reader) where T : class
    {
      return First<T>(reader, false, true);
    }

    private static T First<T>(IDataReader reader, bool orDefault, bool throwIfNotSingle) where T : class
    {
      if (orDefault && throwIfNotSingle)
        throw new ArgumentException("orDefault and throwIfNotSingle booleans can't be both true.");

      if (reader.Read())
      {
        Dictionary<int, PropertyInfo> props = GetDataReaderColumns<T>(reader);
        T row = MapNextRow<T>(reader, props);

        if (throwIfNotSingle && reader.Read())
          throw new Exception("Result set contains more than one row.");

        return row;
      }
      else
      {
        if (orDefault)
          return default(T);

        throw new Exception("Empty result set.");
      }
    }

    private static T MapNextRow<T>(IDataReader reader, Dictionary<int, PropertyInfo> props) where T : class
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
