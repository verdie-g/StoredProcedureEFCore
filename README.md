 # How to call a stored procedure in an ASP.NET core app

This code add a static method to *DbContext* named *ExecuteStoredProcedure*.
The latter calls a stored procedure and maps the result into an enumerable of
the specified type. If the model type is not specified, it will return a
boolean gotten from the SQL Server return statement.

```csharp
using (var context = new DataAccess.TestContext())
{
    IEnumerable<ResultModel> res = context.ExecuteStoredProcedure<ResultModel>("[dbo].[StoredProcedureName]",
        new StoredProcedureParameter("param_name", value));
}
```

If the field's DB column name contains underscores, the mapper will require a
*Column* attribute over the C# property. This attribute is optionnal.

Useful files are:
- DataAccessBase.cs shows how to call a stored procedure
- ColumnAttribute.cs is an attribute to specify the colmun name of property
  in the database
- StoredProcedureParameter.cs is a model to add parameters to a procedure
- DbTools.cs contains the method *ExecuteStoredProcedure* and *AutoMap*
- FieldInfo.cs is a model to cache property informations

## Why ?

This repository was made in response of the following Entity Framework's issues : 
- [Raw store access APIs: Support for ad hoc mapping of arbitrary types #1862](https://github.com/aspnet/EntityFramework/issues/1862)
- [Stored procedure mapping support #245](https://github.com/aspnet/EntityFramework/issues/245)
