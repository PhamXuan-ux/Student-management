using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Student.Management.DataAccess.DataAccess;
using Student.Management.Domain.Entities;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("🚀 Starting Student Management Application...");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // Add services to the container
    builder.Services.AddControllersWithViews();
    builder.Services.AddRazorPages();

    // Register database and identity
    RegisterDatabaseAndIdentity(builder.Services, builder.Configuration);

    // Configure Application Cookie
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
    });

    builder.Services.AddDatabaseDeveloperPageExceptionFilter();

    var app = builder.Build();

    // Configure the HTTP request pipeline
    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseMigrationsEndPoint();
        Log.Information("Running in Development mode");
    }
    else
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
        Log.Information("Running in Production mode");
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();
    app.UseRouting();
    app.UseAuthentication();
    app.UseAuthorization();

    // Map routes
    app.MapControllerRoute(
        name: "areas",
        pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    app.MapRazorPages();

    // 🔹 Seed data
    await EnsureSeedDataAsync(app);

    Log.Information("✅ Application started successfully");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ Application terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}

// 🔹 Method để register database và identity
static void RegisterDatabaseAndIdentity(IServiceCollection services, IConfiguration configuration)
{
    // Register DbContext
    services.AddDbContext<StudentManagementDbContext>(options =>
        options.UseSqlServer(
            configuration.GetConnectionString("DefaultConnection"),
            sqlOptions => sqlOptions.MigrationsAssembly("Student.Management.DataAccess")
        ));

    // Register Identity
    services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password settings
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;

        // Lockout settings
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;

        // User settings
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        options.User.RequireUniqueEmail = true;

        // SignIn settings
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
    })
    .AddEntityFrameworkStores<StudentManagementDbContext>()
    .AddDefaultTokenProviders();

    // Configure Identity cookies
    services.ConfigureApplicationCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.SlidingExpiration = true;
    });
}

// 🔹 Seed data method
async Task EnsureSeedDataAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var serviceProvider = scope.ServiceProvider;

    try
    {
        var dbContext = serviceProvider.GetRequiredService<StudentManagementDbContext>();

        Log.Information("🔄 Checking database connection...");

        // Kiểm tra kết nối database trước
        var canConnect = await dbContext.Database.CanConnectAsync();
        if (!canConnect)
        {
            Log.Information("🔄 Database doesn't exist, creating...");
            await dbContext.Database.EnsureCreatedAsync();
        }

        Log.Information("🔄 Applying database migrations...");
        await dbContext.Database.MigrateAsync();
        Log.Information("✅ Database migrations applied successfully");

        var roleMgr = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userMgr = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        // Tạo roles với retry logic
        await CreateRolesWithRetry(roleMgr);

        // Tạo admin account với retry logic
        await CreateAdminWithRetry(userMgr);

        // 🔹 Gọi seeder đầy đủ
        Log.Information("🔄 Starting full database seeding...");
        await Student.Management.DataAccess.SeedData.DatabaseSeeder.SeedAsync(serviceProvider);
        Log.Information("✅ Full database seeding completed!");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "❌ Error during database seeding");
        // Không throw để app vẫn chạy được
    }
}

// Helper method để tạo roles với retry
async Task CreateRolesWithRetry(RoleManager<IdentityRole> roleMgr)
{
    string[] roles = { "Admin", "Teacher", "Student" };

    foreach (var role in roles)
    {
        try
        {
            if (!await roleMgr.RoleExistsAsync(role))
            {
                await roleMgr.CreateAsync(new IdentityRole(role));
                Log.Information($"✅ Created role: {role}");
            }
            else
            {
                Log.Information($"ℹ️ Role {role} already exists");
            }
        }
        catch (Exception ex)
        {
            Log.Warning($"Failed to create role {role}, retrying... Error: {ex.Message}");
            // Chờ 1 giây và thử lại
            await Task.Delay(1000);
            if (!await roleMgr.RoleExistsAsync(role))
            {
                await roleMgr.CreateAsync(new IdentityRole(role));
                Log.Information($"✅ Created role: {role} on retry");
            }
        }
    }
}

// Helper method để tạo admin với retry
async Task CreateAdminWithRetry(UserManager<ApplicationUser> userMgr)
{
    var adminEmail = "admin@btecfpt.edu.vn";

    try
    {
        var admin = await userMgr.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            admin = new ApplicationUser
            {
                Id = "admin-001",
                UserName = adminEmail,
                Email = adminEmail,
                FullName = "System Administrator",
                Address = "41 Le Duan, Hai Chau, Da Nang",
                Role = "Admin",
                DateCreated = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await userMgr.CreateAsync(admin, "Admin@123");
            if (result.Succeeded)
            {
                await userMgr.AddToRoleAsync(admin, "Admin");
                Log.Information("✅ Created default admin account: admin@btecfpt.edu.vn / Admin@123");
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Log.Error("❌ Failed to create admin: {Errors}", errors);
            }
        }
        else
        {
            Log.Information("ℹ️ Admin account already exists");
        }
    }
    catch (Exception ex)
    {
        Log.Warning($"Failed to create admin, retrying... Error: {ex.Message}");
        await Task.Delay(1000);
        // Thử lại
        var admin = await userMgr.FindByEmailAsync(adminEmail);
        if (admin == null)
        {
            // Thử tạo lại admin
        }
    }
}