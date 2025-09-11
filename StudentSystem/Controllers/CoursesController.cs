using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentSystem.Data;
using StudentSystem.Models;
using StudentSystem.ViewModels;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace StudentSystem.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<user> _userManager;

        public CoursesController(ApplicationDbContext context, UserManager<user> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.CurrentUserRole = user.roles;
            ViewBag.Teachers = (await _userManager.GetUsersInRoleAsync(Enums.Roles.Teacher.ToString())).ToList();

            IQueryable<Course> courses;
            if (user.roles == Enums.Roles.Admin.ToString())
            {
                courses = _context.Courses
                    .Include(c => c.Teacher)
                    .Include(c => c.Enrollments);
            }
            else if (user.roles == Enums.Roles.Teacher.ToString())
            {
                courses = _context.Courses
                    .Where(c => c.TeacherId == user.Id)
                    .Include(c => c.Teacher)
                    .Include(c => c.Enrollments);
            }
            else // Student
            {
                courses = _context.Courses
                    .Where(c => c.Enrollments.Any(e => e.StudentId == user.Id))
                    .Include(c => c.Teacher)
                    .Include(c => c.Enrollments);
            }

            return View(await courses.ToListAsync());
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(string name, string description, string teacherId, int status)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                ModelState.AddModelError("Name", "Course name is required.");
            }

            if (string.IsNullOrWhiteSpace(teacherId))
            {
                ModelState.AddModelError("TeacherId", "Teacher selection is required.");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CurrentUserRole = Enums.Roles.Admin.ToString();
                ViewBag.Teachers = (await _userManager.GetUsersInRoleAsync(Enums.Roles.Teacher.ToString())).ToList();
                return View("Index", await _context.Courses.Include(c => c.Teacher).Include(c => c.Enrollments).ToListAsync());
            }

            var teacher = await _userManager.FindByIdAsync(teacherId);
            if (teacher == null || !await _userManager.IsInRoleAsync(teacher, Enums.Roles.Teacher.ToString()))
            {
                ModelState.AddModelError("TeacherId", "Invalid teacher selected.");
                ViewBag.CurrentUserRole = Enums.Roles.Admin.ToString();
                ViewBag.Teachers = (await _userManager.GetUsersInRoleAsync(Enums.Roles.Teacher.ToString())).ToList();
                return View("Index", await _context.Courses.Include(c => c.Teacher).Include(c => c.Enrollments).ToListAsync());
            }

            var course = new Course
            {
                Name = name,
                Description = description,
                TeacherId = teacherId,
                Status = (Enums.Status)status,
                Enrollments = new List<Enrollment>(),
                Attendances = new List<ClassAttendance>()
            };

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Course created successfully.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [Route("Courses/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index");
            }

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Course deleted successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Teacher)
                .Include(c => c.Enrollments)
                .Include(c => c.Attendances)
                .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Index");
            }

            var user = await _userManager.GetUserAsync(User);
            var userRole = user.roles;

            var model = new CourseViewModel
            {
                Course = course,
                CurrentUserRole = userRole,
                Attendances = course.Attendances.ToList(),
                Enrollments = course.Enrollments.ToList(),
                UserEnrollment = course.Enrollments.FirstOrDefault(e => e.StudentId == user.Id)
            };

            if (userRole == Enums.Roles.Admin.ToString() || userRole == Enums.Roles.Teacher.ToString())
            {
                model.AvailableStudents = (await _userManager.GetUsersInRoleAsync(Enums.Roles.Student.ToString())).ToList();
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddStudent(int courseId, string studentId)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Details", new { id = courseId });
            }

            var student = await _userManager.FindByIdAsync(studentId);
            if (student == null || !await _userManager.IsInRoleAsync(student, Enums.Roles.Student.ToString()))
            {
                TempData["Error"] = "Invalid student selected.";
                return RedirectToAction("Details", new { id = courseId });
            }

            if (course.Enrollments.Any(e => e.StudentId == studentId))
            {
                TempData["Error"] = "Student is already enrolled.";
                return RedirectToAction("Details", new { id = courseId });
            }

            var enrollment = new Enrollment
            {
                CourseId = courseId,
                StudentId = studentId
            };

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Student enrolled successfully.";
            return RedirectToAction("Details", new { id = courseId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [Route("Courses/RemoveStudent/{courseId}/{studentId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveStudent(int courseId, string studentId)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (enrollment == null)
            {
                TempData["Error"] = "Enrollment not found.";
                return RedirectToAction("Details", new { id = courseId });
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Student removed successfully.";
            return RedirectToAction("Details", new { id = courseId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddGrade(int courseId, string studentId, int grade)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (enrollment == null)
            {
                TempData["Error"] = "Enrollment not found.";
                return RedirectToAction("Details", new { id = courseId });
            }

            if (grade < 0 || grade > 100)
            {
                TempData["Error"] = "Grade must be between 0 and 100.";
                return RedirectToAction("Details", new { id = courseId });
            }

            enrollment.Grade = grade;
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Grade updated successfully.";
            return RedirectToAction("Details", new { id = courseId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateComment(int courseId, string studentId, string comment)
        {
            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (enrollment == null)
            {
                TempData["Error"] = "Enrollment not found.";
                return RedirectToAction("Details", new { id = courseId });
            }

            enrollment.Comment = comment;
            _context.Enrollments.Update(enrollment);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Comment updated successfully.";
            return RedirectToAction("Details", new { id = courseId });
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAttendance(AttendanceViewModel model)
        {
            var course = await _context.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == model.CourseId);

            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Details", new { id = model.CourseId });
            }

            if (model.Date == default)
            {
                TempData["Error"] = "Date is required.";
                return RedirectToAction("Details", new { id = model.CourseId });
            }

            // Normalize date to UTC (important fix for PostgreSQL)
            var dateUtc = DateTime.SpecifyKind(model.Date.Date, DateTimeKind.Utc);

            var existingAttendance = await _context.ClassAttendance
                .Where(a => a.CourseId == model.CourseId && a.Date == dateUtc)
                .ToListAsync();

            if (existingAttendance.Any())
            {
                TempData["Error"] = "Attendance for this date has already been registered.";
                return RedirectToAction("Details", new { id = model.CourseId });
            }

            var enrolledStudentIds = course.Enrollments.Select(e => e.StudentId).ToList();

            foreach (var studentId in enrolledStudentIds)
            {
                bool isPresent = model.StudentPresence != null && model.StudentPresence.ContainsKey(studentId);

                var attendance = new ClassAttendance
                {
                    CourseId = model.CourseId,
                    StudentId = studentId,
                    Date = dateUtc,
                    IsPresent = isPresent
                };

                _context.ClassAttendance.Add(attendance);
            }

            await _context.SaveChangesAsync();
            TempData["Success"] = "Attendance registered successfully.";
            return RedirectToAction("Details", new { id = model.CourseId });
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, Enums.Status status)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                TempData["Error"] = "Course not found.";
                return RedirectToAction("Details", new { id });
            }

            course.Status = status;
            _context.Courses.Update(course);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Course status updated successfully.";
            return RedirectToAction("Details", new { id });
        }
    }
}