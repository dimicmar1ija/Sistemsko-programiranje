using Lab3SysProg;
using org.w3c.dom;
using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text;
class Program
{
    public static void Main()
    {
        Server server = new Server();
        try
        {
            //ne znam da li treba, radi i bez
            //string modelpath = "C:\\openNLP_models\\"; 
            //java.io.FileInputStream modelInpStream = new java.io.FileInputStream(modelpath + "en-sent.bin");
            //opennlp.tools.sentdetect.SentenceModel sentenceModel = new opennlp.tools.sentdetect.SentenceModel(modelInpStream);
            //opennlp.tools.sentdetect.SentenceDetectorME SentenceDetectorME = new opennlp.tools.sentdetect.SentenceDetectorME(sentenceModel);

            server.Start();
            Console.ReadLine();
            server.Stop();
         
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}