using SqlReflect.Attributes;
using System;
using System.Data;
using System.Linq;
using System.Reflection;

namespace SqlReflect {
    public abstract class DynamicDataMapper : AbstractDataMapper {
        public readonly string getAllStmt;
        public readonly string getByIdStmt;
        public readonly string insertStmt;
        public readonly string deleteStmt;
        public readonly string updateStmt;

        public DynamicDataMapper(Type klass, string connStr, bool withCache) : base(connStr, withCache) {
            TableAttribute table = klass.GetCustomAttribute<TableAttribute>();
            if(table == null) throw new InvalidOperationException(klass.Name + " should be annotated with Table custom attribute !!!!");

            PropertyInfo pk = klass
                .GetProperties()
                .First(p => p.IsDefined(typeof(PKAttribute)));

            string columns = String
                .Join(",", klass.GetProperties().Where(p => p != pk || !((PKAttribute) pk.GetCustomAttribute(typeof(PKAttribute))).AutoIncrement)
                .Select(p => p.PropertyType.FullName.StartsWith("System.") ? p.Name : p.PropertyType.GetProperties().First(pi => pi.IsDefined(typeof(PKAttribute))).Name));

            getAllStmt = "SELECT * FROM " + table.Name;
            getByIdStmt = getAllStmt + " WHERE " + pk.Name + "=";
            insertStmt = "INSERT INTO " + table.Name + "(" + columns + ") OUTPUT INSERTED." + pk.Name + " VALUES ({0})";
            deleteStmt = "DELETE FROM " + table.Name + " WHERE " + pk.Name + "=";
            updateStmt = "UPDATE " + table.Name + " SET {0} WHERE " + pk.Name + "={1}";
        }

        protected override string SqlGetAll() {
            return getAllStmt;
        }

        protected override string SqlGetById(object id) {
            return getByIdStmt + id;
        }

        public Object Caster(Object o) {
            return o.GetType() == typeof(DBNull) ? null : o;
        }

        public static String ResolveNull(Object o) {
            return o == null ? "NULL" : ("'" + o.ToString() + "'");
        }
    }
}