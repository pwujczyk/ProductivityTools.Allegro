﻿using Newtonsoft.Json.Serialization;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ProductivityTools.Purchases.Contract;
using ProductivityTools.SeleniumExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using System.Threading;


namespace ProductivityTools.Allegro.Selenium
{
    public class SeleniumCalls
    {
        IWebDriver Chrome;
        Action<string> Log;

        public SeleniumCalls()
        {
            this.Log = (s) =>{ };
            ChromeOptions options = new ChromeOptions();
            this.Chrome = new ChromeDriver(options);
        }


        public SeleniumCalls(bool logToConsole) : this()
        {
            if (logToConsole)
            {
                this.Log = s => Console.WriteLine(s);
            }
        }

        public List<Purchase> GetPurchases(int count, bool printInnerHtml)
        {
            List<Purchase> purchases = GetPurchasesItems();
            int maxCount = count > purchases.Count ? purchases.Count : count;
            for (int i = 0; i < maxCount; i++)
            {
                var purchase = purchases[i];
                FillPurchase(purchase, printInnerHtml);
            }
            //foreach (var purchase in purchases)
            //{
            //    FillPurchase(purchase);
            //}
            return purchases;
        }

        private void FillPurchase(Purchase purchase, bool printInnerHtml)
        {
            new FillPurchaseClass(this.Chrome, purchase, printInnerHtml);
            new FillReturnClass(this.Chrome, purchase);
            Log(Environment.NewLine);
        }

        private class FillPurchaseClass
        {
            private Purchase Purchase;
            private IWebElement detailsContainer;

            public FillPurchaseClass(IWebDriver Chrome, Purchase purchase, bool printInnerHtml)
            {
                this.Purchase = purchase;
                Chrome.Url = $"{Addresses.Purchased}/{purchase.ExternalSystemId}";
                detailsContainer = Chrome.FindElement(By.XPath("//*[@data-box-name='Main grid']"));

                GetPurchaseItems();
                var statusField = detailsContainer.FindElementByMultipleClass("_7qjq4 _1dd5x");
                purchase.Status = statusField.InnerText();

                FillSeller();
                FillDelivery();
                FillPayment(printInnerHtml);
            }

            private void FillSeller()
            {
                var sellerField = detailsContainer.FindElementsByMultipleClass("_3kk7b _vnd3k _1h8s6 _1nucm");
                Purchase.Dealer.Name = sellerField[0].InnerText();
                Purchase.Dealer.Address = sellerField[1].InnerText();
                var sellerContactField = detailsContainer.FindElementsByMultipleClass("_3kk7b _vnd3k _1h8s6 _umw2u");
                Purchase.Dealer.Phone = sellerContactField[0].InnerText();
                Purchase.Dealer.Email = sellerContactField[1].InnerText();
            }

            private void FillDelivery()
            {
                if (detailsContainer.FindElements(By.Id("delivery-address")).Count > 0)
                {
                    var deliveryAddresField = detailsContainer.FindElement(By.Id("delivery-address"));
                    Purchase.DeliveryAddress = deliveryAddresField.InnerText();

                    var phoneField = detailsContainer.FindElementByMultipleClass("_s8izy");
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
                            delivery.Status = deliveryStatusElement.Text;

                            //DeliveryNumber
                            var deliveryIdCombo = detailsContainer.FindElementByMultipleClass("_ydq9t _3kk7b _vnd3k _1h8s6 _alw8w");
                            //var deliveryIdCombo = deliverySectionElement.FindElementByMultipleClass("_ydq9t _3kk7b _vnd3k _1h8s6 _1nucm");
                            var deliveryIdSpan = deliveryIdCombo.FindElement(By.TagName("span"));
                            delivery.Number = deliveryIdSpan.Text;
                        }
                    }
                }
            }

