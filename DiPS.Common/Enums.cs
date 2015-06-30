using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiPS.Common
{
    public class MessageType
    {
        public const string Subscribe = "subscribe";
        public const string Unsubscribe = "unsubscribe";
        public const string Publish = "publish";
        public const string EventFired ="eventfired";
    }
}
