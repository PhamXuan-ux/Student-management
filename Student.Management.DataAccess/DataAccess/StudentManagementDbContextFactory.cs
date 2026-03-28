using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Student.Management.DataAccess.DataAccess
{
    public class StudentManagementDbContextFactory : IDesignTimeDbContextFactory<StudentManagementDbContext>
    {
        public StudentManagementDbContext CreateDbContext(string[] args)
        {
            // Lấy cấu hình từ appsettings.json của project web chính
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Management.Project"))
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<StudentManagementDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new StudentManagementDbContext(optionsBuilder.Options);
        }
    }
}

