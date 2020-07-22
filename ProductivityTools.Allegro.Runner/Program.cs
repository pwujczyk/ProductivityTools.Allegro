using ProductivityTools.Allegro.App;
using System;

namespace ProductivityTools.Allegro.Runner
{
    class Program
    {
        static void Main(string[] args)
        {
            Application app = new Application();
            app.GetPurchases();
            Console.WriteLine("Hello World!");
        }
    }
}
