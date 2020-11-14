using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ProductivityTools.Allegro.ServiceBus
{
    public class ServiceBusSender
    {
        static string ConnectionString = "Endpoint=sb://ptpurchase.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=aKF7vPH8lucb0gIDfmNS5D3LxAd4RemjZ+b5Rul6Pao=";
        string QuenePath = "allegroquene";

        public void Send(object o)
        {
            var queneClient = new QueueClient(ConnectionString, QuenePath);

            string json = JsonConvert.SerializeObject(o);
            var message = new Message(Encoding.UTF8.GetBytes(json));
            queneClient.SendAsync(message).Wait();

            queneClient.CloseAsync().Wait();
        }
    }
}
