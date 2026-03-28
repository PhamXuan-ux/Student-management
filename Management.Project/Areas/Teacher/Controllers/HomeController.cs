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

namespace Student.Management.Areas.Teacher.Controllers
{
    [Area("Teacher")]
    [Authorize(Roles = "Teacher")]
    public class HomeController : Controller
    {
        private readonly StudentManagementDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public HomeController(StudentManagementDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. DASHBOARD TỔNG QUAN
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Login", "Account", new { area = "" });

            // Lấy tất cả lớp học của teacher
            var myClasses = await _context.Classes
                .Include(c => c.ClassStudents)
                .Include(c => c.Schedules)
                .Include(c => c.Program)
                .Include(c => c.Course)
                .Where(c => c.TeacherId == user.Id && c.IsActive)
                .ToListAsync();

            // Thống kê
            ViewBag.TotalClasses = myClasses.Count;
            ViewBag.TotalStudents = myClasses.Sum(c => c.ClassStudents.Count);
            ViewBag.ActiveClasses = myClasses.Count(c => c.StartDate <= DateTime.Now && c.EndDate >= DateTime.Now);

            // Lịch dạy trong tuần
            var weeklySchedules = await GetWeeklySchedule(user.Id);
            ViewBag.WeeklySchedule = weeklySchedules;

            // Lớp học cần chấm điểm
            var classesNeedGrading = myClasses.Where(c =>
                c.ClassStudents.Any(cs => cs.Grade == null) &&
                c.EndDate >= DateTime.Now
            ).ToList();
            ViewBag.ClassesNeedGrading = classesNeedGrading;

            return View(myClasses);
        }

        // 2. QUẢN LÝ LỚP HỌC - CHI TIẾT LỚP
        public async Task<IActionResult> ClassDetails(int id)
        {
            var user = await _userManager.GetUserAsync(User);

            var classObj = await _context.Classes
                .Include(c => c.ClassStudents)
                    .ThenInclude(cs => cs.StudentEntity)
                    .ThenInclude(s => s.Profile)
                .Include(c => c.ClassStudents)
                    .ThenInclude(cs => cs.StudentEntity)
                    .ThenInclude(s => s.ApplicationStudent)
                .Include(c => c.Course)
                .Include(c => c.Program)
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.Id == id && c.TeacherId == user.Id);

            if (classObj == null)
            {
                TempData["Error"] = "Class not found or access denied.";
                return RedirectToAction("Index");
            }

            // Lấy điểm chi tiết
            var grades = await _context.Grades
                .Where(g => g.ClassId == id)
                .ToListAsync();
            ViewBag.Grades = grades;

            // Thống kê lớp
            var studentCount = classObj.ClassStudents.Count;
            var gradedCount = classObj.ClassStudents.Count(cs => cs.Grade.HasValue);
            ViewBag.GradedPercentage = studentCount > 0 ? (gradedCount * 100 / studentCount) : 0;
            ViewBag.AverageGrade = classObj.ClassStudents.Where(cs => cs.Grade.HasValue).Average(cs => cs.Grade) ?? 0;

            return View(classObj);
        }

