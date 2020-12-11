using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflect;
using SqlReflectTest.Model;
using System;

namespace SqlReflectTest {
    [TestClass]
    public class ListenTest {
        protected static readonly string NORTHWIND = @"
                    Server=(LocalDB)\MSSQLLocalDB;
                    Integrated Security=true;
                    AttachDbFileName=" + Environment.CurrentDirectory + "\\data\\NORTHWND.MDF";

        [TestMethod]
        public void LazyTest() {
            int[] count = { 0 };
            new ReflectDataMapper<int, Category>(NORTHWIND, true, () => ++count[0]).GetAll();
            Assert.AreEqual(0, count[0]);
        }

        [TestMethod]
        public void EagerTest() {
            int[] count = { 0 };
            new ReflectDataMapper(typeof(Category), NORTHWIND, true, () => ++count[0]).GetAll();
            Assert.IsTrue(count[0] > 0);
        }
    }
}
