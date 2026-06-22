using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public interface IRecord
    {
        int ID { get; set; }
        bool Validate();     // returns true if data is valid
        string GetSummary(); // returns a display string
    }
}
