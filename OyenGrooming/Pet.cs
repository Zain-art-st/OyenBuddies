using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    // Abstract middle layer — gives meaning to inheritance
    public abstract class Animal : BaseEntity
    {
        public string Name { get; set; }
        public string Species { get; set; } // Dog, Cat, Bird, etc.
        public string Breed { get; set; }
        public string Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }

        public int AgeInYears => DateOfBirth.HasValue
            ? (int)((DateTime.Today - DateOfBirth.Value).TotalDays / 365.25)
            : 0;

        public abstract string GetSpeciesIcon(); // each species returns its own icon
    }

    // Concrete class
    public class Pet : Animal
    {
        public int CustomerID { get; set; }
        public string Color { get; set; }
        public decimal Weight { get; set; }
        public string Allergies { get; set; }
        public string MedicalNotes { get; set; }
        public bool IsActive { get; set; } = true;

        public override bool Validate()
        {
            return !string.IsNullOrWhiteSpace(Name)
                && !string.IsNullOrWhiteSpace(Species)
                && CustomerID > 0;
        }

        public override string GetSummary()
        {
            return $"{Name} ({Breed ?? Species}) — {AgeInYears} yr(s) old";
        }

        public override string GetSpeciesIcon()
        {
            return Species?.ToLower() switch
            {
                "dog" => "🐶",
                "cat" => "🐱",
                "bird" => "🐦",
                "rabbit" => "🐰",
                _ => "🐾"
            };
        }
    }
}
