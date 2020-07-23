using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityTools.Allegro.Selenium.Model
{
    public class PurchaseItem
    {
        public string Name { get; set; }
        public decimal SinglePrice { get; set; }
        public int Amount { get; set; }
        public int TotalPrice { get; set; }
    }
}
