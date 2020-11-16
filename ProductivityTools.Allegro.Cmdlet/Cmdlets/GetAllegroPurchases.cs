using ProductivityTools.Allegro.Cmdlet.Commands;
using ProductivityTools.Purchases.Contract;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace ProductivityTools.Allegro.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "AllegroPurchases")]
    public class GetAllegroPurchases : PSCmdlet.PSCmdletPT
    {
        [Parameter]
        public int Count { get; set; } = 2;

        [Parameter]
        public SwitchParameter SendToPurchase { get; set; }

        public List<Purchase> PurchaseList { get; set; }

        protected override void ProcessRecord()
        {
            this.AddCommand(new Get(this));
            this.AddCommand(new SendToPurchase(this));
            this.ProcessCommands();
            base.ProcessRecord();
        }
    }

}
