using Microsoft.Extensions.Configuration;
using ProductivityTools.Allegro.App;
using ProductivityTools.MasterConfiguration;
using ProductivityTools.Purchases.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
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
                .AddMasterConfiguration(force:true)
                .Build();
            var login = configuration["Login"];
            var password = configuration["Password"];

            ProductivityTools.Allegro.ServiceBus.ServiceBusSender sender = new ServiceBus.ServiceBusSender();
            sender.Send();
            //sender.send();

            //Application app = new Application();
            //var purchases=app.GetPurchases(login, password);
            //foreach(var purchase in purchases)
            //{
            //    Console.Write(purchase.Items.Select(x => x.Name));
            //}
            Console.WriteLine("Hello from TimeTrackingCommandAll");
        }
    }
}
