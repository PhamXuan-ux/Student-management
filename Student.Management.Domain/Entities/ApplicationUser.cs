using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Student.Management.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [MaxLength(255)]
        public string? FullName { get; set; }

        [MaxLength(500)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? Role { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}