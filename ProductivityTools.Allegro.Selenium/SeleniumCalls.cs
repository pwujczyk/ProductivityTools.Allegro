using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ProductivityTools.SeleniumExtensions;
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

        public void GetPurchases(string login, string password)
        {
            Login(login, password);
        }

        private void Login(string login, string password)
        {
            this.Chrome.Url = Addresses.LoginPage;

            IWebElement opboxfragment = this.Chrome.FindElement(By.ClassName("opbox-fragment"));

            var go = opboxfragment.GetElementByInnerText("button", "przejdź dalej");
            go.Click();

            var usernamefield = this.Chrome.FindElement(By.Id("username"));
            usernamefield.SendKeys(login);

            var passwordfield = this.Chrome.FindElement(By.Id("password"));
            passwordfield.SendKeys(password);

            var loginbutton = this.Chrome.FindElement(By.Id("login-button"));
            loginbutton.Click();
        }
    }
}
