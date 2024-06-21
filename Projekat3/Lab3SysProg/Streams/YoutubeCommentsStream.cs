using Lab3SysProg.Model;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;

namespace Lab3SysProg.Streams
{
    class YoutubeCommentsStream
    {
        /*https:/youtube.googleapis.com/youtube/v3/commentThreads?
         part=snippet%2Creplies&videoId=_VB39Jo8mAQ&key=[YOUR_API_KEY]
       */
        // /?ids=1,2,3,4...
        private readonly Subject<YoutubeComment> subject = new Subject<YoutubeComment>();
        private const string apiKey = "AIzaSyD9tfQvk_jRPhcPQ7s-lMUXbCoAWRE9-YI";
        private string baseUrl = "https://youtube.googleapis.com/youtube/v3/commentThreads?part=snippet&maxResults=10&&videoId=";
        private string[] ids;
        private HttpClient httpClient = new HttpClient();
        public YoutubeCommentsStream(string[] ids)
        {
            this.ids = ids;
        }
        public IDisposable Subscribe(IObserver<YoutubeComment> observer)
        {
            return subject.Subscribe(observer);
        }
        public async Task GetComments()
        {
            try
            {
                foreach (var id in ids)
                {
                    var result = await httpClient.GetAsync(baseUrl + id + $"&key={apiKey}");
                    result.EnsureSuccessStatusCode();
                    //dodati proveru ispravnosti rezultata
                    string resultString = await result.Content.ReadAsStringAsync();
                    JObject data= JObject.Parse(resultString);
                    var items=data.SelectToken("items");
                    if (items != null)
                    {
                        foreach (var item in items)
                        {
                            var text = item.SelectToken("snippet.topLevelComment.snippet.textOriginal");
                            YoutubeComment yt = new YoutubeComment(text.ToString());
                            subject.OnNext(yt);
                        }
                    }
                }
                subject.OnCompleted();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        public IObservable<YoutubeComment> GetProxy()
        {
            return subject.SubscribeOn(ThreadPoolScheduler.Instance).ObserveOn(Scheduler.CurrentThread);
        }

    }
}
