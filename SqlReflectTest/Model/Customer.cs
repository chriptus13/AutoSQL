using SqlReflect.Attributes;

namespace SqlReflectTest.Model {
    [Table("Customers")]
    public class Customer {
        [PK(false)]
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }

        public override string ToString() {
            return CompanyName;
        }

        public override bool Equals(object obj) {
            return obj is Customer customer &&
                   CustomerID == customer.CustomerID &&
                   CompanyName == customer.CompanyName &&
                   ContactName == customer.ContactName &&
                   Address == customer.Address &&
                   City == customer.City &&
                   Region == customer.Region &&
                   PostalCode == customer.PostalCode &&
                   Country == customer.Country &&
                   Phone == customer.Phone &&
                   Fax == customer.Fax;
        }

        public override int GetHashCode() {
            var hashCode = -931275965;
            hashCode = hashCode * -1521134295 + CustomerID.GetHashCode();
            hashCode = hashCode * -1521134295 + CompanyName.GetHashCode();
            hashCode = hashCode * -1521134295 + ContactName.GetHashCode();
            hashCode = hashCode * -1521134295 + Address.GetHashCode();
            hashCode = hashCode * -1521134295 + City.GetHashCode();
            hashCode = hashCode * -1521134295 + Region.GetHashCode();
            hashCode = hashCode * -1521134295 + PostalCode.GetHashCode();
            hashCode = hashCode * -1521134295 + Country.GetHashCode();
            hashCode = hashCode * -1521134295 + Phone.GetHashCode();
            hashCode = hashCode * -1521134295 + Fax.GetHashCode();

            return hashCode;
        }
    }
}