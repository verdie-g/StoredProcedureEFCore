using System;
using System.Data;

namespace StoredProcedureEFCore
{
  /// <summary>
  /// Stored procedure builder
  /// </summary>
  public interface IStoredProcBuilder : IDisposable
  {
    /// <summary>
    /// Add parameter
    /// </summary>
    /// <param name="name">Parameter's name</param>
    /// <param name="val">Parameter's value</param>
    /// <returns></returns>
    IStoredProcBuilder AddParam(string name, object val);

    /// <summary>
    /// Add return value parameter
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="retParam"></param>
    /// <returns></returns>
    IStoredProcBuilder ReturnValue<T>(out IReturnParameter<T> retParam);

    /// <summary>
    /// Execute the stored procedure
    /// </summary>
    /// <param name="action">Actions to do with the result sets</param>
    void Exec(Action<IDataReader> action);

    /// <summary>
    /// Execute the stored procedure
    /// </summary>
    void ExecNonQuery();

    /// <summary>
    /// Execute the stored procedure and return the first column of the first row
    /// </summary>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="val"></param>
    void ExecScalar<T>(out T val);
  }
}
