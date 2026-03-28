using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Student.Management.Domain.Entities
{
    public class Enrollment : BaseEntity
    {
        [Required, StringLength(50)]
        public string Code { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow; // ✅ mặc định khi thêm mới

        [StringLength(500)]
        public string? Note { get; set; }

        [Required, StringLength(50)]
        public string Status { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public int StudentEntityId { get; set; }

        [ForeignKey(nameof(StudentEntityId))]
        public virtual StudentEntity StudentEntity { get; set; }

        public virtual ICollection<EnrollmentDetail> EnrollmentDetails { get; set; }

        [NotMapped]
        public virtual Course? Course => EnrollmentDetails?.FirstOrDefault()?.Course;
    }
}
