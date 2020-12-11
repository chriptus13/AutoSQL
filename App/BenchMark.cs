using SqlReflect;
using SqlReflectTest.Model;
using SqlReflectTest.DataMappers;
using System;
using System.Collections;
using SqlReflectTest;

namespace App {
    // TP2 - Exercicio 4
    public class BenchMark {
        private static readonly string connStr = @"
                    Server=(LocalDB)\MSSQLLocalDB;
                    Integrated Security=true;
                    AttachDbFileName=" + Environment.CurrentDirectory + "\\data\\NORTHWND.MDF";

        public static void Main(string[] args) {
            Console.WriteLine("IMPORTANT: No caching shall be used for these tests to ensure that the results given " +
                "by NBench are a result of the different DataMappers!");

            Console.WriteLine("\nPress ENTER to Start Customer Test");
            Console.ReadLine();

            Console.WriteLine("############## Customer");

            NBench.Bench(() => CustomerDynamic(), "Dynamic Test");
            NBench.Bench(() => CustomerReflect(), "Reflect Test");
            NBench.Bench(() => CustomerEmit(), "Emit Test");

            Console.WriteLine("\nPress ENTER to Start Employee Test");
            Console.ReadLine();

            Console.WriteLine("############## Employee");

            NBench.Bench(() => EmployeeDynamic(), "Dynamic Test");
            NBench.Bench(() => EmployeeReflect(), "Reflect Test");
            NBench.Bench(() => EmployeeEmit(), "Emit Test");

            Console.WriteLine("\nPress ENTER to Start Product Test");
            Console.ReadLine();

            Console.WriteLine("############## Product");

            NBench.Bench(() => ProductDynamic(), "Dynamic Test");
            NBench.Bench(() => ProductReflect(), "Reflect Test");
            NBench.Bench(() => ProductEmit(), "Emit Test");

            Console.WriteLine("\nAs can be seen the best performance is either given by the Standard Dynamic DataMapper or by Emited DataMapper.\n" +
                "On the other hand Reflection DataMapper tends to have less performance compared to the other two, " +
                "this is due to the reflection interface being quite more intensive.");
            Console.WriteLine("\nPress ENTER to Exit...");
            Console.ReadLine();
        }

        private static Customer insertTestC = new Customer() {
            CustomerID = "TESTE",
            CompanyName = "Testing code",
            ContactName = "Programmer",
            Address = "IP",
            City = "Lisbon",
            PostalCode = "IDK",
            Region = "Potato",
            Country = "Test",
            Phone = "None",
            Fax = "see above"
        };

        private static Customer updateTestC = new Customer() {
            CustomerID = "ALFKI",
            CompanyName = "Testing code",
            ContactName = "Programmer",
            Address = "IP",
            City = "Lisbon",
            PostalCode = "IDK",
            Region = "Test",
            Country = "Test",
            Phone = "None",
            Fax = "see above"
        };

        public static void CustomerDynamic() {
            IDataMapper test = new CustomerDataMapper(typeof(Customer), connStr, true);
            object id = test.Insert(insertTestC);
            IEnumerable res = test.GetAll();
            Customer actual = (Customer) test.GetById(id);
            test.Delete(actual);
            Customer original = (Customer) test.GetById("ALFKI");
            test.Update(updateTestC);
            test.Update(original);

        }

        public static void CustomerReflect() {
            IDataMapper test = new ReflectDataMapper(typeof(Customer), connStr, true);
            IEnumerable res = test.GetAll();
            object id = test.Insert(insertTestC);
            Customer actual = (Customer) test.GetById(id);
            test.Delete(actual);
            Customer original = (Customer) test.GetById("ALFKI");
            test.Update(updateTestC);
            test.Update(original);
        }

        public static void CustomerEmit() {
            IDataMapper test = EmitDataMapper.Build(typeof(Customer), connStr, true);
            object id = test.Insert(insertTestC);
            IEnumerable res = test.GetAll();
            Customer actual = (Customer) test.GetById(id);
            test.Delete(actual);
            Customer original = (Customer) test.GetById("ALFKI");
            test.Update(updateTestC);
            test.Update(original);
        }

