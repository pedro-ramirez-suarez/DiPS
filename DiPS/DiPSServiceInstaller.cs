using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace DiPS
{
    [RunInstaller(true)]
    public partial class DiPSServiceInstaller : System.Configuration.Install.Installer
    {
        private ServiceInstaller serviceInstaller;
        private DiPSService diPSService1;
        private ServiceProcessInstaller processInstaller;

        public DiPSServiceInstaller()
        {
            
            processInstaller = new ServiceProcessInstaller();
            serviceInstaller = new ServiceInstaller();

            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = "DiPS Service";
            serviceInstaller.Description = "Distributed Publish Subscribe service";

            Installers.Add(serviceInstaller);
            Installers.Add(processInstaller);
        }

        private void InitializeComponent()
        {
            this.diPSService1 = new DiPS.DiPSService();
            // 
            // diPSService1
            // 
            this.diPSService1.ExitCode = 0;
            this.diPSService1.ServiceName = "DiPS Service";

        }
    }
}
