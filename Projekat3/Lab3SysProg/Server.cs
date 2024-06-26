﻿using Lab3SysProg.Observers;
using Lab3SysProg.Streams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lab3SysProg
{
    class Server
    {
        private readonly HttpListener listener;
        private string[] idsList;
        string[] prefixes = new string[]
        {
             "http://localhost:5050/",
             "http://127.0.0.1:5050/",
        };
        public Server()
        {
            listener = new HttpListener();
            foreach (string prefix in prefixes)
            {
                listener.Prefixes.Add(prefix);
            }
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
                    ProcessRequest(context);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public async void ProcessRequest(HttpListenerContext context)
        {
            await Task.Run(async () =>
            {

                HttpListenerRequest request = context.Request;
                //Console.WriteLine($"Request: {request.RawUrl}, Thread: {Thread.CurrentThread.ManagedThreadId}");
                try
                {
                    if (context.Request.HttpMethod != "GET")
                    {
                        MakeResponse(400, context, "Get method required.");
                        return;
                    }
                    if (context.Request.Url?.Query == "")
                    {
                        MakeResponse(400, context, "Empty query.");
                        return;
                    }
                    var param = context.Request.Url?.Query!.Remove(0, 1);
                    if (param == null || param == "")
                    {
                        MakeResponse(400, context, "Null query.");
                        return;
                    }
                    string allParams = context.Request.Url?.Query!;
                    var parameters = HttpUtility.ParseQueryString(allParams);
                    if (parameters.Count != 1)
                    {
                        MakeResponse(400, context, "Request must have one parameter.");
                        return;
                    }
                    var ids = parameters["ids"];
                    if (ids == "" || ids == null)
                    {
                        MakeResponse(400, context, "Bad request.");
                        return;
                    }
                    var values = ids.Split(",");
                    idsList = new string[values.Length];
                    for (int i = 0; i < values.Length; i++)
                    {
                        idsList[i] = values[i];
                    }

                    YoutubeCommentsStream stream = new YoutubeCommentsStream(idsList);
                    string[] categories1 = new string[] { "undefined", "a", "v" };
                    string[] categories2 = new string[] { "i", "c" };

                    StringWrapper wrapper1, wrapper2;
                    wrapper1 = new();
                    wrapper2 = new();

                    YoutubeCommentsObserver observer1 = new("Observer1", categories1, idsList, wrapper1);
                    YoutubeCommentsObserver observer2 = new("Observer2", categories2, idsList, wrapper2);

                    var proxy = stream.SubscribeOnObserveOn();
                    var subscription1 = proxy.Subscribe(observer1);
                    var subscription2 = proxy.Subscribe(observer2);

                    await stream.GetComments();

                    subscription1.Dispose();
                    subscription2.Dispose();

                    if (wrapper1.Value == "" || wrapper2.Value == "")
                    {
                        MakeResponse(404, context, "Results were not found.");
                    }
                    else
                    {
                        MakeResponse(200, context, wrapper1.Value + wrapper2.Value);
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    MakeResponse(400, context, ex.ToString());
                }
            });
        }
        public void Stop()
        {
            Console.WriteLine("Server stopped.");
            listener.Stop();
            listener.Close();
        }

        public void MakeResponse(int responseCode, HttpListenerContext context, string text)
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
                    Console.WriteLine(e.Message);
                    return;
                }
                return;
            }
            else if (responseCode == 200)
            {
                response.ContentType = "text/html";
                body = $@"<html>
                    <head><title>Named entity recognition</title></head>
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
                    Console.WriteLine(e.Message);
                    return;
                }
                return;
            }
            else if(responseCode==404)
            {
                response.ContentType = "text/html";
                body = $@"<html>
                    <head><title>Not found</title></head>
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
                    Console.WriteLine(e.Message);
                    return;
                }
                return;
            }
        }
    }
}
