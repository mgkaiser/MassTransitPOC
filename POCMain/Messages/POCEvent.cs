using POCInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POCMain.Messages
{
    public class POCEvent : IPOCEvent
    {
        public string LastName { get; set; }
        public string FirstName { get; set; }
    }
}
