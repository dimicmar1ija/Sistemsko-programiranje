using Lab3SysProg.Model;
using NLTKSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lab3SysProg.Observers
{
    class YoutubeCommentsObserver : IObserver<YoutubeComment>
    {
        public string Name { get; set; }
        public string[] Ids { get; set; }

        private object lockObj = new object();
        private static string[] listOfWords = WordBank.ReturnAllWords().Split("\n");
        private static ConcurrentDictionary<string, string> wordMap = null;
        //The default concurrency level is equal to the number of CPUs. The higher the concurrency level is
        // the more concurrent write operations can take place without interference and blocking.
        public Dictionary<string, int> CounterMap = new Dictionary<string, int>();
        private HttpListenerContext httpContext;
        private Server server;
        public YoutubeCommentsObserver(string name, string[] categories, string[] ids, HttpListenerContext context,Server server)
        {
            Name = name;
            Ids = ids;

            foreach (var cat in categories)
            {
                CounterMap[cat] = 0;
            }
            if (wordMap == null)
            {
                lock (lockObj)
                {
                    if (wordMap == null)
                    {
                        wordMap = new ConcurrentDictionary<string, string>(Environment.ProcessorCount * 2, 5000);
                        foreach (var word in listOfWords)
                        {
                            string[] tmp = word.Split("\t"); //izdvoji reci iz svakog reda
                            tmp[1] = tmp[1].Replace(" ", ""); //izbaci razmak ispred
                            wordMap[tmp[1]] = tmp[2]; //(rec,tip)
                        }
                    }
                }
            }
            httpContext = context;
            this.server = server;
        }
        public void OnCompleted()
        {
            //i ovo
            string toPrint= "<ul>";
            foreach (var item in CounterMap)
            {
                toPrint += $"<li>{item.Key} : {item.Value}</li>";
            }
            toPrint += "</ul><br>";
            server.MakeResponse(200, httpContext, toPrint);
        }

        public void OnError(Exception error)
        {
            Console.WriteLine("Error in observer: " + error.Message);
            server.MakeResponse(400, httpContext, error.Message);
        }

        public void OnNext(YoutubeComment value)
        {
            //Configuration.Default.AddApiKey("Apikey", "362cfa6f-7bc2-4aa9-a3ca-d58d0e89ca13");
            //var apiInstance = new ExtractEntitiesApi();
            //var vred = new ExtractEntitiesRequest(value.Text);
            //ExtractEntitiesResponse result = apiInstance.ExtractEntitiesPost(vred);
            //Console.WriteLine(result.Entities.Count);
            //foreach (var entity in result.Entities)
            //{
            //    Console.WriteLine(entity.ToString());
            //}
            string[] text = Tokenizer.Tokenize(value.Text);
            //Console.WriteLine(value.Text);

            foreach (var item in text)
            {
                string category = "";
                wordMap.TryGetValue(item, out category);
                if (string.IsNullOrEmpty(category))
                {
                    category = "undefined";
                }
                int counter;
                if (CounterMap.TryGetValue(category, out counter))
                {
                    ++counter;
                    CounterMap[category] = counter;
                }
            }
        }
    }
}
