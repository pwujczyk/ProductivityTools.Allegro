﻿using ProductivityTools.Allegro.Selenium;
using ProductivityTools.Purchases.Contract;
using System;
using System.Collections.Generic;

namespace ProductivityTools.Allegro.App
{
    public class Application
    {
        public List<Purchase> GetPurchases(string login, string password)
        {
            SeleniumCalls calls = new SeleniumCalls();
            calls.Login(login, password);
            var purchases=calls.GetPurchases();
            return purchases;

        }
    }
}
