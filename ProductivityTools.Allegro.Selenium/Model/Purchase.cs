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
        public string Status { get; set; }
        public List<PurchaseItem> Items { get; set; }
        public Dealer Dealer { get; set; }
        public Payment Payment { get; set; }
        public Return Return { get; set; }
        public List<Delivery> Delivery { get; set; }

        public Purchase(string purchaseId)
        {
            this.PurchaseId = purchaseId;
            this.Dealer = new Dealer();
            this.Payment = new Payment();        
            this.Delivery = new List<Delivery>();
        }
    }
}
