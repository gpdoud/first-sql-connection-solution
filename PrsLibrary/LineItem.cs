using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PrsLibrary {

    internal class LineItem : PrsTable {

        public int Id { get; set; }
        public int PurchaseRequestId { get; set; }
        public PurchaseRequest PurchaseRequest { get; private set; }
        public int ProductId { get; set; }
        public Product Product { get; private set; }
        public int Quantity { get; set; }

        private static void AddSqlInsertUpdateParameters(SqlCommand Cmd, LineItem lineItem) {
            Cmd.Parameters.Add(new SqlParameter("@PurchaseRequestId", lineItem.PurchaseRequestId));
            Cmd.Parameters.Add(new SqlParameter("@ProductId", lineItem.ProductId));
            Cmd.Parameters.Add(new SqlParameter("@Quantity", lineItem.Quantity));
        }

        public static bool Insert(LineItem lineItem) {
            string Sql = string.Format("insert into [lineItem] " +
                " (PurchaseRequestId, ProductId, Quantity) " +
                " values " +
                " (@PurchaseRequestId, @ProductId, @Quantity)");
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");

            AddSqlInsertUpdateParameters(Cmd, lineItem);

            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Insert Failed!");
            // get the last id inserted
            lineItem.Id = GetLastIdGenerated(ConnStr, "lineItem");

            lineItem.PurchaseRequest = PurchaseRequest.Select(lineItem.PurchaseRequestId);
            lineItem.Product = Product.Select(lineItem.ProductId);

            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static bool Update(LineItem lineItem) {
            string Sql = string.Format("UPDATE [lineItem] Set " +
                    " PurchaseRequestId = @PurchaseRequestId, " +
                    " ProductId = @ProductId, " +
                    " Quantity = @Quantity " +
                    " WHERE ID = @Id; ");
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            Cmd.Parameters.Add(new SqlParameter("@id", lineItem.Id));
            AddSqlInsertUpdateParameters(Cmd, lineItem);

            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Update Failed!");

            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static bool Delete(LineItem lineItem) {
            string Sql = string.Format("DELETE from [lineItem] where ID = @id");
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            Cmd.Parameters.Add(new SqlParameter("@id", lineItem.Id));
            int recsAffected = ExecuteSqlInsUpdDelCommand(Cmd, "Delete Failed!");
            Cmd.Connection.Close();
            return (recsAffected == 1);
        }
        public static LineItemCollection Select(string WhereClause, string OrderByClause) {
            string Sql = string.Format("SELECT * from [lineItem] WHERE ({0}) ORDER BY {1}", WhereClause, OrderByClause);
            //string ConnStr = @"Server=DSI-WORKSTATION\SQLEXPRESS;Database=prs;Trusted_Connection=True;";
            SqlCommand Cmd = CreateConnection(ConnStr, Sql, "Connection didn't open");
            SqlDataReader Reader = Cmd.ExecuteReader();
            //if (!Reader.HasRows) {
            //    throw new ApplicationException("Result set has no rows!");
            //}
            LineItemCollection lineItems = new LineItemCollection();
            while (Reader.Read()) {
                int id = Reader.GetInt32(Reader.GetOrdinal("Id"));
                int purchaseRequestId = Reader.GetInt32(Reader.GetOrdinal("PurchaseRequestId"));
                int productId = Reader.GetInt32(Reader.GetOrdinal("ProductId"));
                int quantity = Reader.GetInt32(Reader.GetOrdinal("Quantity"));

                LineItem lineItem = new LineItem();
                lineItem.Id = id;
                lineItem.PurchaseRequestId = purchaseRequestId;
                lineItem.ProductId = productId;
                lineItem.Quantity = quantity;

                // get the PurchaseRequest
                lineItem.PurchaseRequest = PurchaseRequest.Select(lineItem.PurchaseRequestId);
                // get the Product
                lineItem.Product = Product.Select(lineItem.ProductId);

                lineItems.Add(lineItem);
            }

            Cmd.Connection.Close();
            return lineItems;

        }
        public static LineItem Select(int Id) {
            LineItemCollection lineItems = LineItem.Select($"Id = {Id}", "Id");
            LineItem lineItem = (lineItems.Count == 1) ? lineItems[0] : null;
            return lineItem;
        }
        public static bool Delete(int Id) {
            LineItem lineItem = LineItem.Select(Id);
            if (lineItem == null) {
                return false;
            }
            bool rc = LineItem.Delete(lineItem);
            return rc;
        }

        public LineItem() {
            this.Quantity = 1;
        }
    }
}
