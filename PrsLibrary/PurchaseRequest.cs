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
        public LineItemCollection LineItems { get; set; }

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
            //if (!Reader.HasRows) {
            //    throw new ApplicationException("Result set has no rows!");
            //}
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

                PurchaseRequest purchaseRequest = new PurchaseRequest {
                    Id = id,
                    UserId = userId,
                    Description = description,
                    Justification = justification,
                    DateNeeded = dateNeeded,
                    DeliveryMode = deliveryMode,
                    DocsAttached = docsAttached,
                    Status = status,
                    Total = total,
                    SubmittedDate = submittedDate
                };

                // get the user
                purchaseRequest.User = User.Select(purchaseRequest.UserId);
                // get the line items
                purchaseRequest.LineItems = GetLineItems(purchaseRequest.Id);

                purchaseRequests.Add(purchaseRequest);
            }

            Cmd.Connection.Close();
            return purchaseRequests;

        }
        private void UpdateLineItemsProperty() {
            this.LineItems = GetLineItems(this.Id);
        }
        private static LineItemCollection GetLineItems(int PurchaseRequestId) {
            LineItemCollection lineItems
                = LineItem.Select($"PurchaseRequestId = {PurchaseRequestId}", "Id");
            return lineItems;
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
            LineItems = new LineItemCollection();
        }

        public bool AddLineItem(int ProductId, int Quantity) {
            Product product = Product.Select(ProductId);
            LineItem lineItem = new LineItem {
                PurchaseRequestId = this.Id,
                ProductId = ProductId,
                Quantity = Quantity
            };
            bool rc = LineItem.Insert(lineItem);
            if (!rc)
                throw new ApplicationException("Insert of line item failed!");
            this.Total += Quantity * product.Price;
            rc = PurchaseRequest.Update(this);
            UpdateLineItemsProperty();
            return rc;
        }
        public bool DeleteLineItem(int LineItemId) {
            LineItem lineItem = LineItem.Select(LineItemId);
            if(lineItem == null) {
                throw new ApplicationException("Line item to delete is not found");
            }
            decimal amount = lineItem.Product.Price * lineItem.Quantity;
            bool rc = LineItem.Delete(lineItem);
            if(!rc) {
                throw new ApplicationException("Line item delete failed!");
            }
            this.Total -= amount;
            rc = PurchaseRequest.Update(this);
            if (!rc) {
                throw new ApplicationException("Purchase Request update failed!");
            }
            UpdateLineItemsProperty();
            return rc;
        }
        public bool UpdateLineItem(int LineItemId, int NewQuantity) {
            LineItem lineItem = LineItem.Select(LineItemId);
            if (lineItem == null) {
                throw new ApplicationException("Line item to update is not found");
            }
            if(NewQuantity < 0) {
                throw new ApplicationException("New Quantity cannot be less then zero");
            }
            decimal oldAmount = lineItem.Product.Price * lineItem.Quantity;
            lineItem.Quantity = NewQuantity;
            decimal newAmount = lineItem.Product.Price * lineItem.Quantity;
            decimal changeTotal = newAmount - oldAmount;
            bool rc = LineItem.Update(lineItem);
            if (!rc) {
                throw new ApplicationException("Line item update failed!");
            }
            this.Total += changeTotal;
            rc = PurchaseRequest.Update(this);
            if (!rc) {
                throw new ApplicationException("Purchase Request update failed!");
            }
            UpdateLineItemsProperty();
            return rc;
        }
    }
}
