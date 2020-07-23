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
            var detailsContainer=this.Chrome.FindElement(By.XPath("//*[@data-box-name='Main grid']"));
            var titleboxes = detailsContainer.FindElements(By.XPath("//*[@class='p15b4 _3kk7b _vnd3k _1plx6']"));
            foreach (var titleBox in titleboxes)
            {
              

                var title=titleBox.FindElement(By.TagName("Span"));
                Console.WriteLine(title.GetAttribute("innerHTML"));
            }

            Console.WriteLine();

        }

        public List<Purchase> GetPurchasesItems()
        {
            var result = new List<Purchase>();
            Thread.Sleep(2000);
            this.Chrome.Url = Addresses.Purchased;

            var ordersTable=this.Chrome.FindElement(By.Id("my-orders-listing"));
            var orders=ordersTable.FindElementsByIdPart("order-id");
            foreach(var order in orders)
            {
                var detailsLink=order.FindElementByInnerText("a", "Szczegóły");
                var detailsLinkAddress= detailsLink.GetAttribute("href");
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
