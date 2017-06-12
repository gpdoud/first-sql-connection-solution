using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace PrsLibrary {

    public class Vendor : PrsTable {

        public int Id { get; set; }
        public string  Code { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsRecommended { get; set; }

        private static void AddSqlInsertUpdateParameters(SqlCommand Cmd, Vendor vendor) {
            Cmd.Parameters.Add(new SqlParameter("@code", vendor.Code));
            Cmd.Parameters.Add(new SqlParameter("@name", vendor.Name));
            Cmd.Parameters.Add(new SqlParameter("@address", vendor.Address));
            Cmd.Parameters.Add(new SqlParameter("@city", vendor.City));
            Cmd.Parameters.Add(new SqlParameter("@state", vendor.State));
            Cmd.Parameters.Add(new SqlParameter("@zip", vendor.Zip));
            Cmd.Parameters.Add(new SqlParameter("@phone", vendor.Phone));
            Cmd.Parameters.Add(new SqlParameter("@email", vendor.Email));
            Cmd.Parameters.Add(new SqlParameter("@isRecommended", vendor.IsRecommended));
        }

        public static bool Insert(Vendor vendor) {
            string Sql = string.Format("insert into [vendor] " +
                " (Code, Name, Address, City, State, Zip, Phone, Email, IsRecommended) " +
                " values " +
                " (@code, @name, @address, @city, @state, @zip, @phone, @email, @isRecommended)");
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");

            AddSqlInsertUpdateParameters(Cmd, vendor);

            int recsAffected = Cmd.ExecuteNonQuery();
            if (recsAffected != 1) {
                throw new ApplicationException("Insert Failed!");
            }
            // get the last id inserted
            vendor.Id = GetLastIdGenerated(ConnStr, "Vendor");

            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static bool Update(Vendor vendor) {
            string Sql = string.Format("UPDATE [vendor] Set " +
                    " Code = @code, " +
                    " Name = @name, " +
                    " Address = @address, " +
                    " City = @city, " +
                    " State = @state, " +
                    " Zip = @zip, " +
                    " Phone = @phone, " +
                    " Email = @email, " +
                    " IsRecommended = @isRecommended " +
                    " WHERE ID = @Id; ");
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            Cmd.Parameters.Add(new SqlParameter("@id", vendor.Id));
            AddSqlInsertUpdateParameters(Cmd, vendor);

            int recsAffected = Cmd.ExecuteNonQuery();
            if (recsAffected != 1) {
                throw new ApplicationException("Update Failed!");
            }
            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static bool Delete(Vendor vendor) {
            string Sql = string.Format("DELETE from [vendor] where ID = @id");
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            Cmd.Parameters.Add(new SqlParameter("@id", vendor.Id));
            int recsAffected = Cmd.ExecuteNonQuery();
            if (recsAffected != 1) {
                throw new ApplicationException("Delete Failed!");
            }
            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static VendorCollection Select(string WhereClause, string OrderByClause) {
            string Sql = string.Format("SELECT * from [vendor] WHERE ({0}) ORDER BY {1}", WhereClause, OrderByClause);
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            SqlDataReader Reader = Cmd.ExecuteReader();
            if (!Reader.HasRows) {
                throw new ApplicationException("Result set has no rows!");
            }
            VendorCollection vendors = new VendorCollection();
            while (Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                string code = Reader.GetString(Reader.GetOrdinal("Code"));
                string name = Reader.GetString(Reader.GetOrdinal("Name"));
                string address = Reader.GetString(Reader.GetOrdinal("Address"));
                string city = Reader.GetString(Reader.GetOrdinal("City"));
                string state = Reader.GetString(Reader.GetOrdinal("State"));
                string zip = Reader.GetString(Reader.GetOrdinal("Zip"));
                string phone = Reader.GetString(Reader.GetOrdinal("Phone"));
                string email = Reader.GetString(Reader.GetOrdinal("Email"));
                bool isRecommended = Reader.GetBoolean(Reader.GetOrdinal("IsRecommended"));

                Vendor vendor = new Vendor();
                vendor.Id = id;
                vendor.Code = code;
                vendor.Name = name;
                vendor.Address = address;
                vendor.City = city;
                vendor.State = state;
                vendor.Zip = zip;
                vendor.Email = email;
                vendor.Phone = phone;
                vendor.IsRecommended = isRecommended;

                vendors.Add(vendor);
            }

            Cmd.Connection.Close();
            return vendors;

        }
        public static Vendor Select(int Id) {
            VendorCollection vendors = Vendor.Select($"Id = {Id}", "Id");
            Vendor vendor = (vendors.Count == 1) ? vendors[0] : null;
            return vendor;
        }
        public static bool Delete(int Id) {
            Vendor vendor = Vendor.Select(Id);
            if (vendor == null) {
                return false;
            }
            bool rc = Vendor.Delete(vendor);
            return rc;
        }
    }
}
