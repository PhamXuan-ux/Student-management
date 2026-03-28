using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student.Management.Domain.Entities
{
    public class ClassStudent : BaseEntity
    {
        public int ClassId { get; set; }
        [ForeignKey(nameof(ClassId))]
        public virtual Class Class { get; set; }

        public int StudentEntityId { get; set; }
        [ForeignKey(nameof(StudentEntityId))]
        public virtual StudentEntity StudentEntity { get; set; }

        public DateTime JoinedOn { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
        public double? Grade { get; set; }
    }
}
