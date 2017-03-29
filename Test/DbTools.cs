using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Test
{
    public static class DbTools
    {
        public static Dictionary<string, PropertyInfo[]> properties { get; set; } = new Dictionary<string, PropertyInfo[]>();

        public static List<T> ExecuteStoredProcedure<T>(this DbContext context, string name, Func<IDataReader, T> projection, params string[] parameters) where T :  new()
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
            while (reader.Read())
            {
                yield return reader.ToModel<T>();
            }
        }

        private static T ToModel<T>(this IDataReader reader)
        {
            T obj = Activator.CreateInstance<T>();
            Type objType = typeof(T);
            PropertyInfo[] props;
            try
            {
                props = properties[objType.FullName];
            }
            catch
            {
                props = objType.GetProperties();
                properties[objType.FullName] = props;
            }
            foreach (PropertyInfo prop in props)
            {
                if (Equals(reader[prop.Name], DBNull.Value))
                    continue;
                prop.SetValue(obj, reader[prop.Name]);
            }
            return obj;
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
