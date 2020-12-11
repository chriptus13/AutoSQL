using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    [TestClass]
    public class ProductDMGReflectTest : AbstractGenericProductDataMapperTest {
        public ProductDMGReflectTest() : base(
            new ReflectDataMapper<int, Product>(NORTHWIND),
            new ReflectDataMapper<int, Category>(NORTHWIND),
            new ReflectDataMapper<int, Supplier>(NORTHWIND)) { }

        [TestMethod]
        public new void TestProductGetAll() {
            base.TestProductGetAll();
        }

        [TestMethod]
        public new void TestProductGetById() {
            base.TestProductGetById();
        }

        [TestMethod]
        public new void TestProductInsertAndDelete() {
            base.TestProductInsertAndDelete();
        }

        [TestMethod]
        public new void TestProductUpdate() {
            base.TestProductUpdate();
        }
    }
}
