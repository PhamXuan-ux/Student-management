using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Student.Management.Domain.Entities
{
    public class CourseDepartment : BaseEntity
    {
        public int CourseId { get; set; }
        [ForeignKey(nameof(CourseId))]
        public virtual Course Course { get; set; }

        public int DepartmentId { get; set; }
        [ForeignKey(nameof(DepartmentId))]
        public virtual Department Department { get; set; }
    }
}