            private void FillPayment(bool printInnerHtml)
            {
                string paymentCss = "_1d2pv _3kk7b _vnd3k _1h8s6 _1nucm";
                var paymentbox = detailsContainer.FindElement(By.Id("opbox-myorder-payment"));
                if (paymentbox.FindElementsByMultipleClass(paymentCss).Count > 0)
                {
                    Func<string, string, string> GetValueUnderHeader = (s, tag) =>
                    {
                        var valueMethodLabel = paymentbox.FindElementByInnerText("div", s, printInnerHtml);
                        if (valueMethodLabel == null) return null;
                        var valueMethodBox = valueMethodLabel.Parent();
                        var debug = valueMethodBox.InnerHtml();
                        var paragraphs = valueMethodBox.FindElements(By.TagName(tag));
                        var r = paragraphs.InnersText();
                        return r;
                    };

                    Purchase.Payment.Type = GetValueUnderHeader("Metoda płatności", "p");
                    Purchase.Payment.Status = GetValueUnderHeader("Status płatności", "p");
                    if (Purchase.Payment.Status.Trim() == "Płatność zakończona")
                    {
                        var kwotaWplaty = GetValueUnderHeader("Kwota wpłaty", "span").Replace("zł", "").Replace(" ", "");
                        Purchase.Payment.Amount = kwotaWplaty == null ? null : (decimal?)decimal.Parse(kwotaWplaty);
                        Purchase.Payment.Date = DateTime.Parse(GetValueUnderHeader("Data zakończenia płatności", "p"));
                    }
                }
            }

            private void GetPurchaseItems()
            {
                List<PurchaseItem> result = new List<PurchaseItem>();

                var titleboxes = detailsContainer.FindElementsByMultipleClass("_35enf _1779c _hho8x _1yyhi");
                foreach (var titleBox in titleboxes)
                {
                    var divs = titleBox.FindElements(By.TagName("div"));
                    PurchaseItem item = new PurchaseItem();
                    result.Add(item);

                    var title = titleBox.FindElement(By.TagName("Span"));
                    item.Name = divs[1].InnerText().Replace(Environment.NewLine, string.Empty);

                    // var amountAndPrice = divs[3].InnerText();
                    var amount = divs[3].InnerText();
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
                var detailsContainer = Chrome.FindElement(By.XPath("//*[@data-box-name='Main grid']"));
                string returnSectionSelector = "opbox-myorder-returns";
                var returnSectionElements = detailsContainer.FindElements(By.Id(returnSectionSelector));
                if (returnSectionElements.Count > 0)
                {
                    var returnSectionElement = returnSectionElements[0];
                    var returnNumberElement = returnSectionElement.FindElements(By.TagName("a"));

                    if (returnNumberElement.Count > 0)
                    {
                        this.Purchase.ReturnNumber = returnNumberElement[0].InnerText();
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
                foreach (var returnedElement in returnedElements)
                {
                    // this.Purchase.Return.Items.Add(purchaseItem);
                    var name = returnedElement.FindElementByMultipleClass("_xu6h2 _3kk7b _18y38 _otc6c _19orx");
                    var returnName = name.InnerText().Replace(Environment.NewLine, " ");
                    var returnNameNormalized = Regex.Replace(returnName, @"\s", "");
                    var purchaseItem = this.Purchase.Items.First(x =>
                      {
                          var purchaseNameNormalized = Regex.Replace(x.Name, @"\s", "");
                          return purchaseNameNormalized == returnNameNormalized;
                       });

                    var amount = returnedElement.FindElementByMultipleClass("_3kk7b _t0xzz _knu61");
                    var amountString = amount.InnerText();
                    string value = Regex.Replace(amountString, "[A-Za-z ]", "");
                    int parsedValue = int.Parse(value);
                    purchaseItem.ReturnedAmount = parsedValue;

                    //var wholeItemCostElement = returnedElement.FindElementByMultipleClass("_1svub _1svub _lf05o");
                    //var wholeItemCostString = wholeItemCostElement.InnerText();
                    //string wholeItemCost = Regex.Replace(wholeItemCostString, "[A-Za-zęł]", "");
                    //decimal wholeItemCostValue = decimal.Parse(wholeItemCost);
                    //purchaseItem.SinglePrice = wholeItemCostValue / purchaseItem.Amount;                 

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
