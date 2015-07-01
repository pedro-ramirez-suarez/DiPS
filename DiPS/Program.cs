using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DiPS
{
    class Program
    {
        static void Main(string[] args)
        {


            // running as service
            //using (var service = new DiPSService())
            //    ServiceBase.Run(service);
            DiPSService.ServiceStart();


            Console.WriteLine("DiPS Server");

            Console.WriteLine("Listening on port {0}, press ESC key to exit", ConfigurationManager.AppSettings["port"]);
            while (true)
                if (Console.ReadKey().Key == ConsoleKey.Escape)
                {
                    //DiPSService.ServiceStop();
                    break;
                }


        }
    }
}
