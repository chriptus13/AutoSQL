using SqlReflect;
using SqlReflectTest.Model;
using System;
using System.Data;
using System.Text;

namespace SqlReflectTest.DataMappers {
    public class EmployeeDynamicDataMapper : DynamicDataMapper {
        public EmployeeDynamicDataMapper(Type klass, string connStr, bool withCache) : base(klass, connStr, withCache) { }

        protected override object Load(IDataReader dr) {
            return new Employee {
                EmployeeID = (int) dr["EmployeeID"],
                LastName = (string) Caster(dr["LastName"]),
                FirstName = (string) Caster(dr["FirstName"]),
                Title = (string) Caster(dr["Title"]),
                TitleOfCourtesy = (string) Caster(dr["TitleOfCourtesy"]),
                Address = (string) Caster(dr["Address"]),
                City = (string) Caster(dr["City"]),
                Region = (string) Caster(dr["Region"]),
                PostalCode = (string) Caster(dr["PostalCode"]),
                Country = (string) Caster(dr["Country"]),
                HomePhone = (string) Caster(dr["HomePhone"]),
                Extension = (string) Caster(dr["Extension"])
            };
        }

        protected override string SqlDelete(object target) {
            return deleteStmt + "'" + ((Employee) target).EmployeeID + "'";
        }

        protected override string SqlInsert(object target) {
            Employee e = (Employee) target;
            StringBuilder str = new StringBuilder();
            str.Append('\'').Append(e.LastName).Append("', '")
                .Append(e.FirstName).Append("', '")
                .Append(e.Title).Append("', '")
                .Append(e.TitleOfCourtesy).Append("', '")
                .Append(e.Address).Append("', '")
                .Append(e.City).Append("', '")
                .Append(e.Region).Append("', '")
                .Append(e.PostalCode).Append("', '")
                .Append(e.Country).Append("', '")
                .Append(e.HomePhone).Append("', '")
                .Append(e.Extension).Append('\'');
            return String.Format(insertStmt, str.ToString());
        }

        protected override string SqlUpdate(object target) {
            Employee e = (Employee) target;
            return String.Format(updateStmt,
                "LastName=" + (e.LastName == null ? "NULL," : "'" + e.LastName + "',") +
                "FirstName=" + (e.FirstName == null ? "NULL," : "'" + e.FirstName + "',") +
                "Title=" + (e.Title == null ? "NULL," : "'" + e.Title + "',") +
                "TitleOfCourtesy=" + (e.TitleOfCourtesy == null ? "NULL," : "'" + e.TitleOfCourtesy + "',") +
                "Address=" + (e.Address == null ? "NULL," : "'" + e.Address + "',") +
                "City=" + (e.City == null ? "NULL," : "'" + e.City + "',") +
                "Region=" + (e.Region == null ? "NULL," : "'" + e.Region + "',") +
                "PostalCode=" + (e.PostalCode == null ? "NULL," : "'" + e.PostalCode + "',") +
                "Country=" + (e.Country == null ? "NULL," : "'" + e.Country + "',") +
                "HomePhone=" + (e.HomePhone == null ? "NULL," : "'" + e.HomePhone + "',") +
                "Extension=" + (e.Extension == null ? "NULL," : "'" + e.Extension + "'"),
                "" + e.EmployeeID);
        }
    }
}
