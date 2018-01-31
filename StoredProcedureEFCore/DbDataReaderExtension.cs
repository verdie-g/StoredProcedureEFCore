using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
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
      Action<T, object>[] setters = MapColumnsToSetters<T>(reader);
      while (reader.Read())
      {
        T row = MapNextRow(reader, setters);
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
      Action<T, object>[] setters = MapColumnsToSetters<T>(reader);
      while (await reader.ReadAsync())
      {
        T row = await MapNextRowAsync(reader, setters);
        res.Add(row);
      }
      return res;
    }

    /// <summary>
    /// Map the first column to a list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static List<T> Column<T>(this DbDataReader reader) where T : IComparable
    {
      return Column<T>(reader, 0);
    }

    /// <summary>
    /// Map the specified column to a list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <param name="columnName">Name of the column to read. Use first column if null</param>
    /// <returns></returns>
    public static List<T> Column<T>(this DbDataReader reader, string columnName) where T : IComparable
    {
      int ordinal = columnName is null ? 0 : reader.GetOrdinal(columnName);
      return Column<T>(reader, ordinal);
    }

    /// <summary>
    /// Map the first column to a list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <returns></returns>
    public static Task<List<T>> ColumnAsync<T>(this DbDataReader reader) where T : IComparable
    {
      return ColumnAsync<T>(reader, 0);
    }

    /// <summary>
    /// Map the specified column to a list
    /// </summary>
    /// <typeparam name="T">Model</typeparam>
    /// <param name="reader"></param>
    /// <param name="columnName">Name of the column to read. Use first column if null</param>
    /// <returns></returns>
    public static Task<List<T>> ColumnAsync<T>(this DbDataReader reader, string columnName) where T : IComparable
    {
      int ordinal = columnName is null ? 0 : reader.GetOrdinal(columnName);
      return ColumnAsync<T>(reader, ordinal);
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
      Action<TValue, object>[] setters = MapColumnsToSetters<TValue>(reader);

      var res = new Dictionary<TKey, TValue>();
      while (reader.Read())
      {
        TKey key = (TKey)reader.GetValue(0);
        TValue val = MapNextRow(reader, setters, 1);
        SetPropertyValue(reader, setters, val, 0);
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
      Action<TValue, object>[] setters = MapColumnsToSetters<TValue>(reader);

      var res = new Dictionary<TKey, TValue>();
      while (await reader.ReadAsync())
      {
        TKey key = (TKey)reader.GetValue(0);
        TValue val = await MapNextRowAsync(reader, setters, 1);
        await SetPropertyValueAsync(reader, setters, val, 0);
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
      Action<TValue, object>[] setters = MapColumnsToSetters<TValue>(reader);

      var res = new Dictionary<TKey, List<TValue>>();
      while (reader.Read())
      {
        TKey key = (TKey)reader.GetValue(0);
        TValue val = MapNextRow(reader, setters, 1);
        SetPropertyValue(reader, setters, val, 0);

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
      Action<TValue, object>[] setters = MapColumnsToSetters<TValue>(reader);

      var res = new Dictionary<TKey, List<TValue>>();
      while (await reader.ReadAsync())
      {
        TKey key = (TKey)reader.GetValue(0);
        TValue val = await MapNextRowAsync(reader, setters, 1);
        await SetPropertyValueAsync(reader, setters, val, 0);

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

    private static List<T> Column<T>(DbDataReader reader, int ordinal) where T : IComparable
    {
      var res = new List<T>();
      while (reader.Read())
      {
        T value = reader.IsDBNull(ordinal) ? default(T) : (T)reader.GetValue(ordinal);
        res.Add(value);
      }
      return res;
    }

    private static async Task<List<T>> ColumnAsync<T>(DbDataReader reader, int ordinal) where T : IComparable
    {
      var res = new List<T>();
      while (await reader.ReadAsync())
      {
        T value = await reader.IsDBNullAsync(ordinal) ? default(T) : (T)reader.GetValue(ordinal);
        res.Add(value);
      }
      return res;
    }

    private static T First<T>(DbDataReader reader, bool orDefault, bool throwIfNotSingle) where T : class, new()
    {
      if (reader.Read())
      {
        Action<T, object>[] setters = MapColumnsToSetters<T>(reader);
        T row = MapNextRow(reader, setters);

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
        Action<T, object>[] setters = MapColumnsToSetters<T>(reader);
        T row = await MapNextRowAsync(reader, setters);

        if (throwIfNotSingle && await reader.ReadAsync())
          throw new InvalidOperationException("Sequence contains more than one element");

        return row;
      }

      if (orDefault)
        return default(T);

      throw new InvalidOperationException("Sequence contains no element");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T MapNextRow<T>(DbDataReader reader, Action<T, object>[] setters, int columnOffset = 0) where T : class, new()
    {
      T row = new T();
      for (int i = columnOffset; i < reader.FieldCount; i++)
      {
        SetPropertyValue(reader, setters, row, i);
      }
      return row;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async Task<T> MapNextRowAsync<T>(DbDataReader reader, Action<T, object>[] setters, int columnOffset = 0) where T : class, new()
    {
      T row = new T();
      for (int i = columnOffset; i < reader.FieldCount; i++)
      {
        await SetPropertyValueAsync(reader, setters, row, i);
      }
      return row;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void SetPropertyValue<T>(DbDataReader reader, Action<T, object>[] setters, T row, int i) where T : class, new()
    {
      Debug.Assert(i >= 0 && i < reader.FieldCount);

      if (setters[i] == null)
        return;

      object value = reader.IsDBNull(i) ? null : reader.GetValue(i);
      setters[i].DynamicInvoke(row, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static async Task SetPropertyValueAsync<T>(DbDataReader reader, Action<T, object>[] setters, T row, int i) where T : class, new()
    {
      Debug.Assert(i >= 0 && i < reader.FieldCount);

      if (setters[i] == null)
        return;

      object value = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
      setters[i](row, value);
    }

    private static Action<T, object>[] MapColumnsToSetters<T>(DbDataReader reader) where T : class, new()
    {
      var res = new Action<T, object>[reader.FieldCount];
      Type modelType = typeof(T);
      for (int i = 0; i < reader.FieldCount; i++)
      {
        string name = reader.GetName(i).Replace("_", "");
        PropertyInfo prop = modelType.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop == null)
          continue;

        ParameterExpression instance = Expression.Parameter(prop.DeclaringType, "instance");
        ParameterExpression argument = Expression.Parameter(typeof(object), "value");
        MethodCallExpression setterCall = Expression.Call(instance, prop.GetSetMethod(), Expression.Convert(argument, prop.PropertyType));
        res[i] = (Action<T, object>)Expression.Lambda(setterCall, instance, argument).Compile();
      }
      return res;
    }
  }
}
