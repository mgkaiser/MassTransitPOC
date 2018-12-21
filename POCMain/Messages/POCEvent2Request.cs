using POCInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POCMain.Messages
{
    public class POCEvent2Request : IPOCEvent2Request
    {
        public int eventId { get; set; }
    }
}
