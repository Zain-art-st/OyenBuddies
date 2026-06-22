using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public class Customer : BaseEntity
    {
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation property — one customer has many pets
        public List<Pet> Pets { get; set; } = new List<Pet>();

        public override bool Validate()
        {
            return !string.IsNullOrWhiteSpace(FullName)
                && !string.IsNullOrWhiteSpace(Phone);
        }

        public override string GetSummary()
        {
            return $"{FullName} — {Phone} ({Pets.Count} pet(s))";
        }
    }
}
