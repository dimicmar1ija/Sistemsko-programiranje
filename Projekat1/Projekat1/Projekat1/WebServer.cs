using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace WebServer
{
    public class WebServer
    {
        private readonly WeatherApi weatherApiClient;
        private readonly LRUCache cache;
        private readonly HttpListener listener;
        private bool disposed = false;

        public WebServer(string baseUrl, int cacheCapacity, string address = "localhost", int port = 5050)
        {
            weatherApiClient = new WeatherApi(baseUrl);
            cache = new LRUCache(cacheCapacity);
            listener = new HttpListener();
            listener.Prefixes.Add($"http://{address}:{port}/");
        }

        public void Start()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Web server started...");
                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ProcessRequest(object? state)
        {
            try
            {
                if (state == null)
                {
                    return;
                }
                Console.WriteLine("Processing request...");
                var context = (HttpListenerContext)state;
                if (context.Request.HttpMethod != "GET")
                {
                    MakeResponse(400, context, "Get method required.");
                    throw new Exception("Get method required.");
                }
                if (context.Request.Url?.Query == "")
                {
                    MakeResponse(400, context, "Empty query.");
                    throw new Exception("Empty query.");
                }
                var param = context.Request.Url?.Query.Remove(0, 1).Split("&");
                if (param == null)
                {
                    MakeResponse(400, context, "Null query.");
                    throw new Exception("Null query.");
                }
                if (param.Length != 4)
                {
                    MakeResponse(400, context, "Query must have four parameters.");
                    throw new Exception("Query must have four parameters.");
                }
                var query = param[0].Split("=")[1];
                var days = param[1].Split("=")[1];
                var aqi = param[2].Split("=")[1];
                var alerts = param[3].Split("=")[1];

                JObject data;
                Stopwatch sw = new();
                sw.Start();

                string request = context.Request.Url!.Query.Remove(0, 1);
                Console.WriteLine(request);
                if (cache.ContainsKey(request))
                {
                    byte[] cachedResponse = cache.Get(request);
                    sw.Stop();

                    Console.WriteLine("Cached response found.");
                    data = JObject.Parse(Encoding.UTF8.GetString(cachedResponse));

                    Console.WriteLine($"Time elapsed when obtaining data from cache: {sw.Elapsed}");
                }
                else
                {
                    string weatherForecast = weatherApiClient.GetWeatherForecast(query, int.Parse(days), aqi, alerts);
                    sw.Stop();

                    Console.WriteLine($"Time elapsed when obtaining data from API: {sw.Elapsed}");

                    data = JObject.Parse(weatherForecast);
                    cache.Set(request, Encoding.UTF8.GetBytes(weatherForecast));
                }
                //Console.WriteLine("Weather Forecast:");
                //ispis konzola
                //Console.WriteLine(data);
                //ispis postman
                MakeResponse(200, context, data.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
            }
        }

        public void Stop()
        {
            Console.WriteLine("Server stopped.");
            if (!disposed) return;
            else
            {
                listener.Stop();
                listener.Close();
                disposed = true;
            }
        }

        private void MakeResponse(int responseCode, HttpListenerContext context, string text)
        {
            var response = context.Response;
            response.StatusCode = responseCode;

            string body;
            if (responseCode == 400)
            {
                response.ContentType = "text/html";
                body = $@"<html>
                    <head><title>Bad Request</title></head>
                    <body>
                        {text}
                    </body>
                         </html>";
                try
                {
                    response.OutputStream.Write(Encoding.ASCII.GetBytes(body));
                    response.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    return;
                }
                return;
            }
            else if (responseCode == 200)
            {
                body = text;
                try
                {
                    response.OutputStream.Write(Encoding.ASCII.GetBytes(body));
                    response.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
                    return;
                }
                return;
            }
        }
    }
}