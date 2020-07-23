using Newtonsoft.Json.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ProductivityTools.Allegro.Selenium.Model;
using ProductivityTools.SeleniumExtensions;
using System;
using System.Collections.Generic;
using System.Threading;

namespace ProductivityTools.Allegro.Selenium
{
    public class SeleniumCalls
    {
        IWebDriver Chrome;

        public SeleniumCalls()
        {
            ChromeOptions options = new ChromeOptions();
            this.Chrome = new ChromeDriver(options);
        }

        public void GetPurchases()
        {
            List<Purchase> purchases = GetPurchasesItems();
            foreach (var purchase in purchases)
            {
                FillPurchase(purchase);
            }
        }

        private void FillPurchase(Purchase purchase)
        {
            this.Chrome.Url = $"{Addresses.Purchased}/{purchase.PurchaseId}";
            var detailsContainer = this.Chrome.FindElement(By.XPath("//*[@data-box-name='Main grid']"));
            purchase.Items = GetPurchaseItems(detailsContainer);


            var deliveryAddresField = detailsContainer.FindElement(By.Id("delivery-address"));
            purchase.DeliveryAddress = deliveryAddresField.InnerText();

            var phoneField = detailsContainer.FindElementByMultipleClass("mqway y1hv2 _s8izy");
            purchase.ReceipmentPhone = phoneField.InnerText();

            var sellerField = detailsContainer.FindElementsByMultipleClass("_3kk7b _vnd3k _1h8s6 _1nucm");
            purchase.SellerName = sellerField[0].InnerText();
            purchase.SellerAddres = sellerField[1].InnerText();
            var sellerContactField = detailsContainer.FindElementsByMultipleClass("_3kk7b _vnd3k _1h8s6 _umw2u");
            purchase.SellerPhone = sellerContactField[0].InnerText();
            purchase.SellerEmail = sellerContactField[1].InnerText();

            Console.WriteLine();

        }

        private List<PurchaseItem> GetPurchaseItems(IWebElement detailsContainer)
        {
            List<PurchaseItem> result = new List<PurchaseItem>();

            var titleboxes = detailsContainer.FindElementsByMultipleClass("p15b4 _3kk7b _vnd3k _1plx6");
            foreach (var titleBox in titleboxes)
            {
                PurchaseItem item = new PurchaseItem();
                result.Add(item);


                var title = titleBox.FindElement(By.TagName("Span"));
                item.Name = title.InnerHtml();

                var amountAndPrice = titleBox.FindElementByMultipleClass("c1npm trz41 _3kk7b _t0xzz _1t6t8");
                var amount = amountAndPrice.InnerText();
                item.Amount = int.Parse(amount.Substring(0, amount.IndexOf('x')).Trim());
                item.SinglePrice = decimal.Parse(amount.TrimEnd('z', 'ł').Substring(amount.IndexOf('x') + 1).Trim());
            }
            return result;
        }

        public List<Purchase> GetPurchasesItems()
        {
            var result = new List<Purchase>();
            Thread.Sleep(2000);
            this.Chrome.Url = Addresses.Purchased;

            var ordersTable = this.Chrome.FindElement(By.Id("my-orders-listing"));
            var orders = ordersTable.FindElementsByIdPart("order-id");
            foreach (var order in orders)
            {
                var detailsLink = order.FindElementByInnerText("a", "Szczegóły");
                var detailsLinkAddress = detailsLink.GetAttribute("href");
                string purchaseId = detailsLinkAddress.Substring(detailsLinkAddress.LastIndexOf("/"));
                Purchase purchase = new Purchase(purchaseId);
                result.Add(purchase);
            }

            return result;
        }

        public void Login(string login, string password)
        {
            this.Chrome.Url = Addresses.LoginPage;

            IWebElement opboxfragment = this.Chrome.FindElement(By.ClassName("opbox-fragment"));

            Thread.Sleep(1000);
            var go = opboxfragment.FindElementByInnerText("button", "przejdź dalej");
            if (go != null)
            {
                go.Click();
            }

            var usernamefield = this.Chrome.FindElement(By.Id("username"));
            usernamefield.SendKeys(login);

            var passwordfield = this.Chrome.FindElement(By.Id("password"));
            passwordfield.SendKeys(password);

            var loginbutton = this.Chrome.FindElement(By.Id("login-button"));
            loginbutton.Click();
        }
    }
}
