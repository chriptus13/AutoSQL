using SqlReflect;
using SqlReflectTest.Model;
using System;
using System.Data;
using System.Text;

namespace SqlReflectTest.DataMappers {
    public class ProductDynamicDataMapper : DynamicDataMapper {
        readonly IDataMapper categories, suppliers;

        public ProductDynamicDataMapper(Type klass, string connStr, bool withCache) : base(klass, connStr, withCache) {
            categories = new CategoryDataMapper(connStr);
            suppliers = new SupplierDataMapper(connStr);
        }

        protected override object Load(IDataReader dr) {
            return new Product {
                ProductID = (int) Caster(dr["ProductID"]),
                ProductName = (string) Caster(dr["ProductName"]),
                Supplier = (Supplier) suppliers.GetById(dr["SupplierID"]),
                Category = (Category) categories.GetById(dr["CategoryID"]),
                UnitsInStock = (short) Caster(dr["UnitsInStock"]),
                UnitsOnOrder = (short) Caster(dr["UnitsOnOrder"]),
                ReorderLevel = (short) Caster(dr["ReorderLevel"])
            };
        }

        protected override string SqlDelete(object target) {
            return deleteStmt + '\'' + ((Product) target).ProductID + '\'';
        }

        protected override string SqlInsert(object target) {
            Product p = (Product) target;
            StringBuilder str = new StringBuilder();
            str.Append('\'').Append(p.ProductName).Append("', '")
                .Append(p.Supplier.SupplierID).Append("', '")
                .Append(p.Category.CategoryID).Append("', '")
                .Append(p.UnitsInStock).Append("', '")
                .Append(p.UnitsOnOrder).Append("', '")
                .Append(p.ReorderLevel).Append('\'');
            return String.Format(insertStmt, str.ToString());
        }

        protected override string SqlUpdate(object target) {
            Product p = (Product) target;
            return String.Format(updateStmt,
                "ProductName=" + (p.ProductName == null ? "NULL," : "'" + p.ProductName + "',") +
                "SupplierID=" + p.Supplier.SupplierID + "," +
                "CategoryID=" + p.Category.CategoryID + "," +
                "UnitsInStock=" + p.UnitsInStock + "," +
                "UnitsOnOrder=" + p.UnitsOnOrder + "," +
                "ReorderLevel=" + p.ReorderLevel,
                "" + p.ProductID);
        }
    }
}
