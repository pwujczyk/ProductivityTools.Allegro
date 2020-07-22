using ProductivityTools.Allegro.Selenium;
using System;

namespace ProductivityTools.Allegro.App
{
    public class Application
    {
        public void GetPurchases()
        {
            SeleniumCalls calls = new SeleniumCalls();
            calls.GetPurchases("xxx","xxx");
        }
    }
}
