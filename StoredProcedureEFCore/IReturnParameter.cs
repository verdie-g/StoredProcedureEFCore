namespace StoredProcedureEFCore
{
  public interface IReturnParameter<T>
  {
    T Value { get; }
  }
}
