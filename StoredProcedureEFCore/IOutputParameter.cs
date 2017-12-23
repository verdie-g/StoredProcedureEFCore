namespace StoredProcedureEFCore
{
  public interface IOutputParam<T>
  {
    T Value { get; }
  }
}
