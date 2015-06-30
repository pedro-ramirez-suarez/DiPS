using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiPS.Common
{
    public class DiPSEvent
    {

        public string ClientId { get; set; }

        public string EventName { get; set; }

        public string Context { get; set; }

        public string EventParameter { get; set; }

        public string MessageType { get; set; } // Use the constants defined in the MessageType class


        /// <summary>
        /// The Data as a typed object
        /// </summary>
        private dynamic _PayLoad;
        
        public dynamic PayLoad 
        {
            get
            {

                if (EventParameter != null && _PayLoad == null)
                {
                    try
                    {
                        //Type t = Type.GetType(ParameterType, true);
                        _PayLoad = JsonConvert.DeserializeObject(EventParameter);
                    }
                    catch (Exception e)
                    {

                    }
                }
                return _PayLoad;
            }
        }

    }
}
