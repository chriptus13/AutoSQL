using SqlReflect.Attributes;
using System.Collections.Generic;

namespace SqlReflectTest.Model {
    [Table("Employees")]
    public class EmployeeP3 {
        [PK]
        public int EmployeeID { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Title { get; set; }
        public string TitleOfCourtesy { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string HomePhone { get; set; }
        public string Extension { get; set; }
        public IEnumerable<Order> Orders { get; set; } 
    }
}
