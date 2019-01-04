using POCInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace POCMain.Messages
{
    public class POCEvent2Response : IPOCEvent2Response
    {
        public string Response { get; set; }
        public int ResultId { get; set; }
    }
}
