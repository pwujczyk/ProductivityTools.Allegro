using Newtonsoft.Json.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ProductivityTools.Allegro.Selenium.Model;
using ProductivityTools.SeleniumExtensions;
using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
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
            new FillPurchaseClass(this.Chrome, purchase);
            new FillReturnClass(this.Chrome, purchase);
            Console.WriteLine();
        }

        private class FillPurchaseClass
        {
            private Purchase Purchase;
            private IWebElement detailsContainer;

            public FillPurchaseClass(IWebDriver Chrome, Purchase purchase)
            {
                this.Purchase = purchase;
                Chrome.Url = $"{Addresses.Purchased}/{purchase.PurchaseId}";
                detailsContainer = Chrome.FindElement(By.XPath("//*[@data-box-name='Main grid']"));

                GetPurchaseItems();
                var statusField = detailsContainer.FindElementByMultipleClass("w1eai trz41");
                purchase.Status = statusField.InnerText();

                FillSeller();
                FillDelivery();
                FillPayment();
            }

            private void FillSeller()
            {
                var sellerField = detailsContainer.FindElementsByMultipleClass("_3kk7b _vnd3k _1h8s6 _1nucm");
                Purchase.SellerName = sellerField[0].InnerText();
                Purchase.SellerAddres = sellerField[1].InnerText();
                var sellerContactField = detailsContainer.FindElementsByMultipleClass("_3kk7b _vnd3k _1h8s6 _umw2u");
                Purchase.SellerPhone = sellerContactField[0].InnerText();
                Purchase.SellerEmail = sellerContactField[1].InnerText();
            }

            private void FillDelivery()
            {
                if (detailsContainer.FindElements(By.Id("delivery-address")).Count > 0)
                {
                    var deliveryAddresField = detailsContainer.FindElement(By.Id("delivery-address"));
                    Purchase.DeliveryAddress = deliveryAddresField.InnerText();

                    var phoneField = detailsContainer.FindElementByMultipleClass("mqway y1hv2 _s8izy");
                    Purchase.ReceipmentPhone = phoneField.InnerText();
                }

                //DeliveryDate
                string deliverySectionSelector = "_1jtqp _3kk7b _vnd3k _1plx6";
                var deliverySectionElements = detailsContainer.FindElementsByMultipleClass(deliverySectionSelector);
                if (deliverySectionElements.Count > 0)
                {
                    foreach (var deliverySectionElement in deliverySectionElements)
                    {
                        var delivery = new Delivery();
                        Purchase.Delivery.Add(delivery);

                        string deliveryStatusSelector = "ls2xj2 tl1r7i pe6nb p15b4 m3cbb ptrkmx tlr0ph";
                        var deliveryStatusElements = deliverySectionElement.FindElementsByMultipleClass(deliveryStatusSelector);
                        if (deliveryStatusElements.Count > 0)
                        {
                            var deliveryStatusElement = deliverySectionElement.FindElementByMultipleClass(deliveryStatusSelector);
                            delivery.DeliveryStatus = deliveryStatusElement.Text;

                            //DeliveryNumber
                            var deliveryIdCombo = detailsContainer.FindElementByMultipleClass("_ydq9t _3kk7b _vnd3k _1h8s6 _alw8w");
                            //var deliveryIdCombo = deliverySectionElement.FindElementByMultipleClass("_ydq9t _3kk7b _vnd3k _1h8s6 _1nucm");
                            var deliveryIdSpan = deliveryIdCombo.FindElement(By.TagName("span"));
                            delivery.DeliveryNumber = deliveryIdSpan.Text;
                        }
                    }
                }
            }

            private void FillPayment()
            {
                string paymentCss = "_1d2pv _3kk7b _vnd3k _1h8s6 _1nucm";
                var paymentbox = detailsContainer.FindElement(By.Id("opbox-myorder-payment"));
                if (paymentbox.FindElementsByMultipleClass(paymentCss).Count > 0)
                {
                    Func<string, string, string> GetValueUnderHeader = (s, tag) =>
                    {
                        var valueMethodLabel = paymentbox.FindElementByInnerText("div", s, true);
                        if (valueMethodLabel == null) return null;
                        var valueMethodBox = valueMethodLabel.Parent();
                        var debug = valueMethodBox.InnerHtml();
                        var paragraphs = valueMethodBox.FindElements(By.TagName(tag));
                        var r = paragraphs.InnersText();
                        return r;
                    };

                    Purchase.PaymentType = GetValueUnderHeader("Metoda płatności", "p");
                    Purchase.PaymentStatus = GetValueUnderHeader("Status płatności", "p");
                    if (Purchase.PaymentStatus.Trim() == "Płatność zakończona")
                    {
                        var kwotaWplaty = GetValueUnderHeader("Kwota wpłaty", "span").Replace("zł", "").Replace(" ", "");
                        Purchase.PaymentAmount = kwotaWplaty == null ? null : (decimal?)decimal.Parse(kwotaWplaty);
                        Purchase.PaymentDate = DateTime.Parse(GetValueUnderHeader("Data zakończenia płatności", "p"));
                    }
                }
            }

            private void GetPurchaseItems()
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
                this.Purchase.Items = result;
            }
        }

        private class FillReturnClass
        {
            private Purchase Purchase;
            private IWebElement returnContainer;

            public FillReturnClass(IWebDriver Chrome, Purchase purchase)
            {
                this.Purchase = purchase;
                Chrome.Url = $"{Addresses.Purchased}/{purchase.PurchaseId}";
                var detailsContainer = Chrome.FindElement(By.XPath("//*[@data-box-name='Main grid']"));
                string returnSectionSelector = "opbox-myorder-returns";
                var returnSectionElements = detailsContainer.FindElements(By.Id(returnSectionSelector));
                if (returnSectionElements.Count > 0)
                {
                    var returnSectionElement = returnSectionElements[0];
                    var returnNumberElement = returnSectionElement.FindElements(By.TagName("a"));
                    
                    if (returnNumberElement.Count > 0)
                    {
                        this.Purchase.Return.Id = returnNumberElement[0].InnerText();
                        returnNumberElement[0].Click();
                        returnContainer = Chrome.FindElement(By.Id("no-printable-content"));
                        FillReturnItems();
                    }
                }
            }

            

            private void FillReturnItems()
            {
                Thread.Sleep(1000);
                var returnedElements = returnContainer.FindElementsByMultipleClass("_11245_1bzGV _1yyhi");
                foreach(var returnedElement in returnedElements)
                {
                    var purchaseItem = new PurchaseItem();
                    this.Purchase.Return.Items.Add(purchaseItem);
                    var name= returnedElement.FindElementByMultipleClass("_xu6h2 _3kk7b _18y38 _otc6c _19orx");
                    purchaseItem.Name = name.InnerText();

                    var amount = returnedElement.FindElementByMultipleClass("_3kk7b _t0xzz _knu61");
                    var amountString = amount.InnerText();
                    string value = Regex.Replace(amountString, "[A-Za-z ]", "");
                    int parsedValue = int.Parse(value);
                    purchaseItem.Amount = parsedValue;

                    var wholeItemCostElement = returnedElement.FindElementByMultipleClass("_1svub _1svub _lf05o");
                    var wholeItemCostString = wholeItemCostElement.InnerText();
                    string wholeItemCost = Regex.Replace(wholeItemCostString, "[A-Za-zęł]", "");
                    decimal wholeItemCostValue = decimal.Parse(wholeItemCost);
                    purchaseItem.SinglePrice = wholeItemCostValue / purchaseItem.Amount;

                    //var singleItemCostElement = returnedElement.FindElementByMultipleClass("_1svub _11245_1nV9_");
                    //var singleItemCostString = singleItemCostElement.InnerText();
                    //string singleItemCost = Regex.Replace(singleItemCostString, "[A-Za-zęł]", "");
                    //decimal singleItemCostValue = decimal.Parse(singleItemCost);
                    //purchaseItem.SinglePrice = singleItemCostValue;

                    

                }
            }
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
