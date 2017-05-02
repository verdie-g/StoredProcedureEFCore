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
        /// Call a stored procedure
        /// </summary>
        /// <typeparam name="T">Type of the result object</typeparam>
        /// <param name="context"></param>
        /// <param name="name">Procedure's name</param>
        /// <param name="parameters">Procedure's parameters</param>
        /// <returns></returns>
        public static List<T> ExecuteStoredProcedure<T>(this DbContext context, string name, params StoredProcedureParameter[] parameters)
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
        public static bool ExecuteStoredProcedure(this DbContext context, string name, params StoredProcedureParameter[] parameters)
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

        private static DbCommand CallStoredProcedure(this DbContext context, string name, params StoredProcedureParameter[] parameters)
        {
            DbCommand command = context.Database.GetDbConnection().CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = name;
            foreach (var parameter in parameters)
            {
                DbParameter param = command.CreateParameter();
                param.ParameterName = '@' + parameter.Name;
                param.Value = parameter.Value;
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
        public static List<T> AutoMap<T>(this IDataReader reader)
        {
            var res = new List<T>();
            FieldInfo[] fields = FieldInfo.GetModelFieldInfos(typeof(T));

            while (reader.Read())
            {
                T obj = Activator.CreateInstance<T>();
                foreach (FieldInfo field in fields)
                    field.Property.SetValue(obj, reader[field.ColumnName]);
                res.Add(obj);
            }
            return res;
        }
    }
}
