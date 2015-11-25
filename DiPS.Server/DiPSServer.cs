using DiPS.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiPSWebSockets.Server;
using System.Threading;

namespace DiPS.Server
{
    public class DiPSServer : WebSocketBehavior
    {
        /// <summary>
        /// The sockets server
        /// </summary>
        static WebSocketServer dipsServer;

        /// <summary>
        /// A collection of subscribers to events
        /// </summary>
        static Dictionary<string, List<string>> _EventsAndSubscribers = new Dictionary<string, List<string>>();

        /// <summary>
        /// Used to send messages in other thread async
        /// </summary>
        public class AsyncMessage
        {
            public DiPSEvent Event { get; set; }
            public string SessionId { get; set; }
        }
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

        


        /// <summary>
        /// Process messages received and takes proper action, all messages are json objects as string
        /// </summary>
        protected override void OnMessage(DiPSWebSockets.MessageEventArgs e)
        {
            try
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
                        if (!_EventsAndSubscribers.ContainsKey(dEvent.EventName))
                            return;
                        var clients = _EventsAndSubscribers[dEvent.EventName];
                        clients.ForEach((c) =>
                        {
                            //match the session id with the clientid
                            var session = Sessions.Sessions.FirstOrDefault(s => s.Context.QueryString["clientid"] == c);
                            //if the session was found, send the message
                            dEvent.MessageType = MessageType.EventFired;
                            if (session != null) //send it in another thread, this is helpful when we have many subscribers
                                ThreadPool.QueueUserWorkItem(new WaitCallback(SendInThreadAsync), new AsyncMessage { Event = dEvent, SessionId = session.ID });

                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// Used to send the message in another thread async
        /// </summary>
        private void SendInThreadAsync(object toSend)
        {
            AsyncMessage _event = toSend as AsyncMessage;
            Sessions.SendToAsync(JsonConvert.SerializeObject(_event.Event), _event.SessionId, (complete) => {  });
        }

        /// <summary>
        /// This occurs when a user is connected to the DiPS server
        /// </summary>
        protected override void OnOpen()
        {
            base.OnOpen(); 
            
        }

        /// <summary>
        /// We dont want to remove the subscriptions on close, because the client may reconnect, and when it does, it uses the same clientid
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClose(DiPSWebSockets.CloseEventArgs e)
        {
            base.OnClose(e);
        }

        /// <summary>
        /// Do nothing for now, TODO: log the errors
        /// </summary>
        protected override void OnError(DiPSWebSockets.ErrorEventArgs e)
        {
            base.OnError(e);
        }
    }
}
