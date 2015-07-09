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
        //This is to run the app as a background service(not a windows service)
        static ManualResetEvent _quit = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            ///The service will start on the port and url specified on the .config file
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
