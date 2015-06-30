using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiPS
{
    class Program
    {
        static void Main(string[] args)
        {
            int port = int.Parse(ConfigurationManager.AppSettings["port"]);
            string url = ConfigurationManager.AppSettings["url"];
            DiPS.Server.DiPSServer.Start(url, port);

            Console.WriteLine("DiPS Server");

            Console.WriteLine("Listening on port {0}, press ESC key to exit",port);
            while (true)
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                    break;
            
        }
    }
}
