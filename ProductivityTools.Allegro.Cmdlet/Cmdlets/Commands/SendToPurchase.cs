using Microsoft.Extensions.Configuration;
using ProductivityTools.Allegro.App;
using ProductivityTools.MasterConfiguration;
using ProductivityTools.Purchases.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace ProductivityTools.Allegro.Cmdlet.Commands
{
    public class SendToPurchase : PSCmdlet.PSCommandPT<GetAllegroPurchases>
    {
        public SendToPurchase(GetAllegroPurchases cmdletType) : base(cmdletType)
        {
        }

        protected override bool Condition => this.Cmdlet.SendToPurchase.IsPresent;

        protected override void Invoke()
        {
            ProductivityTools.Allegro.ServiceBus.ServiceBusSender sender = new ServiceBus.ServiceBusSender();
            foreach (var purchase in this.Cmdlet.PurchaseList)
            {
                sender.Send(purchase);
            }
        }
    }
}
