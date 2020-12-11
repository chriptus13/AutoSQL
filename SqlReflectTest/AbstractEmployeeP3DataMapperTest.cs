using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;
using System;
using System.Collections.Generic;

namespace SqlReflectTest {
    public abstract class AbstractEmployeeP3DataMapperTest {
        protected static readonly string NORTHWIND = @"
                    Server=(LocalDB)\MSSQLLocalDB;
                    Integrated Security=true;
                    AttachDbFileName=" + Environment.CurrentDirectory + "\\data\\NORTHWND.MDF";

        readonly IDataMapper<int, EmployeeP3> employee;
        readonly IDataMapper<int, Order> orders;

        public AbstractEmployeeP3DataMapperTest(IDataMapper<int, EmployeeP3> employee, IDataMapper<int, Order> orders) {
            this.employee = employee;
            this.orders = orders;
        }

        public void TestEmployeeGetAll() {
            IEnumerable<EmployeeP3> res = employee.GetAll();
            int count = 0;
            foreach(object p in res) {
                count++;
            }
            Assert.AreEqual(9, count);
        }

        public void TestEmployeeGetById() {
            EmployeeP3 e = (EmployeeP3) employee.GetById(1);
            Assert.AreEqual("Davolio", e.LastName);
            Assert.AreEqual("Nancy", e.FirstName);
            Assert.AreEqual("Sales Representative", e.Title);
            IEnumerable<Order> a = e.Orders;
            IEnumerable<Order> b = ((AbstractDataMapper<int, Order>) orders).Get("SELECT * FROM Orders WHERE EmployeeID = 1");
            int countA = 0;
            int countB = 0;
            foreach(Order o in a)
                countA++;

            foreach(Order o in b)
                countB++;
            Assert.AreEqual(countA, countB);
        }

        public void TestEmployeeInsertAndDelete() {
            EmployeeP3 e = new EmployeeP3() {
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
            int id = employee.Insert(e);
            //
            // Get the new employee object from database
            //
            EmployeeP3 actual = (EmployeeP3) employee.GetById(id);
            Assert.AreEqual(e.LastName, actual.LastName);
            Assert.AreEqual(e.FirstName, actual.FirstName);
            //
            // Delete the created employee from database
            //
            employee.Delete(actual);
            object res = employee.GetById(id);
            actual = res != null ? (EmployeeP3) res : default(EmployeeP3);
            Assert.IsNull(actual);
        }

        public void TestEmployeeUpdate() {
            EmployeeP3 original = (EmployeeP3) employee.GetById(1);
            EmployeeP3 modified = new EmployeeP3() {
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
            EmployeeP3 actual = (EmployeeP3) employee.GetById(1);
            Assert.AreEqual(modified.FirstName, actual.FirstName);
            Assert.AreEqual(modified.LastName, actual.LastName);
            employee.Update(original);
            actual = (EmployeeP3) employee.GetById(1);
            Assert.AreEqual("Davolio", actual.LastName);
            Assert.AreEqual("Nancy", actual.FirstName);
            Assert.AreEqual("Sales Representative", actual.Title);
        }
    }
}
