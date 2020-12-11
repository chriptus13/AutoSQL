using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    [TestClass]
    public class CategoryDMGReflectTest : AbstractGenericCategoryDataMapperTest {
        public CategoryDMGReflectTest() : base(new ReflectDataMapper<int, Category>(NORTHWIND)) { }

        [TestMethod]
        public new void TestCategoryGetAll() {
            base.TestCategoryGetAll();
        }

        [TestMethod]
        public new void TestCategoryGetById() {
            base.TestCategoryGetById();
        }

        [TestMethod]
        public new void TestCategoryInsertAndDelete() {
            base.TestCategoryInsertAndDelete();
        }

        [TestMethod]
        public new void TestCategoryUpdate() {
            base.TestCategoryUpdate();
        }
    }
}
