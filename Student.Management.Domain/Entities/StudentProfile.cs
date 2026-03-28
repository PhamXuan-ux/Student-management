using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Student.Management.Domain.Entities
{
public class StudentProfile : BaseEntity
{
     [Required]
     public bool IsActive { get; set; } = true;
        [StringLength(20)]
    public string? Phone { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Address { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Email { get; set; } = string.Empty;

       
    public DateTime? DateOfBirth { get; set; }

    

 
    public int StudentEntityId { get; set; }

    [ForeignKey(nameof(StudentEntityId))]
    public virtual StudentEntity? Student { get; set; }
}
}