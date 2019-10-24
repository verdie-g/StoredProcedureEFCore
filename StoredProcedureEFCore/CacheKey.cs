using System;
using System.Linq;

namespace StoredProcedureEFCore
{
    internal class CacheKey
    {
        public CacheKey(string[] columns, Type t)
        {
            _columns = columns;
            _type = t;

            unchecked
            {
                _hashCode = 17;
                _hashCode = (_hashCode * 31) + t.GetHashCode();
                for (int i = 0; i < columns.Length; ++i)
                {
                    _hashCode = (_hashCode * 31) + _columns[i].GetHashCode();
                }
            }
        }

        private readonly string[] _columns;
        private readonly Type _type;
        private readonly int _hashCode;

        public override int GetHashCode() => _hashCode;

        public override bool Equals(object obj)
        {
            var k = obj as CacheKey;
            if (k == null)
                return false;
            return k._type == _type && k._columns.SequenceEqual(_columns);
        }
    }
}
