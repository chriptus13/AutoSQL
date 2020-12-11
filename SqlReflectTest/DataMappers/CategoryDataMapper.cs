﻿using SqlReflect;
using SqlReflectTest.Model;
using System;
using System.Data;

namespace SqlReflectTest.DataMappers {
    public class CategoryDataMapper : AbstractDataMapper {
        const string COLUMNS = "CategoryName, Description";
        const string SQL_GET_ALL = @"SELECT CategoryID, " + COLUMNS + " FROM Categories";
        const string SQL_GET_BY_ID = SQL_GET_ALL + " WHERE CategoryID=";
        const string SQL_INSERT = "INSERT INTO Categories (" + COLUMNS + ") OUTPUT INSERTED.CategoryID VALUES ";
        const string SQL_DELETE = "DELETE FROM Categories WHERE CategoryID = ";
        const string SQL_UPDATE = "UPDATE Categories SET CategoryName={1}, Description={2} WHERE CategoryID = {0}";

        public CategoryDataMapper(string connStr) : base(connStr) { }

        protected override string SqlGetAll() {
            return SQL_GET_ALL;
        }

        protected override string SqlGetById(object id) {
            return SQL_GET_BY_ID + id;
        }

        protected override object Load(IDataReader dr) {
            return new Category {
                CategoryID = (int) dr["CategoryID"],
                CategoryName = (string) dr["CategoryName"],
                Description = (string) dr["Description"]
            };
        }

        protected override string SqlInsert(object target) {
            Category c = (Category) target;
            string values = "'" + c.CategoryName + "' , '" + c.Description + "'";
            return SQL_INSERT + "(" + values + ")";
        }

        protected override string SqlUpdate(object target) {
            Category c = (Category) target;
            return String.Format(SQL_UPDATE,
                c.CategoryID,
                "'" + c.CategoryName + "'",
                "'" + c.Description + "'");
        }

        protected override string SqlDelete(object target) {
            return SQL_DELETE + ((Category) target).CategoryID;
        }
    }
}
