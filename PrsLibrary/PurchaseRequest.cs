using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PrsLibrary {

    public class PurchaseRequest : PrsTable {

        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; private set; }
        public string Description { get; set; }
        public string Justification { get; set; }
        public DateTime DateNeeded { get; set; }
        public string DeliveryMode { get; set; }
        public bool DocsAttached { get; set; }
        public string Status { get; set; }
        public decimal Total { get; private set; }
        public DateTime SubmittedDate { get; private set; }

        private static void AddSqlInsertUpdateParameters(SqlCommand Cmd, PurchaseRequest purchaseRequest) {
            Cmd.Parameters.Add(new SqlParameter("@UserId", purchaseRequest.UserId));
            Cmd.Parameters.Add(new SqlParameter("@Description", purchaseRequest.Description));
            Cmd.Parameters.Add(new SqlParameter("@Justification", purchaseRequest.Justification));
            Cmd.Parameters.Add(new SqlParameter("@DateNeeded", purchaseRequest.DateNeeded));
            Cmd.Parameters.Add(new SqlParameter("@DeliveryMode", purchaseRequest.DeliveryMode));
            Cmd.Parameters.Add(new SqlParameter("@DocsAttached", purchaseRequest.DocsAttached));
            Cmd.Parameters.Add(new SqlParameter("@Status", purchaseRequest.Status));
            Cmd.Parameters.Add(new SqlParameter("@Total", purchaseRequest.Total));
            Cmd.Parameters.Add(new SqlParameter("@SubmittedDate", purchaseRequest.SubmittedDate));
        }

        public static bool Insert(PurchaseRequest purchaseRequest) {
            string Sql = string.Format("insert into [purchaseRequest] " +
                " (UserId, Description, Justification, DateNeeded, DeliveryMode, DocsAttached, Status, Total, SubmittedDate) " +
                " values " +
                " (@UserId, @Description, @Justification, @DateNeeded, @DeliveryMode, @DocsAttached, @Status, @Total, @SubmittedDate)");
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");

            AddSqlInsertUpdateParameters(Cmd, purchaseRequest);

            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Insert Failed!");
            // get the last id inserted
            purchaseRequest.Id = GetLastIdGenerated(ConnStr, "purchaseRequest");

            purchaseRequest.User = User.Select(purchaseRequest.UserId);

            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static bool Update(PurchaseRequest purchaseRequest) {
            string Sql = string.Format("UPDATE [purchaseRequest] Set " +
                    " UserId = @UserId, " +
                    " Description = @Description, " +
                    " Justification = @Justification, " +
                    " DateNeeded = @DateNeeded, " +
                    " DeliveryMode = @DeliveryMode, " +
                    " DocsAttached = @DocsAttached, " +
                    " Status = @Status, " +
                    " Total = @Total, " +
                    " SubmittedDate = @SubmittedDate " +
                    " WHERE ID = @Id; ");
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            Cmd.Parameters.Add(new SqlParameter("@id", purchaseRequest.Id));
            AddSqlInsertUpdateParameters(Cmd, purchaseRequest);

            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Update Failed!");

            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static bool Delete(PurchaseRequest purchaseRequest) {
            string Sql = string.Format("DELETE from [purchaseRequest] where ID = @id");
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            Cmd.Parameters.Add(new SqlParameter("@id", purchaseRequest.Id));
            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Delete Failed!");
            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static PurchaseRequestCollection Select(string WhereClause, string OrderByClause) {
            string Sql = string.Format("SELECT * from [purchaseRequest] WHERE ({0}) ORDER BY {1}", WhereClause, OrderByClause);
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            SqlDataReader Reader = Cmd.ExecuteReader();
            if (!Reader.HasRows) {
                throw new ApplicationException("Result set has no rows!");
            }
            PurchaseRequestCollection purchaseRequests = new PurchaseRequestCollection();
            while (Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                int userId = Reader.GetInt32(Reader.GetOrdinal("UserId"));
                string description = Reader.GetString(Reader.GetOrdinal("Description"));
                string justification = Reader.GetString(Reader.GetOrdinal("Justification"));
                DateTime dateNeeded = Reader.GetDateTime(Reader.GetOrdinal("DateNeeded"));
                string deliveryMode = Reader.GetString(Reader.GetOrdinal("DeliveryMode"));
                bool docsAttached = Reader.GetBoolean(Reader.GetOrdinal("DocsAttached"));
                string status = Reader.GetString(Reader.GetOrdinal("Status"));
                decimal total = Reader.GetDecimal(Reader.GetOrdinal("Total"));
                DateTime submittedDate = Reader.GetDateTime(Reader.GetOrdinal("SubmittedDate"));

                PurchaseRequest purchaseRequest = new PurchaseRequest();
                purchaseRequest.Id = id;
                purchaseRequest.UserId = userId;
                purchaseRequest.Description = description;
                purchaseRequest.Justification = justification;
                purchaseRequest.DateNeeded = dateNeeded;
                purchaseRequest.DeliveryMode = deliveryMode;
                purchaseRequest.DocsAttached = docsAttached;
                purchaseRequest.Status = status;
                purchaseRequest.Total = total;
                purchaseRequest.SubmittedDate = submittedDate;

                // get the user
                purchaseRequest.User = User.Select(purchaseRequest.UserId);

                purchaseRequests.Add(purchaseRequest);
            }

            Cmd.Connection.Close();
            return purchaseRequests;

        }
        public static PurchaseRequest Select(int Id) {
            PurchaseRequestCollection purchaseRequests = PurchaseRequest.Select($"Id = {Id}", "Id");
            PurchaseRequest purchaseRequest = (purchaseRequests.Count == 1) ? purchaseRequests[0] : null;
            return purchaseRequest;
        }
        public static bool Delete(int Id) {
            PurchaseRequest purchaseRequest = PurchaseRequest.Select(Id);
            if (purchaseRequest == null) {
                return false;
            }
            bool rc = PurchaseRequest.Delete(purchaseRequest);
            return rc;
        }

        public PurchaseRequest() {
            this.DateNeeded = DateTime.Now.AddDays(7);  // needed in 7 days
            this.DeliveryMode = "USPS"; // post office
            this.DocsAttached = false; // no docs
            this.Status = "New"; // a new request
            this.Total = 0.0M; // 
            this.SubmittedDate = DateTime.Now;
        }
    }
}
