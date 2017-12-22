using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xunit;

namespace StoredProcedureEFCore.Tests
{
  public class IDataReaderTests
  {
    private List<TestModel> _testModelsCollection = new List<TestModel>()
    {
      new TestModel(1, '2', 3, 4, 5, 6, 7, 8, 9, 10.11f, 12.13, true, "15, 16", DateTime.Now, 18.19M, YN.Perhaps),
      new TestModel()
    };

    [Fact]
    public void TestToList()
    {
      IDataReader r = CreateFakeDataReader(0, 1);
      List<TestModel> resultSet = r.ToList<TestModel>();
      Assert.Equal(resultSet.Count, 2);
      TestModelEqual(resultSet[0], 0);
      TestModelEqual(resultSet[1], 1);
    }

    [Fact]
    public void TestToDictionary()
    {
      IDataReader r = CreateFakeDataReader(0, 1);
      Dictionary<sbyte, TestModel> resultSet = r.ToDictionary<sbyte, TestModel>();
      Assert.Equal(resultSet.Count, 2);
      TestModelEqual(resultSet[_testModelsCollection[0].Sb], 0);
      TestModelEqual(resultSet[_testModelsCollection[1].Sb], 1);
    }

    [Fact]
    public void TestToLookup()
    {
      IDataReader r = CreateFakeDataReader(0, 1);
      Dictionary<sbyte, List<TestModel>> resultSet = r.ToLookup<sbyte, TestModel>();
      Assert.Equal(resultSet.Count, 2);
      TestModelEqual(resultSet[_testModelsCollection[0].Sb][0], 0);
      TestModelEqual(resultSet[_testModelsCollection[1].Sb][0], 1);
    }

    [Fact]
    public void TestToSet()
    {
      IDataReader r = CreateFakeDataReader(0, 1);
      HashSet<sbyte> resultSet = r.ToSet<sbyte>();
      Assert.Equal(resultSet.Count, 2);
      resultSet.Contains(_testModelsCollection[0].Sb);
      resultSet.Contains(_testModelsCollection[1].Sb);
    }

    /*
    [Fact]
    public void TestFirst()
    {
      IDataReader r = CreateFakeDataReader(0);
      TestModel tm = r.First<TestModel>();
      TestModelEqual(tm, 0);
    }

    [Fact]
    public void TestFirstOnEmpty()
    {
      IDataReader r = CreateFakeDataReader();
      Assert.Throws<Exception>(() => r.First<TestModel>());
    }
    */

    [Fact]
    public void TestFirstOrDefault()
    {
      IDataReader r = CreateFakeDataReader(0);
      TestModel tm = r.FirstOrDefault<TestModel>();
      TestModelEqual(tm, 0);
    }

    [Fact]
    public void TestFirstOrDefaultOnEmpty()
    {
      IDataReader r = CreateFakeDataReader();
      Assert.Null(r.FirstOrDefault<TestModel>());
    }

    [Fact]
    public void TestSingle()
    {
      IDataReader r = CreateFakeDataReader(0);
      TestModel tm = r.Single<TestModel>();
      TestModelEqual(tm, 0);
    }

    [Fact]
    public void TestSingleOnEmpty()
    {
      IDataReader r = CreateFakeDataReader();
      Assert.Throws<Exception>(() => r.Single<TestModel>());
    }

    [Fact]
    public void TestSingleOnNotSingle()
    {
      IDataReader r = CreateFakeDataReader(0, 1);
      Assert.Throws<Exception>(() => r.Single<TestModel>());
    }

    [Fact]
    public void TestColumn()
    {
      IDataReader r = CreateFakeDataReader(0, 1);
      List<sbyte> col = r.Column<sbyte>();
      Assert.Equal(col.Count, 2);
      Assert.Equal(col[0], _testModelsCollection[0].Sb);
      Assert.Equal(col[1], _testModelsCollection[1].Sb);
    }


    [Fact]
    public void TestColumn2()
    {
      IDataReader r = CreateFakeDataReader(0, 1);
      List<ulong> col = r.Column<ulong>("Ul");
      Assert.Equal(col.Count, 2);
      Assert.Equal(col[0], _testModelsCollection[0].Ul);
      Assert.Equal(col[1], _testModelsCollection[1].Ul);
    }

    private void TestModelEqual(TestModel tm1, int i)
    {
      TestModel tm2 = _testModelsCollection[i];
      Assert.Equal(tm1.Sb, tm2.Sb);
      Assert.Equal(tm1.C, tm2.C);
      Assert.Equal(tm1.S, tm2.S);
      Assert.Equal(tm1.I, tm2.I);
      Assert.Equal(tm1.L, tm2.L);
      Assert.Equal(tm1.B, tm2.B);
      Assert.Equal(tm1.Us, tm2.Us);
      Assert.Equal(tm1.Ui, tm2.Ui);
      Assert.Equal(tm1.Ul, tm2.Ul);
      Assert.Equal(tm1.F, tm2.F);
      Assert.Equal(tm1.D, tm2.D);
      Assert.Equal(tm1.Bo, tm2.Bo);
      Assert.Equal(tm1.Str, tm2.Str);
      Assert.Equal(tm1.Date, tm2.Date);
      Assert.Equal(tm1.Dec, tm2.Dec);
      Assert.Equal(tm1.En, tm2.En);
    }

    private IDataReader CreateFakeDataReader(params int[] indexes)
    {
      IEnumerable<TestModel> data = indexes.Select(i => (TestModel)_testModelsCollection[i].Clone());
      return new EnumerableDataReader<TestModel>(data);
    }
  }
}
