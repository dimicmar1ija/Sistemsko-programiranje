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
                            string[] tmp = word.Split("\t");
                            tmp[1] = tmp[1].Replace(" ", "");
                            wordMap[tmp[1]] = tmp[2];
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
            string toPrint= "";
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
            string[] text = Tokenizer.Tokenize(value.Text);
            foreach(var item in text)
            {
                string category = "";
                wordMap.TryGetValue(item, out category);
                if(string.IsNullOrEmpty(category))
                {
                    category = "undefined";
                }
                int counter;
                if(CounterMap.TryGetValue(category,out counter))
                {
                    ++counter;
                    CounterMap[category] = counter;
                }
            }
        }
    }
}
