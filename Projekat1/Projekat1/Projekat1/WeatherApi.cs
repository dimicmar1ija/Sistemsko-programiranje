using System;
using System.Collections.Generic;
using RestSharp;

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

        public string GetWeatherForecast(string query, int days, string aqi, string alerts)
        {
            try
            {
                string url = $"{baseUrl}&q={query}&days={days}&aqi={aqi}&alerts={alerts}";
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    return responseBody;
                }
                else
                {
                    Console.WriteLine("Error: Failed to retrieve weather forecast.");
                    return "";
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

