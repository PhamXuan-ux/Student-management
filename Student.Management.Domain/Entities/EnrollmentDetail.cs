using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Management.Domain.Entities
{
    public class EnrollmentDetail : BaseEntity
    {
        [Required]

        [Column(TypeName = "decimal(18,2)")] 
        public decimal Price { get; set; }

        [Required]
        public int Quantity { get; set; }

        [StringLength(500)]
        public string Note { get; set; }

        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }

        public int EnrollmentId { get; set; }
        [ForeignKey(nameof(EnrollmentId))]
        public virtual Enrollment Enrollment { get; set; }
    }
}
