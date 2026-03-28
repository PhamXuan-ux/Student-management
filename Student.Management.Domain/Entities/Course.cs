using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Management.Domain.Entities
{
    public class Course : BaseEntity
    {
        [Required]
        public bool IsActive { get; set; }
        [StringLength(200)]
        public string CourseName { get; set; }

        [StringLength(200)]
        public string Instructor { get; set; }

        [StringLength(200)]
        public string FullName { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }
        public bool? Available { get; set; }
        public DateTime? CreatedOn { get; set; }

        

        public int ProgramId { get; set; }

        [ForeignKey(nameof(ProgramId))]
        public virtual Program Program { get; set; }

        public virtual ICollection<EnrollmentDetail> EnrollmentDetails { get; set; }
        public virtual ICollection<CourseDepartment> CourseDepartments { get; set; }

        public virtual Department? Department
        {
            get
            {
                return CourseDepartments?.FirstOrDefault()?.Department;
            }
        }
    }
}
