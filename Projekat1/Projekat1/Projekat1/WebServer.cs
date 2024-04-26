using System;
using System.Threading;

namespace WebServer
{
    public class WebServer
    {
        private readonly string _apiKey;
        private readonly string _baseUrl;
        private readonly WeatherApiClient _weatherApiClient;
        private readonly LRUCache _cache;

        public WebServer(string apiKey, string baseUrl, int cacheCapacity)
        {
            _apiKey = apiKey;
            _baseUrl = baseUrl;
            _weatherApiClient = new WeatherApiClient(baseUrl);
            _cache = new LRUCache(cacheCapacity);
        }

        public void Start()
        {
            Console.WriteLine("Web server started...");

            // primanje zahteva
            ThreadPool.QueueUserWorkItem(ProcessRequest, null);
        }

        private void ProcessRequest(object state)
        {
            try
            {
                //obrada zahteva
                Console.WriteLine("Processing request...");

                //pretraga vremenske prognoze
                string query = "Belgrade";
                int days = 1;
                string aqi = "yes";
                string alerts = "no";

                byte[] cachedResponse = _cache.Get(query);
                if (cachedResponse != null)
                {
                    Console.WriteLine("Cached response found.");
                    Console.WriteLine("Weather Forecast:");
                    Console.WriteLine(System.Text.Encoding.UTF8.GetString(cachedResponse));
                }
                else
                {
                    string weatherForecast = _weatherApiClient.GetWeatherForecast(query, days, aqi, alerts, _apiKey);
                    Console.WriteLine("Weather Forecast:");
                    Console.WriteLine(weatherForecast);
                    _cache.Set(query, System.Text.Encoding.UTF8.GetBytes(weatherForecast));
                }

                //logika za slanje odgovora klijentu
                SendResponseToClient("Sample response to client");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
            }
        }

        private void SendResponseToClient(string response)
        {
            //odgovor klijentu
            Console.WriteLine($"Sending response to client: {response}");
        }
    }
}


