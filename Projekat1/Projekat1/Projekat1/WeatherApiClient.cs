using System;
using System.Collections.Generic;
using RestSharp;

namespace WebServer
{
    public class WeatherApiClient
    {
        private readonly string _baseUrl;

        public WeatherApiClient(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public string GetWeatherForecast(string query, int days, string aqi, string alerts, string apiKey)
        {
            try
            {
                var client = new RestClient(_baseUrl);
                var request = new RestRequest("forecast.json", Method.Get);
                request.AddParameter("key", apiKey);
                request.AddParameter("q", query);
                request.AddParameter("days", days);
                request.AddParameter("aqi", aqi);
                request.AddParameter("alerts", alerts);

                var response = client.Execute(request);

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return "Error: Weather forecast not found.";
                }
                else if (!string.IsNullOrEmpty(response.Content))
                {
                    //Provera tipa odgovora
                    try
                    {
                        var json = System.Text.Json.JsonDocument.Parse(response.Content);
                        // ovde moze da se doda dodatna provera da li je JSON u očekivanom formatu?
                    }
                    catch (Exception ex)
                    {
                        return $"Error: Invalid response format - {ex.Message}";
                    }
                    return response.Content;
                }
                else
                {
                    return "Error: Failed to retrieve weather forecast.";
                }
            }
            catch (Exception ex)
            {
                //ovde mozemo da dodamo vrste gresaka 
                return $"Error: {ex.GetType().Name} - {ex.Message}";
            }
        }   
    }
}