        // 3. CHẤM ĐIỂM - API
        [HttpPost]
        public async Task<IActionResult> SaveGrade(int classId, int studentEntityId, string gradeType, double score)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var classObj = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == classId && c.TeacherId == user.Id);

                if (classObj == null)
                    return Json(new { success = false, error = "Access denied." });

                // Validate score
                if (score < 0 || score > 10)
                    return Json(new { success = false, error = "Score must be between 0 and 10." });

                var grade = await _context.Grades
                    .FirstOrDefaultAsync(g => g.ClassId == classId &&
                                            g.StudentEntityId == studentEntityId &&
                                            g.GradeType == gradeType);

                if (grade == null)
                {
                    grade = new Grade
                    {
                        ClassId = classId,
                        StudentEntityId = studentEntityId,
                        GradeType = gradeType,
                        Score = score,
                        MaxScore = 10,
                        Weight = GetWeightByGradeType(gradeType),
                        CreatedDate = DateTime.Now,
                        Note = $"{gradeType} grade - {DateTime.Now:MM/dd/yyyy}"
                    };
                    _context.Grades.Add(grade);
                }
                else
                {
                    grade.Score = score;
                    grade.CreatedDate = DateTime.Now;
                    _context.Grades.Update(grade);
                }

                await _context.SaveChangesAsync();

                // Cập nhật điểm tổng kết
                await UpdateFinalGrade(classId, studentEntityId);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // 4. QUẢN LÝ ĐIỂM DANH
        public async Task<IActionResult> Attendance(int classId)
        {
            var user = await _userManager.GetUserAsync(User);

            var classObj = await _context.Classes
                .Include(c => c.ClassStudents)
                    .ThenInclude(cs => cs.StudentEntity)
                    .ThenInclude(s => s.Profile)
                .Include(c => c.Schedules)
                .FirstOrDefaultAsync(c => c.Id == classId && c.TeacherId == user.Id);

            if (classObj == null)
            {
                TempData["Error"] = "Class not found or access denied.";
                return RedirectToAction("Index");
            }

            // Lấy điểm danh 30 ngày gần nhất
            var recentDate = DateTime.Now.AddDays(-30);
            var attendances = await _context.Attendances
                .Where(a => a.ClassId == classId && a.AttendanceDate >= recentDate)
                .OrderByDescending(a => a.AttendanceDate)
                .ToListAsync();

            ViewBag.Attendances = attendances;
            ViewBag.Today = DateTime.Today;

            return View(classObj);
        }

        [HttpPost]
        public async Task<IActionResult> MarkAttendance(int classId, int studentEntityId, DateTime date, string status, string notes = "")
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var classObj = await _context.Classes
                    .FirstOrDefaultAsync(c => c.Id == classId && c.TeacherId == user.Id);

                if (classObj == null)
                    return Json(new { success = false, error = "Access denied." });

                var attendance = await _context.Attendances
                    .FirstOrDefaultAsync(a => a.ClassId == classId &&
                                            a.StudentEntityId == studentEntityId &&
                                            a.AttendanceDate.Date == date.Date);

                if (attendance == null)
                {
                    attendance = new Attendance
                    {
                        ClassId = classId,
                        StudentEntityId = studentEntityId,
                        AttendanceDate = date,
                        Status = status,
                        Notes = notes,
                        RecordedDate = DateTime.Now
                    };
                    _context.Attendances.Add(attendance);
                }
                else
                {
                    attendance.Status = status;
                    attendance.Notes = notes;
                    attendance.RecordedDate = DateTime.Now;
                    _context.Attendances.Update(attendance);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // 5. BÁO CÁO & THỐNG KÊ
        public async Task<IActionResult> Reports()
        {
            var user = await _userManager.GetUserAsync(User);

            var myClasses = await _context.Classes
                .Include(c => c.ClassStudents)
                .Include(c => c.Course)
                .Include(c => c.Program)
                .Where(c => c.TeacherId == user.Id)
                .ToListAsync();

            var reportData = new List<dynamic>();

            foreach (var classObj in myClasses)
            {
                var students = classObj.ClassStudents;
                var totalStudents = students.Count;
                var gradedStudents = students.Count(cs => cs.Grade.HasValue);
                var passedStudents = students.Count(cs => cs.Grade.HasValue && cs.Grade >= 5);
                var averageGrade = students.Where(cs => cs.Grade.HasValue).Average(cs => cs.Grade) ?? 0;

                // Điểm danh thống kê
                var recentAttendances = await _context.Attendances
                    .Where(a => a.ClassId == classObj.Id && a.AttendanceDate >= DateTime.Now.AddDays(-30))
                    .ToListAsync();

                var totalAttendances = recentAttendances.Count;
                var presentCount = recentAttendances.Count(a => a.Status == "Present");
                var attendanceRate = totalAttendances > 0 ? (double)presentCount / totalAttendances * 100 : 0;

                reportData.Add(new
                {
                    ClassName = classObj.ClassName,
                    Course = classObj.Course?.CourseName,
                    Program = classObj.Program?.Name,
                    TotalStudents = totalStudents,
                    GradedStudents = gradedStudents,
                    PassedStudents = passedStudents,
                    PassRate = totalStudents > 0 ? Math.Round((double)passedStudents / totalStudents * 100, 2) : 0,
                    AverageGrade = Math.Round(averageGrade, 2),
                    AttendanceRate = Math.Round(attendanceRate, 2),
                    StartDate = classObj.StartDate?.ToString("MM/dd/yyyy"),
                    EndDate = classObj.EndDate?.ToString("MM/dd/yyyy")
                });
            }

            // Thống kê tổng quan
            ViewBag.TotalClasses = myClasses.Count;
            ViewBag.TotalStudents = myClasses.Sum(c => c.ClassStudents.Count);
            ViewBag.OverallPassRate = reportData.Any() ?
            Math.Round(reportData.Average(r => (double)r.PassRate), 2) : 0;

            ViewBag.OverallAttendanceRate = reportData.Any() ?
                Math.Round(reportData.Average(r => (double)r.AttendanceRate), 2) : 0;

            return View(reportData);
        }

        [HttpPost]
        public async Task<IActionResult> GenerateReport(int classId, string reportType, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var classObj = await _context.Classes
                    .Include(c => c.ClassStudents)
                    .ThenInclude(cs => cs.StudentEntity)
                    .ThenInclude(s => s.Profile)
                    .Include(c => c.Course)
                    .FirstOrDefaultAsync(c => c.Id == classId && c.TeacherId == user.Id);

                if (classObj == null)
                    return Json(new { success = false, error = "Class not found." });

                var reportData = new
                {
                    ClassName = classObj.ClassName,
                    Course = classObj.Course?.CourseName,
                    GeneratedDate = DateTime.Now.ToString("MM/dd/yyyy HH:mm"),
                    ReportType = reportType,
                    Period = $"{startDate?.ToString("MM/dd/yyyy")} to {endDate?.ToString("MM/dd/yyyy")}",
                    Students = classObj.ClassStudents.Select(cs => new
                    {
                        Name = cs.StudentEntity?.Profile?.FullName ?? "Unknown",
                        Email = cs.StudentEntity?.Profile?.Email ?? "Unknown",
                        Grade = cs.Grade.HasValue ? cs.Grade.Value.ToString("0.0") : "Not Graded",
                        Status = cs.Grade.HasValue ? (cs.Grade >= 5 ? "Pass" : "Fail") : "Pending"
                    }).ToList()
                };

                return Json(new { success = true, data = reportData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // 6. PROFILE
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            var profile = await _context.TeacherProfile.FirstOrDefaultAsync(p => p.TeacherId == user.Id);

            if (profile == null)
            {
                profile = new TeacherProfile
                {
                    TeacherId = user.Id,
                    FullName = user.FullName,
                    Phone = user.PhoneNumber,
                    Address = user.Address
                };
            }

            // Thống kê của teacher
            var myClasses = await _context.Classes
                .Where(c => c.TeacherId == user.Id)
                .ToListAsync();
            ViewBag.TotalClasses = myClasses.Count;
            ViewBag.ActiveClasses = myClasses.Count(c => c.IsActive && c.EndDate >= DateTime.Now);

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateProfile(TeacherProfile model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var profile = await _context.TeacherProfile.FindAsync(model.Id);
                    if (profile == null)
                    {
                        _context.TeacherProfile.Add(model);
                    }
                    else
                    {
                        profile.Phone = model.Phone;
                        profile.Address = model.Address;
                        profile.DateOfBirth = model.DateOfBirth;
                        _context.TeacherProfile.Update(profile);
                    }

                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Profile updated successfully!";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error updating profile: {ex.Message}";
                }
            }
            return RedirectToAction("Profile");
        }

        // PRIVATE HELPER METHODS
        private async Task<List<dynamic>> GetWeeklySchedule(string teacherId)
        {
            var schedules = await _context.Schedules
                .Include(s => s.Class)
                    .ThenInclude(c => c.Program)
                .Include(s => s.Class)
                    .ThenInclude(c => c.Course)
                .Where(s => s.Class.TeacherId == teacherId && s.IsActive)
                .OrderBy(s => s.DayOfWeek)
                .ThenBy(s => s.StartTime)
                .ToListAsync();

            var daysOrder = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };

            return schedules.Select(s => new
            {
                Day = s.DayOfWeek,
                Time = $"{s.StartTime} - {s.EndTime}",
                ClassName = s.Class.ClassName,
                Course = s.Class.Course?.CourseName,
                Room = s.Room,
                Program = s.Class.Program?.Name
            }).Cast<dynamic>().ToList();
        }

        private double GetWeightByGradeType(string gradeType)
        {
            return gradeType.ToLower() switch
            {
                "quiz" => 20,
                "assignment" => 30,
                "midterm" => 20,
                "final" => 30,
                _ => 25
            };
        }

        private async Task UpdateFinalGrade(int classId, int studentEntityId)
        {
            var grades = await _context.Grades
                .Where(g => g.ClassId == classId && g.StudentEntityId == studentEntityId)
                .ToListAsync();

            if (grades.Any())
            {
                var weightedAverage = grades.Sum(g => g.Score * g.Weight) / grades.Sum(g => g.Weight);

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
}