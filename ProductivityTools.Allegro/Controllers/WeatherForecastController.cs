using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ProductivityTools.Allegro.Controllers
{
    class AccessToken
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public long expires_in { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IConfiguration Configuration;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IConfiguration configuration)
        {
            _logger = logger;
            Configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            string authUrl = "https://allegro.pl/auth/oauth/token?grant_type=client_credentials";
            string clientId = Configuration.GetValue<string>("ClientId");
            string clientSecret = Configuration.GetValue<string>("ClientSecret");
            string credentials = String.Format("{0}:{1}", clientId, clientSecret);
            AccessToken accesstoken;
            using (var client = new HttpClient())
            {
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(credentials)));

                //Prepare Request Body
                List<KeyValuePair<string, string>> requestData = new List<KeyValuePair<string, string>>();
                requestData.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));

                FormUrlEncodedContent requestBody = new FormUrlEncodedContent(requestData);

                //Request Token
                var request = client.PostAsync(authUrl, requestBody).Result;
                var response = request.Content.ReadAsStringAsync();
                string s = response.Result;
                accesstoken=JsonConvert.DeserializeObject<AccessToken>(s);
            }


            using (var client = new HttpClient())
            {
                //Define Headers
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.allegro.public.v1+json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accesstoken.access_token);


                //Request Token
                var request = client.GetAsync("https://allegro.pl/order/events").Result;
                var response = request.Content.ReadAsStringAsync();
                string s = response.Result;
                accesstoken = JsonConvert.DeserializeObject<AccessToken>(s);
            }

            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}
