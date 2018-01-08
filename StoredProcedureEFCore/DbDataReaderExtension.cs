using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StoredProcedureEFCore
{
  public static class DbDataReaderExtension
  {
    /// <summary>
    /// Map reader to a model list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static List<T> ToList<T>(this DbDataReader reader) where T : class, new()
    {
      var res = new List<T>();
      PropertyInfo[] props = GetDataReaderColumns<T>(reader);
      while (reader.Read())
      {
        T row = MapNextRow<T>(reader, props);
        res.Add(row);
      }
      return res;
    }

    /// <summary>
    /// Map reader to a model list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static async Task<List<T>> ToListAsync<T>(this DbDataReader reader) where T : class, new()
    {
      var res = new List<T>();
      PropertyInfo[] props = GetDataReaderColumns<T>(reader);
      while (await reader.ReadAsync())
      {
        T row = await MapNextRowAsync<T>(reader, props);
        res.Add(row);
      }
      return res;
    }

    /// <summary>
    /// Map the first or the specified column to a list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <param name="columnName">Name of the column to read</param>
    /// <returns></returns>
    public static List<T> Column<T>(this DbDataReader reader, string columnName = null) where T : IComparable
    {
      int ord = columnName == null ? 0 : reader.GetOrdinal(columnName);

      var res = new List<T>();
      while (reader.Read())
      {
        T value = reader.IsDBNull(ord) ? default(T) : (T)reader.GetValue(ord);
        res.Add(value);
      }
      return res;
    }

    /// <summary>
    /// Map the first or the specified column to a list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <param name="columnName">Name of the column to read</param>
    /// <returns></returns>
    public static async Task<List<T>> ColumnAsync<T>(this DbDataReader reader, string columnName = null) where T : IComparable
    {
      int ord = columnName == null ? 0 : reader.GetOrdinal(columnName);

      var res = new List<T>();
      while (await reader.ReadAsync())
      {
        T value = await reader.IsDBNullAsync(ord) ? default(T) : (T)reader.GetValue(ord);
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
    public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this DbDataReader reader) where TKey : IComparable where TValue : class, new()
    {
      PropertyInfo[] props = GetDataReaderColumns<TValue>(reader);

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
    /// Create a dictionary using the first column as a key. Keys must be unique
    /// </summary>
    /// <typeparam name="TKey">Type of the keys</typeparam>
    /// <typeparam name="TValue">Type of the values</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static async Task<Dictionary<TKey, TValue>> ToDictionaryAsync<TKey, TValue>(this DbDataReader reader) where TKey : IComparable where TValue : class, new()
    {
      PropertyInfo[] props = GetDataReaderColumns<TValue>(reader);

      var res = new Dictionary<TKey, TValue>();
      while (await reader.ReadAsync())
      {
        TKey key = (TKey)reader.GetValue(0);
        TValue val = await MapNextRowAsync<TValue>(reader, props, 1);
        await SetPropertyValueAsync(reader, props, val, 0);
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
    public static Dictionary<TKey, List<TValue>> ToLookup<TKey, TValue>(this DbDataReader reader) where TKey : IComparable where TValue : class, new()
    {
      PropertyInfo[] props = GetDataReaderColumns<TValue>(reader);

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
    /// Create a dictionary using the first column as a key
    /// </summary>
    /// <typeparam name="TKey">Type of the keys</typeparam>
    /// <typeparam name="TValue">Type of the values</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static async Task<Dictionary<TKey, List<TValue>>> ToLookupAsync<TKey, TValue>(this DbDataReader reader) where TKey : IComparable where TValue : class, new()
    {
      PropertyInfo[] props = GetDataReaderColumns<TValue>(reader);

      var res = new Dictionary<TKey, List<TValue>>();
      while (await reader.ReadAsync())
      {
        TKey key = (TKey)reader.GetValue(0);
        TValue val = await MapNextRowAsync<TValue>(reader, props, 1);
        await SetPropertyValueAsync(reader, props, val, 0);

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
    public static HashSet<T> ToSet<T>(this DbDataReader reader) where T : IComparable
    {
      var res = new HashSet<T>();
      while (reader.Read())
      {
        T val = (T)reader.GetValue(0);
        res.Add(val);
      }
      return res;
    }

    /// <summary>
    /// Create a set with the first column
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static async Task<HashSet<T>> ToSetAsync<T>(this DbDataReader reader) where T : IComparable
    {
      var res = new HashSet<T>();
      while (await reader.ReadAsync())
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
    public static T First<T>(this DbDataReader reader) where T : class, new()
    {
      return First<T>(reader, false, false);
    }

    /// <summary>
    /// Map reader's first row to a model or return default value if the result set is empty
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static T FirstOrDefault<T>(this DbDataReader reader) where T : class, new()
    {
      return First<T>(reader, true, false);
    }

    /// <summary>
    /// Map reader's first row to a model or throw an exception if result set contains more than one row
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static T Single<T>(this DbDataReader reader) where T : class, new()
    {
      return First<T>(reader, false, true);
    }

    /// <summary>
    /// Map reader's first row to a model or return default value if the result set is empty
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static T SingleOrDefault<T>(this DbDataReader reader) where T : class, new()
    {
      return First<T>(reader, true, true);
    }

    /// <summary>
    /// Map reader's first row to a model
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static async Task<T> FirstAsync<T>(this DbDataReader reader) where T : class, new()
    {
      return await FirstAsync<T>(reader, false, false);
    }

    /// <summary>
    /// Map reader's first row to a model or return default value if the result set is empty
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static async Task<T> FirstOrDefaultAsync<T>(this DbDataReader reader) where T : class, new()
    {
      return await FirstAsync<T>(reader, true, false);
    }

    /// <summary>
    /// Map reader's first row to a model or throw an exception if result set contains more than one row
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static async Task<T> SingleAsync<T>(this DbDataReader reader) where T : class, new()
    {
      return await FirstAsync<T>(reader, false, true);
    }

    /// <summary>
    /// Map reader's first row to a model or return default value if the result set is empty
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static async Task<T> SingleOrDefaultAsync<T>(this DbDataReader reader) where T : class, new()
    {
      return await FirstAsync<T>(reader, true, true);
    }

    private static T First<T>(DbDataReader reader, bool orDefault, bool throwIfNotSingle) where T : class, new()
    {
      if (reader.Read())
      {
        PropertyInfo[] props = GetDataReaderColumns<T>(reader);
        T row = MapNextRow<T>(reader, props);

        if (throwIfNotSingle && reader.Read())
          throw new InvalidOperationException("Sequence contains more than one element");

        return row;
      }

      if (orDefault)
        return default(T);

      throw new InvalidOperationException("Sequence contains no element");
    }

    private static async Task<T> FirstAsync<T>(DbDataReader reader, bool orDefault, bool throwIfNotSingle) where T : class, new()
    {
      if (await reader.ReadAsync())
      {
        PropertyInfo[] props = GetDataReaderColumns<T>(reader);
        T row = await MapNextRowAsync<T>(reader, props);

        if (throwIfNotSingle && reader.Read())
          throw new InvalidOperationException("Sequence contains more than one element");

        return row;
      }

      if (orDefault)
        return default(T);

      throw new InvalidOperationException("Sequence contains no element");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T MapNextRow<T>(DbDataReader reader, PropertyInfo[] props, int columnOffset = 0) where T : class, new()
    {
      T row = new T();
      for (int i = columnOffset; i < reader.FieldCount; i++)
      {
        SetPropertyValue(reader, props, row, i);
      }
      return row;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async Task<T> MapNextRowAsync<T>(DbDataReader reader, PropertyInfo[] props, int columnOffset = 0) where T : class, new()
    {
      T row = new T();
      for (int i = columnOffset; i < reader.FieldCount; i++)
      {
        await SetPropertyValueAsync(reader, props, row, i);
      }
      return row;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetPropertyValue<T>(DbDataReader reader, PropertyInfo[] props, T row, int i) where T : class, new()
    {
      Debug.Assert(i >= 0 && i < reader.FieldCount);

      if (props[i] == null)
        return;

      object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
      props[i].SetValue(row, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async Task SetPropertyValueAsync<T>(DbDataReader reader, PropertyInfo[] props, T row, int i) where T : class, new()
    {
      Debug.Assert(i >= 0 && i < reader.FieldCount);

      if (props[i] == null)
        return;

      object value = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
      props[i].SetValue(row, value);
    }

    private static PropertyInfo[] GetDataReaderColumns<T>(DbDataReader reader) where T : class, new()
    {
      var res = new PropertyInfo[reader.FieldCount];
      Type modelType = typeof(T);
      for (int i = 0; i < reader.FieldCount; i++)
      {
        string name = reader.GetName(i);
        string nameNoUnderscore = Regex.Replace(name, "[_-]", "");
        res[i] = modelType.GetProperty(nameNoUnderscore, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
      }
      return res;
    }
  }
}
