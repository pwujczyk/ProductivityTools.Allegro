using Microsoft.Extensions.Configuration;
using ProductivityTools.Allegro.App;
using System;
using ProductivityTools.MasterConfiguration;

namespace ProductivityTools.Allegro.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
               .AddMasterConfiguration(true)
               .Build();
            var login = configuration["Login"];
            var password = configuration["Password"];

            Application app = new Application();
            app.GetPurchases(login, password);

            Console.WriteLine("Hello World!");
        }
    }
}
