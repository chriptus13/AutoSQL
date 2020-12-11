using SqlReflect;
using SqlReflectTest.Model;
using System;
using System.Data;
using System.Text;

namespace SqlReflectTest {
    public class CustomerDataMapper : DynamicDataMapper {
        public CustomerDataMapper(Type klass, string connStr, bool withCache) : base(klass, connStr, withCache) { }

        protected override object Load(IDataReader dr) {
            return new Customer {
                CustomerID = (String) dr["CustomerID"],
                CompanyName = (String) Caster(dr["CompanyName"]),
                ContactName = (String) Caster(dr["ContactName"]),
                Address = (String) Caster(dr["Address"]),
                City = (String) Caster(dr["City"]),
                Region = (String) Caster(dr["Region"]),
                PostalCode = (String) Caster(dr["PostalCode"]),
                Country = (String) Caster(dr["Country"]),
                Phone = (String) Caster(dr["Phone"]),
                Fax = (String) Caster(dr["Fax"])
            };
        }

        protected override string SqlDelete(object target) {
            return deleteStmt + '\'' + ((Customer) target).CustomerID + '\'';
        }

        protected override string SqlInsert(object target) {
            Customer c = (Customer) target;
            StringBuilder str = new StringBuilder();
            str.Append('\'').Append(c.CustomerID).Append("',")
                .Append(c.CompanyName != null ? "'" + c.CompanyName : "NULL").Append(c.CompanyName == null ? "," : "',")
                .Append(c.ContactName != null ? "'" + c.ContactName : "NULL").Append(c.ContactName == null ? "," : "',")
                .Append(c.Address != null ? "'" + c.Address : "NULL").Append(c.Address == null ? "," : "',")
                .Append(c.City != null ? "'" + c.City : "NULL").Append(c.City == null ? "," : "',")
                .Append(c.Region != null ? "'" + c.Region : "NULL").Append(c.Region == null ? "," : "',")
                .Append(c.PostalCode != null ? "'" + c.PostalCode : "NULL").Append(c.PostalCode == null ? "," : "',")
                .Append(c.Country != null ? "'" + c.Country : "NULL").Append(c.Country == null ? "," : "',")
                .Append(c.Phone != null ? "'" + c.Phone : "NULL").Append(c.Phone == null ? "," : "',")
                .Append(c.Fax != null ? "'" + c.Fax : "NULL").Append(c.Fax == null ? " " : "'");
            return String.Format(insertStmt, str.ToString());
        }

        protected override string SqlUpdate(object target) {
            Customer c = (Customer) target;
            return String.Format(updateStmt,
                "CompanyName=" + (c.CompanyName == null ? "NULL," : "'" + c.CompanyName + "',") +
                "ContactName=" + (c.ContactName == null ? "NULL," : "'" + c.ContactName + "',") +
                "Address=" + (c.Address == null ? "NULL," : "'" + c.Address + "',") +
                "City=" + (c.City == null ? "NULL," : "'" + c.City + "',") +
                "Region=" + (c.Region == null ? "NULL," : "'" + c.Region + "',") +
                "PostalCode=" + (c.PostalCode == null ? "NULL," : "'" + c.PostalCode + "',") +
                "Country=" + (c.Country == null ? "NULL," : "'" + c.Country + "',") +
                "Phone=" + (c.Phone == null ? "NULL," : "'" + c.Phone + "',") +
                "Fax=" + (c.Fax == null ? "NULL" : "'" + c.Fax + "'"),
                "'" + c.CustomerID + "'");
        }
    }
}
