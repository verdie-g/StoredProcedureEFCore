using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Test
{
    public static class DbTools
    {
        public static List<T> ExecuteStoredProcedure<T>(this DbContext context, string name, Func<IDataReader, T> projection, params string[] parameters)
        {
            var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = name;
            int cpt = 0;
            foreach (var parameter in parameters)
            {
                var param = command.CreateParameter();
                param.ParameterName = "@p"+cpt;
                param.Value = parameter;
                command.Parameters.Add(param);
                cpt++;
            }
            context.Database.OpenConnection();
            var res = command.ExecuteReader();
            
            return res.Select<T>(projection).ToList();
        }


        public static List<T> ExecuteStoredProcedure2<T>(this DbContext context, string name)
        {

            var command = context.Database.GetDbConnection().CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = name;
            context.Database.OpenConnection();
            var res = command.ExecuteReader();

            return DataReaderMapToList<T>(res);
        }
        
        public static List<T> ExecuteStoredProcedure3<T>(this DbContext context, string name)
        {

            DbCommand command = context.Database.GetDbConnection().CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = name;
            context.Database.OpenConnection();
            IDataReader res = command.ExecuteReader();
            return res.AutoMap<T>().ToList();
        }

        public static IEnumerable<T> Select<T>(this IDataReader reader, Func<IDataReader, T> projection)
        {
            while (reader.Read())
            {
                yield return projection(reader);
            }
        }

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

        private static List<T> DataReaderMapToList<T>(IDataReader dr)
        {
            List<T> list = new List<T>();
            T obj = default(T);
            while (dr.Read())
            {
                obj = Activator.CreateInstance<T>();
                foreach (PropertyInfo prop in obj.GetType().GetProperties())
                {
                    if (!Equals(dr[prop.Name], DBNull.Value))
                    {
                        prop.SetValue(obj, dr[prop.Name]);
                    }
                }
                list.Add(obj);
            }
            return list;
        }
    }
}
