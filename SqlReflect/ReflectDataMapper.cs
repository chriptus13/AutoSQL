using SqlReflect.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SqlReflect {
    public class ReflectDataMapper : AbstractDataMapper {
        readonly Type type;
        readonly String connStr;
        readonly PropertyInfo pk;
        readonly String tableName;
        readonly String columns;
        readonly List<PropertyInfo> colsProperties;
        readonly static Dictionary<Type, ReflectDataMapper> dictionary = new Dictionary<Type, ReflectDataMapper>();

        public ReflectDataMapper(Type klass, string connStr) : this(klass, connStr, false) { }

        public ReflectDataMapper(Type klass, string connStr, bool withCache) : this(klass, connStr, withCache, () => { }) { }
        public ReflectDataMapper(Type klass, string connStr, bool withCache, Action action) : base(connStr, withCache, action) {
            this.connStr = connStr;
            type = klass;
            if(!dictionary.TryGetValue(klass, out ReflectDataMapper dm)) {
                tableName = ((TableAttribute) klass.GetCustomAttribute(typeof(TableAttribute))).Name;
                colsProperties = new List<PropertyInfo>(klass.GetProperties());
                int aux = 0;
                foreach(PropertyInfo p in colsProperties) {
                    if(p.IsDefined(typeof(PKAttribute))) pk = p;
                    else {
                        String pName = "";
                        if(!p.PropertyType.FullName.StartsWith("System.")) {
                            Type t = p.PropertyType;
                            PropertyInfo _pk = t.GetProperties().First(pi => pi.IsDefined(typeof(PKAttribute)));
                            pName = _pk.Name;
                        } else pName = p.Name;

                        if(aux == 0) {
                            aux++;
                            columns += pName;
                        } else columns += ", " + pName;
                    }
                }
                dictionary.Add(klass, this);
            } else {
                tableName = dm.tableName;
                colsProperties = dm.colsProperties;
                columns = dm.columns;
                pk = dm.pk;
            }
        }

        protected override object Load(IDataReader dr) {
            Object res = Activator.CreateInstance(type);
            foreach(PropertyInfo x in colsProperties)
                if(!x.PropertyType.FullName.StartsWith("System.")) {
                    Type t = x.PropertyType;
                    if(!dictionary.TryGetValue(t, out ReflectDataMapper dm)) dm = new ReflectDataMapper(t, connStr);
                    Object o = dm.GetById(dr[dm.pk.Name]);
                    x.SetValue(res, o, null);
                } else {
                    var aux = dr[x.Name];
                    if(aux.GetType() != typeof(DBNull)) x.SetValue(res, dr[x.Name], null);
                }
            return res;
        }

        protected override string SqlGetAll() {
            return "SELECT * FROM " + tableName;
        }

        protected override string SqlGetById(object id) {
            return "SELECT * FROM " + tableName + " WHERE " + pk.Name + "=" + id;
        }

        protected override string SqlInsert(object target) {
            StringBuilder str = new StringBuilder("INSERT INTO " + tableName + " (");
            bool isAutoIncrement = ((PKAttribute) pk.GetCustomAttribute(typeof(PKAttribute))).AutoIncrement;
            str.Append(columns);
            if(!isAutoIncrement) str.Append(", ").Append(pk.Name);
            str.Append(") OUTPUT INSERTED.").Append(pk.Name).Append(" VALUES (");

            int aux = 0;
            foreach(PropertyInfo p in target.GetType().GetProperties()) {
                if(!p.IsDefined(typeof(PKAttribute))) {
                    if(aux++ != 0) str.Append(", ");
                    Object curr;

                    if(!p.PropertyType.FullName.StartsWith("System.")) {
                        if(!dictionary.TryGetValue(p.PropertyType, out ReflectDataMapper dm)) dm = new ReflectDataMapper(p.PropertyType, connStr);
                        Object buff = p.GetValue(target);
                        curr = buff.GetType().GetProperty(dm.pk.Name).GetValue(buff);
                    } else curr = p.GetValue(target);

                    if(curr == null) str.Append("NULL");
                    else if(curr.GetType() == typeof(String)) str.Append('\'').Append(curr).Append('\'');
                    else str.Append(curr);
                }
            }
            if(!isAutoIncrement) {
                if(aux == 1) str.Append(pk.GetType() == typeof(String) ? ("'" + pk.GetValue(target) + "'") : pk.GetValue(target));
                else str.Append(", ").Append(pk.PropertyType == typeof(String) ? ("'" + pk.GetValue(target) + "'") : pk.GetValue(target));
            }

            return str.Append(" )").ToString();
        }

        protected override string SqlDelete(object target) {
            return "DELETE FROM " + tableName + " WHERE " + pk.Name + " = " +
                (target.GetType().GetProperty(pk.Name).PropertyType == typeof(string) ?
                ("'" + target.GetType().GetProperty(pk.Name).GetValue(target) + "'") :
                "" + target.GetType().GetProperty(pk.Name).GetValue(target));
        }

        protected override string SqlUpdate(object target) {
            StringBuilder str = new StringBuilder("UPDATE " + tableName + " SET ");
            int aux = 0;
            foreach(PropertyInfo p in target.GetType().GetProperties())
                if(!p.IsDefined(typeof(PKAttribute))) {
                    if(aux++ != 0) str.Append(", ");
                    if(!p.PropertyType.FullName.StartsWith("System.")) {
                        Type t = p.PropertyType;
                        PropertyInfo _pk = t.GetProperties().First(pi => pi.IsDefined(typeof(PKAttribute)));
                        Object buff = p.GetValue(target);
                        var test = buff.GetType().GetProperty(_pk.Name).GetValue(buff);
                        str.Append(_pk.Name).Append('=').Append(test.GetType() == typeof(string) ? "'" + test + "'" : test);
                    } else {
                        var x = p.GetValue(target);
                        str.Append(p.Name).Append('=').Append(x == null ? "NULL" : "'" + x + "'");
                    }
                }
            return str.Append(" WHERE ").Append(pk.Name).Append('=').Append(pk.PropertyType == typeof(String) ? "'" + pk.GetValue(target) + "'" : pk.GetValue(target)).ToString();
        }
    }

    public class ReflectDataMapper<K, V> : AbstractDataMapper<K, V> {
        public readonly string connStr;

        public readonly string getAllStmt;
        public readonly string getByIdStmt;
        public readonly string insertStmt;
        public readonly string deleteStmt;
        public readonly string updateStmt;

        public readonly PropertyInfo pk;

        public ReflectDataMapper(string connStr) : this(connStr, false) { }

        public ReflectDataMapper(string connStr, bool withCache) : this(connStr, withCache, () => { }) { }

        public ReflectDataMapper(string connStr, bool withCache, Action action) : base(connStr, withCache, action) {
            this.connStr = connStr;
            Type klass = typeof(V);
            TableAttribute table = klass.GetCustomAttribute<TableAttribute>();
            if(table == null) throw new InvalidOperationException(klass.Name + " should be annotated with Table custom attribute !!!!");

            pk = klass.GetProperties().First(p => p.IsDefined(typeof(PKAttribute)));

            string columns = String.Join(",", klass.GetProperties()
                .Where(p => p.PropertyType.Name != typeof(IEnumerable<>).Name && (p != pk || !((PKAttribute) pk.GetCustomAttribute(typeof(PKAttribute))).AutoIncrement))
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

        /** Generic Part **/

        protected override V Load(IDataReader dr) {
            object result = Activator.CreateInstance(typeof(V));
            bool isEnumerable;
            // Unassigned generic type of ReflectDataMapper
            Type unassignedGenericType = typeof(ReflectDataMapper<,>);
            foreach(PropertyInfo p in typeof(V).GetProperties()) {
                if((isEnumerable = p.PropertyType.Name == typeof(IEnumerable<>).Name) || !p.PropertyType.FullName.StartsWith("System.")) { // IEnumerable and Complex types
                    Type type = isEnumerable ? p.PropertyType.GetGenericArguments()[0] : p.PropertyType;

                    // PropertyInfo of Primary Key from the type
                    PropertyInfo pkInfo = type.GetProperties().First(x => x.IsDefined(typeof(PKAttribute)));

                    // Creates the ReflectDataMapper with assigned types: Primary Key type and type
                    Type assignedGenericType = unassignedGenericType.MakeGenericType(new Type[] { pkInfo.PropertyType, type });

                    // Creates new DataMapper for the type and loads values into result object
                    Object dm = Activator.CreateInstance(assignedGenericType, connStr);
                    Object value;
                    if(isEnumerable) {
                        string sql = "SELECT * FROM " +
                            type.GetCustomAttribute<TableAttribute>().Name +
                            " WHERE " + pk.Name + " = " + dr[pk.Name];
                        value = assignedGenericType.GetMethod("Get").Invoke(dm, new Object[] { sql });
                    } else value = assignedGenericType.GetMethod("GetById").Invoke(dm, new Object[] { dr[pkInfo.Name] });
                    p.SetValue(result, value);
                } else {    // Simple types
                    var aux = dr[p.Name];
                    if(aux.GetType() != typeof(DBNull)) p.SetValue(result, aux);
                }
            }
            return (V) result;
        }

        protected override string SqlGetById(K id) {
            return getByIdStmt + "'" + id + "'";
        }

        protected override string SqlInsert(V target) {
            StringBuilder str = new StringBuilder();
            foreach(PropertyInfo p in typeof(V).GetProperties()) {
                if(p.PropertyType.Name != typeof(IEnumerable<>).Name) {
                    if(!p.IsDefined(typeof(PKAttribute)) ||
                        !((PKAttribute) p.GetCustomAttribute(typeof(PKAttribute))).AutoIncrement) {
                        if(str.Length != 0) str.Append(',');
                        if(!p.PropertyType.FullName.StartsWith("System.")) {
                            PropertyInfo pInfo = p.PropertyType.GetProperties().First(x => x.IsDefined(typeof(PKAttribute)));
                            Object o = p.GetValue(target);
                            str.Append('\'').Append(o.GetType().GetProperty(pInfo.Name).GetValue(o)).Append('\'');
                        } else {
                            var value = p.GetValue(target);
                            if(value == null) str.Append("NULL");
                            else str.Append('\'').Append(value).Append('\'');
                        }
                    }
                }
            }
            return String.Format(insertStmt, str.ToString());
        }

        protected override string SqlDelete(V target) {
            return deleteStmt + "'" + target.GetType().GetProperty(pk.Name).GetValue(target) + "'";
        }

        protected override string SqlUpdate(V target) {
            StringBuilder str = new StringBuilder();
            foreach(PropertyInfo p in typeof(V).GetProperties()) {
                if(p.PropertyType.Name != typeof(IEnumerable<>).Name) {
                    if(!p.IsDefined(typeof(PKAttribute))) {
                        if(str.Length != 0) str.Append(',');
                        if(!p.PropertyType.FullName.StartsWith("System.")) {
                            PropertyInfo pInfo = p.PropertyType.GetProperties().First(x => x.IsDefined(typeof(PKAttribute)));
                            Object o = p.GetValue(target);
                            str.Append(pInfo.Name).Append("='").Append(o.GetType().GetProperty(pInfo.Name).GetValue(o)).Append('\'');
                        } else {
                            var value = p.GetValue(target);
                            str.Append(p.Name);
                            if(value == null) str.Append("= NULL");
                            else str.Append("='").Append(value).Append('\'');
                        }
                    }
                }
            }
            return String.Format(updateStmt, str.ToString(), "'" + target.GetType().GetProperty(pk.Name).GetValue(target) + "'");
        }

        /** No Generic Part **/

        protected override object _Load(IDataReader dr) {
            return Load(dr);
        }

        protected override string SqlGetById(object id) {
            return SqlGetById((K) id);
        }

        protected override string SqlInsert(object target) {
            return SqlInsert((V) target);
        }

        protected override string SqlDelete(object target) {
            return SqlDelete((V) target);
        }

        protected override string SqlUpdate(object target) {
            return SqlUpdate((V) target);
        }
    }
}
