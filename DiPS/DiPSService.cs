using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DiPS
{
    #region Nested classes to support running as service
    

    /// <summary>
    /// Right now the service run as a background app, but this is preparation to expose it as a normal Windows Service
    /// </summary>
    public class DiPSService : ServiceBase
    {
        public const string Service_Name = "DiPS Service";
        public DiPSService()
        {
            base.ServiceName = DiPSService.Service_Name;
        }

        protected override void OnStart(string[] args)
        {
            DiPSService.ServiceStart();
        }

        protected override void OnStop()
        {
            DiPSService.ServiceStop();
        }

        /// <summary>
        /// The service will start on the url and port specified on the .config file
        /// </summary>
        public  static void ServiceStart()
        {
            int port = int.Parse(ConfigurationManager.AppSettings["port"]);
            string url = ConfigurationManager.AppSettings["url"];
            DiPS.Server.DiPSServer.Start(url, port);
        }

        public  static void ServiceStop()
        {
            DiPS.DiPSService.ServiceStop();
        }
    }
    #endregion
}
