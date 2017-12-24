using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Reflection;

namespace StoredProcedureEFCore.Tests
{
  /// <summary>
  /// IDataReader that can be used for "reading" an IEnumerable<T> collection
  /// </summary>
  public class EnumerableDataReader<T> : IDataReader
  {
    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="collection">The collection to be read</param>
    /// <param name="fields">The list of public field/properties to read from each T (in order), OR if no fields are given only one field will be available: T itself</param>
    public EnumerableDataReader(IEnumerable<T> collection, params string[] fields)
    {
      if (collection == null)
        throw new ArgumentNullException("collection");

      m_Enumerator = collection.GetEnumerator();

      if (m_Enumerator == null)
        throw new NullReferenceException("collection does not implement GetEnumerator");

      SetFields(fields);
    }
    private IEnumerator<T> m_Enumerator;
    private T m_Current = default(T);
    private bool m_EnumeratorState = false;

    private void SetFields(ICollection<string> fields)
    {
      Type type = typeof(T);
      PropertyInfo[] props = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
      foreach (var prop in props)
        m_Fields.Add(new Property(prop));
    }

    private List<BaseField> m_Fields = new List<BaseField>();

    #region IDisposable Members
    public void Dispose()
    {
      if (m_Enumerator != null)
      {
        m_Enumerator.Dispose();
        m_Enumerator = null;
        m_Current = default(T);
        m_EnumeratorState = false;
      }
      m_Closed = true;
    }
    #endregion

    #region IDataReader Members
    public void Close()
    {
      m_Closed = true;
    }
    private bool m_Closed = false;

    public int Depth
    {
      get { return 0; }
    }

    public DataTable GetSchemaTable()
    {
      var dt = new DataTable();
      foreach (BaseField field in m_Fields)
      {
        dt.Columns.Add(new DataColumn(field.Name, field.Type));
      }
      return dt;
    }

    public bool IsClosed
    {
      get { return m_Closed; }
    }

    public bool NextResult()
    {
      return false;
    }

    public bool Read()
    {
      if (IsClosed)
        throw new InvalidOperationException("DataReader is closed");
      m_EnumeratorState = m_Enumerator.MoveNext();
      m_Current = m_EnumeratorState ? m_Enumerator.Current : default(T);
      return m_EnumeratorState;
    }

    public int RecordsAffected
    {
      get { return -1; }
    }
    #endregion

    #region IDataRecord Members
    public int FieldCount
    {
      get { return m_Fields.Count; }
    }

    public Type GetFieldType(int i)
    {
      if (i < 0 || i >= m_Fields.Count)
        throw new IndexOutOfRangeException();
      return m_Fields[i].Type;
    }

    public string GetDataTypeName(int i)
    {
      return GetFieldType(i).Name;
    }

    public string GetName(int i)
    {
      if (i < 0 || i >= m_Fields.Count)
        throw new IndexOutOfRangeException();
      return m_Fields[i].Name;
    }

    public int GetOrdinal(string name)
    {
      for (int i = 0; i < m_Fields.Count; i++)
        if (m_Fields[i].Name == name)
          return i;
      throw new IndexOutOfRangeException("name");
    }

    public bool IsDBNull(int i)
    {
      return GetValue(i) == null;
    }

    public object this[string name]
    {
      get { return GetValue(GetOrdinal(name)); }
    }

    public object this[int i]
    {
      get { return GetValue(i); }
    }

    public object GetValue(int i)
    {
      if (IsClosed || !m_EnumeratorState)
        throw new InvalidOperationException("DataReader is closed or has reached the end of the enumerator");
      if (i < 0 || i >= m_Fields.Count)
        throw new IndexOutOfRangeException();
      return m_Fields[i].GetValue(m_Current);
    }

    public int GetValues(object[] values)
    {
      int length = Math.Min(m_Fields.Count, values.Length);
      for (int i = 0; i < length; i++)
        values[i] = GetValue(i);
      return length;
    }

