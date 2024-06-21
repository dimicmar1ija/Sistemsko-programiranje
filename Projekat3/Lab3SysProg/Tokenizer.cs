using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3SysProg
{
    class Tokenizer
    {
        private static string[] interpunctionCharacters = new string[]
        {".",",","?","!",":",";","-","_","@","~","#","$","%","^","&","*","(",")","+" };

        public static string[] Tokenize(string text)
        {
            foreach (string stavka in interpunctionCharacters)
            {
                text = text.Replace(stavka, "");
            }
            text = text.ToLower();
            text = text.Replace("\n", " ");
            string[] returnList = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return returnList;
        }
    }
}
