using Lab3SysProg;
using System.Diagnostics;
using System.Net;
using System.Text;
class Program
{
    public static void Main()
    {
        Server server = new Server();
        try
        {
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