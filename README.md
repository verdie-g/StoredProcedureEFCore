 # Execute stored procedures with Entity Framework Core

DbContext extension with *LoadStoredProc* method which creates
an IStoredProcBuilder.

The method handles :
- Extra column in result set
- Extra property in model
- Null values in result set
- Underscores or hypens in result set column names ("column_name" is mapped to ColumnName property)
- Int (db) to enumeration (result model) mapping

## Example

```csharp
List<Model> rows = null;

ctx.LoadStoredProc("dbo.ListAll")
   .AddParam("limit", 300L)
   .AddParam("limitOut", out IOutParam<long> limitOut)
   .Exec(r => rows = r.ToList<Model>());

long limitOutValue = limitOut.Value;

ctx.LoadStoredProc("dbo.ReturnBoolean")
   .AddParam("boolean_to_return", true)
   .ReturnValue(out IOutParam<bool> retParam)
   .ExecNonQuery();

bool b = retParam.Value;

ctx.LoadStoredProc("dbo.ListAll")
   .AddParam("limit", 1L)
   .ExecScalar(out long l);
```

## API

### DbContext
```csharp
IStoredProcBuilder             LoadStoredProc(string name)
```

### DbDataReader
```csharp
List<T>                        ToList<T>()
Dictionary<TKey, TValue>       ToDictionary<TKey, TValue>()
Dictionary<TKey, List<TValue>> ToLookup<TKey, TValue>()
HashSet<T>                     ToSet<T>()
List<T>                        Column<T>()
T                              First<T>()
T                              FirstOrDefault<T>()
T                              Single<T>()
T                              SingleOrDefault<T>()
```

### IStoredProcBuilder
```csharp
IStoredProcBuilder             AddParam<T>(string name, T val)                             // Input parameter
IStoredProcBuilder             AddParam<T>(string name, T val, out OutParam<T> outParam)   // Input/Ouput parameter
IStoredProcBuilder             AddParam<T>(string name, out IOutParam<T> outParam)         // Ouput parameter
IStoredProcBuilder             ReturnValue<T>(out IOutParam<T> retParam)
void                           Exec(Action<DbDataReader> action)
void                           ExecNonQuery()
void                           ExecScalar<T>(out T val)
```

## Installation

` Install-Package StoredProcedureEFCore `

## Why ?

Stored procedure execution is not supported in entity framework core:
- [Raw store access APIs: Support for ad hoc mapping of arbitrary types #1862](https://github.com/aspnet/EntityFramework/issues/1862)
- [Stored procedure mapping support #245](https://github.com/aspnet/EntityFramework/issues/245)
