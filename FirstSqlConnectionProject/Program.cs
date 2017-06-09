using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using PrsLibrary;

namespace FirstSqlConnectionProject {
    class Program {
        static void Main(string[] args) {

            //            string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            //            SqlConnection Conn = new SqlConnection(ConnStr);
            //            Conn.Open();
            //            if(Conn.State != ConnectionState.Open) {
            //                throw new ApplicationException("Connection didn't open");
            //            }

            //            // Insert
            //            string SqlInsert = @"insert [user] (UserName, Password, FirstName, LastName, Phone, Email, IsAdmin, IsReviewer)
            //values(@username, 'password', 'Greg', 'Doud', '513-703-7315', 'gpdoud@gmail.com', @isadmin, 1)";
            //            SqlCommand CmdInsert = new SqlCommand(SqlInsert, Conn);
            //            CmdInsert.Parameters.Add(new SqlParameter("@username", "gpdoud"));
            //            CmdInsert.Parameters.Add(new SqlParameter("@isadmin", "1"));
            //            int recsAffected = CmdInsert.ExecuteNonQuery();
            //            if(recsAffected != 1) {
            //                throw new ApplicationException("Insert failed!");
            //            }

            // Select
            string whereClause = "LastName = 'Doud'";
            string orderByClause = "UserName desc";
            UserCollection users = User.Select(whereClause, orderByClause);
            User user = users[0];
            user.FirstName = "Greg";
            bool success = User.Update(user);
            success = User.Delete(user);
            User user2Insert = new User {
                UserName = "gpdoud",
                Password = "password",
                FirstName = "Nbr 23",
                LastName = "Doud",
                Phone = "513-555-1212",
                Email = "gdoud@maxtrain.com",
                IsReviewer = false,
                IsAdmin = false
            };
            success = User.Insert(user2Insert);

            VendorCollection vendors = Vendor.Select("1 = 1", "Id");
            Vendor vendor = new Vendor {
                Code = "Krog0010",
                Name = "Kroger",
                Address = "123 Any Street",
                City = "Cincinnati", State = "OH", Zip = "45201",
                Email = "info@kroger.com", Phone = "513-555-1212",
                IsRecommended = true
            };
            bool rc = Vendor.Insert(vendor);
            vendor.Code = "KROG1111";
            rc = Vendor.Update(vendor);
            rc = Vendor.Delete(vendor);

            ProductCollection products = Product.Select("1 = 1", "Id");
            Product product = new Product {
                VendorId = 2,
                Name = "Widget1",
                VendorPartNumber = "BWidget",
                Price = 12345.67M,
                Unit = "Each",
                PhotoPath = "http://www.maxtrain.com"
            };
            rc = Product.Insert(product);
            product.Price = 76543.21m;
            rc = Product.Update(product);
            rc = Product.Delete(product);

            int i = 0;
        }
    }
}
