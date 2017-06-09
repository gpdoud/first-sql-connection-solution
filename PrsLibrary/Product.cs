using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace PrsLibrary {

    public class Product : PrsTable {

        public int Id { get; set; }
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public string Name { get; set; }
        public string VendorPartNumber { get; set; }
        public decimal Price { get; set; }
        public string Unit { get; set; }
        public string PhotoPath { get; set; }

        private static void AddSqlInsertUpdateParameters(SqlCommand Cmd, Product product) {
            Cmd.Parameters.Add(new SqlParameter("@VendorId", product.VendorId));
            Cmd.Parameters.Add(new SqlParameter("@Name", product.Name));
            Cmd.Parameters.Add(new SqlParameter("@VendorPartNumber", product.VendorPartNumber));
            Cmd.Parameters.Add(new SqlParameter("@Price", product.Price));
            Cmd.Parameters.Add(new SqlParameter("@Unit", product.Unit));
            Cmd.Parameters.Add(new SqlParameter("@PhotoPath", product.PhotoPath));
        }

        public static bool Insert(Product product) {
            string Sql = string.Format("insert into [product] " +
                " (VendorId, Name, VendorPartNumber, Price, Unit, PhotoPath) " +
                " values " +
                " (@VendorId, @Name, @VendorPartNumber, @Price, @Unit, @PhotoPath)");
            string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");

            AddSqlInsertUpdateParameters(Cmd, product);

            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Insert failed!");

            product.Id = GetLastIdGenerated(ConnStr, "Product");

            // get the vendor
            VendorCollection vendors = Vendor.Select("Id = " + product.VendorId, "Id");
            if(vendors.Count > 0) {
                product.Vendor = vendors[0];
            }

            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static bool Update(Product product) {
            string Sql = string.Format("UPDATE [product] Set " +
                    " VendorId = @VendorId, " +
                    " Name = @Name, " +
                    " VendorPartNumber = @VendorPartNumber, " +
                    " Price = @Price, " +
                    " Unit = @Unit, " +
                    " PhotoPath = @PhotoPath " +
                    " WHERE ID = @Id; ");
            string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            Cmd.Parameters.Add(new SqlParameter("@id", product.Id));
            AddSqlInsertUpdateParameters(Cmd, product);

            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Update Failed!");

            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static bool Delete(Product product) {
            string Sql = string.Format("DELETE from [product] where ID = @id");
            string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            Cmd.Parameters.Add(new SqlParameter("@id", product.Id));

            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Delete Failed!");

            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static ProductCollection Select(string WhereClause, string OrderByClause) {
            string Sql = string.Format("SELECT * from [product] WHERE ({0}) ORDER BY {1}", WhereClause, OrderByClause);
            string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            SqlDataReader Reader = Cmd.ExecuteReader();
            if (!Reader.HasRows) {
                throw new ApplicationException("Result set has no rows!");
            }
            ProductCollection products = new ProductCollection();
            while (Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                int vendorId = Reader.GetInt32(Reader.GetOrdinal("VendorId"));
                string name = Reader.GetString(Reader.GetOrdinal("Name"));
                string vendorPartNumber = Reader.GetString(Reader.GetOrdinal("VendorPartNumber"));
                decimal price = Reader.GetDecimal(Reader.GetOrdinal("Price"));
                string unit = Reader.GetString(Reader.GetOrdinal("Unit"));
                string photoPath = String.Empty;

                if(!Reader.IsDBNull(Reader.GetOrdinal("PhotoPath"))) {
                    photoPath = Reader.GetString(Reader.GetOrdinal("PhotoPath"));
                }
                Vendor vendor = Vendor.Select(vendorId);

                Product product = new Product();
                product.Id = id;
                product.Vendor = vendor;
                product.VendorId = vendor.Id;
                product.Name = name;
                product.VendorPartNumber = vendorPartNumber;
                product.Price = price;
                product.Unit = unit;
                product.PhotoPath = photoPath;

                products.Add(product);
            }

            Cmd.Connection.Close();
            return products;

        }
        public static Product Select(int Id) {
            ProductCollection products = Product.Select($"Id = {Id}", "Id");
            Product product = (products.Count == 1) ? products[0] : null;
            return product;
        }
        public static bool Delete(int Id) {
            Product product = Product.Select(Id);
            if (product == null) {
                return false;
            }
            bool rc = Product.Delete(product);
            return rc;
        }
    }

}
