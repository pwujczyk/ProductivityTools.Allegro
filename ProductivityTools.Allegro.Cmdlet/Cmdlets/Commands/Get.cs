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

            //ProductivityTools.Allegro.ServiceBus.ServiceBusSender sender = new ServiceBus.ServiceBusSender();
            //Purchase p = new Purchase("Fdsa");
            //p.Status = "fdsa";
            //sender.Send(p);
            

            Application app = new Application();
            var purchases=app.GetPurchases(login, password,this.Cmdlet.Count);
            for (int i = 0; i < this.Cmdlet.Count; i++)
            {
                Console.Write(purchases[i].Dealer.Name);
                foreach(var item in purchases[i].Items)
                {
                    Console.WriteLine(item.Name);
                }
                Console.WriteLine();
            }
            Console.WriteLine("Hello from TimeTrackingCommandAll");
        }
    }
}
