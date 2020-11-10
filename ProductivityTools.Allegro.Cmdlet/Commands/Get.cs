using Microsoft.Extensions.Configuration;
using ProductivityTools.Allegro.App;
using ProductivityTools.MasterConfiguration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityTools.Allegro.Cmdlet.Commands
{
    public class Get : PSCmdlet.PSCommandPT<GetAllegroPurchases>
    {
        public Get(GetAllegroPurchases cmdletType) : base(cmdletType)
        {
        }
        protected override bool Condition => true;

        protected override void Invoke()
        {
            var configuration = new ConfigurationBuilder()
                .AddMasterConfiguration(true)
                .Build();
            var login = configuration["Login"];
            var password = configuration["Password"];

            Application app = new Application();
            app.GetPurchases(login, password);
            Console.WriteLine("Hello from TimeTrackingCommandAll");
        }
    }
}
