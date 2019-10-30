using System;
using Xunit;

namespace StoredProcedureEFCore.Tests
{
    public class PropKeyTest
    {
        [Fact]
        public void SameKeyIfSameType()
        {
            var key1 = Mapper<TestModel>.ComputePropertyKey(Array.Empty<string>());
            var key2 = Mapper<TestModel>.ComputePropertyKey(Array.Empty<string>());
            Assert.Equal(key1, key2);
        }

        [Fact]
        public void SameKeyIfSameTypeSameColumns1()
        {
            var key1 = Mapper<TestModel>.ComputePropertyKey(new[] { "abc" });
            var key2 = Mapper<TestModel>.ComputePropertyKey(new[] { "abc" });
            Assert.Equal(key1, key2);
        }

        [Fact]
        public void SameKeyIfSameTypeSameColumns2()
        {
            var key1 = Mapper<TestModel>.ComputePropertyKey(new[] { "abc", "def" });
            var key2 = Mapper<TestModel>.ComputePropertyKey(new[] { "abc", "def" });
            Assert.Equal(key1, key2);
        }

        [Fact]
        public void KeyDiffersIfColumnsDiffers1()
        {
            var key1 = Mapper<TestModel>.ComputePropertyKey(new[] { "abc" });
            var key2 = Mapper<TestModel>.ComputePropertyKey(new[] { "def" });
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void KeyDiffersIfColumnsDiffers2()
        {
            var key1 = Mapper<TestModel>.ComputePropertyKey(new[] { "abc", "def" });
            var key2 = Mapper<TestModel>.ComputePropertyKey(new[] { "abc", "ghi" });
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void KeyDiffersIfColumnsOrderDiffers()
        {
            var key1 = Mapper<TestModel>.ComputePropertyKey(new[] { "abc", "def" });
            var key2 = Mapper<TestModel>.ComputePropertyKey(new[] { "def", "abc" });
            Assert.NotEqual(key1, key2);
        }

        [Fact]
        public void KeyDiffersIfTypeDiffers()
        {
            var key1 = Mapper<TestModel>.ComputePropertyKey(Array.Empty<string>());
            var key2 = Mapper<TestModelClone>.ComputePropertyKey(Array.Empty<string>());
            Assert.NotEqual(key1, key2);
        }

        private class TestModelClone : TestModel { }
    }
}
