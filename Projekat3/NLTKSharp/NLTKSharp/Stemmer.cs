using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NLTKSharp
{
    public static class Stemmer
    {
        static char[] vowels = { 'a', 'e', 'i', 'o', 'u', 'y' };
        static char[] consonants =
            {
                'b', 'c', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'v',
                'w', 'x', 'z'
            };
        private enum CharState
        {
            Consonant,
            Vowel,
            None
        };

        public static string GetStem(string word)
        {
            string stem = string.Empty;
            word = word.ToLower();
            string step1aResult = ApplyStep1a(word);
            string step1bResult = ApplyStep1b(step1aResult);
            string step1cResult = ApplyStep1c(step1bResult);
            string step2Result = ApplyStep2(step1cResult);
            string step3Result = ApplyStep3(step2Result);
            string step4Result = ApplyStep4(step3Result);
            string step5aResult = ApplyStep5a(step4Result);
            string step5bResult = ApplyStep5b(step5aResult);
            stem = step5bResult;
            return stem;
        }

        static string ApplyStep1a(string input)
        {
            string result = input;
            result = result.ReplaceLastOccurrence("sses", "ss");
            result = result.ReplaceLastOccurrence("ies", string.Empty);
            if(!result.EndsWith("ss"))
            {
                result = result.ReplaceLastOccurrence("s", string.Empty);    
            }
            return result;
        }

        static string ApplyStep1b(string input)
        {
            string result = input;
            bool secondOrThirdStepSuccessful = false;
            bool trimmed;
            int m = input.Measure();
            if (m > 0)
            {
                result = result.ReplaceLastOccurrence("eed", "ee");   
            }
            if (result.HasVowel())
            {
                result = result.ReplaceLastOccurrence("ed", string.Empty, out trimmed);
                if (trimmed)
                {
                    secondOrThirdStepSuccessful = true;
                }
            }
            if (result.HasVowel())
            {
                result = result.ReplaceLastOccurrence("ing", string.Empty, out trimmed);
                if (trimmed)
                {
                    secondOrThirdStepSuccessful = true;
                }
            }
            if (secondOrThirdStepSuccessful)
            {
                result = result.ReplaceLastOccurrence("at", "ate");
                result = result.ReplaceLastOccurrence("bl", "ble");
                result = result.ReplaceLastOccurrence("iz", "ize");
                if (result.Length >= 2)
                {
                    char cL = result[result.Length - 1];
                    char cPen = result[result.Length - 2];
                    if (cL == cPen && consonants.Contains(cL))
                    {
                        if (cL != 'l' && cL != 's' && cL != 'z')
                        {
                            result = result.TrimEnd(cL);
                        }
                    }
                }
                if (result.Measure() == 1)
                {
                    char cL = result[result.Length - 1];
                    char cPen = result[result.Length - 2];
                    char cSL = result[result.Length - 3];
                    bool cvcFormat = (consonants.Contains(cSL) && vowels.Contains(cPen) && consonants.Contains(cL));
                    bool lastCharValid = cL != 'w' && cL != 'x' && cL != 'y';
                    if (cvcFormat && lastCharValid)
                    {
                        result = result + "e";
                    }
                }
            }
            return result;
        }

        static string ApplyStep1c(string input)
        {
            string result = input;
            if (input.HasVowel())
            {
                result = result.ReplaceLastOccurrence("y", "i");
            }
            return result;
        }

        static string ApplyStep2(string input)
        {
            string result = input;
            result = result.ReplaceLastOccurrence("ational", "ate", minMeasure:1);
            result = result.ReplaceLastOccurrence("tional", "tion", 1);
            result = result.ReplaceLastOccurrence("enci", "ence",1);
            result = result.ReplaceLastOccurrence("anci", "ance",1);
            result = result.ReplaceLastOccurrence("izer", "ize",1);
            result = result.ReplaceLastOccurrence("abli", "able",1);
            result = result.ReplaceLastOccurrence("alli", "al", 1);
            result = result.ReplaceLastOccurrence("entli", "ent", 1);
            result = result.ReplaceLastOccurrence("eli", "e", 1);
            result = result.ReplaceLastOccurrence("ousli", "ous", 1);
            result = result.ReplaceLastOccurrence("ization", "ize", 1);
            result = result.ReplaceLastOccurrence("ation", "ate", 1);
            result = result.ReplaceLastOccurrence("ator", "ate", 1);
            result = result.ReplaceLastOccurrence("alism", "al", 1);
            result = result.ReplaceLastOccurrence("iveness", "ive", 1);
            result = result.ReplaceLastOccurrence("fulness", "ful", 1);
            result = result.ReplaceLastOccurrence("ousness", "ous", 1);
            result = result.ReplaceLastOccurrence("aliti", "al", 1);
            result = result.ReplaceLastOccurrence("iviti", "ive", 1);
            result = result.ReplaceLastOccurrence("biliti", "ble", 1);
            return result;
        }

        static string ApplyStep3(string input)
        {
            string result = input;
            result = result.ReplaceLastOccurrence("icate", "ic", minMeasure: 1);
            result = result.ReplaceLastOccurrence("ative", "", 1);
            result = result.ReplaceLastOccurrence("alize", "al", 1);
            result = result.ReplaceLastOccurrence("iciti", "ic", 1);
            result = result.ReplaceLastOccurrence("ical", "ic", 1);
            result = result.ReplaceLastOccurrence("ful", "", 1);
            result = result.ReplaceLastOccurrence("ness", "", 1);
            return result;
        }

        static string ApplyStep4(string input)
        {
            string result = input;
            result = result.ReplaceLastOccurrence("al", "", 2);
            result = result.ReplaceLastOccurrence("ance", "", 2);
            result = result.ReplaceLastOccurrence("ence", "", 2);
            result = result.ReplaceLastOccurrence("er", "", 2);
            result = result.ReplaceLastOccurrence("ic", "", 2);
            result = result.ReplaceLastOccurrence("able", "", 2);
            result = result.ReplaceLastOccurrence("ible", "", 2);
            result = result.ReplaceLastOccurrence("ant", "", 2);
            result = result.ReplaceLastOccurrence("ement", "", 2);
            result = result.ReplaceLastOccurrence("ment", "", 2);
            result = result.ReplaceLastOccurrence("ent", "", 2);
            if (result.Measure() > 1)
            {
                char cL = result[result.Length - 1];
                if (cL == 's' || cL == 't')
                {
                    result = result.ReplaceLastOccurrence("ion", "");
                }
            }
            result = result.ReplaceLastOccurrence("ou", "", 2);
            result = result.ReplaceLastOccurrence("ism", "", 2);
            result = result.ReplaceLastOccurrence("ate", "", 2);
            result = result.ReplaceLastOccurrence("iti", "", 2);
            result = result.ReplaceLastOccurrence("ous", "", 2);
            result = result.ReplaceLastOccurrence("ive", "", 2);
            result = result.ReplaceLastOccurrence("ize", "", 2);
            return result;
        }

        static string ApplyStep5a(string input)
        {
            string result = input;
            result = result.ReplaceLastOccurrence("e", "", 2);
            if (result.Measure() > 1)
            {
                char cL = result[result.Length - 1];
                if (cL != 'o')
                {
                    result = result.ReplaceLastOccurrence("e", "");
                }
            }
            return result;
        }

        static string ApplyStep5b(string input)
        {
            string result = input;
            if (result.Measure() > 1)
            {
                char cL = result[result.Length - 1];
                char cPen = result[result.Length - 2];
                if (cL == cPen && (cL == 'd' || cL == 'l'))
                {
                    result = result.ReplaceLastOccurrence(cL.ToString(), "");
                }
            }
            return result;
        }

        public static int Measure(this string word)
        {
            CharState charState = CharState.None;
            int measure = 0;
            
            for (int i = 0; i < word.Length; i++)
            {
                char ch = word[i];
                if (vowels.Contains(ch))
                {
                    charState = CharState.Vowel;
                }
                else if (consonants.Contains(ch))
                {
                    if ((measure == 0 && (charState == CharState.None || charState == CharState.Vowel))||(measure > 0 && charState == CharState.Vowel))
                    {
                        measure += 1;
                    }
                    
                    charState = CharState.Consonant;
                }
            }
            return measure;
        }

        static bool HasVowel(this string input)
        {
            return input.All(c => vowels.Contains(c));
        }

        static string ReplaceLastOccurrence(this string Source, string Find, string Replace, out bool trimmed)
        {
            string result = Source;
            bool foundString = false;
            if (!result.EndsWith(Find))
            {
                trimmed = false;
                return result;
            }
            int Place = Source.LastIndexOf(Find);
            if (Place > -1)
            {
                foundString = true;
                result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            }
            trimmed = foundString;
            return result;
        }

        static string ReplaceLastOccurrence(this string Source, string Find, string Replace)
        {
            string result = Source;
            if (!result.EndsWith(Find)) return result;
            int Place = Source.LastIndexOf(Find);
            if (Place > -1)
            {
                result = Source.Remove(Place, Find.Length).Insert(Place, Replace);    
            }
            return result;
        }

        static string ReplaceLastOccurrence(this string Source, string Find, string Replace, int minMeasure)
        {
            string result = Source;
            if (!result.EndsWith(Find)) return result;
            if (Source.Measure() >= minMeasure)
            {
                int Place = Source.LastIndexOf(Find);
                result = Source.Remove(Place, Find.Length).Insert(Place, Replace);
            }
            return result;
        }
    }
}
