using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;

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
            Login();
        }

        private void Login()
        {
            this.Chrome.Url = Addresses.LoginPage;
        }
    }
}
