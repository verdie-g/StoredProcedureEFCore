using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace StoredProcedure
{
    public static class DbTools
    {
        /// <summary>
        /// Call a stored procedure
        /// </summary>
        /// <typeparam name="T">Type of the result object</typeparam>
        /// <param name="context"></param>
        /// <param name="name">Procedure's name</param>
        /// <param name="parameters">Procedure's parameters</param>
        /// <returns></returns>
        public static List<T> ExecuteStoredProcedure<T>(this DbContext context, string name, params (string, object)[] parameters)
        {
            using (IDataReader reader = context.CallStoredProcedure(name, parameters).ExecuteReader())
            {
                return reader.AutoMap<T>();
            }
        }

        /// <summary>
        /// Call a stored procedure that only return a boolean
        /// </summary>
        /// <param name="context"></param>
        /// <param name="name">Procedure's name</param>
        /// <param name="parameters">Procedure's parameters</param>
        /// <returns></returns>
        public static bool ExecuteStoredProcedure(this DbContext context, string name, params (string, object)[] parameters)
        {
            DbCommand command = context.CallStoredProcedure(name, parameters);
            DbParameter returnParameter = command.CreateParameter();
            returnParameter.ParameterName = "@out";
            returnParameter.DbType = DbType.Boolean;
            returnParameter.Direction = ParameterDirection.ReturnValue;
            command.Parameters.Add(returnParameter);
            command.ExecuteNonQuery();
            return Convert.ToBoolean(returnParameter.Value);
        }

        private static DbCommand CallStoredProcedure(this DbContext context, string name, params (string name, object value)[] parameters)
        {
            DbCommand command = context.Database.GetDbConnection().CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = name;
            foreach (var parameter in parameters)
            {
                DbParameter param = command.CreateParameter();
                param.ParameterName = '@' + parameter.name;
                param.Value = parameter.value;
                command.Parameters.Add(param);
            }
            context.Database.OpenConnection();
            return command;
        }

        /// <summary>
        /// Map a data reader to a model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="reader"></param>
        /// <returns></returns>
        private static List<T> AutoMap<T>(this IDataReader reader)
        {
            var res = new List<T>();
            Dictionary<string, PropertyInfo> props = reader.GetColumnsPropertyInfos<T>();
            while (reader.Read())
            {
                T row = Activator.CreateInstance<T>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string name = reader.GetName(i);
                    if (props.TryGetValue(name, out PropertyInfo prop))
                    {
                        object value = reader.GetValue(i);
                        prop.SetValue(row, value == DBNull.Value ? null : value);
                    }
                } 
                res.Add(row);
            }
            return res;
        }

        private static Dictionary<string, PropertyInfo> GetColumnsPropertyInfos<T>(this IDataReader reader)
        {
            var res = new Dictionary<string, PropertyInfo>(reader.FieldCount);
            Type modelType = typeof(T);
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string name = reader.GetName(i);
                string nameNoUnderscore = name.Replace("_", "").Replace("-", "");
                PropertyInfo prop = modelType.GetProperty(nameNoUnderscore, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    res[name] = prop;
                }
            }
            return res;
        }

    }
}
