using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student.Management.Domain.Entities
{
    public class TeacherProfile : BaseEntity
    {
        [StringLength(450)]
        public string TeacherId { get; set; } 

        [ForeignKey(nameof(TeacherId))]
        public virtual ApplicationUser? User { get; set; }

        [Required, StringLength(200)]
        public string FullName { get; set; } = string.Empty;

        public string? Phone { get; set; }
        public string? Address { get; set; }
        public DateTime? DateOfBirth { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;
    }
}