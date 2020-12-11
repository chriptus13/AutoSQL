using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    [TestClass]
    public class CustomerDataMapperReflectTest : AbstractCustomerDataMapperTest {
        public CustomerDataMapperReflectTest() : base(new ReflectDataMapper(typeof(Customer), NORTHWIND, true)) { }

        [TestMethod]
        public void TestCustomerGetAllReflect() {
            TestCustomerGetAll();
        }

        [TestMethod]
        public void TestCustomerGetByIdReflect() {
            TestCustomerGetById();
        }

        [TestMethod]
        public void TestCustomerInsertAndDeleteReflect() {
            TestCustomerInsertAndDelete();
        }

        [TestMethod]
        public void TestCustomerUpdateReflect() {
            TestCustomerUpdate();
        }
    }
}
