using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public class InvoiceItem : BaseEntity
    {
        public int InvoiceID { get; set; }
        public int? ServiceID { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; } = 1;
        public decimal UnitPrice { get; set; }
        public decimal LineTotal => Quantity * UnitPrice;

        public override bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Description)
                && Quantity > 0
                && UnitPrice >= 0;
        }

        public override string GetSummary()
        {
            return $"{Description}  x{Quantity}  @RM{UnitPrice:F2}  =  RM{LineTotal:F2}";
        }
    }
}
