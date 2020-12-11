using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    [TestClass]
    public class CustomerDMGReflectTest : AbstractGenericCustomerDataMapperTest {

        public CustomerDMGReflectTest() : base(new ReflectDataMapper<string, Customer>(NORTHWIND)) { }

        [TestMethod]
        public new void TestCustomerGetAll() {
            base.TestCustomerGetAll();
        }

        [TestMethod]
        public new void TestCustomerGetById() {
            base.TestCustomerGetById();
        }

        [TestMethod]
        public new void TestCustomerInsertAndDelete() {
            base.TestCustomerInsertAndDelete();
        }

        [TestMethod]
        public new void TestCustomerUpdate() {
            base.TestCustomerUpdate();
        }
    }
}
