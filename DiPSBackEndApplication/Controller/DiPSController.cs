using DiPS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiPSWebSockets.Server;
using System.Threading;
using DiPS.Client;

namespace DiPSBackEndApplication.Controller
{
    /// <summary>
    /// Move this to DiPS
    /// </summary>
    public abstract class DiPSController
    {

        protected DiPSClient DiPSClient { get; private set; }
        /// <summary>
        /// Ctor, we need the client to subscribe all the events in here
        /// </summary>
        public DiPSController(DiPSClient client)
        {
            DiPSClient = client;
            var myType = this.GetType();
            //get all the methods
            var methods = myType.GetMethods();
            foreach (var m in methods)
            {
                //register only public methods
                if (!m.IsPublic)
                    continue;

                client.Subscribe(myType.Name + "." + m.Name, (param) =>
                {
                    try
                    {
                        var pars = new List<object>();
                        pars.Add(param);
                        m.Invoke(this, pars.ToArray());
                    }
                    catch (Exception e)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(e);
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                });
            }
        }


    }
}
