using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityTools.Allegro.Selenium.Model
{
    public class Purchase
    {
        public string PurchaseId { get; set; }
        public List<PurchaseItem> Items { get; set; }

        public Purchase(string purchaseId)
        {
            this.PurchaseId = purchaseId;
        }
    }
}
