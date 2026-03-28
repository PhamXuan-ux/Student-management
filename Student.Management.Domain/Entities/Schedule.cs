using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student.Management.Domain.Entities
{
    public class Schedule : BaseEntity
    {
        public int ClassId { get; set; }

        [ForeignKey(nameof(ClassId))]
        public virtual Class Class { get; set; }

        [Required, StringLength(20)]
        public string DayOfWeek { get; set; } = string.Empty; // Monday, Tuesday, etc.

        [Required, StringLength(10)]
        public string StartTime { get; set; } = string.Empty; // HH:mm format

        [Required, StringLength(10)]
        public string EndTime { get; set; } = string.Empty; // HH:mm format

        [Required, StringLength(200)]
        public string Room { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}