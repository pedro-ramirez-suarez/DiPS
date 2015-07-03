using DiPS.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiPSWebSockets;

namespace DiPS.Client
{
    public class DiPSClient
    {

        private string _ServerAddress;
        private WebSocket comms;
        private Dictionary<string, Action<dynamic>> _Subscriptions = new Dictionary<string, Action<dynamic>>();
        private string _ClientId;
        public DiPSClient(string serverAddress)
        {
            _ClientId = Guid.NewGuid().ToString().Replace("-","");
            _ServerAddress = string.Format("{0}/?clientid={1}",serverAddress,_ClientId);
            comms = new WebSocket(_ServerAddress);
            
            comms.Connect();
            comms.OnMessage += comms_OnMessage;
            comms.OnError += comms_OnError;
            
        }

        /// <summary>
        /// try to restart the comms
        /// </summary>
        void comms_OnError(object sender, ErrorEventArgs e)
        {
            //Reset();
        }

        /// <summary>
        /// message received
        /// </summary>
        void comms_OnMessage(object sender, MessageEventArgs e)
        {
            //Need to know event name and parameters
            DiPSEvent dEvent = JsonConvert.DeserializeObject<DiPSEvent>(e.Data);
            //check if we have a subscription
            if(dEvent.MessageType == MessageType.EventFired &&  _Subscriptions.ContainsKey(dEvent.EventName))
            {
                _Subscriptions[dEvent.EventName](dEvent.PayLoad);
            }
        }


        public void Reset()
        {
            comms.Close();
            comms.Connect();
        }

        public void Subscribe(string eventName, Action<dynamic> action)
        {
            if (!_Subscriptions.ContainsKey(eventName))
            {
                //send the subscrition to the server
                DiPSEvent dEvent = new DiPSEvent { EventName = eventName, ClientId = _ClientId, MessageType = MessageType.Subscribe };
                comms.Send(JsonConvert.SerializeObject(dEvent));
                //add it locally
                _Subscriptions.Add(eventName, action);
            }
        }

        public void Unsubscribe(string eventName)
        {
            if (!_Subscriptions.ContainsKey(eventName))
            {
                //unsubscribe from the server
                DiPSEvent dEvent = new DiPSEvent { EventName = eventName, ClientId = _ClientId, MessageType = MessageType.Unsubscribe };
                comms.Send(JsonConvert.SerializeObject(dEvent));
                //then locally
                _Subscriptions.Remove(eventName);
            }
        }

        public void Publish(string eventName, object eventParameter)
        {
            //send the event to the server
            DiPSEvent dEvent = new DiPSEvent 
            { 
                EventName = eventName, 
                ClientId = _ClientId, 
                MessageType = MessageType.Publish ,
                EventParameter = JsonConvert.SerializeObject(eventParameter)

            };
            
            comms.Send(JsonConvert.SerializeObject(dEvent));
        }

    }
}
