﻿using System;
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
        //This is to run the app as a background process(it is not a windows service)
        static ManualResetEvent _quit = new ManualResetEvent(false);
        static void Main(string[] args)
        {
            ///The service will start on the port and url specified on the .config file
            try
            {
                DiPSService.ServiceStart();
                Console.WriteLine("DiPS Server");
                Console.WriteLine("Listening on port {0}, type Ctr + C' to stop the service", ConfigurationManager.AppSettings["port"]);
                Console.CancelKeyPress += (sender, eArgs) =>
                {
                    _quit.Set();
                    eArgs.Cancel = true;
                };
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(e);
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Press any key to exit");
                Console.ReadKey();
                _quit.Set();
            }
            _quit.WaitOne();
        }

        
    }
}
