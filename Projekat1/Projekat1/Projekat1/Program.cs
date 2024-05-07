using System;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiKey = "4bfbbbb6dec341f1b98101838242604"; //a4d1a083444548719ac122453242404
            string baseUrl = $"http://api.weatherapi.com/v1/forecast.json?key={apiKey}";
            int cacheCapacity = 10;
            var server = new WebServer(baseUrl, cacheCapacity);
            try
            {
                server.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.ReadLine();
            server.Stop();
        }
    }
}
