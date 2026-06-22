using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public class Service : BaseEntity
    {
        public string ServiceName { get; set; }
        public string Category { get; set; }
        public decimal Price { get; set; }
        public int DurationMins { get; set; }
        public bool IsActive { get; set; } = true;

        public override bool Validate()
        {
            return !string.IsNullOrWhiteSpace(ServiceName)
                && Price >= 0
                && DurationMins > 0;
        }

        public override string GetSummary()
        {
            return $"{ServiceName} — RM {Price:F2} ({DurationMins} mins)";
        }
    }
}
