using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflectTest.DataMappers;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    [TestClass]
    public class ProductDynamicDataMapperTest : AbstractProductDataMapperTest {
        public ProductDynamicDataMapperTest() : base(
            new ProductDynamicDataMapper(typeof(Product), NORTHWIND, true),
            new CategoryDataMapper(NORTHWIND),
            new SupplierDataMapper(NORTHWIND)) { }

        [TestMethod]
        public void TestProductGetAllDynamic() {
            TestProductGetAll();
        }

        [TestMethod]
        public void TestProductGetByIdDynamic() {
            TestProductGetById();
        }

        [TestMethod]
        public void TestProductInsertAndDeleteDynamic() {
            TestProductInsertAndDelete();
        }

        [TestMethod]
        public void TestProductUpdateDynamic() {
            TestProductUpdate();
        }
    }
}
