using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyAPI.Data
{
    public class AppUser: IdentityUser<Guid>
    {
        [Required]
        [MaxLength(100)]
        public required string Nickname { get; set; }
        public string? Avatar { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public DateTime? Dob { get; set; } // Date of Birth
        public DateTime? CreatedAt { get; set; } = DateTime.Now;
        public ICollection<ReadingHistory> ReadingHistories { get; set; } = new List<ReadingHistory>();

    }
}
