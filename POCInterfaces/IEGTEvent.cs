using System;
using System.Collections.Generic;
using System.Text;

namespace POCInterfaces
{
    public interface IEGTEvent
    {
        string EventType { get; set; }
        string SenderId { get; set; }
        string Message { get; set; }
    }
}
