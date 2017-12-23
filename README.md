 # Execute stored procedures with Entity Framework Core

DbContext extension with *LoadStoredProc* method which creates
an IStoredProcBuilder.

The method handles :
- Extra column in result set
- Extra property in model
- Null values in result set
- Underscores or hypens in result set column names ("column_name" is mapped to ColumnName property)
- Int (db) to enumeration (result model) mapping

```csharp
List<Model> rows = null;

ctx.LoadStoredProc("dbo.ListAll")
   .AddParam("limit", 300)
   .AddOutputParam("limitOut", out IOutputParam<long> limitOut)
   .Exec(r => rows = r.ToList<Model>());

long limitOutValue = limitOut.Value;

ctx.LoadStoredProc("dbo.ReturnBoolean")
   .AddParam("boolean_to_return", true)
   .ReturnValue(out IOutputParam<bool> retParam)
   .ExecNonQuery();

bool b = retParam.Value;

ctx.LoadStoredProc("dbo.ListAll")
   .AddParam("limit", 1)
   .ExecScalar(out long l);
```

## API

### DbContext
```csharp
IStoredProcBuilder             LoadStoredProc(string name)
```

### IDataReader
```csharp
List<T>                        ToList<T>()
Dictionary<TKey, TValue>       ToDictionary<TKey, TValue>()
Dictionary<TKey, List<TValue>> ToLookup<TKey, TValue>()
HashSet<T>                     ToSet<T>()
List<T>                        Column<T>()
T                              First<T>()
T                              FirstOrDefault<T>()
T                              Single<T>()
```

### IStoredProcBuilder
```csharp
IStoredProcBuilder             AddParam(string name, object val)
IStoredProcBuilder             AddOutputParam<T>(string name, out IOutputParam<T> outParam)
IStoredProcBuilder             ReturnValue<T>(out IReturnParameter<T> retParam)
void                           Exec(Action<IDataReader> action)
void                           ExecNonQuery()
void                           ExecScalar<T>(out T val)
```

## Why ?

Stored procedure execution is not supported in entity framework core:
- [Raw store access APIs: Support for ad hoc mapping of arbitrary types #1862](https://github.com/aspnet/EntityFramework/issues/1862)
- [Stored procedure mapping support #245](https://github.com/aspnet/EntityFramework/issues/245)
