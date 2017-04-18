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
        public string DbName { get; set; }

        public static FieldInfo[] GetModelFieldInfos(Type modelType)
        {
            if (!fieldInfos.TryGetValue(modelType.FullName, out FieldInfo[] res))
            {
                PropertyInfo[] properties = modelType.GetProperties();
                Type attributeType = typeof(FieldAttribute);
                res = Enumerable.Range(0, properties.Length).Select(i => new FieldInfo()).ToArray();

                for (int i = 0; i < properties.Length; ++i)
                {
                    FieldAttribute fieldAttribute = (FieldAttribute)properties[i].GetCustomAttribute(attributeType);
                    res[i].DbName = (fieldAttribute is null) ? properties[i].Name : fieldAttribute.DbName;
                    res[i].Property = properties[i];
                }
                fieldInfos[modelType.FullName] = res;
            }
            return res;
        }

        private static Dictionary<string, FieldInfo[]> fieldInfos = new Dictionary<string, FieldInfo[]>(10); // Stored procedure result model count
    }
}
