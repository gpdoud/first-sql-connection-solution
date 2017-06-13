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

            PurchaseRequestCollection purchaseRequests = PurchaseRequest.Select("1 = 1", "Id");
            PurchaseRequest purchaseRequest = new PurchaseRequest();
            purchaseRequest.UserId = 1;
            purchaseRequest.Description = "My first purchase request";
            purchaseRequest.Justification = "My birthday";
            purchaseRequest.DateNeeded = DateTime.Now;
            purchaseRequest.DeliveryMode = "USPS";
            purchaseRequest.DocsAttached = false;
            purchaseRequest.Status = "New";
            //purchaseRequest.Total = 0.0M;
            //purchaseRequest.SubmittedDate = DateTime.Now;
            rc = PurchaseRequest.Insert(purchaseRequest);
            purchaseRequest.DateNeeded = DateTime.Now.AddDays(1);
            rc = PurchaseRequest.Update(purchaseRequest);
            purchaseRequests = PurchaseRequest.Select("1 = 1", "Id");
            rc = PurchaseRequest.Delete(purchaseRequest);
            purchaseRequests = PurchaseRequest.Select("1 = 1", "Id");

            //LineItemCollection lineitems = LineItem.Select("1 = 1", "Id");
            //LineItem lineitem = new LineItem {
            //    PurchaseRequestId = 3,
            //    ProductId = 3,
            //    Quantity = 1
            //};
            //rc = LineItem.Insert(lineitem);
            //PurchaseRequest NewPurchaseRequest = new PurchaseRequest {
            //    UserId = 2,
            //    Description = "First full add of PR",
            //    Justification = "I just want to"
            //};
            //rc = PurchaseRequest.Insert(NewPurchaseRequest);
            //rc = NewPurchaseRequest.AddLineItem(1, 10);
            //rc = NewPurchaseRequest.AddLineItem(2, 20);
            //rc = NewPurchaseRequest.AddLineItem(3, 30);
            PurchaseRequest NewPurchaseRequest = PurchaseRequest.Select(14);
            rc = NewPurchaseRequest.AddLineItem(7, 1);

            int i = 0;
        }
    }
}
