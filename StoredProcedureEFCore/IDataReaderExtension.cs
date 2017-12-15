using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text.RegularExpressions;

namespace StoredProcedureEFCore
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
    /// Map first column to a list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static List<T> Column<T>(this IDataReader reader) where T : IComparable
    {
      var res = new List<T>();
      while (reader.Read())
      {
        T value = reader.IsDBNull(0) ? default(T) : (T)reader.GetValue(0);
        res.Add(value);
      }
      return res;
    }

    /// <summary>
    /// Create a dictionary using the first column as a key. Keys must be unique
    /// </summary>
    /// <typeparam name="TKey">Type of the keys</typeparam>
    /// <typeparam name="TValue">Type of the values</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IDataReader reader) where TKey : IComparable where TValue : class
    {
      Dictionary<int, PropertyInfo> props = GetDataReaderColumns<TValue>(reader);

      var res = new Dictionary<TKey, TValue>();
      while (reader.Read())
      {
        TKey key = (TKey)reader.GetValue(0);
        TValue val = MapNextRow<TValue>(reader, props, 1);
        SetPropertyValue(reader, props, val, 0);
        res[key] = val;
      }
      return res;
    }

    /// <summary>
    /// Create a dictionary using the first column as a key
    /// </summary>
    /// <typeparam name="TKey">Type of the keys</typeparam>
    /// <typeparam name="TValue">Type of the values</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static Dictionary<TKey, List<TValue>> ToLookup<TKey, TValue>(this IDataReader reader) where TKey : IComparable where TValue : class
    {
      Dictionary<int, PropertyInfo> props = GetDataReaderColumns<TValue>(reader);

      var res = new Dictionary<TKey, List<TValue>>();
      while (reader.Read())
      {
        TKey key = (TKey)reader.GetValue(0);
        TValue val = MapNextRow<TValue>(reader, props, 1);
        SetPropertyValue(reader, props, val, 0);

        if (res.ContainsKey(key))
        {
          res[key].Add(val);
        }
        else
        {
          res[key] = new List<TValue>() { val };
        }
      }
      return res;
    }

    /// <summary>
    /// Create a set with the first column
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static HashSet<T> ToSet<T>(this IDataReader reader) where T : IComparable
    {
      Dictionary<int, PropertyInfo> props = GetDataReaderColumns<T>(reader);

      var res = new HashSet<T>();
      while (reader.Read())
      {
        T val = (T)reader.GetValue(0);
        res.Add(val);
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

    private static T MapNextRow<T>(IDataReader reader, Dictionary<int, PropertyInfo> props, int columnOffset = 0) where T : class
    {
      T row = Activator.CreateInstance<T>();
      for (int i = columnOffset; i < reader.FieldCount; i++)
      {
        SetPropertyValue(reader, props, row, i);
      }
      return row;
    }

    private static bool SetPropertyValue<T>(IDataReader reader, Dictionary<int, PropertyInfo> props, T row, int i)
    {
      if (props.TryGetValue(i, out PropertyInfo prop))
      {
        object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
        prop.SetValue(row, value);
        return true;
      }
      return false;
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
