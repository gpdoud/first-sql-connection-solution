using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace PrsLibrary {

    public class PrsTable {
        protected static SqlCommand CreateConnection(string ConnStr, string Sql, string message) {
            SqlConnection Conn = new SqlConnection(ConnStr);
            Conn.Open();
            if (Conn.State != ConnectionState.Open) {
                throw new ApplicationException(message);
            }
            SqlCommand Cmd = new SqlCommand(Sql, Conn);
            return Cmd;
        }
        protected static int GetLastIdGenerated(string ConnStr, string TableName) {
            string sql = string.Format("SELECT IDENT_CURRENT('{0}')", TableName);
            SqlCommand Cmd = CreateConnection(ConnStr, sql, "Failed to get id!");
            object newId = Cmd.ExecuteScalar();
            return int.Parse(newId.ToString());
        }
        protected static int ExecuteSqlInsUpdDelCommand(SqlCommand Cmd, string message) {
            int recsAffected = Cmd.ExecuteNonQuery();
            if (recsAffected != 1) {
                throw new ApplicationException(message);
            }
            return recsAffected;
        }
    }
}
