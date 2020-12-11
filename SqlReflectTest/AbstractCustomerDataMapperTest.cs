using System;
using System.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    public abstract class AbstractCustomerDataMapperTest {
        protected static readonly string NORTHWIND = @"
                    Server=(LocalDB)\MSSQLLocalDB;
                    Integrated Security=true;
                    AttachDbFileName=" +
                        Environment.CurrentDirectory +
                        "\\data\\NORTHWND.MDF";
        readonly IDataMapper customers;

        public AbstractCustomerDataMapperTest(IDataMapper customers) {
            this.customers = customers;
        }

        public void TestCustomerGetAll() {
            IEnumerable res = customers.GetAll();
            int count = 0;
            foreach(object p in res) {
                Console.WriteLine(p);
                count++;
            }
            Assert.AreEqual(91, count);
        }

        public void TestCustomerGetById() {
            Customer c = (Customer) customers.GetById("ALFKI");
            Assert.AreEqual("ALFKI", c.CustomerID);
            Assert.AreEqual("Alfreds Futterkiste", c.CompanyName);
            Assert.AreEqual("Maria Anders", c.ContactName);
            Assert.AreEqual("Obere Str. 57", c.Address);
            Assert.AreEqual("Berlin", c.City);
            Assert.AreEqual("12209", c.PostalCode);
            Assert.IsNull(c.Region);
            Assert.AreEqual("Germany", c.Country);
            Assert.AreEqual("030-0074321", c.Phone);
            Assert.AreEqual("030-0076545", c.Fax);
        }

        public void TestCustomerInsertAndDelete() {
            //
            // Create and Insert new Customer
            // 
            Customer c = new Customer() {
                CustomerID = "TESTE",
                CompanyName = "Testing code",
                ContactName = "Programmer",
                Address = "IP",
                City = "Lisbon",
                PostalCode = "IDK",
                Region = "Potato",
                Country = "Test",
                Phone = "None",
                Fax = "see above"
            };
            object id = customers.Insert(c);
            //
            // Get the new Customer object from database
            //
            Customer actual = (Customer) customers.GetById(id);
            Assert.AreEqual(actual.CustomerID, c.CustomerID);
            Assert.AreEqual(actual.CompanyName, c.CompanyName);
            Assert.AreEqual(actual.ContactName, c.ContactName);
            Assert.AreEqual(actual.Address, c.Address);
            Assert.AreEqual(actual.City, c.City);
            Assert.AreEqual(actual.PostalCode, c.PostalCode);
            Assert.AreEqual(actual.Region, c.Region);
            Assert.AreEqual(actual.Country, c.Country);
            Assert.AreEqual(actual.Phone, c.Phone);
            Assert.AreEqual(actual.Fax, c.Fax);
            //
            // Delete the created Customer from database
            //
            customers.Delete(actual);
            object res = customers.GetById(id);
            actual = res != null ? (Customer) res : default(Customer);
            Assert.IsNull(actual);
        }

        public void TestCustomerUpdate() {
            Customer original = (Customer) customers.GetById("ALFKI");
            Customer c = new Customer() {
                CustomerID = "ALFKI",
                CompanyName = "Testing code",
                ContactName = "Programmer",
                Address = "IP",
                City = "Lisbon",
                PostalCode = "IDK",
                Region = "Test",
                Country = "Test",
                Phone = "None",
                Fax = "see above"
            };
            customers.Update(c);
            Customer actual = (Customer) customers.GetById(c.CustomerID);
            Assert.AreEqual(actual.CustomerID, c.CustomerID);
            Assert.AreEqual(actual.CompanyName, c.CompanyName);
            Assert.AreEqual(actual.ContactName, c.ContactName);
            Assert.AreEqual(actual.Address, c.Address);
            Assert.AreEqual(actual.City, c.City);
            Assert.AreEqual(actual.PostalCode, c.PostalCode);
            Assert.AreEqual(actual.Region, c.Region);
            Assert.AreEqual(actual.Country, c.Country);
            Assert.AreEqual(actual.Phone, c.Phone);
            Assert.AreEqual(actual.Fax, c.Fax);
            customers.Update(original);
            actual = (Customer) customers.GetById(original.CustomerID);
            c = original;
            Assert.AreEqual(actual.CustomerID, c.CustomerID);
            Assert.AreEqual(actual.CompanyName, c.CompanyName);
            Assert.AreEqual(actual.ContactName, c.ContactName);
            Assert.AreEqual(actual.Address, c.Address);
            Assert.AreEqual(actual.City, c.City);
            Assert.AreEqual(actual.PostalCode, c.PostalCode);
            Assert.AreEqual(actual.Country, c.Country);
            Assert.AreEqual(actual.Phone, c.Phone);
            Assert.AreEqual(actual.Fax, c.Fax);
        }
    }

    public abstract class AbstractGenericCustomerDataMapperTest {
        protected static readonly string NORTHWIND = @"
                    Server=(LocalDB)\MSSQLLocalDB;
                    Integrated Security=true;
                    AttachDbFileName=" +
                        Environment.CurrentDirectory +
                        "\\data\\NORTHWND.MDF";
        readonly IDataMapper<string, Customer> customers;

        public AbstractGenericCustomerDataMapperTest(IDataMapper<string, Customer> customers) {
            this.customers = customers;
        }

        public void TestCustomerGetAll() {
            IEnumerable res = customers.GetAll();
            int count = 0;
            foreach(object p in res) {
                Console.WriteLine(p);
                count++;
            }
            Assert.AreEqual(91, count);
        }

        public void TestCustomerGetById() {
            Customer c = (Customer) customers.GetById("ALFKI");
            Assert.AreEqual("ALFKI", c.CustomerID);
            Assert.AreEqual("Alfreds Futterkiste", c.CompanyName);
            Assert.AreEqual("Maria Anders", c.ContactName);
            Assert.AreEqual("Obere Str. 57", c.Address);
            Assert.AreEqual("Berlin", c.City);
            Assert.AreEqual("12209", c.PostalCode);
            Assert.IsNull(c.Region);
            Assert.AreEqual("Germany", c.Country);
            Assert.AreEqual("030-0074321", c.Phone);
            Assert.AreEqual("030-0076545", c.Fax);
        }

        public void TestCustomerInsertAndDelete() {
            //
            // Create and Insert new Customer
            // 
            Customer c = new Customer() {
                CustomerID = "TESTE",
                CompanyName = "Testing code",
                ContactName = "Programmer",
                Address = "IP",
                City = "Lisbon",
                PostalCode = "IDK",
                Region = "Potato",
                Country = "Test",
                Phone = "None",
                Fax = "see above"
            };
            object id = customers.Insert(c);
            //
            // Get the new Customer object from database
            //
            Customer actual = (Customer) customers.GetById(id);
            Assert.AreEqual(actual.CustomerID, c.CustomerID);
            Assert.AreEqual(actual.CompanyName, c.CompanyName);
            Assert.AreEqual(actual.ContactName, c.ContactName);
            Assert.AreEqual(actual.Address, c.Address);
            Assert.AreEqual(actual.City, c.City);
            Assert.AreEqual(actual.PostalCode, c.PostalCode);
            Assert.AreEqual(actual.Region, c.Region);
            Assert.AreEqual(actual.Country, c.Country);
            Assert.AreEqual(actual.Phone, c.Phone);
            Assert.AreEqual(actual.Fax, c.Fax);
            //
            // Delete the created Customer from database
            //
            customers.Delete(actual);
            object res = customers.GetById(id);
            actual = res != null ? (Customer) res : default(Customer);
            Assert.IsNull(actual);
        }

        public void TestCustomerUpdate() {
            Customer original = (Customer) customers.GetById("ALFKI");
            Customer c = new Customer() {
                CustomerID = "ALFKI",
                CompanyName = "Testing code",
                ContactName = "Programmer",
                Address = "IP",
                City = "Lisbon",
                PostalCode = "IDK",
                Region = "Test",
                Country = "Test",
                Phone = "None",
                Fax = "see above"
            };
            customers.Update(c);
            Customer actual = (Customer) customers.GetById(c.CustomerID);
            Assert.AreEqual(actual.CustomerID, c.CustomerID);
            Assert.AreEqual(actual.CompanyName, c.CompanyName);
            Assert.AreEqual(actual.ContactName, c.ContactName);
            Assert.AreEqual(actual.Address, c.Address);
            Assert.AreEqual(actual.City, c.City);
            Assert.AreEqual(actual.PostalCode, c.PostalCode);
            Assert.AreEqual(actual.Region, c.Region);
            Assert.AreEqual(actual.Country, c.Country);
            Assert.AreEqual(actual.Phone, c.Phone);
            Assert.AreEqual(actual.Fax, c.Fax);
            customers.Update(original);
            actual = (Customer) customers.GetById(original.CustomerID);
            c = original;
            Assert.AreEqual(actual.CustomerID, c.CustomerID);
            Assert.AreEqual(actual.CompanyName, c.CompanyName);
            Assert.AreEqual(actual.ContactName, c.ContactName);
            Assert.AreEqual(actual.Address, c.Address);
            Assert.AreEqual(actual.City, c.City);
            Assert.AreEqual(actual.PostalCode, c.PostalCode);
            Assert.AreEqual(actual.Country, c.Country);
            Assert.AreEqual(actual.Phone, c.Phone);
            Assert.AreEqual(actual.Fax, c.Fax);
        }
    }
}
