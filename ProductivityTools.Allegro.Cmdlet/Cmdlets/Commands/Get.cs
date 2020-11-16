using Microsoft.Extensions.Configuration;
using ProductivityTools.Allegro.App;
using ProductivityTools.ConsoleColor;
using ProductivityTools.MasterConfiguration;
using ProductivityTools.Purchases.Contract;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                .AddMasterConfiguration(force: true)
                .Build();
            var login = configuration["Login"];
            var password = configuration["Password"];

            Application app = new Application();
            this.Cmdlet.PurchaseList = app.GetPurchases(login, password, this.Cmdlet.Count);
            for (int i = 0; i < this.Cmdlet.Count; i++)
            {
                ConsoleColors.WriteInColor(this.Cmdlet.PurchaseList[i].Dealer.Name, 76, true);
                foreach (var item in this.Cmdlet.PurchaseList[i].Items)
                {
                    ConsoleColors.WriteInColor(item.Name, 220, true);
                }
                Console.WriteLine();
            }
        }
    }
}
