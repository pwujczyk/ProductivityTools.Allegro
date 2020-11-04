using System;
using System.Collections.Generic;
using System.Text;

namespace ProductivityTools.Allegro.Cmdlet.Commands
{
    public class TimeTrackingCommandAll : PSCmdlet.PSCommandPT<GetAllegroPurchases>
    {
        public TimeTrackingCommandAll(GetAllegroPurchases cmdletType) : base(cmdletType)
        {
        }
        protected override bool Condition => true;

        protected override void Invoke()
        {
            //var login = configuration["Login"];
            //var password = configuration["Password"];

            //Application app = new Application();
            //app.GetPurchases(login, password);
            Console.WriteLine("Hello from TimeTrackingCommandAll");
        }
    }
}
