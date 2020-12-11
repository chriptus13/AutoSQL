using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.DataMappers;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    [TestClass]
    public class EmployeeDynamicDataMapperTest : AbstractEmployeeDataMapperTest {

        public EmployeeDynamicDataMapperTest() : base(new EmployeeDynamicDataMapper(typeof(Employee), NORTHWIND, true)) { }

        [TestMethod]
        public void TestEmployeeGetAllDynamic() {
            TestEmployeeGetAll();
        }

        [TestMethod]
        public void TestEmployeeGetByIdDynamic() {
            TestEmployeeGetById();
        }

        [TestMethod]
        public void TestEmployeeInsertAndDeleteDynamic() {
            TestEmployeeInsertAndDelete();
        }

        [TestMethod]
        public void TestEmployeeUpdateDynamic() {
            TestEmployeeUpdate();
        }
    }

    [TestClass]
    public class EmployeeEmitDataMapperTest : AbstractEmployeeDataMapperTest {

        public EmployeeEmitDataMapperTest() : base(EmitDataMapper.Build(typeof(Employee), NORTHWIND, true)) { }

        [TestMethod]
        public void TestEmployeeGetAllEmit() {
            TestEmployeeGetAll();
        }

        [TestMethod]
        public void TestEmployeeGetByIdEmitc() {
            TestEmployeeGetById();
        }

        [TestMethod]
        public void TestEmployeeInsertAndDeleteEmit() {
            TestEmployeeInsertAndDelete();
        }

        [TestMethod]
        public void TestEmployeeUpdateEmit() {
            TestEmployeeUpdate();
        }
    }
}
