using System;
using System.Management.Automation;

namespace ProductivityTools.Allegro.Cmdlet
{
    [Cmdlet(VerbsCommon.Get, "AllegroPurchases")]
    public class GetAllegroPurchases : PSCmdlet.PSCmdletPT
    {
        protected override void ProcessRecord()
        {
            Console.WriteLine("Hello"); 
            base.ProcessRecord();
        }
    }

}
