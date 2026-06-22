using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public class Invoice : BaseEntity
    {
        public int CustomerID { get; set; }
        public int PetID { get; set; }
        public int? AppointmentID { get; set; }
        public int? QueueID { get; set; }
        public string CustomerName { get; set; }
        public string PetName { get; set; }
        public decimal SubTotal { get; set; }
        public decimal DiscountAmt { get; set; }
        public decimal TaxAmt { get; set; }
        public decimal TotalAmount { get; set; }
        public string PaymentMethod { get; set; } = "Cash";
        public string Status { get; set; } = "Unpaid";
        public string Notes { get; set; }
        public DateTime? PaidAt { get; set; }

        // Navigation
        public List<InvoiceItem> Items { get; set; } = new List<InvoiceItem>();

        public override bool Validate()
        {
            return CustomerID > 0
                && PetID > 0
                && TotalAmount >= 0
                && Items.Count > 0;
        }

        public override string GetSummary()
        {
            return $"Invoice #{ID}  {CustomerName}  RM {TotalAmount:F2}  [{Status}]";
        }
    }
}
