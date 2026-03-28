using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Student.Management.Domain.Entities;

namespace Student.Management.DataAccess.DataAccess
{
    public class StudentManagementDbContext : IdentityDbContext<ApplicationUser>
    {
        public StudentManagementDbContext(DbContextOptions<StudentManagementDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<Program> Program { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Course> Course { get; set; }
        public DbSet<CourseDepartment> CourseDepartment { get; set; }
        public DbSet<StudentEntity> StudentEntity { get; set; }
        public DbSet<StudentProfile> StudentProfile { get; set; }
        public DbSet<ClassStudent> ClassStudents { get; set; }
        public DbSet<Enrollment> Enrollment { get; set; }
        public DbSet<EnrollmentDetail> EnrollmentDetail { get; set; }
        public DbSet<TeacherProfile> TeacherProfile { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Cấu hình các relationship và constraints
            modelBuilder.Entity<CourseDepartment>()
                .HasKey(cd => new { cd.CourseId, cd.DepartmentId });

            modelBuilder.Entity<ClassStudent>()
                .HasKey(cs => new { cs.ClassId, cs.StudentEntityId });

            // Cấu hình cho entity mới - ĐƠN GIẢN HÓA
            modelBuilder.Entity<Schedule>()
                .HasOne(s => s.Class)
                .WithMany(c => c.Schedules)
                .HasForeignKey(s => s.ClassId);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Class)
                .WithMany()
                .HasForeignKey(g => g.ClassId);

            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany()
                .HasForeignKey(g => g.StudentEntityId);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany()
                .HasForeignKey(a => a.ClassId);

            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentEntityId);

            // Configure ApplicationUser
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.FullName).HasMaxLength(255);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.Role).HasMaxLength(50);
            });

            // Configure StudentEntity
            modelBuilder.Entity<StudentEntity>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Relationship with ApplicationUser
                entity.HasOne(e => e.ApplicationStudent)
                      .WithMany()
                      .HasForeignKey(e => e.ApplicationStudentId)
                      .OnDelete(DeleteBehavior.Restrict);

                // One-to-one relationship with StudentProfile
                entity.HasOne(e => e.Profile)
                      .WithOne(sp => sp.Student) // Sửa thành 'Student' thay vì 'StudentEntity'
                      .HasForeignKey<StudentProfile>(sp => sp.StudentEntityId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure StudentProfile - SỬA LẠI PHẦN NÀY
            modelBuilder.Entity<StudentProfile>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Phone).HasMaxLength(20);
                entity.Property(e => e.Address).HasMaxLength(500);
                entity.Property(e => e.FullName).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(200).IsRequired();

                // One-to-one relationship với StudentEntity (sử dụng 'Student' thay vì 'StudentEntity')
                entity.HasOne(e => e.Student)
                      .WithOne(se => se.Profile)
                      .HasForeignKey<StudentProfile>(e => e.StudentEntityId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Enrollment
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Code).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Status).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Note).HasMaxLength(500);

                // Relationship with StudentEntity
                entity.HasOne(e => e.StudentEntity)
                      .WithMany(se => se.Enrollments)
                      .HasForeignKey(e => e.StudentEntityId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure ClassStudent (many-to-many)
            modelBuilder.Entity<ClassStudent>(entity =>
            {
                entity.HasKey(e => new { e.ClassId, e.StudentEntityId });

                entity.HasOne(e => e.Class)
                      .WithMany(c => c.ClassStudents)
                      .HasForeignKey(e => e.ClassId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.StudentEntity)
                      .WithMany(se => se.ClassStudents)
                      .HasForeignKey(e => e.StudentEntityId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure Program
            modelBuilder.Entity<Program>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(500);
            });

            // Configure Class
            modelBuilder.Entity<Class>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.ClassName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Room).HasMaxLength(200);

                entity.HasOne(e => e.Program)
                      .WithMany(p => p.Classes)
                      .HasForeignKey(e => e.ProgramId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Course
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CourseName).HasMaxLength(200);
                entity.Property(e => e.Instructor).HasMaxLength(200);
                entity.Property(e => e.FullName).HasMaxLength(200);

                entity.HasOne(e => e.Program)
                      .WithMany(p => p.Courses)
                      .HasForeignKey(e => e.ProgramId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure CourseDepartment (many-to-many)
            modelBuilder.Entity<CourseDepartment>(entity =>
            {
                entity.HasKey(e => new { e.CourseId, e.DepartmentId });

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.CourseDepartments)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Department)
                      .WithMany(d => d.CourseDepartments)
                      .HasForeignKey(e => e.DepartmentId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Configure EnrollmentDetail
            modelBuilder.Entity<EnrollmentDetail>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.HasOne(e => e.Enrollment)
                      .WithMany(e => e.EnrollmentDetails)
                      .HasForeignKey(e => e.EnrollmentId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Course)
                      .WithMany(c => c.EnrollmentDetails)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}