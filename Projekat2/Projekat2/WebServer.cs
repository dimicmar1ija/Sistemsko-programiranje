using System;
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.VisualBasic;
using Newtonsoft.Json.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task Start()
        {
            try
            {
                listener.Start();
                Console.WriteLine("Web server started...");

                while (true)
                {
                    HttpListenerContext context = listener.GetContext();
                    //ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                    await Task.Run(()=>
                    {
                        ProcessRequest(context);
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task ProcessRequest(object? state)
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
                    await MakeResponse(400, context, "Get method required.");
                    throw new Exception("Get method required.");
                }
                if (context.Request.Url?.Query == "")
                {
                    await MakeResponse(400, context, "Empty query.");
                    throw new Exception("Empty query.");
                }
                var param = context.Request.Url?.Query!.Remove(0, 1).Split("&");
                if (param == null)
                {
                    await MakeResponse(400, context, "Null query.");
                    throw new Exception("Null query.");
                }
                if (param.Length != 4)
                {
                    await MakeResponse(400, context, "Query must have four parameters.");
                    throw new Exception("Query must have four parameters.");
                }

                string allParams = context.Request.Url?.Query!;
                var parameters = HttpUtility.ParseQueryString(allParams);

                var query = parameters["query"];
                var days = parameters["days"];
                var aqi = parameters["aqi"];
                var alerts = parameters["alerts"];

                JObject data;
                Stopwatch sw = new();
                sw.Start();

                string request = $"{query}{days}{aqi}{alerts}";
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
                    string weatherForecast = await weatherApiClient.GetWeatherForecast(query, int.Parse(days), aqi, alerts);
                    sw.Stop();

                    Console.WriteLine($"Time elapsed when obtaining data from API: {sw.Elapsed}");
                    if (weatherForecast == "")
                    {
                        await MakeResponse(400, context, "No forecast exists for this request.");
                        throw new Exception("No forecast exists for this request.");
                    }
                    data = JObject.Parse(weatherForecast);
                    cache.Set(request, Encoding.UTF8.GetBytes(weatherForecast));
                }
                //Console.WriteLine("Weather Forecast:");
                //ispis konzola
                //Console.WriteLine(data);
                //ispis postman
                await MakeResponse(200, context, data.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing request: {ex.Message}");
            }
            finally
            {
                await MakeResponse(400, (HttpListenerContext)state!, "Server error.");
            }
        }

        public void Stop()
        {
            Console.WriteLine("Server stopped.");
            if (disposed) return;
            else
            {
                listener.Stop();
                listener.Close();
                cache.Clear();
                disposed = true;
            }
        }

        private async Task MakeResponse(int responseCode, HttpListenerContext context, string text)
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
                    await response.OutputStream.WriteAsync(Encoding.ASCII.GetBytes(body)); 
                    response.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                return;
            }
            else if (responseCode == 200)
            {
                body = text;
                try
                {
                    await response.OutputStream.WriteAsync(Encoding.ASCII.GetBytes(body));
                    response.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    return;
                }
                return;
            }
        }
    }
}