using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Student.Management.Domain.Entities
{
    public class StudentEntity : BaseEntity
    {
        public bool IsActive { get; set; }

        public string? ApplicationStudentId { get; set; }

        [ForeignKey(nameof(ApplicationStudentId))]
        public virtual ApplicationUser? ApplicationStudent { get; set; }

        public virtual StudentProfile? Profile { get; set; }

        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<ClassStudent> ClassStudents { get; set; } = new List<ClassStudent>();

        public virtual Class? Class => ClassStudents?.FirstOrDefault()?.Class;

        [NotMapped]
        public string FullName => Profile?.FullName ?? ApplicationStudent?.FullName ?? string.Empty;

        [NotMapped]
        public string Email => ApplicationStudent?.Email ?? Profile?.Email ?? string.Empty;
    }
}
