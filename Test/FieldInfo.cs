using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Test
{
    public class FieldInfo
    {
        public PropertyInfo Property { get; set; }
        public string ColumnName { get; set; }

        private static int StoredProcedureCount = 10;
        private static Dictionary<string, FieldInfo[]> fieldInfos = new Dictionary<string, FieldInfo[]>(StoredProcedureCount);

        public static FieldInfo[] GetModelFieldInfos(Type modelType)
        {
            if (!fieldInfos.TryGetValue(modelType.FullName, out FieldInfo[] res))
            {
                PropertyInfo[] properties = modelType.GetProperties();
                Type attributeType = typeof(ColumnAttribute);
                res = Enumerable.Range(0, properties.Length).Select(i => new FieldInfo()).ToArray();

                for (int i = 0; i < properties.Length; ++i)
                {
                    ColumnAttribute fieldAttribute = (ColumnAttribute)properties[i].GetCustomAttribute(attributeType);
                    res[i].ColumnName = (fieldAttribute is null) ? properties[i].Name : fieldAttribute.ColumnName;
                    res[i].Property = properties[i];
                }
                fieldInfos[modelType.FullName] = res;
            }
            return res;
        }
    }
}
