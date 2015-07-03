using DiPS.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiPSWebSockets.Server;

namespace DiPS.Server
{
    public class DiPSServer : WebSocketBehavior
    {

        static WebSocketServer dipsServer;
        public static void Start(string url, int port)
        {
            try
            {
                dipsServer = new WebSocketServer(port);
                dipsServer.AddWebSocketService<DiPSServer>("/" + url);
                dipsServer.Start();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static void Stop()
        {
            try
            {
                dipsServer.Stop();
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static  Dictionary<string, List<string>> _EventsAndSubscribers = new Dictionary<string, List<string>>();



        protected override void OnMessage(DiPSWebSockets.MessageEventArgs e)
        {
            base.OnMessage(e);
            //The clientId must be sent by the client
            var dEvent = JsonConvert.DeserializeObject<DiPSEvent>(e.Data);
            var clientId = Context.QueryString["clientid"];
            
            switch (dEvent.MessageType)
            {
                case MessageType.Subscribe:
                    if (!_EventsAndSubscribers.ContainsKey(dEvent.EventName))
                        _EventsAndSubscribers.Add(dEvent.EventName, new List<string>());
                    if (!_EventsAndSubscribers[dEvent.EventName].Contains(dEvent.ClientId))
                        _EventsAndSubscribers[dEvent.EventName].Add(dEvent.ClientId);
                    break;
                case MessageType.Unsubscribe:
                    if (_EventsAndSubscribers.ContainsKey(dEvent.EventName) && _EventsAndSubscribers[dEvent.EventName].Contains(dEvent.ClientId))
                        _EventsAndSubscribers[dEvent.EventName].Remove(dEvent.ClientId);
                    break;
                case MessageType.Publish:
                    var clients = _EventsAndSubscribers[dEvent.EventName];
                    clients.ForEach((c) => {
                        //match the session id with the clientid
                        var session = Sessions.Sessions.FirstOrDefault(s => s.Context.QueryString["clientid"] == c);
                        //if the session was found, send the message
                        dEvent.MessageType = MessageType.EventFired;
                        if(session!= null) //maybe send it in another thread?
                            Sessions.SendToAsync(JsonConvert.SerializeObject(dEvent), session.ID, (complecte) => { /*Do nothing*/ });
                    });
                    break;
            }
        }

        /// <summary>
        /// This occurs when a user is connected to the DiPS server
        /// </summary>
        protected override void OnOpen()
        {
            base.OnOpen(); 
            
        }

        protected override void OnClose(DiPSWebSockets.CloseEventArgs e)
        {
            var clientId = Context.QueryString["clientid"];
            //remove all subscriptions
            foreach (var k in _EventsAndSubscribers.Keys)
                _EventsAndSubscribers[k].Remove(clientId);
            base.OnClose(e);
        }

        protected override void OnError(DiPSWebSockets.ErrorEventArgs e)
        {
            //for now do nothing
            //TODO: Check for common errors and check if we remove the clientid
            
            
            base.OnError(e);
        }
    }
}
