using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Student.Management.DataAccess.DataAccess;
using Student.Management.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Student.Management.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class HomeController : Controller
    {
        private readonly StudentManagementDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(StudentManagementDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. DASHBOARD - Tổng quan
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

            var student = await GetStudentWithData(user.Id);
            if (student == null) return RedirectToAction("Profile");

            // Lấy dữ liệu từ database
            var currentClasses = await GetCurrentClasses(student.Id);
            var weeklySchedule = await GetWeeklySchedule(student.Id);
            var recentGrades = await GetRecentGrades(student.Id);
            var attendanceStats = await GetAttendanceStatistics(student.Id);

            // Tính toán từ dữ liệu database
            var gpa = CalculateGPA(student.Id);
            var totalCredits = currentClasses.Count * 3;

            ViewBag.CurrentClasses = currentClasses;
            ViewBag.WeeklySchedule = weeklySchedule;
            ViewBag.RecentGrades = recentGrades;
            ViewBag.AttendanceStats = attendanceStats;
            ViewBag.GPA = gpa;
            ViewBag.TotalCredits = totalCredits;
            ViewBag.TotalClasses = currentClasses.Count;

            return View(student);
        }
        // 2. LỊCH HỌC - Sửa lại để load đầy đủ dữ liệu
        public async Task<IActionResult> Schedule()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                TempData["Error"] = "User not found. Please login again.";
                return RedirectToAction("Login", "Account", new { area = "" });
            }

            // Tìm student entity
            var student = await _context.StudentEntity
                .FirstOrDefaultAsync(s => s.ApplicationStudentId == user.Id);

            if (student == null)
            {
                TempData["Error"] = "Student profile not found. Please contact administrator.";
                return RedirectToAction("Profile");
            }

            // CÁCH 1: Sử dụng query chính xác hơn
            var schedules = await _context.Schedules
                .Include(s => s.Class) // Load Class
                    .ThenInclude(c => c.Course) // Load Course từ Class
                .Include(s => s.Class) // Load Class lại để include Teacher
                    .ThenInclude(c => c.Teacher) // Load Teacher từ Class
                .Where(s => s.IsActive &&
                           s.Class.IsActive &&
                           s.Class.ClassStudents.Any(cs =>
                               cs.StudentEntityId == student.Id &&
                               cs.IsActive))
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            // Nếu không có lịch học, lấy tất cả lịch học để debug
            if (!schedules.Any())
            {
                // Debug: lấy tất cả lịch học có sẵn
                var allSchedules = await _context.Schedules
                    .Include(s => s.Class)
                        .ThenInclude(c => c.Course)
                    .Include(s => s.Class)
                        .ThenInclude(c => c.Teacher)
                    .Where(s => s.IsActive && s.Class.IsActive)
                    .OrderBy(s => s.DayOfWeek)
                    .ThenBy(s => s.StartTime)
                    .Take(5)
                    .ToListAsync();

                ViewBag.Schedules = allSchedules;
                ViewBag.IsDemoData = true;
                ViewBag.DebugInfo = $"No schedules found for student {student.Id}. Showing {allSchedules.Count} sample schedules.";
            }
            else
            {
                ViewBag.Schedules = schedules;
                ViewBag.IsDemoData = false;
                ViewBag.DebugInfo = $"Found {schedules.Count} schedules for student {student.Id}";
            }

            // Debug: kiểm tra dữ liệu
            Console.WriteLine($"Student ID: {student.Id}");
            Console.WriteLine($"Number of schedules: {schedules.Count}");

            foreach (var schedule in schedules)
            {
                Console.WriteLine($"Schedule: {schedule.DayOfWeek} {schedule.StartTime}-{schedule.EndTime}");
                Console.WriteLine($"  Class: {schedule.Class?.ClassName}");
                Console.WriteLine($"  Course: {schedule.Class?.Course?.CourseName}");
                Console.WriteLine($"  Teacher: {schedule.Class?.Teacher?.FullName}");
            }

            ViewBag.StudentName = user.FullName;

            return View();
        }

        // 3. ĐIỂM SỐ
        public async Task<IActionResult> Grades()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.StudentEntity
                .FirstOrDefaultAsync(s => s.ApplicationStudentId == user.Id);

            if (student == null)
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction("Index");
            }

            // Lấy điểm từ database
            var detailedGrades = await _context.Grades
                .Include(g => g.Class)
                    .ThenInclude(c => c.Course)
                .Include(g => g.Class)
                    .ThenInclude(c => c.Teacher)
                .Where(g => g.StudentEntityId == student.Id)
                .OrderByDescending(g => g.CreatedDate)
                .ToListAsync();

            // Lấy điểm tổng kết từ database
            var classGrades = await _context.ClassStudents
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Course)
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Teacher)
                .Where(cs => cs.StudentEntityId == student.Id)
                .Select(cs => new
                {
                    CourseName = cs.Class.Course.CourseName,
                    ClassName = cs.Class.ClassName,
                    Instructor = cs.Class.Teacher.FullName,
                    StartDate = cs.Class.StartDate,
                    EndDate = cs.Class.EndDate,
                    FinalGrade = cs.Grade,
                    Status = cs.Grade.HasValue ? (cs.Grade >= 5 ? "Passed" : "Failed") : "In Progress"
                })
                .ToListAsync();

            // Tính GPA từ database
            var validGrades = classGrades.Where(c => c.FinalGrade.HasValue).Select(c => c.FinalGrade.Value).ToList();
            var gpa = validGrades.Any() ? Math.Round(validGrades.Average(), 2) : 0;

            // Thống kê
            var totalCourses = classGrades.Count;
            var passedCourses = classGrades.Count(c => c.Status == "Passed");
            var failedCourses = classGrades.Count(c => c.Status == "Failed");
            var inProgressCourses = classGrades.Count(c => c.Status == "In Progress");

            ViewBag.DetailedGrades = detailedGrades;
            ViewBag.ClassGrades = classGrades;
            ViewBag.GPA = gpa;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.PassedCourses = passedCourses;
            ViewBag.FailedCourses = failedCourses;
            ViewBag.InProgressCourses = inProgressCourses;

            return View();
        }

        // 4. KHÓA HỌC
        public async Task<IActionResult> Courses()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.StudentEntity
                .FirstOrDefaultAsync(s => s.ApplicationStudentId == user.Id);

            if (student == null)
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction("Index");
            }

            // Lấy khóa học từ database
            var myCourses = await _context.ClassStudents
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Course)
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Teacher)
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Program)
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Schedules)
                .Where(cs => cs.StudentEntityId == student.Id)
                .OrderByDescending(cs => cs.Class.StartDate)
                .ToListAsync();

            // Phân loại từ database
            var currentCourses = myCourses.Where(cs =>
                cs.Class.IsActive &&
                cs.Class.StartDate <= DateTime.Now &&
                cs.Class.EndDate >= DateTime.Now).ToList();

            var pastCourses = myCourses.Where(cs =>
                cs.Class.EndDate < DateTime.Now).ToList();

            var upcomingCourses = myCourses.Where(cs =>
                cs.Class.StartDate > DateTime.Now).ToList();

            ViewBag.CurrentCourses = currentCourses;
            ViewBag.PastCourses = pastCourses;
            ViewBag.UpcomingCourses = upcomingCourses;
            ViewBag.TotalCourses = myCourses.Count;

            return View();
        }

        // 5. PROFILE
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var studentEntity = await _context.StudentEntity
                .Include(s => s.Profile)
                .Include(s => s.ClassStudents)
                    .ThenInclude(cs => cs.Class)
                        .ThenInclude(c => c.Program)
                .FirstOrDefaultAsync(s => s.ApplicationStudentId == user.Id);

            if (studentEntity == null)
            {
                // Tạo mới nếu chưa có (sẽ được tạo tự động từ hệ thống)
                TempData["Error"] = "Student profile not found. Please contact administrator.";
                return RedirectToAction("Index");
            }

            if (studentEntity.Profile == null)
            {
                studentEntity.Profile = new StudentProfile
                {
                    FullName = user.FullName ?? "Student",
                    Email = user.Email,
                    Phone = user.PhoneNumber,
                    Address = user.Address,
                    StudentEntityId = studentEntity.Id,
                    IsActive = true
                };
                _context.StudentProfile.Add(studentEntity.Profile);
                await _context.SaveChangesAsync();
            }

            // Lấy thống kê từ database
            var totalCourses = await _context.ClassStudents
                .CountAsync(cs => cs.StudentEntityId == studentEntity.Id);

            var completedCourses = await _context.ClassStudents
                .CountAsync(cs => cs.StudentEntityId == studentEntity.Id && cs.Grade.HasValue && cs.Grade >= 5);

            var currentCourses = await _context.ClassStudents
                .CountAsync(cs => cs.StudentEntityId == studentEntity.Id &&
                                 cs.Class.IsActive &&
                                 cs.Class.StartDate <= DateTime.Now &&
                                 cs.Class.EndDate >= DateTime.Now);

            ViewBag.TotalCourses = totalCourses;
            ViewBag.CompletedCourses = completedCourses;
            ViewBag.CurrentCourses = currentCourses;

            return View(studentEntity.Profile);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(StudentProfile model)
        {
            if (ModelState.IsValid)
            {
                var profile = await _context.StudentProfile.FindAsync(model.Id);
                if (profile == null)
                {
                    _context.StudentProfile.Add(model);
                }
                else
                {
                    profile.Phone = model.Phone;
                    profile.Address = model.Address;
                    profile.DateOfBirth = model.DateOfBirth;
                    _context.StudentProfile.Update(profile);
                }
                await _context.SaveChangesAsync();
                TempData["Success"] = "Profile updated successfully!";
            }
            return RedirectToAction("Profile");
        }

        // 6. ĐIỂM DANH (nếu cần)
        public async Task<IActionResult> Attendance()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.StudentEntity
                .FirstOrDefaultAsync(s => s.ApplicationStudentId == user.Id);

            if (student == null)
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction("Index");
            }

            // Lấy điểm danh từ database
            var attendance = await _context.Attendances
                .Include(a => a.Class)
                    .ThenInclude(c => c.Course)
                .Include(a => a.Class)
                    .ThenInclude(c => c.Teacher)
                .Where(a => a.StudentEntityId == student.Id)
                .OrderByDescending(a => a.AttendanceDate)
                .Take(50)
                .ToListAsync();

            // Thống kê từ database
            var totalAttendances = attendance.Count;
            var presentDays = attendance.Count(a => a.Status == "Present");
            var attendanceRate = totalAttendances > 0 ? Math.Round((double)presentDays / totalAttendances * 100, 1) : 0;

            ViewBag.Attendance = attendance;
            ViewBag.AttendanceRate = attendanceRate;
            ViewBag.TotalAttendances = totalAttendances;
            ViewBag.PresentDays = presentDays;

            return View();
        }

        // ========== PRIVATE HELPER METHODS ==========
        private async Task<StudentEntity> GetStudentWithData(string userId)
        {
            return await _context.StudentEntity
                .Include(s => s.Profile)
                .Include(s => s.ClassStudents)
                    .ThenInclude(cs => cs.Class)
                        .ThenInclude(c => c.Program)
                .FirstOrDefaultAsync(s => s.ApplicationStudentId == userId);
        }

        private async Task<List<ClassStudent>> GetCurrentClasses(int studentId)
        {
            return await _context.ClassStudents
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Course)
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Teacher)
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Schedules)
                .Where(cs => cs.StudentEntityId == studentId &&
                             cs.Class.IsActive &&
                             cs.Class.StartDate <= DateTime.Now &&
                             cs.Class.EndDate >= DateTime.Now)
                .ToListAsync();
        }

        private async Task<List<object>> GetWeeklySchedule(int studentId)
        {
            var classStudents = await _context.ClassStudents
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Schedules)
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Course)
                .Include(cs => cs.Class)
                    .ThenInclude(c => c.Teacher)
                .Where(cs => cs.StudentEntityId == studentId &&
                             cs.Class.IsActive &&
                             cs.Class.StartDate <= DateTime.Now &&
                             cs.Class.EndDate >= DateTime.Now)
                .ToListAsync();

            var schedules = classStudents
                .SelectMany(cs => cs.Class.Schedules)
                .Where(s => s.IsActive)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .Select(s => new
                {
                    Day = s.DayOfWeek,
                    Time = $"{s.StartTime} - {s.EndTime}",
                    Course = s.Class.Course.CourseName,
                    Teacher = s.Class.Teacher.FullName,
                    Room = s.Room,
                    ClassName = s.Class.ClassName
                })
                .ToList<object>();

            return schedules;
        }

        private async Task<List<Grade>> GetRecentGrades(int studentId)
        {
            return await _context.Grades
                .Include(g => g.Class)
                    .ThenInclude(c => c.Course)
                .Where(g => g.StudentEntityId == studentId)
                .OrderByDescending(g => g.CreatedDate)
                .Take(5)
                .ToListAsync();
        }

        private async Task<object> GetAttendanceStatistics(int studentId)
        {
            var attendance = await _context.Attendances
                .Where(a => a.StudentEntityId == studentId && a.AttendanceDate >= DateTime.Now.AddDays(-30))
                .ToListAsync();

            var total = attendance.Count;
            var present = attendance.Count(a => a.Status == "Present");
            var rate = total > 0 ? Math.Round((double)present / total * 100, 1) : 0;

            return new
            {
                Total = total,
                Present = present,
                Rate = rate
            };
        }

        private double CalculateGPA(int studentId)
        {
            var grades = _context.ClassStudents
                .Where(cs => cs.StudentEntityId == studentId && cs.Grade.HasValue)
                .Select(cs => cs.Grade.Value)
                .ToList();

            return grades.Any() ? Math.Round(grades.Average(), 2) : 0;
        }
    }
}