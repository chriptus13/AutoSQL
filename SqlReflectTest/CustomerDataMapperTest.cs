using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    [TestClass]
    public class CustomerDataMapperTest : AbstractCustomerDataMapperTest {

        public CustomerDataMapperTest() : base(new CustomerDataMapper(typeof(Customer), NORTHWIND, true)) { }

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

    [TestClass]
    public class CustomerEmitDataMapperTest : AbstractCustomerDataMapperTest {

        public CustomerEmitDataMapperTest() : base(EmitDataMapper.Build(typeof(Customer), NORTHWIND, true)) { }

        [TestMethod]
        public void TestCustomerEmitGetAll() {
            TestCustomerGetAll();
        }

        [TestMethod]
        public void TestCustomerEmitGetById() {
            TestCustomerGetById();
        }

        [TestMethod]
        public void TestCustomerEmitInsertAndDelete() {
            TestCustomerInsertAndDelete();
        }

        [TestMethod]
        public void TestCustomerEmitUpdate() {
            TestCustomerUpdate();
        }
    }
}
