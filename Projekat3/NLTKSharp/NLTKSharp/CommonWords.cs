using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace NLTKSharp
{
    public static class CommonWords
    {
        private static Dictionary<string, int> frequencyDictionary; 

        static CommonWords()
        {
            string resource_5000words = Properties.Resources._5000words;
            frequencyDictionary = new Dictionary<string, int>();
            List<string> sentenceList =
                resource_5000words.Split(new[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries).ToList();
            foreach (string sentence in sentenceList)
            {
                string[] splitted = sentence.Split('\t');
                frequencyDictionary[splitted[1].Trim().ToLower()] = Convert.ToInt32(splitted[3]);
            }
        }

        public static int GetFrequency(string word)
        {
            int count = frequencyDictionary.ContainsKey(word.ToLower()) ? frequencyDictionary[word.ToLower()] : 0;
            return count;
        }
    }
}
