using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student.Management.Domain.Entities
{
    public class Class : BaseEntity
    {
        [Required]
        public bool IsActive { get; set; } = true;
        [Required, StringLength(100)]
        public string ClassName { get; set; }

        [StringLength(200)]
        public string Room { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        

        public int ProgramId { get; set; }

        [ForeignKey(nameof(ProgramId))]
        public virtual Program Program { get; set; }

        // Thêm CourseId
        public int? CourseId { get; set; }

        [ForeignKey(nameof(CourseId))]
        public virtual Course? Course { get; set; }

        [StringLength(450)]
        public string? TeacherId { get; set; }

        [ForeignKey(nameof(TeacherId))]
        public virtual ApplicationUser? Teacher { get; set; }
        public virtual ICollection<Schedule> Schedules { get; set; }

        public virtual ICollection<ClassStudent> ClassStudents { get; set; }
    }
}