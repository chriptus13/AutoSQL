using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SqlReflectTest.Model;
using SqlReflect;
using System.Collections;

namespace SqlReflectTest {
    public abstract class AbstractProductDataMapperTest {
        protected static readonly string NORTHWIND = @"
                    Server=(LocalDB)\MSSQLLocalDB;
                    Integrated Security=true;
                    AttachDbFileName=" + Environment.CurrentDirectory + "\\data\\NORTHWND.MDF";

        readonly IDataMapper prods;
        readonly IDataMapper categories;
        readonly IDataMapper suppliers;

        public AbstractProductDataMapperTest(IDataMapper prods, IDataMapper categories, IDataMapper suppliers) {
            this.prods = prods;
            this.categories = categories;
            this.suppliers = suppliers;
        }

        public void TestProductGetAll() {
            IEnumerable res = prods.GetAll();
            int count = 0;
            foreach(object p in res) {
                Console.WriteLine(p);
                count++;
            }
            Assert.AreEqual(77, count);
        }

        public void TestProductGetById() {
            Product p = (Product) prods.GetById(10);
            Assert.AreEqual("Ikura", p.ProductName);
            Assert.AreEqual("Seafood", p.Category.CategoryName);
            Assert.AreEqual("Tokyo Traders", p.Supplier.CompanyName);
        }

        public void TestProductInsertAndDelete() {
            //
            // Create and insert a new product
            //
            Category c = (Category) categories.GetById(4);
            Supplier s = (Supplier) suppliers.GetById(17);
            Product p = new Product() {
                Category = c,
                Supplier = s,
                ProductName = "Bacalhau",
                ReorderLevel = 23,
                UnitsInStock = 100,
                UnitsOnOrder = 40
            };
            object id = prods.Insert(p);
            //
            // Get the new product object from database
            //
            Product actual = (Product) prods.GetById(id);
            Assert.AreEqual(p.ProductName, actual.ProductName);
            Assert.AreEqual(p.UnitsInStock, actual.UnitsInStock);
            //
            // Delete the created product from database
            //
            prods.Delete(actual);
            actual = (Product) prods.GetById(id);
            Assert.IsNull(actual);
        }

        public void TestProductUpdate() {
            Product original = (Product) prods.GetById(10);
            Category c = (Category) categories.GetById(4);
            Supplier s = (Supplier) suppliers.GetById(17);

            Product modified = new Product() {
                ProductID = original.ProductID,
                Category = c,
                Supplier = s,
                ProductName = "Bacalhau",
                ReorderLevel = 23,
                UnitsInStock = 100,
                UnitsOnOrder = 40
            };
            prods.Update(modified);
            Product actual = (Product) prods.GetById(10);
            Assert.AreEqual("Bacalhau", actual.ProductName);
            Assert.AreEqual("Dairy Products", actual.Category.CategoryName);
            Assert.AreEqual("Svensk Sjöföda AB", actual.Supplier.CompanyName);

            prods.Update(original);
            actual = (Product) prods.GetById(10);
            Assert.AreEqual("Ikura", actual.ProductName);
            Assert.AreEqual("Seafood", actual.Category.CategoryName);
            Assert.AreEqual("Tokyo Traders", actual.Supplier.CompanyName);
        }
    }

    public abstract class AbstractGenericProductDataMapperTest {
        protected static readonly string NORTHWIND = @"
                    Server=(LocalDB)\MSSQLLocalDB;
                    Integrated Security=true;
                    AttachDbFileName=" + Environment.CurrentDirectory + "\\data\\NORTHWND.MDF";

        readonly IDataMapper<int, Product> prods;
        readonly IDataMapper<int, Category> categories;
        readonly IDataMapper<int, Supplier> suppliers;

        public AbstractGenericProductDataMapperTest(IDataMapper<int, Product> prods, IDataMapper<int, Category> categories, IDataMapper<int, Supplier> suppliers) {
            this.prods = prods;
            this.categories = categories;
            this.suppliers = suppliers;
        }

        public void TestProductGetAll() {
            IEnumerable res = prods.GetAll();
            int count = 0;
            foreach(object p in res) {
                Console.WriteLine(p);
                count++;
            }
            Assert.AreEqual(77, count);
        }

        public void TestProductGetById() {
            Product p = (Product) prods.GetById(10);
            Assert.AreEqual("Ikura", p.ProductName);
            Assert.AreEqual("Seafood", p.Category.CategoryName);
            Assert.AreEqual("Tokyo Traders", p.Supplier.CompanyName);
        }

        public void TestProductInsertAndDelete() {
            //
            // Create and insert a new product
            //
            Category c = (Category) categories.GetById(4);
            Supplier s = (Supplier) suppliers.GetById(17);
            Product p = new Product() {
                Category = c,
                Supplier = s,
                ProductName = "Bacalhau",
                ReorderLevel = 23,
                UnitsInStock = 100,
                UnitsOnOrder = 40
            };
            object id = prods.Insert(p);
            //
            // Get the new product object from database
            //
            Product actual = (Product) prods.GetById(id);
            Assert.AreEqual(p.ProductName, actual.ProductName);
            Assert.AreEqual(p.UnitsInStock, actual.UnitsInStock);
            //
            // Delete the created product from database
            //
            prods.Delete(actual);
            actual = (Product) prods.GetById(id);
            Assert.IsNull(actual);
        }

        public void TestProductUpdate() {
            Product original = (Product) prods.GetById(10);
            Category c = (Category) categories.GetById(4);
            Supplier s = (Supplier) suppliers.GetById(17);

            Product modified = new Product() {
                ProductID = original.ProductID,
                Category = c,
                Supplier = s,
                ProductName = "Bacalhau",
                ReorderLevel = 23,
                UnitsInStock = 100,
                UnitsOnOrder = 40
            };
            prods.Update(modified);
            Product actual = (Product) prods.GetById(10);
            Assert.AreEqual("Bacalhau", actual.ProductName);
            Assert.AreEqual("Dairy Products", actual.Category.CategoryName);
            Assert.AreEqual("Svensk Sjöföda AB", actual.Supplier.CompanyName);

            prods.Update(original);
            actual = (Product) prods.GetById(10);
            Assert.AreEqual("Ikura", actual.ProductName);
            Assert.AreEqual("Seafood", actual.Category.CategoryName);
            Assert.AreEqual("Tokyo Traders", actual.Supplier.CompanyName);
        }
    }
}
