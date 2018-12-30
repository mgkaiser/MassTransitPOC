using System;
using System.Collections.Generic;
using System.Text;

namespace POCInterfaces
{
    public interface IPOCEvent2Response
    {
        string Response { get; set; }
        int ResultId { get; set; }
    }
}