        private static Employee tester = new Employee() {
            LastName = "Silva",
            FirstName = "Manuel",
            Title = "Sales Representative",
            TitleOfCourtesy = "Mr.",
            Address = "There",
            City = "This one",
            Region = "That one",
            PostalCode = "IDK",
            Country = "Portugal",
            HomePhone = "(206) 555-9857",
            Extension = "100"
        };

        public static void EmployeeDynamic() {
            IDataMapper test = new EmployeeDynamicDataMapper(typeof(Employee), connStr, true);
            object id = test.Insert(tester);
            IEnumerable res = test.GetAll();
            Employee actual = (Employee) test.GetById(id);
            test.Delete(actual);
            Employee original = (Employee) test.GetById(1);
            test.Update(tester);
            test.Update(original);
        }

        public static void EmployeeReflect() {
            IDataMapper test = new ReflectDataMapper(typeof(Employee), connStr, true);
            object id = test.Insert(tester);
            IEnumerable res = test.GetAll();
            Employee actual = (Employee) test.GetById(id);
            test.Delete(actual);
            Employee original = (Employee) test.GetById(1);
            test.Update(tester);
            test.Update(original);
        }

        public static void EmployeeEmit() {
            IDataMapper test = EmitDataMapper.Build(typeof(Employee), connStr, true);
            object id = test.Insert(tester);
            IEnumerable res = test.GetAll();
            Employee actual = (Employee) test.GetById(id);
            test.Delete(actual);
            Employee original = (Employee) test.GetById(1);
            test.Update(tester);
            test.Update(original);
        }

        private static Product ProductBuilder(Category c, Supplier s) {
            return new Product() {
                Category = c,
                Supplier = s,
                ProductName = "Bacalhau",
                ReorderLevel = 23,
                UnitsInStock = 100,
                UnitsOnOrder = 40
            };
        }

        public static void ProductDynamic() {
            IDataMapper test = new ProductDataMapper(connStr);
            IDataMapper categories = new CategoryDataMapper(connStr);
            IDataMapper suppliers = new SupplierDataMapper(connStr);
            IEnumerable res = test.GetAll();
            Category c = (Category) categories.GetById(4);
            Supplier s = (Supplier) suppliers.GetById(17);

            object id = test.Insert(ProductBuilder(c, s));
            Product actual = (Product) test.GetById(id);
            test.Delete(actual);

            Product original = (Product) test.GetById(10);
            c = (Category) categories.GetById(4);
            s = (Supplier) suppliers.GetById(17);


            test.Update(ProductBuilder(c, s));
            test.Update(original);
        }

        public static void ProductReflect() {
            IDataMapper test = new ReflectDataMapper(typeof(Product), connStr, true);
            IDataMapper categories = new ReflectDataMapper(typeof(Category), connStr, true);
            IDataMapper suppliers = new ReflectDataMapper(typeof(Supplier), connStr, true);
            IEnumerable res = test.GetAll();
            Category c = (Category) categories.GetById(4);
            Supplier s = (Supplier) suppliers.GetById(17);

            object id = test.Insert(ProductBuilder(c, s));
            Product actual = (Product) test.GetById(id);
            test.Delete(actual);

            Product original = (Product) test.GetById(10);
            c = (Category) categories.GetById(4);
            s = (Supplier) suppliers.GetById(17);


            test.Update(ProductBuilder(c, s));
            test.Update(original);
        }

        public static void ProductEmit() {
            IDataMapper test = EmitDataMapper.Build(typeof(Product), connStr, true);
            IDataMapper categories = EmitDataMapper.Build(typeof(Category), connStr, true);
            IDataMapper suppliers = EmitDataMapper.Build(typeof(Supplier), connStr, true);
            
            Category c = (Category) categories.GetById(4);
            Supplier s = (Supplier) suppliers.GetById(17);

            object id = test.Insert(ProductBuilder(c, s));
            IEnumerable res = test.GetAll();
            Product actual = (Product) test.GetById(id);
            test.Delete(actual);

            Product original = (Product) test.GetById(10);
            c = (Category) categories.GetById(4);
            s = (Supplier) suppliers.GetById(17);


            test.Update(ProductBuilder(c, s));
            test.Update(original);
        }
    }
}
