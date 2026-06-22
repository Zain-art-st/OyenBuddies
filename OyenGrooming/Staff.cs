using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OyenGrooming
{
    public class Staff : BaseEntity
    {
        public string FullName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public bool IsActive { get; set; } = true;

        public override bool Validate()
        {
            return !string.IsNullOrWhiteSpace(FullName)
                && !string.IsNullOrWhiteSpace(Username)
                && !string.IsNullOrWhiteSpace(Email)
                && Email.Contains("@")
                && !string.IsNullOrWhiteSpace(Role)
                && !string.IsNullOrWhiteSpace(PasswordHash);
        }

        public override string GetSummary()
        {
            return $"{FullName} ({Role}) — {Email}";
        }

        // Simple hash (use BCrypt in production)
        public static string HashPassword(string password)
        {
            var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}