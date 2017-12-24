namespace StoredProcedureEFCore
{
  public interface IOutParam<T>
  {
    T Value { get; }
  }
}
