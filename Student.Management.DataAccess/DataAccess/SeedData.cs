using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Student.Management.DataAccess.DataAccess;
using Student.Management.Domain.Entities;

namespace Student.Management.DataAccess.SeedData
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            var context = services.GetRequiredService<StudentManagementDbContext>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure database is created and migrated
            await context.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);
            await SeedAdminAsync(userManager);
            await SeedTeachersAsync(userManager);
            await SeedStudentsAsync(userManager);
            await SeedTeacherProfilesAsync(context, userManager);
            await SeedDepartmentsAsync(context);
            await SeedProgramsAsync(context);
            await SeedCoursesAsync(context);
            await SeedCourseDepartmentsAsync(context);
            await SeedClassesAsync(context, userManager);
            await SeedStudentEntitiesAsync(context, userManager);
            await SeedStudentProfilesAsync(context, userManager);
            await SeedClassStudentsAsync(context);
            await SeedEnrollmentsAsync(context);
            await SeedEnrollmentDetailsAsync(context);
            await SeedSchedulesAsync(context);
            await SeedGradesAsync(context);
            await SeedAttendanceAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Teacher", "Student" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@btecfpt.edu.vn";
            var admin = await userManager.FindByEmailAsync(adminEmail);

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

                var result = await userManager.CreateAsync(admin, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }

        private static async Task SeedTeachersAsync(UserManager<ApplicationUser> userManager)
        {
            var teachers = new[]
            {
                new { Id = "teacher-001", Email = "tranvan.hung@dnu.edu.vn", Name = "Tran Van Hung", Phone = "0901111111", Address = "54 Nguyen Luong Bang, Lien Chieu, Da Nang" },
                new { Id = "teacher-002", Email = "nguyenthimy@dnu.edu.vn", Name = "Nguyen Thi My", Phone = "0902222222", Address = "123 Ton Duc Thang, Hai Chau, Da Nang" },
                new { Id = "teacher-003", Email = "levan.phuc@dnu.edu.vn", Name = "Le Van Phuc", Phone = "0903333333", Address = "456 Dien Bien Phu, Thanh Khe, Da Nang" },
                new { Id = "teacher-004", Email = "phamthu.ha@dnu.edu.vn", Name = "Pham Thu Ha", Phone = "0904444444", Address = "789 Ngo Thi Sy, Son Tra, Da Nang" },
                new { Id = "teacher-005", Email = "hoangminh.tuan@dnu.edu.vn", Name = "Hoang Minh Tuan", Phone = "0905555555", Address = "321 Hoang Dieu, Hai Chau, Da Nang" },
                new { Id = "teacher-006", Email = "nguyenvan.khanh@dnu.edu.vn", Name = "Nguyen Van Khanh", Phone = "0906666666", Address = "159 Tran Hung Dao, Son Tra, Da Nang" },
                new { Id = "teacher-007", Email = "tranthi.lan@dnu.edu.vn", Name = "Tran Thi Lan", Phone = "0907777777", Address = "753 Le Loi, Hai Chau, Da Nang" },
                new { Id = "teacher-008", Email = "phamduc.anh@dnu.edu.vn", Name = "Pham Duc Anh", Phone = "0908888888", Address = "456 Hai Ba Trung, Hai Chau, Da Nang" },
                new { Id = "teacher-009", Email = "vuthi.hue@dnu.edu.vn", Name = "Vu Thi Hue", Phone = "0909999999", Address = "321 Nguyen Van Linh, Lien Chieu, Da Nang" },
                new { Id = "teacher-010", Email = "leminh.quan@dnu.edu.vn", Name = "Le Minh Quan", Phone = "0911111111", Address = "987 Hoang Dieu, Hai Chau, Da Nang" }
            };

            foreach (var teacher in teachers)
            {
                var user = await userManager.FindByEmailAsync(teacher.Email);
                if (user == null)
                {
                    var newTeacher = new ApplicationUser
                    {
                        Id = teacher.Id,
                        UserName = teacher.Email,
                        Email = teacher.Email,
                        FullName = teacher.Name,
                        PhoneNumber = teacher.Phone,
                        Address = teacher.Address,
                        Role = "Teacher",
                        DateCreated = DateTime.UtcNow,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newTeacher, "Teacher@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newTeacher, "Teacher");
                    }
                }
            }
        }

        private static async Task SeedTeacherProfilesAsync(StudentManagementDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.TeacherProfile.Any())
            {
                var teacherUsers = await userManager.Users
                    .Where(u => u.Role == "Teacher")
                    .ToListAsync();

                var teacherProfiles = new List<TeacherProfile>();
                var random = new Random();

                foreach (var teacher in teacherUsers)
                {
                    var year = random.Next(1975, 1985);
                    var month = random.Next(1, 13);
                    var day = random.Next(1, 29);

                    teacherProfiles.Add(new TeacherProfile
                    {
                        TeacherId = teacher.Id,
                        FullName = teacher.FullName ?? "Teacher",
                        Phone = teacher.PhoneNumber ?? "0900000000",
                        Address = teacher.Address ?? "Da Nang",
                        DateOfBirth = new DateTime(year, month, day),
                        IsActive = true
                    });
                }

                await context.TeacherProfile.AddRangeAsync(teacherProfiles);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedStudentsAsync(UserManager<ApplicationUser> userManager)
        {
            var students = new[]
            {
                new { Id = "student-001", Email = "nguyenvan.anh@gmail.com", Name = "Nguyen Van Anh", Phone = "0911111111", Address = "123 Tran Phu, Hai Chau, Da Nang" },
                new { Id = "student-002", Email = "tranthi.bich@gmail.com", Name = "Tran Thi Bich", Phone = "0912222222", Address = "456 Hung Vuong, Hai Chau, Da Nang" },
                new { Id = "student-003", Email = "levan.cuong@gmail.com", Name = "Le Van Cuong", Phone = "0913333333", Address = "789 Ong Ich Khiem, Hai Chau, Da Nang" },
                new { Id = "student-004", Email = "phamthuy.duong@gmail.com", Name = "Pham Thuy Duong", Phone = "0914444444", Address = "321 Nguyen Chi Thanh, Hai Chau, Da Nang" },
                new { Id = "student-005", Email = "hoangviet.anh@gmail.com", Name = "Hoang Viet Anh", Phone = "0915555555", Address = "654 Le Duan, Hai Chau, Da Nang" },
                new { Id = "student-006", Email = "vuthi.mai@gmail.com", Name = "Vu Thi Mai", Phone = "0916666666", Address = "987 Nguyen Van Linh, Lien Chieu, Da Nang" },
                new { Id = "student-007", Email = "dangminh.duc@gmail.com", Name = "Dang Minh Duc", Phone = "0917777777", Address = "147 Hoang Van Thu, Hai Chau, Da Nang" },
                new { Id = "student-008", Email = "buitien.hung@gmail.com", Name = "Bui Tien Hung", Phone = "0918888888", Address = "258 Phan Chau Trinh, Hai Chau, Da Nang" },
                new { Id = "student-009", Email = "ngothu.huong@gmail.com", Name = "Ngo Thu Huong", Phone = "0919999999", Address = "369 Hai Phong, Tan Binh, Da Nang" },
                new { Id = "student-010", Email = "dotan.phat@gmail.com", Name = "Do Tan Phat", Phone = "0921111111", Address = "741 Yen Bai, Hai Chau, Da Nang" },
                new { Id = "student-011", Email = "trinhvan.hai@gmail.com", Name = "Trinh Van Hai", Phone = "0922222222", Address = "852 Nguyen Huu Tho, Hai Chau, Da Nang" },
                new { Id = "student-012", Email = "lamthi.kim@gmail.com", Name = "Lam Thi Kim", Phone = "0923333333", Address = "963 Trieu Nu Vuong, Son Tra, Da Nang" },
                new { Id = "student-013", Email = "caominh.quan@gmail.com", Name = "Cao Minh Quan", Phone = "0924444444", Address = "159 An Thuong 4, Ngu Hanh Son, Da Nang" },
                new { Id = "student-014", Email = "lythuy.linh@gmail.com", Name = "Ly Thuy Linh", Phone = "0925555555", Address = "753 Vo Van Kiet, Son Tra, Da Nang" },
                new { Id = "student-015", Email = "vongoc.bao@gmail.com", Name = "Vo Ngoc Bao", Phone = "0926666666", Address = "486 Nui Thanh, Hai Chau, Da Nang" },
                new { Id = "student-016", Email = "dinhvan.hieu@gmail.com", Name = "Dinh Van Hieu", Phone = "0927777777", Address = "279 Le Hong Phong, Hai Chau, Da Nang" },
                new { Id = "student-017", Email = "maithi.thuy@gmail.com", Name = "Mai Thi Thuy", Phone = "0928888888", Address = "684 Nguyen Thi Minh Khai, Hai Chau, Da Nang" },
                new { Id = "student-018", Email = "haanh.vu@gmail.com", Name = "Ha Anh Vu", Phone = "0929999999", Address = "375 Tran Cao Van, Thanh Khe, Da Nang" },
                new { Id = "student-019", Email = "nguyenthanh.tung@gmail.com", Name = "Nguyen Thanh Tung", Phone = "0931111111", Address = "582 Phan Tu, Ngu Hanh Son, Da Nang" },
                new { Id = "student-020", Email = "tranthuy.tien@gmail.com", Name = "Tran Thuy Tien", Phone = "0932222222", Address = "795 Hoang Hoa Tham, Lien Chieu, Da Nang" }
            };

            foreach (var student in students)
            {
                var user = await userManager.FindByEmailAsync(student.Email);
                if (user == null)
                {
                    var newStudent = new ApplicationUser
                    {
                        Id = student.Id,
                        UserName = student.Email,
                        Email = student.Email,
                        FullName = student.Name,
                        PhoneNumber = student.Phone,
                        Address = student.Address,
                        Role = "Student",
                        DateCreated = DateTime.UtcNow,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(newStudent, "Student@123");
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newStudent, "Student");
                    }
                }
            }
        }

        private static async Task SeedDepartmentsAsync(StudentManagementDbContext context)
        {
            if (!context.Department.Any())
            {
                var departments = new[]
                {
                    new Department { Title = "Information Technology", Description = "Training in programming, information systems, and artificial intelligence", IsActive = true },
                    new Department { Title = "Software Engineering", Description = "Professional software engineer training, application and system development", IsActive = true },
                    new Department { Title = "Computer Science", Description = "Fundamental research in computer science, algorithms and data structures", IsActive = true },
                    new Department { Title = "Network and Cybersecurity", Description = "Training experts in computer networks and system security", IsActive = true },
                    new Department { Title = "Data Engineering", Description = "Specialization in data science, big data analytics and AI", IsActive = true }
                };

                await context.Department.AddRangeAsync(departments);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProgramsAsync(StudentManagementDbContext context)
        {
            if (!context.Program.Any())
            {
                var programs = new[]
                {
                    new Program { Name = "Bachelor of Information Technology", Description = "4-year undergraduate program in Information Technology", IsActive = true },
                    new Program { Name = "Bachelor of Software Engineering", Description = "4-year undergraduate program in Software Engineering", IsActive = true },
                    new Program { Name = "Bachelor of Computer Science", Description = "4-year undergraduate program in Computer Science", IsActive = true },
                    new Program { Name = "Bachelor of Cybersecurity", Description = "4-year undergraduate program in Network Security", IsActive = true },
                    new Program { Name = "Bachelor of Data Science", Description = "4-year undergraduate program in Data Science and AI", IsActive = true }
                };

                await context.Program.AddRangeAsync(programs);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedCoursesAsync(StudentManagementDbContext context)
        {
            if (!context.Course.Any())
            {
                var courses = new[]
                {
                    new Course { CourseName = "C# Programming Fundamentals", Instructor = "Tran Van Hung", FullName = "Tran Van Hung", Price = 2500000, Available = true, CreatedOn = DateTime.Now.AddDays(-60), IsActive = true, ProgramId = 1 },
                    new Course { CourseName = "Database Management Systems", Instructor = "Nguyen Thi My", FullName = "Nguyen Thi My", Price = 2200000, Available = true, CreatedOn = DateTime.Now.AddDays(-55), IsActive = true, ProgramId = 1 },
                    new Course { CourseName = "Web Development with ASP.NET", Instructor = "Le Van Phuc", FullName = "Le Van Phuc", Price = 3000000, Available = true, CreatedOn = DateTime.Now.AddDays(-50), IsActive = true, ProgramId = 2 },
                    new Course { CourseName = "Advanced Algorithms", Instructor = "Pham Thu Ha", FullName = "Pham Thu Ha", Price = 2800000, Available = true, CreatedOn = DateTime.Now.AddDays(-45), IsActive = true, ProgramId = 3 },
                    new Course { CourseName = "Network Security Principles", Instructor = "Hoang Minh Tuan", FullName = "Hoang Minh Tuan", Price = 3200000, Available = true, CreatedOn = DateTime.Now.AddDays(-40), IsActive = true, ProgramId = 4 },
                    new Course { CourseName = "Data Science with Python", Instructor = "Nguyen Van Khanh", FullName = "Nguyen Van Khanh", Price = 3500000, Available = true, CreatedOn = DateTime.Now.AddDays(-35), IsActive = true, ProgramId = 5 },
                    new Course { CourseName = "Software Engineering Practices", Instructor = "Tran Thi Lan", FullName = "Tran Thi Lan", Price = 2600000, Available = true, CreatedOn = DateTime.Now.AddDays(-30), IsActive = true, ProgramId = 2 },
                    new Course { CourseName = "Mobile App Development", Instructor = "Pham Duc Anh", FullName = "Pham Duc Anh", Price = 2900000, Available = true, CreatedOn = DateTime.Now.AddDays(-25), IsActive = true, ProgramId = 2 },
                    new Course { CourseName = "Artificial Intelligence Fundamentals", Instructor = "Vu Thi Hue", FullName = "Vu Thi Hue", Price = 3800000, Available = true, CreatedOn = DateTime.Now.AddDays(-20), IsActive = true, ProgramId = 5 },
                    new Course { CourseName = "Cloud Computing Architecture", Instructor = "Le Minh Quan", FullName = "Le Minh Quan", Price = 3300000, Available = true, CreatedOn = DateTime.Now.AddDays(-15), IsActive = true, ProgramId = 4 }
                };

                await context.Course.AddRangeAsync(courses);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedCourseDepartmentsAsync(StudentManagementDbContext context)
        {
            if (!context.CourseDepartment.Any())
            {
                var courseDepartments = new[]
                {
                    new CourseDepartment { CourseId = 1, DepartmentId = 1 },
                    new CourseDepartment { CourseId = 1, DepartmentId = 2 },
                    new CourseDepartment { CourseId = 2, DepartmentId = 1 },
                    new CourseDepartment { CourseId = 2, DepartmentId = 5 },
                    new CourseDepartment { CourseId = 3, DepartmentId = 2 },
                    new CourseDepartment { CourseId = 4, DepartmentId = 3 },
                    new CourseDepartment { CourseId = 5, DepartmentId = 4 },
                    new CourseDepartment { CourseId = 6, DepartmentId = 5 },
                    new CourseDepartment { CourseId = 7, DepartmentId = 2 },
                    new CourseDepartment { CourseId = 8, DepartmentId = 2 },
                    new CourseDepartment { CourseId = 9, DepartmentId = 5 },
                    new CourseDepartment { CourseId = 10, DepartmentId = 4 }
                };

                await context.CourseDepartment.AddRangeAsync(courseDepartments);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedClassesAsync(StudentManagementDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.Classes.Any())
            {
                var teachers = await userManager.Users
                    .Where(u => u.Role == "Teacher")
                    .OrderBy(t => t.Email) // Thêm OrderBy để đảm bảo thứ tự cố định
                    .Take(10)
                    .ToListAsync();

                // Kiểm tra null trước khi gán
                if (teachers.Count < 7) return; // Đảm bảo có đủ giáo viên để gán bên dưới

                var courses = await context.Course.Take(10).ToListAsync();

                var classes = new[]
                {
            new Class {
                ClassName = "SE101 - Introduction to Software Engineering",
                Room = "A101 - Building A",
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2025, 1, 15),
                IsActive = true,
                ProgramId = 2,
                TeacherId = teachers[0].Id, // Tran Van Hung
                CourseId = 1 // C# Programming
            },
            new Class {
                ClassName = "DB201 - Database Systems",
                Room = "B201 - Building B",
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2025, 1, 15),
                IsActive = true,
                ProgramId = 1,
                TeacherId = teachers[1].Id, // Nguyen Thi My
                CourseId = 2 // Database Management
            },
            new Class {
                ClassName = "WD301 - Web Development",
                Room = "A201 - Building A",
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2025, 1, 15),
                IsActive = true,
                ProgramId = 2,
                TeacherId = teachers[2].Id, // Le Van Phuc
                CourseId = 3 // Web Development
            },
            new Class {
                ClassName = "AL401 - Advanced Algorithms",
                Room = "C101 - Building C",
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2025, 1, 15),
                IsActive = true,
                ProgramId = 3,
                TeacherId = teachers[3].Id, // Pham Thu Ha
                CourseId = 4 // Advanced Algorithms
            },
            new Class {
                ClassName = "NS501 - Network Security",
                Room = "Lab-01 - Lab Building",
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2025, 1, 15),
                IsActive = true,
                ProgramId = 4,
                TeacherId = teachers[4].Id, // Hoang Minh Tuan
                CourseId = 5 // Network Security
            },
            // Thêm nhiều lớp hơn để mỗi teacher có ít nhất 1 lớp
            new Class {
                ClassName = "DS601 - Data Science",
                Room = "B101 - Building B",
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2025, 1, 15),
                IsActive = true,
                ProgramId = 5,
                TeacherId = teachers[5].Id, // Nguyen Van Khanh
                CourseId = 6 // Data Science
            },
            new Class {
                ClassName = "SE701 - Software Engineering Practices",
                Room = "A102 - Building A",
                StartDate = new DateTime(2024, 9, 1),
                EndDate = new DateTime(2025, 1, 15),
                IsActive = true,
                ProgramId = 2,
                TeacherId = teachers[6].Id, // Tran Thi Lan
                CourseId = 7 // Software Engineering
            }
        };

                await context.Classes.AddRangeAsync(classes);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedStudentEntitiesAsync(StudentManagementDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.StudentEntity.Any())
            {
                var studentUsers = await userManager.Users
                    .Where(u => u.Role == "Student")
                    .ToListAsync();

                var studentEntities = studentUsers.Select(user => new StudentEntity
                {
                    IsActive = true,
                    ApplicationStudentId = user.Id
                }).ToList();

                await context.StudentEntity.AddRangeAsync(studentEntities);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedStudentProfilesAsync(StudentManagementDbContext context, UserManager<ApplicationUser> userManager)
        {
            if (!context.StudentProfile.Any())
            {
                var studentEntities = await context.StudentEntity.ToListAsync();
                var studentProfiles = new List<StudentProfile>();
                var random = new Random();

                foreach (var student in studentEntities)
                {
                    var user = await userManager.FindByIdAsync(student.ApplicationStudentId);
                    if (user != null)
                    {
                        var year = random.Next(2000, 2004);
                        var month = random.Next(1, 13);
                        var day = random.Next(1, 29);

                        studentProfiles.Add(new StudentProfile
                        {
                            Phone = user.PhoneNumber ?? "0901234567",
                            Address = user.Address ?? "41 Le Duan, Da Nang",
                            FullName = user.FullName ?? "Student Name",
                            Email = user.Email ?? "student@example.com",
                            IsActive = true,
                            StudentEntityId = student.Id,
                            DateOfBirth = new DateTime(year, month, day)
                        });
                    }
                }

                await context.StudentProfile.AddRangeAsync(studentProfiles);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedClassStudentsAsync(StudentManagementDbContext context)
        {
            if (!context.ClassStudents.Any())
            {
                var classStudents = new List<ClassStudent>();
                var students = await context.StudentEntity.Take(20).ToListAsync();
                var classes = await context.Classes
                    .Include(c => c.Course) // Thêm include
                    .Include(c => c.Teacher) // Thêm include
                    .Take(5).ToListAsync();

                // Điểm cố định từ 6.5 đến 10.0
                double[] fixedGrades = { 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 10.0 };
                var random = new Random();

                // Phân phối sinh viên vào các lớp
                int studentIndex = 0;
                foreach (var classItem in classes)
                {
                    // Mỗi lớp có 10-15 sinh viên
                    var studentCount = random.Next(10, 16);

                    for (int i = 0; i < studentCount && studentIndex < students.Count; i++)
                    {
                        var student = students[studentIndex];

                        if (!classStudents.Any(cs => cs.ClassId == classItem.Id && cs.StudentEntityId == student.Id))
                        {
                            classStudents.Add(new ClassStudent
                            {
                                ClassId = classItem.Id,
                                StudentEntityId = student.Id,
                                JoinedOn = new DateTime(2024, 9, 1).AddDays(random.Next(0, 7)),
                                IsActive = true,
                                Grade = fixedGrades[random.Next(0, fixedGrades.Length)],
                                // Thêm thông tin navigation (nếu cần)
                                Class = classItem,
                                StudentEntity = student
                            });
                        }
                        studentIndex++;
                    }
                }

                await context.ClassStudents.AddRangeAsync(classStudents);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedEnrollmentsAsync(StudentManagementDbContext context)
        {
            if (!context.Enrollment.Any())
            {
                var students = await context.StudentEntity.Take(15).ToListAsync();
                var enrollments = new List<Enrollment>();

                foreach (var student in students)
                {
                    enrollments.Add(new Enrollment
                    {
                        Code = $"ENR-{student.Id:00000}-2024-01",
                        CreatedOn = new DateTime(2024, 8, 15),
                        Note = $"Đăng ký học kỳ 1 năm học 2024-2025",
                        Status = "Completed",
                        IsActive = true,
                        StudentEntityId = student.Id
                    });
                }

                await context.Enrollment.AddRangeAsync(enrollments);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedEnrollmentDetailsAsync(StudentManagementDbContext context)
        {
            if (!context.EnrollmentDetail.Any())
            {
                var enrollments = await context.Enrollment.Take(15).ToListAsync();
                var courses = await context.Course.Take(5).ToListAsync();
                var enrollmentDetails = new List<EnrollmentDetail>();

                foreach (var enrollment in enrollments)
                {
                    // Mỗi đăng ký có 3 môn học
                    var selectedCourses = courses.OrderBy(x => Guid.NewGuid()).Take(3);

                    foreach (var course in selectedCourses)
                    {
                        enrollmentDetails.Add(new EnrollmentDetail
                        {
                            Price = course.Price ?? 0,
                            Quantity = 1,
                            Note = $"Đăng ký môn {course.CourseName} học kỳ 2024-2025",
                            CourseId = course.Id,
                            EnrollmentId = enrollment.Id
                        });
                    }
                }

                await context.EnrollmentDetail.AddRangeAsync(enrollmentDetails);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedSchedulesAsync(StudentManagementDbContext context)
        {
            if (!context.Schedules.Any())
            {
                var classes = await context.Classes
                    .Include(c => c.Course)
                    .Include(c => c.Teacher)
                    .Take(5)
                    .ToListAsync();

                var schedules = new List<Schedule>();
                var random = new Random();

                // Tạo danh sách các khung giờ không trùng nhau
                var timeSlots = new[]
                {
            new { Day = "Monday", Time = "07:30 - 09:00", Room = "A101 - Building A" },
            new { Day = "Monday", Time = "09:10 - 10:40", Room = "A102 - Building A" },
            new { Day = "Monday", Time = "13:00 - 14:30", Room = "B201 - Building B" },
            new { Day = "Tuesday", Time = "07:30 - 09:00", Room = "A101 - Building A" },
            new { Day = "Tuesday", Time = "09:10 - 10:40", Room = "A201 - Building A" },
            new { Day = "Tuesday", Time = "14:40 - 16:10", Room = "B202 - Building B" },
            new { Day = "Wednesday", Time = "09:10 - 10:40", Room = "B201 - Building B" },
            new { Day = "Wednesday", Time = "13:00 - 14:30", Room = "A101 - Building A" },
            new { Day = "Thursday", Time = "07:30 - 09:00", Room = "A102 - Building A" },
            new { Day = "Thursday", Time = "14:40 - 16:10", Room = "A101 - Building A" },
            new { Day = "Friday", Time = "09:10 - 10:40", Room = "A201 - Building A" },
            new { Day = "Friday", Time = "13:00 - 14:30", Room = "B201 - Building B" }
        };

                var usedTimeSlots = new HashSet<string>();

                foreach (var classItem in classes)
                {
                    // Mỗi lớp có 2 buổi học/tuần
                    var availableSlots = timeSlots
                        .Where(ts => !usedTimeSlots.Contains($"{ts.Day}_{ts.Time}_{ts.Room}"))
                        .OrderBy(x => random.Next())
                        .Take(2)
                        .ToList();

                    foreach (var slot in availableSlots)
                    {
                        var startEnd = slot.Time.Split(" - ");
                        schedules.Add(new Schedule
                        {
                            ClassId = classItem.Id,
                            DayOfWeek = slot.Day,
                            StartTime = startEnd[0],
                            EndTime = startEnd[1],
                            Room = slot.Room,
                            IsActive = true,
                            CreatedDate = new DateTime(2025, 12, 1)
                        });

                        // Đánh dấu slot đã dùng
                        usedTimeSlots.Add($"{slot.Day}_{slot.Time}_{slot.Room}");
                    }
                }

                await context.Schedules.AddRangeAsync(schedules);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedGradesAsync(StudentManagementDbContext context)
        {
            if (!context.Grades.Any())
            {
                var classStudents = await context.ClassStudents.ToListAsync();
                var grades = new List<Grade>();
                var random = new Random();

                // Điểm cố định từ 6.5 đến 10.0
                double[] fixedScores = { 6.5, 7.0, 7.5, 8.0, 8.5, 9.0, 9.5, 10.0 };

                foreach (var classStudent in classStudents)
                {
                    // Mỗi sinh viên trong lớp có 4 loại điểm
                    var gradeTypes = new[]
                    {
                        new { Type = "Quiz", Weight = 20 },
                        new { Type = "Assignment", Weight = 30 },
                        new { Type = "Midterm", Weight = 20 },
                        new { Type = "Final", Weight = 30 }
                    };

                    foreach (var gradeType in gradeTypes)
                    {
                        grades.Add(new Grade
                        {
                            ClassId = classStudent.ClassId,
                            StudentEntityId = classStudent.StudentEntityId,
                            GradeType = gradeType.Type,
                            Score = fixedScores[random.Next(0, fixedScores.Length)],
                            MaxScore = 10.0,
                            Weight = gradeType.Weight,
                            CreatedDate = new DateTime(2024, 9, 1).AddDays(random.Next(30, 120)),
                            Note = $"{gradeType.Type} evaluation - Semester 2024-2025"
                        });
                    }
                }

                await context.Grades.AddRangeAsync(grades);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAttendanceAsync(StudentManagementDbContext context)
        {
            if (!context.Attendances.Any())
            {
                var classStudents = await context.ClassStudents.ToListAsync();
                var attendances = new List<Attendance>();
                var random = new Random();

                var startDate = new DateTime(2024, 9, 2);
                var endDate = new DateTime(2024, 12, 20);

                foreach (var classStudent in classStudents)
                {
                    var currentDate = startDate;
                    while (currentDate <= endDate)
                    {
                        // Chỉ điểm danh vào các ngày trong tuần (Monday-Friday)
                        if (currentDate.DayOfWeek >= DayOfWeek.Monday && currentDate.DayOfWeek <= DayOfWeek.Friday)
                        {
                            // 90% có mặt, 5% vắng có phép, 5% vắng không phép
                            var statusRandom = random.Next(1, 101);
                            string status;

                            if (statusRandom <= 90) status = "Present";
                            else if (statusRandom <= 95) status = "AbsentWithPermission";
                            else status = "AbsentWithoutPermission";

                            attendances.Add(new Attendance
                            {
                                ClassId = classStudent.ClassId,
                                StudentEntityId = classStudent.StudentEntityId,
                                AttendanceDate = currentDate,
                                Status = status,
                                Notes = status == "Present" ? "" : "Ghi chú vắng học",
                                RecordedDate = currentDate.AddHours(8)
                            });
                        }
                        currentDate = currentDate.AddDays(1);
                    }
                }

                await context.Attendances.AddRangeAsync(attendances);
                await context.SaveChangesAsync();
            }
        }


    }
}