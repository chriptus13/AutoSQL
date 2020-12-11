using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    [TestClass]
    public class EmployeeP3DataMapperTest : AbstractEmployeeP3DataMapperTest {

        public EmployeeP3DataMapperTest() : base(new ReflectDataMapper<int, EmployeeP3>(NORTHWIND, true), new ReflectDataMapper<int, Order>(NORTHWIND, true)) { }

        [TestMethod]
        public void TestEmployeeP3GetAll() {
            TestEmployeeGetAll();
        }

        [TestMethod]
        public void TestEmployeeP3GetById() {
            TestEmployeeGetById();
        }

        [TestMethod]
        public void TestEmployeeP3InsertAndDelete() {
            TestEmployeeInsertAndDelete();
        }

        [TestMethod]
        public void TestEmployeeP3Update() {
            TestEmployeeUpdate();
        }
    }
}
