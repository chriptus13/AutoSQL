using SqlReflect;
using SqlReflectTest.Model;
using System;
using System.Data;

namespace SqlReflectTest.DataMappers {
    public class SupplierDataMapper : AbstractDataMapper {
        const string SQL_GET_ALL = @"SELECT SupplierID, CompanyName, ContactName, ContactTitle, Address, City, Region, PostalCode, Country, Phone, Fax
                                     FROM Suppliers";
        const string SQL_GET_BY_ID = SQL_GET_ALL + " WHERE SupplierID=";

        public SupplierDataMapper(string connStr) : base(connStr) { }

        protected override string SqlGetAll() {
            return SQL_GET_ALL;
        }

        protected override string SqlGetById(object id) {
            return SQL_GET_BY_ID + id;
        }

        protected override string SqlInsert(object target) {
            throw new NotImplementedException();
        }

        protected override string SqlUpdate(object target) {
            throw new NotImplementedException();
        }

        protected override string SqlDelete(object target) {
            throw new NotImplementedException();
        }

        protected override object Load(IDataReader dr) {
            return new Supplier {
                SupplierID = (int) dr["SupplierID"],
                CompanyName = (string) dr["CompanyName"],
                ContactName = (string) dr["ContactName"],
                ContactTitle = (string) dr["ContactTitle"],
                Address = (string) dr["Address"],
                City = (string) dr["City"],
                Region = dr["Region"] as String,
                PostalCode = (string) dr["PostalCode"],
                Country = (string) dr["Country"],
                Phone = (string) dr["Phone"],
                Fax = dr["Fax"] as String
            };
        }
    }
}
