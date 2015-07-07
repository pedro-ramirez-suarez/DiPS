using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiPS
{
    class Program
    {

        static ManualResetEvent _quit = new ManualResetEvent(false);
        static void Main(string[] args)
        {


            // running as service
            //using (var service = new DiPSService())
            //    ServiceBase.Run(service);
            DiPSService.ServiceStart();


            Console.WriteLine("DiPS Server");

            Console.WriteLine("Listening on port {0}, type Ctr+C' to stop the service", ConfigurationManager.AppSettings["port"]);

            Console.CancelKeyPress += (sender, eArgs) =>
            {
                _quit.Set();
                eArgs.Cancel = true;
            };

            _quit.WaitOne();
        }

        
    }
}
