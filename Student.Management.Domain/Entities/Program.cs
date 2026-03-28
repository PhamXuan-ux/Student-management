using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Management.Domain.Entities
{
    public class Program : BaseEntity
    {
        [Required, StringLength(200)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public virtual ICollection<Course> Courses { get; set; }
        public virtual ICollection<Class> Classes { get; set; }

    }
}