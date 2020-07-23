using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityTools.Allegro.Selenium.Model
{
    public class Purchase
    {
        public string PurchaseId { get; set; }
        public string DeliveryAddress { get; set; }
        public string ReceipmentPhone { get; set; }
        public List<PurchaseItem> Items { get; set; }
        public string SellerName { get; set; }
        public string SellerAddres { get; set; }
        public string SellerPhone { get; set; }
        public string SellerEmail { get; set; }
        public string Status { get; set; }

        public Purchase(string purchaseId)
        {
            this.PurchaseId = purchaseId;
        }
    }
}
