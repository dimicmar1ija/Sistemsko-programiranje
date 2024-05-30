using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace WebServer
{
    public class WeatherApi
    {
        private readonly string baseUrl;
        private readonly HttpClient client;

        public WeatherApi(string baseUrl)
        {
            this.baseUrl = baseUrl;
            client = new HttpClient();
        }

        public async Task<string> GetWeatherForecast(string query, int days, string aqi, string alerts)
        {
            try
            {
                string url = $"{baseUrl}&q={query}&days={days}&aqi={aqi}&alerts={alerts}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;
                }
                else
                {
                    Console.WriteLine("Error: Failed to retrieve weather forecast.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                //throw new Exception(ex.Message);
                Console.WriteLine(ex.Message);
                return null;
            }
        }
    }
}
