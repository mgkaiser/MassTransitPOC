using System;
using System.Collections.Generic;
using System.Text;

namespace POCInterfaces
{
    public interface IPOCEvent
    {
        string LastName { get; set; }
        string FirstName { get; set; }
        bool Fail { get; set; }
    }
}
