using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace PrsLibrary {

    public class User : PrsTable {

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public bool IsReviewer { get; set; }
        public bool IsAdmin { get; set; }

        private static void AddSqlInsertUpdateParameters(SqlCommand Cmd, User user) {
            Cmd.Parameters.Add(new SqlParameter("@username", user.UserName));
            Cmd.Parameters.Add(new SqlParameter("@password", user.Password));
            Cmd.Parameters.Add(new SqlParameter("@firstname", user.FirstName));
            Cmd.Parameters.Add(new SqlParameter("@lastname", user.LastName));
            Cmd.Parameters.Add(new SqlParameter("@phone", user.Phone));
            Cmd.Parameters.Add(new SqlParameter("@email", user.Email));
            Cmd.Parameters.Add(new SqlParameter("@isreviewer", user.IsReviewer));
            Cmd.Parameters.Add(new SqlParameter("@isadmin", user.IsAdmin));
        }

        public static bool Insert(User user) {
            string Sql = string.Format("insert into [user] " +
                " (UserName, Password, FirstName, LastName, Phone, Email, IsReviewer, IsAdmin) " +
                " values " +
                " (@username, @password, @firstname, @lastname, @phone, @email, @isreviewer, @isadmin)");
            string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");

            AddSqlInsertUpdateParameters(Cmd, user);

            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Insert Failed!");
            //int recsAffected = Cmd.ExecuteNonQuery();
            //if (recsAffected != 1) {
            //    throw new ApplicationException("Insert Failed!");
            //}
            user.Id = GetLastIdGenerated(ConnStr, "User");
            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static bool Update(User user) {
            string Sql = string.Format("UPDATE [user] Set " +
                    " UserName = @username, " +
                    " Password = @password, " +
                    " FirstName = @firstname, " +
                    " LastName = @lastname, " +
                    " Phone = @phone, " +
                    " Email = @email, " +
                    " IsReviewer = @isreviewer, " +
                    " IsAdmin = @isadmin " +
                    " WHERE ID = @Id; ");
            string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            Cmd.Parameters.Add(new SqlParameter("@id", user.Id));
            AddSqlInsertUpdateParameters(Cmd, user);

            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Update Failed!");

            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static bool Delete(User user) {
            string Sql = string.Format("DELETE from [user] where ID = @id");
            string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            Cmd.Parameters.Add(new SqlParameter("@id", user.Id));
            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Delete Failed!");

            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static UserCollection Select(string whereClause, string orderByClause) {
            string Sql = string.Format("SELECT * from [User] WHERE ({0}) ORDER BY {1}", whereClause, orderByClause);
            string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            SqlDataReader Reader = Cmd.ExecuteReader();
            if (!Reader.HasRows) {
                throw new ApplicationException("Result set has no rows!");
            }
            UserCollection users = new UserCollection();
            while (Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                string userName = Reader.GetString(Reader.GetOrdinal("UserName"));
                string password = Reader.GetString(Reader.GetOrdinal("Password"));
                string firstName = Reader.GetString(Reader.GetOrdinal("FirstName"));
                string lastName = Reader.GetString(Reader.GetOrdinal("LastName"));
                string phone = Reader.GetString(Reader.GetOrdinal("Phone"));
                string email = Reader.GetString(Reader.GetOrdinal("Email"));
                bool isReviewer = Reader.GetBoolean(Reader.GetOrdinal("IsReviewer"));
                bool isAdmin = Reader.GetBoolean(Reader.GetOrdinal("IsAdmin"));

                User user = new User();
                user.Id = id;
                user.UserName = userName;
                user.Password = password;
                user.FirstName = firstName;
                user.LastName = lastName;
                user.Email = email;
                user.Phone = phone;
                user.IsReviewer = isReviewer;
                user.IsAdmin = isAdmin;

                users.Add(user);
            }

            Cmd.Connection.Close();
            return users;
        }

        public static User Select(int Id) {
            UserCollection users = User.Select($"Id = {Id}", "Id");
            User user = (users.Count == 1) ? users[0] : null;
            return user;
        }
        public static bool Delete(int Id) {
            User user = User.Select(Id);
            if(user == null) {
                return false;
            }
            bool rc = User.Delete(user);
            return rc;
        }
    }
}
