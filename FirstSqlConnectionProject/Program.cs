using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace FirstSqlConnectionProject {
    class Program {
        static void Main(string[] args) {

            string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlConnection Conn = new SqlConnection(ConnStr);
            Conn.Open();
            if(Conn.State != ConnectionState.Open) {
                throw new ApplicationException("Connection didn't open");
            }

            // Insert
            string SqlInsert = @"insert [user] (UserName, Password, FirstName, LastName, Phone, Email, IsAdmin, IsReviewer)
values(@username, 'password', 'Greg', 'Doud', '513-703-7315', 'gpdoud@gmail.com', @isadmin, 1)";
            SqlCommand CmdInsert = new SqlCommand(SqlInsert, Conn);
            CmdInsert.Parameters.Add(new SqlParameter("@username", "gpdoud"));
            CmdInsert.Parameters.Add(new SqlParameter("@isadmin", "1"));
            int recsAffected = CmdInsert.ExecuteNonQuery();
            if(recsAffected != 1) {
                throw new ApplicationException("Insert failed!");
            }

            // Select
            string Sql = "Select * from [user]";
            SqlCommand Cmd = new SqlCommand(Sql, Conn);
            SqlDataReader Reader = Cmd.ExecuteReader();
            if(!Reader.HasRows) {
                throw new ApplicationException("Result set has no rows!");
            }
            while(Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                string userName = Reader.GetString(Reader.GetOrdinal("UserName"));
                string firstName = Reader.GetString(Reader.GetOrdinal("FirstName"));
                string lastName = Reader.GetString(Reader.GetOrdinal("LastName"));
                string phone = Reader.GetString(Reader.GetOrdinal("Phone"));
                string email = Reader.GetString(Reader.GetOrdinal("Email"));
                bool isReviewer = Reader.GetBoolean(Reader.GetOrdinal("IsReviewer"));
                bool isAdmin = Reader.GetBoolean(Reader.GetOrdinal("IsAdmin"));

                string message = string.Format("{0} - {1} {2}", id, firstName, lastName);
                System.Diagnostics.Debug.WriteLine(message);
            }

            Conn.Close();
        }
    }
}
