using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Test
{
    public static class DbTools
    {
        /// <summary>
        /// Execute a stored procedure
        /// </summary>
        /// <typeparam name="T">Model</typeparam>
        /// <param name="context"></param>
        /// <param name="name">Procedure's name</param>
        /// <param name="parameters">Procedure's parameters</param>
        /// <returns>Enumeration of the result rows</returns>
        public static IEnumerable<T> ExecuteStoredProcedure<T>(this DbContext context, string name, params StoredProcedureParameter[] parameters)
        {
            DbCommand command = context.Database.GetDbConnection().CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = name;
            foreach (var parameter in parameters)
            {
                var param = command.CreateParameter();
                param.ParameterName = "@" + parameter.Name;
                param.Value = parameter.Value;
                command.Parameters.Add(param);
            }
            context.Database.OpenConnection();
            IDataReader res = command.ExecuteReader();
            return res.AutoMap<T>();
        }

        /// <summary>
        /// Map a data reader to a model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static IEnumerable<T> AutoMap<T>(this IDataReader reader)
        {
            var res = new List<T>();
            FieldInfo[] fieldInfos = FieldInfo.GetModelFieldInfos(typeof(T));

            while (reader.Read())
            {
                T obj = Activator.CreateInstance<T>();
                foreach (FieldInfo field in fieldInfos)
                {
                    if (Equals(reader[field.ColumnName], DBNull.Value))
                        continue;
                    field.Property.SetValue(obj, reader[field.ColumnName]);
                }
                res.Add(obj);
            }
            return res;
        }
    }
}
