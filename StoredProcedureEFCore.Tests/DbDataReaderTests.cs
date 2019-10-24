using Moq;
using Moq.DataReader;
using StoredProcedureEFCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using Xunit;

namespace StoredProcedureEFCore.Tests
{
    public class DbDataReaderTests
    {
        private List<TestModel> _testModelsCollection = new List<TestModel>()
        {
          new TestModel(1, '2', 3, 4, 5, 6, 7, 8, 9, 10.11f, 12.13, true, "15, 16", DateTime.Now, 18.19M, YN.Perhaps),
          new TestModel()
        };

        [Fact]
        public void TestToList()
        {
            DbDataReader mock = CreateDataReaderMock(0, 1).Object;
            List<TestModel> resultSet = mock.ToList<TestModel>();
            Assert.Equal(2, resultSet.Count);
            TestModelEqual(resultSet[0], 0);
            TestModelEqual(resultSet[1], 1);
        }

        [Fact]
        public void TestToDictionary()
        {
            DbDataReader mock = CreateDataReaderMock(0, 1).Object;
            Dictionary<sbyte, TestModel> resultSet = mock.ToDictionary<sbyte, TestModel>(m => m.Sb);
            Assert.Equal(2, resultSet.Count);
            TestModelEqual(resultSet[_testModelsCollection[0].Sb], 0);
            TestModelEqual(resultSet[_testModelsCollection[1].Sb], 1);
        }

        [Fact]
        public void TestToLookup()
        {
            DbDataReader mock = CreateDataReaderMock(0, 1).Object;
            Dictionary<sbyte, List<TestModel>> resultSet = mock.ToLookup<sbyte, TestModel>(m => m.Sb);
            Assert.Equal(2, resultSet.Count);
            TestModelEqual(resultSet[_testModelsCollection[0].Sb][0], 0);
            TestModelEqual(resultSet[_testModelsCollection[1].Sb][0], 1);
        }

        [Fact]
        public void TestToSet()
        {
            DbDataReader mock = CreateDataReaderMock(0, 1).Object;
            HashSet<sbyte> resultSet = mock.ToSet<sbyte>();
            Assert.Equal(2, resultSet.Count);
            resultSet.Contains(_testModelsCollection[0].Sb);
            resultSet.Contains(_testModelsCollection[1].Sb);
        }

        [Fact]
        public void TestFirst()
        {
            DbDataReader mock = CreateDataReaderMock(0).Object;
            TestModel tm = mock.First<TestModel>();
            TestModelEqual(tm, 0);
        }

        [Fact]
        public void TestFirstOnEmpty()
        {
            DbDataReader mock = CreateDataReaderMock().Object;
            Assert.Throws<InvalidOperationException>(() => mock.First<TestModel>());
        }

        [Fact]
        public void TestFirstOrDefault()
        {
            DbDataReader mock = CreateDataReaderMock(0).Object;
            TestModel tm = mock.FirstOrDefault<TestModel>();
            TestModelEqual(tm, 0);
        }

        [Fact]
        public void TestFirstOrDefaultOnEmpty()
        {
            DbDataReader mock = CreateDataReaderMock().Object;
            Assert.Null(mock.FirstOrDefault<TestModel>());
        }

        [Fact]
        public void TestSingle()
        {
            DbDataReader mock = CreateDataReaderMock(0).Object;
            TestModel tm = mock.Single<TestModel>();
            TestModelEqual(tm, 0);
        }

        [Fact]
        public void TestSingleOnEmpty()
        {
            DbDataReader mock = CreateDataReaderMock().Object;
            Assert.Throws<InvalidOperationException>(() => mock.Single<TestModel>());
        }

        [Fact]
        public void TestSingleOnNotSingle()
        {
            DbDataReader mock = CreateDataReaderMock(0, 1).Object;
            Assert.Throws<InvalidOperationException>(() => mock.Single<TestModel>());
        }

        [Fact]
        public void TestSingleOrDefault()
        {
            DbDataReader mock = CreateDataReaderMock(0).Object;
            TestModel tm = mock.SingleOrDefault<TestModel>();
            TestModelEqual(tm, 0);
        }

        [Fact]
        public void TestSingleOrDefaultOnEmpty()
        {
            DbDataReader mock = CreateDataReaderMock().Object;
            Assert.Null(mock.SingleOrDefault<TestModel>());
        }

        [Fact]
        public void TestSingleOrDefaultOnNotSingle()
        {
            DbDataReader mock = CreateDataReaderMock(0, 1).Object;
            Assert.Throws<InvalidOperationException>(() => mock.SingleOrDefault<TestModel>());
        }

        [Fact]
        public void TestColumn()
        {
            DbDataReader mock = CreateDataReaderMock(0, 1).Object;
            List<sbyte> col = mock.Column<sbyte>();
            Assert.Equal(2, col.Count);
            Assert.Equal(col[0], _testModelsCollection[0].Sb);
            Assert.Equal(col[1], _testModelsCollection[1].Sb);
        }

        [Fact]
        public void TestColumn2()
        {
            DbDataReader mock = CreateDataReaderMock(0, 1).Object;
            List<ulong> col = mock.Column<ulong>(nameof(TestModel.Ul));
            Assert.Equal(2, col.Count);
            Assert.Equal(col[0], _testModelsCollection[0].Ul);
            Assert.Equal(col[1], _testModelsCollection[1].Ul);
        }

        [Fact]
        public void TestColumn3()
        {
            DbDataReader mock = CreateDataReaderMock(0, 1).Object;
            List<int> col = mock.Column<int>(3);
            Assert.Equal(2, col.Count);
            Assert.Equal(col[0], _testModelsCollection[0].I);
            Assert.Equal(col[1], _testModelsCollection[1].I);
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

        private Mock<DbDataReader> CreateDataReaderMock(params int[] indexes)
        {
            List<TestModel> data = indexes.Select(i => (TestModel) _testModelsCollection[i].Clone()).ToList();
            var mock = new Mock<DbDataReader>();
            mock.SetupDataReader(data);
            return mock;
        }
    }
}
