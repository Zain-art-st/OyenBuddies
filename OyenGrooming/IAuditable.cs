using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public interface IAuditable
    {
        DateTime CreatedAt { get; set; }
        string GetAuditInfo();
    }
}
