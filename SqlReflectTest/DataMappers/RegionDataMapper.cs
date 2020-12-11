using SqlReflect;
using SqlReflectTest.Model;
using System;
using System.Data.SqlClient;


namespace SqlReflectTest.DataMappers
{
    class RegionDataMapper : AbstractDataMapper
    {
        const string COLUMNS = " RegionID, RegionDescription";
        const string SQL_GET_ALL = "SELECT " + COLUMNS + " FROM Region";
        const string SQL_GET_BY_ID = SQL_GET_ALL + " WHERE RegionID=";
        const string SQL_INSERT = "INSERT INTO Region (" + COLUMNS + ") OUTPUT INSERTED.RegionID VALUES ";
        const string SQL_DELETE = "DELETE FROM Region WHERE RegionId = ";
        const string SQL_UPDATE = "UPDATE Region SET RegionDescription={1} WHERE RegionID = {0}";


        public RegionDataMapper(string connStr) : base(connStr)
        {
        }

        protected override string SqlGetAll()
        {
            return SQL_GET_ALL;
        }
        protected override string SqlGetById(object id)
        {
            return SQL_GET_BY_ID + id;
        }

        protected override string SqlInsert(object target)
        {
            Region r = (Region)target;
            string values = "("
                + r.RegionID + ", "
                + "'" + r.RegionDescription + "'"+
               ")";
            return SQL_INSERT + values;
        }

        protected override string SqlUpdate(object target)
        {
            Region r = (Region)target;
            return String.Format(SQL_UPDATE,
                r.RegionID,
                "'" + r.RegionDescription + "'");
        }

        protected override string SqlDelete(object target)
        {
            Region r = (Region)target;
            return SQL_DELETE + r.RegionID;
        }

        protected override object Load(SqlDataReader dr)
        {
            Region r = new Region();
            r.RegionID = (int)dr["RegionID"];
            r.RegionDescription = ((string)dr["RegionDescription"]).Trim();
            return r;
        }
    }
}
