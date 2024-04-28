using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace WebServer
{
    public class WebServer
    {
        private readonly WeatherApiClient weatherApiClient;
        private readonly LRUCache cache;

        public WebServer(string baseUrl, int cacheCapacity)
        {
            weatherApiClient = new WeatherApiClient(baseUrl);
            cache = new LRUCache(cacheCapacity);
        }

        public void Start()
        {
            Console.WriteLine("Web server started...");

            ThreadPool.QueueUserWorkItem(ProcessRequest, null); 
            //??
        }

        private void ProcessRequest(object state)
        {
            try
            {
                Console.WriteLine("Processing request...");

                //omoguciti unos parametara??
                for (int i = 0; i < 3; i++)
                {
                    string query = "Belgrade";
                    int days = 1;
                    string aqi = "yes";
                    string alerts = "no";

                    byte[] cachedResponse = cache.Get(query);
                    JObject data;
                    if (cachedResponse != null)
                    {
                        Console.WriteLine("Cached response found.");
                        data = JObject.Parse(System.Text.Encoding.UTF8.GetString(cachedResponse));
                    }
                    else
                    {
                        string weatherForecast = weatherApiClient.GetWeatherForecast(query, days, aqi, alerts);
                        data = JObject.Parse(weatherForecast);
                        cache.Set(query, System.Text.Encoding.UTF8.GetBytes(weatherForecast));
                    }
                    //Console.WriteLine(System.Threading.Thread.CurrentThread.ManagedThreadId);
                    Console.WriteLine("Weather Forecast:");
                    Console.WriteLine(data);
                    SendResponseToClient("Sample response to client");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
            }
        }

        private void SendResponseToClient(string response)
        {
            //sta je ovo
            Console.WriteLine($"Sending response to client: {response}");
        }
    }
}


