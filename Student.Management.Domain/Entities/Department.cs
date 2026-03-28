using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Management.Domain.Entities
{
    public class Department : BaseEntity
    {
        [Required, StringLength(200)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual ICollection<CourseDepartment> CourseDepartments { get; set; }
    }
}