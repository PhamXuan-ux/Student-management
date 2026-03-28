using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Student.Management.DataAccess.DataAccess;
using Student.Management.Domain.Entities;


namespace Student.Management.Infrastructure.Configuation
{
    public static class ConfiguationDbAccess
    {
        public static void RegisterDb(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            // Đăng ký DbContext
            services.AddDbContext<StudentManagementDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Đăng ký Identity
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<StudentManagementDbContext>()
                .AddDefaultTokenProviders();
        }
    }
}

