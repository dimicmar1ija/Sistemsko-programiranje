using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3SysProg.Model
{
    class YoutubeComment
    {
        public string Text { get; set; }
        public YoutubeComment() { }

        public YoutubeComment(string text)
        {
            Text = text;
        }
    }
}
