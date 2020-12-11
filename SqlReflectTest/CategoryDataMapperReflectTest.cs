using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;

namespace SqlReflectTest {
    [TestClass]
    public class CategoryDataMapperReflectTest : AbstractCategoryDataMapperTest {
        public CategoryDataMapperReflectTest() : base(new ReflectDataMapper(typeof(Category), NORTHWIND)) { }

        [TestMethod]
        public void TestCategoryGetAllReflect() {
            TestCategoryGetAll();
        }

        [TestMethod]
        public void TestCategoryGetByIdReflect() {
            TestCategoryGetById();
        }

        [TestMethod]
        public void TestCategoryInsertAndDeleteReflect() {
            TestCategoryInsertAndDelete();
        }

        [TestMethod]
        public void TestCategoryUpdateReflect() {
            TestCategoryUpdate();
        }
    }
}
