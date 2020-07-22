using ProductivityTools.Allegro.Selenium;
using System;

namespace ProductivityTools.Allegro.App
{
    public class Application
    {
        public void GetPurchases(string login, string password)
        {
            SeleniumCalls calls = new SeleniumCalls();
            calls.Login(login, password);
            calls.GetPurchases();

        }
    }
}
