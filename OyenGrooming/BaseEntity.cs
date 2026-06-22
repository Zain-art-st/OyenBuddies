using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public abstract class BaseEntity : IRecord, IAuditable
    {
        public int ID { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public abstract bool Validate();
        public abstract string GetSummary();

        public string GetAuditInfo()
        {
            return $"Created: {CreatedAt:dd/MM/yyyy HH:mm}";
        }
    }
}
