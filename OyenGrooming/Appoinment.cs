using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public class Appointment : BaseEntity
    {
        public int CustomerID { get; set; }
        public int PetID { get; set; }
        public int? StaffID { get; set; }
        public int? GroomerID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string TimeSlot { get; set; }
        public string Status { get; set; } // Confirmed, Pending, Completed, Cancelled
        public string ServiceType { get; set; }
        public string CustomerName { get; set; } // for display
        public string PetName { get; set; } // for display

        public override bool Validate()
        {
            return CustomerID > 0
                && PetID > 0
                && AppointmentDate >= DateTime.Today
                && !string.IsNullOrWhiteSpace(TimeSlot);
        }

        public override string GetSummary()
        {
            return $"{AppointmentDate:dd/MM/yyyy} {TimeSlot} — {PetName} ({Status})";
        }
    }
}
