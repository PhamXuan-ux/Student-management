using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student.Management.Domain.Entities
{
    public class Attendance : BaseEntity
    {
        public int ClassId { get; set; }

        [ForeignKey(nameof(ClassId))]
        public virtual Class Class { get; set; }

        public int StudentEntityId { get; set; }

        [ForeignKey(nameof(StudentEntityId))]
        public virtual StudentEntity Student { get; set; }

        public DateTime AttendanceDate { get; set; }

        [Required, StringLength(50)]
        public string Status { get; set; } = string.Empty; // Present, Absent, Late

        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;

        public DateTime RecordedDate { get; set; } = DateTime.Now; //đ 
    }
}