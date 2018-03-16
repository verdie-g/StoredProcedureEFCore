using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace StoredProcedureEFCore
{
  /// <summary>
  /// Mapper <see cref="DbDataReader"/> to model of type <see cref="T"/>
  /// </summary>
  /// <typeparam name="T">Model type</typeparam>
  internal class Mapper<T> where T : class, new()
  {
    private static Dictionary<CacheKey, (int, Action<object, object>)[]> _settersCache = new Dictionary<CacheKey, (int, Action<object, object>)[]>();

    private DbDataReader _reader;
    private (int Ordinal, Action<object, object> Setter)[] _setters;

    public Mapper(DbDataReader reader)
    {
      _reader = reader;
      _setters = MapColumnsToSetters();
    }

    /// <summary>
    /// Map <see cref="DbDataReader"/> to a T and apply an action on it for each row
    /// </summary>
    /// <param name="action">Action to apply to each row</param>
    public void Map(Action<T> action)
    {
      while (_reader.Read())
      {
        T row = MapNextRow();
        action(row);
      }
    }

    /// <summary>
    /// Map <see cref="DbDataReader"/> to a T and apply an action on it for each row
    /// </summary>
    /// <param name="action">Action to apply to each row</param>
    public async Task MapAsync(Action<T> action)
    {
      while (await _reader.ReadAsync())
      {
        T row = await MapNextRowAsync();
        action(row);
      }
    }

    public T MapNextRow()
    {
      T row = new T();
      for (int i = 0; i < _setters.Length; ++i)
      {
        object value = _reader.IsDBNull(_setters[i].Ordinal) ? null : _reader.GetValue(_setters[i].Ordinal);
        _setters[i].Setter(row, value);
      }
      return row;
    }

    public async Task<T> MapNextRowAsync()
    {
      T row = new T();
      for (int i = 0; i < _setters.Length; ++i)
      {
        object value = await _reader.IsDBNullAsync(_setters[i].Ordinal) ? null : _reader.GetValue(_setters[i].Ordinal);
        _setters[i].Setter(row, value);
      }
      return row;
    }

    private (int, Action<object, object>)[] MapColumnsToSetters()
    {
      Type modelType = typeof(T);

      string[] columns = new string[_reader.FieldCount];
      for (int i = 0; i < _reader.FieldCount; ++i)
        columns[i] = _reader.GetName(i);

      var key = new CacheKey(columns, modelType);
      if (_settersCache.TryGetValue(key, out (int, Action<object, object>)[] s))
      {
        return s;
      }

      var setters = new List<(int, Action<object, object>)>(columns.Length);
      for (int i = 0; i < columns.Length; i++)
      {
        string name = columns[i].Replace("_", "");
        PropertyInfo prop = modelType.GetProperty(name, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
        if (prop == null)
          continue;

        ParameterExpression instance = Expression.Parameter(typeof(object), "instance");
        ParameterExpression argument = Expression.Parameter(typeof(object), "value");
        MethodCallExpression setterCall = Expression.Call(Expression.Convert(instance, prop.DeclaringType), prop.GetSetMethod(), Expression.Convert(argument, prop.PropertyType));
        var setter = (Action<object, object>)Expression.Lambda(setterCall, instance, argument).Compile();

        setters.Add((i, setter));
      }
      var settersArray = setters.ToArray(); 
      _settersCache[key] = settersArray;
      return settersArray;
    }
  }
}
