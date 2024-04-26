using System;
using System.Collections.Generic;
using RestSharp;

namespace WebServer
{
    public class WeatherApiClient
    {
        private readonly string baseUrl;
        private readonly HttpClient client; 

        public WeatherApiClient(string baseUrl)
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
                    return "Error: Failed to retrieve weather forecast.";
                }
            }
            catch (Exception ex)
            {
                return $"Error: {ex.GetType().Name} - {ex.Message}";
            }
        }
    }
}

