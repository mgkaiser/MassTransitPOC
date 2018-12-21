using POCInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POCMain.Messages
{
    public class EGTEvent : IEGTEvent
    {
        public string EventType { get; set; }
        public string SenderId { get; set; }
        public string Message { get; set; }
    }
}
