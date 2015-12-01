using DiPS.Client;
using DiPSBackEndApplication.Controller;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiPSBackEndApplication.Application
{
    /// <summary>
    /// A DiPS Application initializer
    /// Setup the connection with DiPS and registers all the controller's public methods
    /// Initialize the backend calling the Initialize backend, for a console app, create a static constructor like this:
    /// static Program() 
    /// {    
    ///     InitializeBackEnd();
    /// }
    /// </summary>
    public class DiPSBackEnd
    {
        /// <summary>
        /// The DiPS service
        /// </summary>
        static DiPSClient Client;

        /// <summary>
        /// Static ctor
        /// Registers all DiPSControllers
        /// </summary>
        public static void InitializeBackEnd(bool initializeServer = true)
        {
            if (initializeServer)
                InitializeDiPS();
            Client = new DiPSClient(ConfigurationManager.AppSettings["dipsserver"]);
            RegisterAllControllers();
        }


        /// <summary>
        /// Static ctor
        /// Registers all DiPSControllers
        /// </summary>
        public static void InitializeBackEnd(string dipsConnectionString, bool initializeServer = true)
        {
            if (initializeServer)
                InitializeDiPS();
            Client = new DiPSClient(dipsConnectionString);
            RegisterAllControllers();
        }

        /// <summary>
        /// Launch the DiPS service if is not running
        /// </summary>
        private static void InitializeDiPS()
        {
            var dipsRunning = false;
            var dipsLaunched = false;
            while (true)
            {

                Process[] processlist = Process.GetProcesses();
                foreach (Process theprocess in processlist)
                {
                    if (theprocess.ProcessName.ToLower().Trim().Contains("dips"))
                    {
                        dipsRunning = true;
                        Console.Clear();
                        break;
                    }
                }
                if (dipsRunning)
                    break;
                else if (!dipsLaunched)
                {
                    //attempt to start DiPS
                    Process.Start(ConfigurationManager.AppSettings["dipsserverpath"] + "\\DiPS.exe");
                    //sleeps a few milisecs
                    Thread.Sleep(1000);
                    dipsLaunched = true;
                    continue;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Seems like the DiPS server is not running, please make sure that the server is running.");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Press enter to retry, or another key to exit");
                if (Console.ReadKey().Key != ConsoleKey.Enter)
                    Environment.Exit(1);
            }
        }


        /// <summary>
        /// Register all controllers found in the assembly
        /// </summary>
        private static void RegisterAllControllers()
        {
            var allTypes = Assembly.GetEntryAssembly().GetTypes();
            foreach (var t in allTypes)
            {
                if (t.BaseType == typeof(DiPSController))
                {
                    try
                    {
                        dynamic controller = Activator.CreateInstance(t, Client);
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
        }
    }
}
