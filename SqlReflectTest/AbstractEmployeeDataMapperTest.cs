using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;
using System;
using System.Collections;

namespace SqlReflectTest {
    public abstract class AbstractEmployeeDataMapperTest {
        protected static readonly string NORTHWIND = @"
                    Server=(LocalDB)\MSSQLLocalDB;
                    Integrated Security=true;
                    AttachDbFileName=" + Environment.CurrentDirectory + "\\data\\NORTHWND.MDF";

        readonly IDataMapper employee;

        public AbstractEmployeeDataMapperTest(IDataMapper employee) {
            this.employee = employee;
        }

        public void TestEmployeeGetAll() {
            IEnumerable res = employee.GetAll();
            int count = 0;
            foreach(object p in res) {
                Console.WriteLine(p);
                count++;
            }
            Assert.AreEqual(9, count);
        }

        public void TestEmployeeGetById() {
            Employee e = (Employee) employee.GetById(1);
            Assert.AreEqual("Davolio", e.LastName);
            Assert.AreEqual("Nancy", e.FirstName);
            Assert.AreEqual("Sales Representative", e.Title);
        }

        public void TestEmployeeInsertAndDelete() {
            Employee e = new Employee() {
                LastName = "Silva",
                FirstName = "Manuel",
                Title = "Sales Representative",
                TitleOfCourtesy = "Mr.",
                Address = "There",
                City = "This one",
                Region = "That one",
                PostalCode = "IDK",
                Country = "Portugal",
                HomePhone = "(206) 555-9857",
                Extension = "100"
            };
            object id = employee.Insert(e);
            //
            // Get the new employee object from database
            //
            Employee actual = (Employee) employee.GetById(id);
            Assert.AreEqual(e.LastName, actual.LastName);
            Assert.AreEqual(e.FirstName, actual.FirstName);
            //
            // Delete the created employee from database
            //
            employee.Delete(actual);
            object res = employee.GetById(id);
            actual = res != null ? (Employee) res : default(Employee);
            Assert.IsNull(actual.LastName);
            Assert.IsNull(actual.FirstName);
        }

        public void TestEmployeeUpdate() {
            Employee original = (Employee) employee.GetById(1);
            Employee modified = new Employee() {
                EmployeeID = original.EmployeeID,
                LastName = "Silva",
                FirstName = "Manuel",
                Title = "Sales Representative",
                TitleOfCourtesy = "Mr.",
                Address = "There",
                City = "This one",
                Region = "That one",
                PostalCode = "IDK",
                Country = "Portugal",
                HomePhone = "(206) 555-9857",
                Extension = "100"
            };
            employee.Update(modified);
            Employee actual = (Employee) employee.GetById(1);
            Assert.AreEqual(modified.FirstName, actual.FirstName);
            Assert.AreEqual(modified.LastName, actual.LastName);
            employee.Update(original);
            actual = (Employee) employee.GetById(1);
            Assert.AreEqual("Davolio", actual.LastName);
            Assert.AreEqual("Nancy", actual.FirstName);
            Assert.AreEqual("Sales Representative", actual.Title);
        }
    }
}