    public bool GetBoolean(int i) { return (bool)GetValue(i); }
    public byte GetByte(int i) { return (byte)GetValue(i); }
    public char GetChar(int i) { return (char)GetValue(i); }
    public DateTime GetDateTime(int i) { return (DateTime)GetValue(i); }
    public decimal GetDecimal(int i) { return (decimal)GetValue(i); }
    public double GetDouble(int i) { return (double)GetValue(i); }
    public float GetFloat(int i) { return (float)GetValue(i); }
    public Guid GetGuid(int i) { return (Guid)GetValue(i); }
    public short GetInt16(int i) { return (short)GetValue(i); }
    public int GetInt32(int i) { return (int)GetValue(i); }
    public long GetInt64(int i) { return (long)GetValue(i); }
    public string GetString(int i) { return (string)GetValue(i); }

    public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length) { throw new NotSupportedException(); }
    public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length) { throw new NotSupportedException(); }
    public IDataReader GetData(int i) { throw new NotSupportedException(); }
    #endregion

    #region Helper Classes
    private abstract class BaseField
    {
      public abstract Type Type { get; }
      public abstract string Name { get; }
      public abstract object GetValue(T instance);

      protected static void AddGetter(Type classType, string fieldName, Func<T, object> getter)
      {
        m_GetterDictionary.Add(string.Concat(classType.FullName, fieldName), getter);
      }

      protected static Func<T, object> GetGetter(Type classType, string fieldName)
      {
        Func<T, object> getter = null;
        if (m_GetterDictionary.TryGetValue(string.Concat(classType.FullName, fieldName), out getter))
          return getter;
        return null;
      }
      private static Dictionary<string, Func<T, object>> m_GetterDictionary = new Dictionary<string, Func<T, object>>();
    }

    private class Property : BaseField
    {
      public Property(PropertyInfo info)
      {
        m_Info = info;
        m_DynamicGetter = CreateGetMethod(info);
      }
      private PropertyInfo m_Info;
      private Func<T, object> m_DynamicGetter;

      public override Type Type { get { return m_Info.PropertyType; } }
      public override string Name { get { return m_Info.Name; } }

      public override object GetValue(T instance)
      {
        //return m_Info.GetValue(instance, null); // Reflection is slow
        return m_DynamicGetter(instance);
      }

      // Create dynamic method for faster access instead via reflection
      private Func<T, object> CreateGetMethod(PropertyInfo propertyInfo)
      {
        Type classType = typeof(T);
        Func<T, object> dynamicGetter = GetGetter(classType, propertyInfo.Name);
        if (dynamicGetter == null)
        {
          ParameterExpression instance = Expression.Parameter(classType);
          MemberExpression property = Expression.Property(instance, propertyInfo);
          UnaryExpression convert = Expression.Convert(property, typeof(object));
          dynamicGetter = (Func<T, object>)Expression.Lambda(convert, instance).Compile();
          AddGetter(classType, propertyInfo.Name, dynamicGetter);
        }

        return dynamicGetter;
      }
    }

    private class Field : BaseField
    {
      public Field(FieldInfo info)
      {
        m_Info = info;
        m_DynamicGetter = CreateGetMethod(info);
      }
      private FieldInfo m_Info;
      private Func<T, object> m_DynamicGetter;

      public override Type Type { get { return m_Info.FieldType; } }
      public override string Name { get { return m_Info.Name; } }

      public override object GetValue(T instance)
      {
        //return m_Info.GetValue(instance); // Reflection is slow
        return m_DynamicGetter(instance);
      }

      // Create dynamic method for faster access instead via reflection
      private Func<T, object> CreateGetMethod(FieldInfo fieldInfo)
      {
        Type classType = typeof(T);
        Func<T, object> dynamicGetter = GetGetter(classType, fieldInfo.Name);
        if (dynamicGetter == null)
        {
          ParameterExpression instance = Expression.Parameter(classType);
          MemberExpression property = Expression.Field(instance, fieldInfo);
          UnaryExpression convert = Expression.Convert(property, typeof(object));
          dynamicGetter = (Func<T, object>)Expression.Lambda(convert, instance).Compile();
          AddGetter(classType, fieldInfo.Name, dynamicGetter);
        }

        return dynamicGetter;
      }
    }

    private class Self : BaseField
    {
      public Self()
      {
        m_Type = typeof(T);
      }
      private Type m_Type;

      public override Type Type { get { return m_Type; } }
      public override string Name { get { return string.Empty; } }
      public override object GetValue(T instance) { return instance; }
    }
    #endregion
  }
}
