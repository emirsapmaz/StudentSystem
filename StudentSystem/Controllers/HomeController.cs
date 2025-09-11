using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSystem.Data;
using StudentSystem.Models;
using StudentSystem.ViewModels;
using System.Diagnostics;

namespace StudentSystem.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<user> _userManager;

        public HomeController(ApplicationDbContext context, UserManager<user> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var viewModel = new DashboardViewModel
            {
                User = currentUser,
                UserRole = currentUser.roles
            };

            switch (currentUser.roles.ToLower())
            {
                case "admin":
                    await LoadAdminDashboard(viewModel);
                    break;
                case "teacher":
                    await LoadTeacherDashboard(viewModel, currentUser.Id);
                    break;
                case "student":
                    await LoadStudentDashboard(viewModel, currentUser.Id);
                    break;
                default:
                    return View("Error");
            }

            return View(viewModel);
        }
        private async Task LoadAdminDashboard(DashboardViewModel viewModel)
        {
            var grades = await _context.Enrollments
                .Where(e => e.Grade.HasValue)
                .Select(e => e.Grade.Value)
                .ToListAsync();

            var averageGrade = grades.Any() ? grades.Average() : 0m;

            viewModel.AdminStats = new AdminDashboardStats
            {
                TotalUsers = await _context.Users.CountAsync(),
                TotalStudents = await _context.Users.CountAsync(u => u.roles.ToLower() == "student"),
                TotalTeachers = await _context.Users.CountAsync(u => u.roles.ToLower() == "teacher"),
                TotalCourses = await _context.Courses.CountAsync(),
                ActiveCourses = await _context.Courses.CountAsync(c => c.Status == Enums.Status.Started),
                TotalEnrollments = await _context.Enrollments.CountAsync(),
                AverageGrade = averageGrade,
                RecentEnrollments = await _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .OrderByDescending(e => e.Id)
                    .Take(5)
                    .ToListAsync()
            };
        }

        private async Task LoadTeacherDashboard(DashboardViewModel viewModel, string teacherId)
        {
            var teacherCourses = await _context.Courses
                .Where(c => c.TeacherId == teacherId)
                .Include(c => c.Enrollments)
                .ThenInclude(e => e.Student)
                .ToListAsync();

            var allGrades = teacherCourses
                .SelectMany(c => c.Enrollments)
                .Where(e => e.Grade.HasValue)
                .Select(e => e.Grade.Value)
                .ToList();

            var avgGrade = allGrades.Any() ? allGrades.Average() : 0m;

            viewModel.TeacherStats = new TeacherDashboardStats
            {
                TotalCourses = teacherCourses.Count,
                ActiveCourses = teacherCourses.Count(c => c.Status == Enums.Status.Started),
                TotalStudents = teacherCourses.SelectMany(c => c.Enrollments).Count(),
                AverageGrade = avgGrade,
                Courses = teacherCourses,
                RecentGrades = await _context.Enrollments
                    .Include(e => e.Student)
                    .Include(e => e.Course)
                    .Where(e => e.Course.TeacherId == teacherId && e.Grade.HasValue)
                    .OrderByDescending(e => e.Id)
                    .Take(5)
                    .ToListAsync()
            };
        }

        private async Task LoadStudentDashboard(DashboardViewModel viewModel, string studentId)
        {
            var studentEnrollments = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .ThenInclude(c => c.Teacher)
                .ToListAsync();

            var attendanceRecords = await _context.ClassAttendance
                .Where(a => a.StudentId == studentId)
                .ToListAsync();

            var grades = studentEnrollments
                .Where(e => e.Grade.HasValue)
                .Select(e => e.Grade.Value)
                .ToList();

            var avgGrade = grades.Any() ? grades.Average() : 0m;

            var attendanceCount = attendanceRecords.Count;
            var presentCount = attendanceRecords.Count(a => a.IsPresent);

            viewModel.StudentStats = new StudentDashboardStats
            {
                TotalCourses = studentEnrollments.Count,
                CompletedCourses = studentEnrollments.Count(e => e.Course.Status == Enums.Status.Finished),
                AverageGrade = avgGrade,
                TotalAttendance = attendanceCount,
                PresentDays = presentCount,
                AttendancePercentage = attendanceCount > 0
                    ? (double)presentCount / attendanceCount * 100
                    : 0,
                Enrollments = studentEnrollments,
                RecentAttendance = attendanceRecords
                    .OrderByDescending(a => a.Date)
                    .Take(5)
                    .ToList()
            };
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
