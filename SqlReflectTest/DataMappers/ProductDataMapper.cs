using SqlReflect;
using SqlReflectTest.Model;
using System;
using System.Data;
using System.Text;

namespace SqlReflectTest.DataMappers {
    public class ProductDataMapper : AbstractDataMapper {
        const string COLUMNS = "ProductName, SupplierID, CategoryID, UnitsInStock, UnitsOnOrder, ReorderLevel";
        const string SQL_GET_ALL = "SELECT ProductID, " + COLUMNS + " FROM Products";
        const string SQL_GET_BY_ID = SQL_GET_ALL + " WHERE ProductId=";
        const string SQL_INSERT = "INSERT INTO Products (" + COLUMNS + ") OUTPUT INSERTED.ProductId VALUES ";
        const string SQL_DELETE = "DELETE FROM Products WHERE ProductId = ";
        const string SQL_UPDATE = "UPDATE Products SET {0} WHERE ProductId={1}";

        readonly IDataMapper categories;
        readonly IDataMapper suppliers;

        public ProductDataMapper(string connStr) : base(connStr) {
            categories = new CategoryDataMapper(connStr);
            suppliers = new SupplierDataMapper(connStr);
        }

        protected override string SqlGetAll() {
            return SQL_GET_ALL;
        }
        protected override string SqlGetById(object id) {
            return SQL_GET_BY_ID + id;
        }

        protected override string SqlInsert(object target) {
            Product p = (Product) target;
            string values = "('" + p.ProductName + "', "
                + "'" + p.Supplier.SupplierID + "', "
                + "'" + p.Category.CategoryID + "', "
                + "'" + p.UnitsInStock + "', "
                + "'" + p.UnitsOnOrder + "', "
                + "'" + p.ReorderLevel + "')";
            return SQL_INSERT + values;
        }

        protected override string SqlUpdate(object target) {
            Product p = (Product) target;
            StringBuilder str = new StringBuilder();
            str.Append("ProductName='").Append(p.ProductName).Append("', SupplierID='").Append(p.Supplier.SupplierID)
                .Append("', CategoryID='").Append(p.Category.CategoryID).Append("', UnitsInStock='").Append(p.UnitsInStock)
                .Append("', UnitsOnOrder='").Append(p.UnitsOnOrder).Append("', ReorderLevel='").Append(p.ReorderLevel).Append('\'');
            return String.Format(SQL_UPDATE, str.ToString(), "'" + p.ProductID + "'");
        }

        protected override string SqlDelete(object target) {
            return SQL_DELETE + ((Product) target).ProductID;
        }

        protected override object Load(IDataReader dr) {
            return new Product {
                ProductID = (int) dr["ProductID"],
                ProductName = (string) dr["ProductName"],
                Supplier = (Supplier) suppliers.GetById(dr["SupplierID"]),
                Category = (Category) categories.GetById(dr["CategoryID"]),
                UnitsInStock = (short) dr["UnitsInStock"],
                UnitsOnOrder = (short) dr["UnitsOnOrder"],
                ReorderLevel = (short) dr["ReorderLevel"]
            };
        }
    }
}
