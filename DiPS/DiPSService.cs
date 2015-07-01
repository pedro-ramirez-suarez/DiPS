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
