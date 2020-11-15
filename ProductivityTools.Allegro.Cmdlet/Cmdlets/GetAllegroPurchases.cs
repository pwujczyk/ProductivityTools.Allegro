using ProductivityTools.Allegro.Cmdlet.Commands;
using System;
using System.Management.Automation;

namespace ProductivityTools.Allegro.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "AllegroPurchases")]
    public class GetAllegroPurchases : PSCmdlet.PSCmdletPT
    {
        [Parameter]
        public int Count { get; set; } = 2;

        protected override void ProcessRecord()
        {
            this.AddCommand(new Get(this));
            this.ProcessCommands();
            base.ProcessRecord();
        }
    }

}
