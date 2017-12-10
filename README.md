 # How to call a stored procedure in an ASP.NET core app

This code add a static method to *DbContext* named *Exec*.
The latter calls a stored procedure and maps the result into a list of
the specified type. If the model type is not specified, it will return a
boolean gotten from the SQL Server return statement.

```csharp
using (var context = new DataAccess.TestContext())
{
  List<ResultModel> res = context.Exec<ResultModel>("[dbo].[StoredProcedureName]", ("param_name", value));
}
```

Interesting files are:
- [DbContextExtension.cs](https://github.com/verdie-g/StoredProcedureDotNetCore/blob/master/StoredProcedure/Extensions/DbContextExtension.cs)
- [IDataReaderExtension.cs](https://github.com/verdie-g/StoredProcedureDotNetCore/blob/master/StoredProcedure/Extensions/IDataReaderExtension.cs)

## Why ?

This repository was made in response of the following Entity Framework's issues : 
- [Raw store access APIs: Support for ad hoc mapping of arbitrary types #1862](https://github.com/aspnet/EntityFramework/issues/1862)
- [Stored procedure mapping support #245](https://github.com/aspnet/EntityFramework/issues/245)
