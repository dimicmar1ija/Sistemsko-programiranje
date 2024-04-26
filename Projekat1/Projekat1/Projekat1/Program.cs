using System;

namespace WebServer
{
    class Program
    {
        static void Main(string[] args)
        {
            string apiKey = "4bfbbbb6dec341f1b98101838242604";
            string baseUrl = "http://api.weatherapi.com/v1";
            int cacheCapacity = 10;

            var server = new WebServer(apiKey, baseUrl, cacheCapacity);
            server.Start();

            Console.ReadLine(); // Zadrzava aplikaciju otvorenom da bi mogli da vidimo logove
        }
    }
}
