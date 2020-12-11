using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace SqlReflect {
    public abstract class AbstractDataMapper : IDataMapper {
        readonly string connStr;
        readonly DataSet cache;
        readonly Action act = () => { };
        public AbstractDataMapper(string connStr) : this(connStr, true) { }
        public AbstractDataMapper(string connStr, bool withCache, Action act) : this(connStr, withCache) { this.act = act; }

        public AbstractDataMapper(string connStr, bool withCache) {
            this.connStr = connStr;
            if(withCache) cache = new DataSet();
        }

        protected abstract string SqlGetById(object id);
        protected abstract string SqlGetAll();
        protected abstract string SqlInsert(object target);
        protected abstract string SqlUpdate(object target);
        protected abstract string SqlDelete(object target);

        protected abstract object Load(IDataReader dr);

        public object GetById(object id) {
            string sql = SqlGetById(id);
            if(cache != null) {
                string tableName = GetTableNameFromSql(sql, "FROM ");
                DataTable table = cache.Tables[tableName];
                if(table == null) GetAll();
            }
            IEnumerator iter = Get(sql).GetEnumerator();
            return iter.MoveNext() ? iter.Current : null;
        }

        public IEnumerable GetAll() {
            return Get(SqlGetAll());
        }

        private IEnumerable Get(string sql) {
            string tableName = GetTableNameFromSql(sql, "FROM ");
            if(cache == null) return GetFromDb(sql, tableName);

            DataTable table = cache.Tables[tableName];
            return table != null ? DataReaderToList(sql, table.CreateDataReader()) : GetFromDb(sql, tableName);
        }

        private IEnumerable GetFromDb(string sql, string tableName) {
            SqlConnection con = new SqlConnection(connStr);
            SqlCommand cmd = null;
            DbDataReader dr = null;
            try {
                cmd = con.CreateCommand();
                cmd.CommandText = sql;
                con.Open();
                dr = cmd.ExecuteReader();
                dr = AddToCache(dr, tableName);
                act.Invoke();
                return DataReaderToList(dr);
            } finally {
                if(dr != null) dr.Dispose();
                if(cmd != null) cmd.Dispose();
                if(con.State != ConnectionState.Closed) con.Close();
            }
        }

        private IList DataReaderToList(IDataReader dr) {
            IList res = new List<object>();
            while(dr.Read()) {
                act.Invoke();
                res.Add(Load(dr));
            }
            return res;
        }

        private IList DataReaderToList(string sql, IDataReader dr) {
            string[] clause = sql
                .ToUpper()
                .Split(new[] { " WHERE " }, StringSplitOptions.None)[1]  // Last part
                .Split('=');
            IList res = new List<object>();
            while(dr.Read()) {
                if(clause != null) {
                    string col = clause[0].Trim();
                    string val = clause[1].Trim();
                    if(!dr[col].ToString().Equals(val)) continue;
                }
                res.Add(Load(dr));
            }
            return res;
        }

        public object Insert(object target) {
            string sql = SqlInsert(target);
            string tableName = GetTableNameFromSql(sql, "INTO ");
            return Execute(sql, tableName);
        }

        public void Delete(object target) {
            string sql = SqlDelete(target);
            string tableName = GetTableNameFromSql(sql, "FROM ");
            Execute(sql, tableName);
        }


        public void Update(object target) {
            string sql = SqlUpdate(target);
            string tableName = GetTableNameFromSql(sql, "UPDATE ");
            Execute(sql, tableName);
        }

        private object Execute(string sql, string tableName) {
            RemoveFromCache(tableName);
            SqlConnection con = new SqlConnection(connStr);
            SqlCommand cmd = null;
            try {
                cmd = con.CreateCommand();
                cmd.CommandText = sql;
                con.Open();
                return cmd.ExecuteScalar();
            } finally {
                if(cmd != null) cmd.Dispose();
                if(con.State != ConnectionState.Closed) con.Close();
            }
        }

        private DbDataReader AddToCache(DbDataReader dr, string tableName) {
            if(cache == null) return dr;
            cache.Tables.Add(tableName).Load(dr);
            return cache.Tables[tableName].CreateDataReader();
        }

        private void RemoveFromCache(string tableName) {
            if(cache != null && cache.Tables.Contains(tableName)) cache.Tables.Remove(tableName);
        }

        private static string GetTableNameFromSql(string sql, string word) {
            return sql
                .ToUpper()
                .Split(new[] { word }, StringSplitOptions.None)[1]  // Last part
                .Split(' ')[0]; // First word
        }
    }

    public abstract class AbstractDataMapper<K, V> : IDataMapper<K, V> {
        readonly string connStr;
        readonly DataSet cache;
        readonly Action action = () => { };


        public AbstractDataMapper(string connStr) : this(connStr, true) { }
        public AbstractDataMapper(string connStr, bool withCache, Action action) : this(connStr, withCache) { this.action = action; }

        public AbstractDataMapper(string connStr, bool withCache) {
            this.connStr = connStr;
            if(withCache) cache = new DataSet();
        }

        /*--------- No Generic Part ---------*/
        protected abstract string SqlGetById(object id);
        protected abstract string SqlGetAll();
        protected abstract string SqlInsert(object target);
        protected abstract string SqlUpdate(object target);
        protected abstract string SqlDelete(object target);
        protected abstract object _Load(IDataReader dr);

        IEnumerable IDataMapper.GetAll() {
            return Get(SqlGetAll());
        }

        public object GetById(object id) {
            string sql = SqlGetById(id);
            if(cache != null) {
                string tableName = GetTableNameFromSql(sql, "FROM ");
                DataTable table = cache.Tables[tableName];
                if(table == null) GetAll();
            }
            IEnumerator iter = Get(sql).GetEnumerator();
            return iter.MoveNext() ? iter.Current : null;
        }

        public void Update(object target) {
            string sql = SqlUpdate(target);
            string tableName = GetTableNameFromSql(sql, "UPDATE ");
            Execute(sql, tableName);
        }

        public object Insert(object target) {
            string sql = SqlInsert(target);
            string tableName = GetTableNameFromSql(sql, "INTO ");
            return Execute(sql, tableName);
        }

        public void Delete(object target) {
            string sql = SqlDelete(target);
            string tableName = GetTableNameFromSql(sql, "FROM ");
            Execute(sql, tableName);
        }

        private object Execute(string sql, string tableName) {
            RemoveFromCache(tableName);
            using(SqlConnection con = new SqlConnection(connStr))
            using(SqlCommand cmd = con.CreateCommand()) {
                cmd.CommandText = sql;
                con.Open();
                return cmd.ExecuteScalar();
            }
        }

        private static string GetTableNameFromSql(string sql, string word) {
            return sql
                .ToUpper()
                .Split(new[] { word }, StringSplitOptions.None)[1]  // Last part
                .Split(' ')[0]; // First word
        }

        public IEnumerable<V> Get(string sql) {
            string tableName = GetTableNameFromSql(sql, "FROM ");
            if(cache == null) return GetFromDb(sql, tableName);

            DataTable table = cache.Tables[tableName];
            return table != null ? DataReaderToEnumerable(sql, table.CreateDataReader()) : GetFromDb(sql, tableName);
        }

        private DbDataReader AddToCache(DbDataReader dr, string tableName) {
            if(cache == null) return dr;
            cache.Tables.Add(tableName).Load(dr);
            return cache.Tables[tableName].CreateDataReader();
        }

        private void RemoveFromCache(string tableName) {
            if(cache != null && cache.Tables.Contains(tableName)) cache.Tables.Remove(tableName);
        }

        private IEnumerable<V> GetFromDb(string sql, string tableName) {
            using(SqlConnection con = new SqlConnection(connStr))
            using(SqlCommand cmd = con.CreateCommand()) {
                cmd.CommandText = sql;
                con.Open();
                using(DbDataReader dr = AddToCache(cmd.ExecuteReader(), tableName)) {
                    while(dr.Read()) {
                        action.Invoke();
                        yield return Load(dr);
                    }
                }
            }
        }

        private IEnumerable<V> DataReaderToEnumerable(string sql, IDataReader dr) {
            string[] clause = sql
                .ToUpper()
                .Split(new[] { " WHERE " }, StringSplitOptions.None)[1]  // Last part
                .Split('=');
            while(dr.Read()) {
                if(clause != null) {
                    string col = clause[0].Trim();
                    string val = clause[1].Trim();
                    if(!dr[col].ToString().Equals(val)) continue;
                }
                action.Invoke();
                yield return Load(dr);
            }
        }

        /*--------- Generic Part ------------*/
        protected abstract string SqlGetById(K id);
        protected abstract string SqlInsert(V target);
        protected abstract string SqlUpdate(V target);
        protected abstract string SqlDelete(V target);
        protected abstract V Load(IDataReader dr);

        public IEnumerable<V> GetAll() {
            return Get(SqlGetAll());
        }

        public V GetByID(K id) {
            string sql = SqlGetById(id);
            if(cache != null) {
                string tableName = GetTableNameFromSql(sql, "FROM ");
                DataTable table = cache.Tables[tableName];
                if(table == null) GetAll();
            }
            IEnumerator<V> iter = Get(sql).GetEnumerator();
            var t = iter.MoveNext() ? iter.Current : default(V);
            while(iter.MoveNext()) ;
            iter.Dispose();
            return t;
        }

        public void Update(V target) {
            string sql = SqlUpdate(target);
            string tableName = GetTableNameFromSql(sql, "UPDATE ");
            Execute(sql, tableName);
        }

        public K Insert(V target) {
            string sql = SqlInsert(target);
            string tableName = GetTableNameFromSql(sql, "INTO ");
            return (K) Execute(sql, tableName);
        }

        public void Delete(V target) {
            string sql = SqlDelete(target);
            string tableName = GetTableNameFromSql(sql, "FROM ");
            Execute(sql, tableName);
        }
    }
}