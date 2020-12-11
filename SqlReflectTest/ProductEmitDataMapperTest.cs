using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    [TestClass]
    public class ProductEmitDataMapperTest : AbstractProductDataMapperTest {
        public ProductEmitDataMapperTest() : base(
            EmitDataMapper.Build(typeof(Product), NORTHWIND, true),
            EmitDataMapper.Build(typeof(Category), NORTHWIND, true),
            EmitDataMapper.Build(typeof(Supplier), NORTHWIND, true)) { }

        [TestMethod]
        public void TestProductGetAllEmit() {
            TestProductGetAll();
        }

        [TestMethod]
        public void TestProductGetByIdEmit() {
            TestProductGetById();
        }

        [TestMethod]
        public void TestProductInsertAndDeleteEmit() {
            TestProductInsertAndDelete();
        }

        [TestMethod]
        public void TestProductUpdateEmit() {
            TestProductUpdate();
        }
    }
}
