using Lab3SysProg;
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
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}