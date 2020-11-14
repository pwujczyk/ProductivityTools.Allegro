using Microsoft.Azure.ServiceBus;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ProductivityTools.Allegro.ServiceBus
{
    public class ServiceBusSender
    {
        static string ConnectionString = "Endpoint=sb://ptpurchase.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=aKF7vPH8lucb0gIDfmNS5D3LxAd4RemjZ+b5Rul6Pao=";
        string QuenePath = "allegroquene";

        public void Send()
        {
            var queneClient = new QueueClient(ConnectionString, QuenePath);
            for (int i = 0; i < 10; i++)
            {
                var content = $"Message {i}";
                var message = new Message(Encoding.UTF8.GetBytes(content));
                queneClient.SendAsync(message).Wait();
                Console.WriteLine($"Sent {i}");
            }

            queneClient.CloseAsync().Wait();
        }
    }
}
