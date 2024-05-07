using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading;
using Newtonsoft.Json.Linq;

namespace WebServer
{
    public class WebServer
    {
        private readonly WeatherApiClient weatherApiClient;
        private readonly LRUCache cache;
        private readonly HttpListener listener;
        private bool disposed = false;
        public WebServer(string baseUrl, int cacheCapacity, string address = "localhost", int port = 5050)
        {
            weatherApiClient = new WeatherApiClient(baseUrl);
            cache = new LRUCache(cacheCapacity);
            listener = new HttpListener();
            listener.Prefixes.Add($"http://{address}:{port}/");
        }

        public void Start()
        {
            Console.WriteLine("Web server started...");
            try
            {
                listener.Start();
                while (listener.IsListening)
                {
                    HttpListenerContext context = listener.GetContext();
                    ThreadPool.QueueUserWorkItem(ProcessRequest, context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            Console.WriteLine("Server is not listening anymore.");
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
                    MakeResponse(context, "GET method required.", true);
                    return;
                }
                if (context.Request.Url?.Query == "")
                {
                    MakeResponse(context, "Empty query.", true);
                    return;
                }
                var param = (context.Request.Url?.Query.Remove(0, 1).Split("&")) ?? throw new Exception("Null query exception.");
                if (param == null)
                {
                    MakeResponse(context, "Null query.", true);
                    throw new Exception("Null query.");
                }
                if (param.Length != 4)
                {
                    MakeResponse(context, "Query must have four parameters.", true);
                    throw new Exception("Query must have four parameters.");
                }
                var query = param[0].Split("=")[1];
                var days = param[1].Split("=")[1];
                var aqi = param[2].Split("=")[1];
                var alerts = param[3].Split("=")[1];
                byte[] cachedResponse = cache.Get(query);
                JObject data;
                if (cachedResponse != null)
                {
                    Console.WriteLine("Cached response found.");
                    data = JObject.Parse(Encoding.UTF8.GetString(cachedResponse));
                }
                else
                {
                    string weatherForecast = weatherApiClient.GetWeatherForecast(query, int.Parse(days), aqi, alerts);
                    data = JObject.Parse(weatherForecast);
                    cache.Set(query, Encoding.UTF8.GetBytes(weatherForecast));
                }
                Console.WriteLine("Weather Forecast:");
                //ispis konzola
                Console.WriteLine(data);
                //ispis postman
                MakeResponse(context, data.ToString(),false);
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

        private void MakeResponse(HttpListenerContext context, string responseContent, bool badRequest)
        {
            var response = context.Response;
            var buffer = Encoding.UTF8.GetBytes(responseContent);
            response.ContentLength64 = buffer.Length;

            try
            {
                using (var outputString = response.OutputStream)
                {
                    outputString.Write(buffer, 0, buffer.Length);
                }

                response.ContentType = "text/html";
                if (badRequest)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response.StatusDescription = "Bad Request";
                }
                else
                {
                    response.StatusCode = (int)HttpStatusCode.OK;
                    response.StatusDescription = "OK";
                }
                response.ContentEncoding = Encoding.UTF8;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error while sending response: {ex.Message}");
            }
            finally
            {
                response.Close();
            }
        }
        /*outputString je kreiran kao using blok kako bi se osiguralo da se ispravno oslobodi
        čak i ako dođe do izuzetka. Takođe, dodat je blok try-catch-finally kako bi se uhvatile
        moguće greške prilikom slanja odgovora i osiguralo da se response uvek zatvori, čak i
        ako dođe do izuzetka.*/
    }
}


