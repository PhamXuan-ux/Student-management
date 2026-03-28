using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering; 
using Microsoft.EntityFrameworkCore;
using Student.Management.DataAccess.DataAccess;
using Student.Management.Domain.Entities;
using System.Linq;
using ProgramEntity = Student.Management.Domain.Entities.Program;

namespace Student.Management.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly StudentManagementDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(StudentManagementDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Grade()
        {
            var classes = await _context.Classes
                .Include(c => c.Course)
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();

            ViewBag.ClassList = new SelectList(classes.Select(c => new
            {
                Id = c.Id,
                DisplayText = $"{c.ClassName} ({c.Course?.CourseName})"
            }), "Id", "DisplayText");

            return View();
        }

        // 2. API: Lấy bảng điểm của một lớp cụ thể
        [HttpGet]
        public async Task<IActionResult> GetGradesByClass(int classId)
        {
            try
            {
                var classStudents = await _context.ClassStudents
                    .Include(cs => cs.StudentEntity)
                        .ThenInclude(s => s.Profile)
                    .Where(cs => cs.ClassId == classId)
                    .ToListAsync();
                var grades = await _context.Grades
                    .Where(g => g.ClassId == classId)
                    .ToListAsync();

                // Cấu trúc dữ liệu trả về cho Frontend
                var data = classStudents.Select(cs => new
                {
                    StudentId = cs.StudentEntityId,
                    StudentName = cs.StudentEntity?.Profile?.FullName ?? "Unknown",
                    StudentCode = cs.StudentEntity?.ApplicationStudentId, // Hoặc mã SV nếu có
                    Email = cs.StudentEntity?.Profile?.Email,
                    FinalGrade = cs.Grade, // Điểm tổng kết hiện tại
                    // Lấy chi tiết từng đầu điểm
                    Quiz = grades.FirstOrDefault(g => g.StudentEntityId == cs.StudentEntityId && g.GradeType == "Quiz")?.Score,
                    Assignment = grades.FirstOrDefault(g => g.StudentEntityId == cs.StudentEntityId && g.GradeType == "Assignment")?.Score,
                    Midterm = grades.FirstOrDefault(g => g.StudentEntityId == cs.StudentEntityId && g.GradeType == "Midterm")?.Score,
                    Final = grades.FirstOrDefault(g => g.StudentEntityId == cs.StudentEntityId && g.GradeType == "Final")?.Score
                }).OrderBy(x => x.StudentName).ToList();

                return Json(new { success = true, data = data });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // 3. API: Lưu điểm (Create or Update)
        [HttpPost]
        public async Task<IActionResult> SaveGrade([FromBody] GradeSubmissionDto model)
        {
            try
            {
                // Validate
                if (model.Score < 0 || model.Score > 10)
                    return Json(new { success = false, message = "Score must be between 0 and 10." });

                var grade = await _context.Grades
                    .FirstOrDefaultAsync(g => g.ClassId == model.ClassId &&
                                            g.StudentEntityId == model.StudentEntityId &&
                                            g.GradeType == model.GradeType);

                if (grade == null)
                {
                    // Create new grade
                    grade = new Grade
                    {
                        ClassId = model.ClassId,
                        StudentEntityId = model.StudentEntityId,
                        GradeType = model.GradeType,
                        Score = model.Score,
                        MaxScore = 10,
                        Weight = GetWeightByGradeType(model.GradeType),
                        CreatedDate = DateTime.UtcNow,
                        Note = $"Updated by Admin on {DateTime.UtcNow:MM/dd/yyyy}"
                    };
                    _context.Grades.Add(grade);
                }
                else
                {
                    // Update existing grade
                    grade.Score = model.Score;
                    grade.CreatedDate = DateTime.UtcNow; // Update modified date
                    _context.Grades.Update(grade);
                }

                await _context.SaveChangesAsync();

                // Tính toán lại điểm tổng kết (Average) cho sinh viên
                await UpdateFinalGrade(model.ClassId, model.StudentEntityId);

                return Json(new { success = true, message = "Saved successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // Helper Class để nhận dữ liệu JSON
        public class GradeSubmissionDto
        {
            public int ClassId { get; set; }
            public int StudentEntityId { get; set; }
            public string GradeType { get; set; }
            public double Score { get; set; }
        }

        // Helper: Lấy trọng số điểm
        private double GetWeightByGradeType(string gradeType)
        {
            return gradeType.ToLower() switch
            {
                "quiz" => 20,
                "assignment" => 30,
                "midterm" => 20,
                "final" => 30,
                _ => 0
            };
        }

        // Helper: Tính điểm trung bình và cập nhật vào bảng ClassStudent
        private async Task UpdateFinalGrade(int classId, int studentEntityId)
        {
            var grades = await _context.Grades
                .Where(g => g.ClassId == classId && g.StudentEntityId == studentEntityId)
                .ToListAsync();

            if (grades.Any())
            {
                
                var totalWeight = grades.Sum(g => g.Weight);
                if (totalWeight > 0)
                {
                    var weightedAverage = grades.Sum(g => g.Score * g.Weight) / totalWeight;

                    var classStudent = await _context.ClassStudents
                        .FirstOrDefaultAsync(cs => cs.ClassId == classId && cs.StudentEntityId == studentEntityId);

                    if (classStudent != null)
                    {
                        classStudent.Grade = Math.Round(weightedAverage, 1);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
        // ==================================================================================
        // SECTION 1: VIEW ACTIONS (Trả về giao diện HTML)
        // ==================================================================================

        public async Task<IActionResult> Index()
        {
            try
            {
                var stats = new
                {
                    TotalStudents = await _context.StudentEntity.CountAsync(),
                    TotalTeachers = await _userManager.Users.CountAsync(u => u.Role == "Teacher"),
                    TotalClasses = await _context.Classes.CountAsync(),
                    TotalCourses = await _context.Course.CountAsync(),
                    RecentEnrollments = await _context.Enrollment.CountAsync(e => e.CreatedOn >= DateTime.UtcNow.AddDays(-7))
                };
                ViewBag.Stats = stats;
                return View();
            }
            catch (Exception) { return View(); }
        }

        public async Task<IActionResult> Users(string role = "")
        {
            var query = _userManager.Users.AsQueryable();
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            ViewBag.CurrentRole = role;
            ViewBag.Roles = new List<string> { "Admin", "Teacher", "Student" };
            return View(await query.OrderBy(u => u.FullName).ToListAsync());
        }

        public async Task<IActionResult> Department()
        {
            return View(await _context.Department.Where(d => d.IsActive).OrderBy(d => d.Title).ToListAsync());
        }

        public async Task<IActionResult> Program()
        {
            return View(await _context.Program.Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync());
        }

        public async Task<IActionResult> Class()
        {
            // 1. Lấy danh sách Class để hiển thị bảng
            var classes = await _context.Classes
                .Include(c => c.Program)
                .Where(c => c.IsActive)
                .OrderByDescending(c => c.StartDate)
                .ToListAsync();

            // 2. Lấy danh sách Program để hiển thị trong Dropdown (Modal Thêm/Sửa)
            var programs = await _context.Program.Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync();
            ViewBag.ProgramList = new SelectList(programs, "Id", "Name");

            return View(classes);
        }

        public async Task<IActionResult> Course()
        {
            // 1. Lấy danh sách Course để hiển thị bảng
            var courses = await _context.Course
                .Include(c => c.Program)
                .Where(c => c.IsActive)
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            // 2. Lấy danh sách Program để hiển thị trong Dropdown
            var programs = await _context.Program.Where(p => p.IsActive).OrderBy(p => p.Name).ToListAsync();
            ViewBag.ProgramList = new SelectList(programs, "Id", "Name");

            return View(courses);
        }

        public async Task<IActionResult> Enrollment()
        {
            // 1. Lấy danh sách Enrollment
            var enrollments = await _context.Enrollment
                .Include(e => e.StudentEntity).ThenInclude(se => se.ApplicationStudent)
                .Where(e => e.IsActive)
                .OrderByDescending(e => e.CreatedOn)
                .ToListAsync();

            // 2. Lấy danh sách Sinh viên cho Dropdown
            // Select ra object có Id (của Entity) và tên hiển thị (của User)
            var students = await _context.StudentEntity
                .Include(s => s.ApplicationStudent)
                .Where(s => s.IsActive && s.ApplicationStudent != null)
                .Select(s => new
                {
                    Id = s.Id,
                    DisplayText = $"{s.ApplicationStudent.FullName} ({s.ApplicationStudent.Email})"
                })
                .ToListAsync();

            ViewBag.StudentList = new SelectList(students, "Id", "DisplayText");

            return View(enrollments);
        }

        // ==================================================================================
        // SECTION 2: API ACTIONS (Trả về JSON cho AJAX/Fetch)
        // ==================================================================================

        [HttpGet]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = new
                {
                    TotalStudents = await _context.StudentEntity.CountAsync(),
                    TotalTeachers = await _userManager.Users.CountAsync(u => u.Role == "Teacher"),
                    TotalClasses = await _context.Classes.CountAsync(),
                    TotalCourses = await _context.Course.CountAsync(),
                    StudentsByDepartment = await GetStudentsByDepartment(),
                    ClassesByProgram = await GetClassesByProgram(),
                    RecentEnrollments = await _context.Enrollment.CountAsync(e => e.CreatedOn >= DateTime.UtcNow.AddDays(-30))
                };
                return Json(new { success = true, data = stats });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        #region Users CRUD API
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] ApplicationUser user)
        {
            // Transaction quan trọng để đảm bảo tạo User xong phải tạo được StudentEntity/Profile
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                if (string.IsNullOrEmpty(user.Email) || string.IsNullOrEmpty(user.FullName))
                    return Json(new { success = false, message = "Email and FullName are required" });

                if (await _userManager.FindByEmailAsync(user.Email) != null)
                    return Json(new { success = false, message = "User with this email already exists" });

                var newUser = new ApplicationUser
                {
                    UserName = user.Email,
                    Email = user.Email,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Role = user.Role,
                    DateCreated = DateTime.UtcNow,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(newUser, "Default@123");
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(user.Role)) await _userManager.AddToRoleAsync(newUser, user.Role);

                    // LOGIC RIÊNG CHO STUDENT
                    if (user.Role == "Student")
                    {
                        var studentEntity = new StudentEntity { ApplicationStudentId = newUser.Id, IsActive = true };
                        _context.StudentEntity.Add(studentEntity);
                        await _context.SaveChangesAsync();

                        var studentProfile = new StudentProfile
                        {
                            StudentEntityId = studentEntity.Id,
                            FullName = newUser.FullName,
                            Email = newUser.Email,
                            Phone = newUser.PhoneNumber ?? "",
                            Address = newUser.Address ?? "",
                            IsActive = true
                        };
                        _context.StudentProfile.Add(studentProfile);
                        await _context.SaveChangesAsync();
                    }

                    await transaction.CommitAsync();
                    return Json(new { success = true, message = "User created successfully", data = newUser });
                }
                else
                {
                    return Json(new { success = false, message = $"User creation failed: {string.Join(", ", result.Errors.Select(e => e.Description))}" });
                }
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateUser([FromBody] ApplicationUser user)
        {
            try
            {
                var existingUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
                if (existingUser == null) return Json(new { success = false, message = "User not found" });

                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.UserName = user.Email;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.Address = user.Address;

                if (!string.IsNullOrEmpty(user.Role) && existingUser.Role != user.Role)
                {
                    var currentRoles = await _userManager.GetRolesAsync(existingUser);
                    await _userManager.RemoveFromRolesAsync(existingUser, currentRoles);
                    await _userManager.AddToRoleAsync(existingUser, user.Role);
                    existingUser.Role = user.Role;
                }

                var result = await _userManager.UpdateAsync(existingUser);

                // Đồng bộ thông tin sang Profile nếu là Student
                if (existingUser.Role == "Student")
                {
                    var studentEntity = await _context.StudentEntity.FirstOrDefaultAsync(s => s.ApplicationStudentId == existingUser.Id);
                    if (studentEntity != null)
                    {
                        var profile = await _context.StudentProfile.FirstOrDefaultAsync(p => p.StudentEntityId == studentEntity.Id);
                        if (profile != null)
                        {
                            profile.FullName = existingUser.FullName;
                            profile.Email = existingUser.Email;
                            profile.Phone = existingUser.PhoneNumber ?? "";
                            profile.Address = existingUser.Address ?? "";
                            _context.StudentProfile.Update(profile);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                if (result.Succeeded) return Json(new { success = true, message = "User updated successfully" });
                return Json(new { success = false, message = "Update failed" });
            }
            catch (Exception ex) { return Json(new { success = false, message = ex.Message }); }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return Json(new { success = false, message = "User not found" });

                if (user.Role == "Student")
                {
                    var studentEntity = await _context.StudentEntity.Include(s => s.Profile).FirstOrDefaultAsync(s => s.ApplicationStudentId == user.Id);
                    if (studentEntity != null)
                    {
                        if (studentEntity.Profile != null) _context.StudentProfile.Remove(studentEntity.Profile);
                        _context.StudentEntity.Remove(studentEntity);
                        await _context.SaveChangesAsync();
                    }
                }

                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await transaction.CommitAsync();
                    return Json(new { success = true, message = "User deleted successfully" });
                }
                return Json(new { success = false, message = "Deletion failed" });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = ex.Message });
            }
        }

        // API hỗ trợ lấy chi tiết 1 User để hiển thị lên Modal Edit
        [HttpGet]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return Json(new { success = false, message = "User not found" });
            return Json(new { success = true, data = user });
        }
        #endregion

        


        #region Department CRUD
        [HttpGet] public async Task<IActionResult> GetDepartment(int id) => Json(new { success = true, data = await _context.Department.FindAsync(id) });

        [HttpPost]
        public async Task<IActionResult> CreateDepartment([FromBody] Department d)
        {
            if (string.IsNullOrEmpty(d.Title)) return Json(new { success = false, message = "Title required" });
            d.IsActive = true; _context.Department.Add(d); await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateDepartment([FromBody] Department d)
        {
            var obj = await _context.Department.FindAsync(d.Id); if (obj == null) return Json(new { success = false });
            obj.Title = d.Title; obj.Description = d.Description;
            await _context.SaveChangesAsync(); return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var obj = await _context.Department.FindAsync(id); if (obj == null) return Json(new { success = false });
            obj.IsActive = false; await _context.SaveChangesAsync(); return Json(new { success = true });
        }
        #endregion

        #region Program CRUD
        [HttpGet] public async Task<IActionResult> GetProgram(int id) => Json(new { success = true, data = await _context.Program.FindAsync(id) });

        [HttpPost]
        public async Task<IActionResult> CreateProgram([FromBody] ProgramEntity p)
        {
            if (string.IsNullOrEmpty(p.Name)) return Json(new { success = false, message = "Name required" });
            p.IsActive = true; _context.Program.Add(p); await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProgram([FromBody] ProgramEntity p)
        {
            var obj = await _context.Program.FindAsync(p.Id); if (obj == null) return Json(new { success = false });
            obj.Name = p.Name; obj.Description = p.Description; obj.IsActive = p.IsActive;
            await _context.SaveChangesAsync(); return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            var obj = await _context.Program.FindAsync(id); if (obj == null) return Json(new { success = false });
            obj.IsActive = false; await _context.SaveChangesAsync(); return Json(new { success = true });
        }
        #endregion

        #region Class CRUD
        [HttpGet] public async Task<IActionResult> GetClass(int id) => Json(new { success = true, data = await _context.Classes.FindAsync(id) });

        [HttpPost]
        public async Task<IActionResult> CreateClass([FromBody] Class c)
        {
            if (string.IsNullOrEmpty(c.ClassName)) return Json(new { success = false, message = "Name required" });
            c.IsActive = true; _context.Classes.Add(c); await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateClass([FromBody] Class c)
        {
            var obj = await _context.Classes.FindAsync(c.Id); if (obj == null) return Json(new { success = false });
            obj.ClassName = c.ClassName; obj.Room = c.Room; obj.ProgramId = c.ProgramId;
            obj.StartDate = c.StartDate; obj.EndDate = c.EndDate;
            await _context.SaveChangesAsync(); return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteClass(int id)
        {
            var obj = await _context.Classes.FindAsync(id); if (obj == null) return Json(new { success = false });
            obj.IsActive = false; await _context.SaveChangesAsync(); return Json(new { success = true });
        }
        #endregion

        #region Course CRUD
        [HttpGet] public async Task<IActionResult> GetCourse(int id) => Json(new { success = true, data = await _context.Course.FindAsync(id) });

        [HttpPost]
        public async Task<IActionResult> CreateCourse([FromBody] Course c)
        {
            c.IsActive = true; c.CreatedOn = DateTime.UtcNow; _context.Course.Add(c); await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCourse([FromBody] Course c)
        {
            var obj = await _context.Course.FindAsync(c.Id); if (obj == null) return Json(new { success = false });
            obj.CourseName = c.CourseName; obj.Instructor = c.Instructor;
            obj.Price = c.Price; obj.ProgramId = c.ProgramId;
            await _context.SaveChangesAsync(); return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var obj = await _context.Course.FindAsync(id); if (obj == null) return Json(new { success = false });
            obj.IsActive = false; await _context.SaveChangesAsync(); return Json(new { success = true });
        }
        #endregion

        #region Enrollment CRUD
        [HttpGet] public async Task<IActionResult> GetEnrollment(int id) => Json(new { success = true, data = await _context.Enrollment.FindAsync(id) });

        [HttpPost]
        public async Task<IActionResult> CreateEnrollment([FromBody] Enrollment e)
        {
            e.IsActive = true; e.CreatedOn = DateTime.UtcNow; _context.Enrollment.Add(e); await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEnrollment([FromBody] Enrollment e)
        {
            var obj = await _context.Enrollment.FindAsync(e.Id); if (obj == null) return Json(new { success = false });
            obj.Code = e.Code; obj.StudentEntityId = e.StudentEntityId;
            obj.Note = e.Note; obj.Status = e.Status;
            await _context.SaveChangesAsync(); return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEnrollment(int id)
        {
            var obj = await _context.Enrollment.FindAsync(id); if (obj == null) return Json(new { success = false });
            obj.IsActive = false; await _context.SaveChangesAsync(); return Json(new { success = true });
        }
        #endregion

        #region Search API
        [HttpGet]
        public async Task<IActionResult> Search(string term)
        {
            if (string.IsNullOrWhiteSpace(term)) return Json(new { success = true, data = new object() });
            term = term.ToLower();

            var users = await _userManager.Users
                .Where(u => u.FullName.ToLower().Contains(term) || u.Email.ToLower().Contains(term))
                .Select(u => new { u.Id, u.FullName, u.Email, Type = "User" }).Take(5).ToListAsync();

            var courses = await _context.Course
                .Where(c => c.CourseName.ToLower().Contains(term))
                .Select(c => new { c.Id, Name = c.CourseName, Type = "Course" }).Take(5).ToListAsync();

            return Json(new { success = true, data = new { users, courses } });
        }
        #endregion

        #region Helper Methods for Dashboard
        private async Task<object> GetStudentsByDepartment()
        {
            var data = await _context.CourseDepartment
                .Include(cd => cd.Department).Include(cd => cd.Course).ThenInclude(c => c.EnrollmentDetails).ThenInclude(ed => ed.Enrollment)
                .Where(cd => cd.Course.IsActive && cd.Department.IsActive)
                .GroupBy(cd => cd.Department.Title)
                .Select(g => new { Department = g.Key, Count = g.SelectMany(cd => cd.Course.EnrollmentDetails).Select(ed => ed.Enrollment.StudentEntityId).Distinct().Count() })
                .OrderByDescending(x => x.Count).ToListAsync();
            return data.Any() ? data : GetDefaultDepartmentStats();
        }

        private async Task<object> GetClassesByProgram()
        {
            var data = await _context.Classes
                .Include(c => c.Program)
                .Where(c => c.IsActive)
                .GroupBy(c => c.Program.Name)
                .Select(g => new { Program = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count).ToListAsync();
            return data.Any() ? data : GetDefaultProgramStats();
        }

        private object GetDefaultDepartmentStats() => new[] { new { Department = "IT", Count = 0 } };
        private object GetDefaultProgramStats() => new[] { new { Program = "Bachelor", Count = 0 } };
        #endregion
    }
}