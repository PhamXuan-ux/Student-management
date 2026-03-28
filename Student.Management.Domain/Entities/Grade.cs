using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student.Management.Domain.Entities
{
    public class Grade : BaseEntity
    {
        public int ClassId { get; set; }

        [ForeignKey(nameof(ClassId))]
        public virtual Class Class { get; set; }

        public int StudentEntityId { get; set; }

        [ForeignKey(nameof(StudentEntityId))]
        public virtual StudentEntity Student { get; set; }

        [Required, StringLength(50)]
        public string GradeType { get; set; } = string.Empty; // Quiz, Assignment, Midterm, Final

        public double Score { get; set; }
        public double MaxScore { get; set; } = 10.0;
        public double Weight { get; set; } // Trọng số %

        public DateTime CreatedDate { get; set; }

        [StringLength(500)]
        public string Note { get; set; } = string.Empty;
    }
}