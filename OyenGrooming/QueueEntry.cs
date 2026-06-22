using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public class QueueEntry : BaseEntity
    {
        public int QueueNumber { get; set; }
        public int CustomerID { get; set; }
        public int PetID { get; set; }
        public int? ServiceID { get; set; }
        public int? GroomerID { get; set; }
        public string CustomerName { get; set; }
        public string PetName { get; set; }
        public string ServiceName { get; set; }
        public string GroomerName { get; set; }
        public string Status { get; set; } = "Waiting";
        public DateTime CheckInTime { get; set; } = DateTime.Now;
        public DateTime? ServedTime { get; set; }
        public string Notes { get; set; }

        public string WaitDuration =>
            $"{(int)(DateTime.Now - CheckInTime).TotalMinutes} min";

        public override bool Validate()
        {
            return CustomerID > 0 && PetID > 0;
        }

        public override string GetSummary()
        {
            return $"#{QueueNumber}  {PetName}  ({CustomerName})  —  {Status}";
        }
    }
}
