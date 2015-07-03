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

            Console.WriteLine("Listening on port {0}, type 'quit' to stop the service", ConfigurationManager.AppSettings["port"]);
            while (true)
                if (Console.ReadLine().ToLower() == "quit")
                {
                    //DiPSService.ServiceStop();
                    break;
                }


        }
    }
}
