using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflectTest.Model;
using SqlReflect;

namespace SqlReflectTest {
    [TestClass]
    public class ProductDataMapperReflectTest : AbstractProductDataMapperTest {
        public ProductDataMapperReflectTest() : base(
            new ReflectDataMapper(typeof(Product), NORTHWIND),
            new ReflectDataMapper(typeof(Category), NORTHWIND),
            new ReflectDataMapper(typeof(Supplier), NORTHWIND)) { }

        [TestMethod]
        public void TestProductGetAllReflect() {
            TestProductGetAll();
        }

        [TestMethod]
        public void TestProductGetByIdReflect() {
            TestProductGetById();
        }

        [TestMethod]
        public void TestProductInsertAndDeleteReflect() {
            TestProductInsertAndDelete();
        }

        [TestMethod]
        public void TestProductUpdateReflect() {
            TestProductUpdate();
        }
    }
}
