using StudentSystem.Models;

namespace StudentSystem.ViewModels
{
    public class DashboardViewModel
    {
        public user User { get; set; }
        public string UserRole { get; set; }
        public AdminDashboardStats? AdminStats { get; set; }
        public TeacherDashboardStats? TeacherStats { get; set; }
        public StudentDashboardStats? StudentStats { get; set; }
    }

    public class AdminDashboardStats
    {
        public int TotalUsers { get; set; }
        public int TotalStudents { get; set; }
        public int TotalTeachers { get; set; }
        public int TotalCourses { get; set; }
        public int ActiveCourses { get; set; }
        public int TotalEnrollments { get; set; }
        public decimal AverageGrade { get; set; }
        public List<Enrollment> RecentEnrollments { get; set; } = new List<Enrollment>();
    }

    public class TeacherDashboardStats
    {
        public int TotalCourses { get; set; }
        public int ActiveCourses { get; set; }
        public int TotalStudents { get; set; }
        public decimal AverageGrade { get; set; }
        public List<Course> Courses { get; set; } = new List<Course>();
        public List<Enrollment> RecentGrades { get; set; } = new List<Enrollment>();
    }

    public class StudentDashboardStats
    {
        public int TotalCourses { get; set; }
        public int CompletedCourses { get; set; }
        public decimal AverageGrade { get; set; }
        public int TotalAttendance { get; set; }
        public int PresentDays { get; set; }
        public double AttendancePercentage { get; set; }
        public List<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public List<ClassAttendance> RecentAttendance { get; set; } = new List<ClassAttendance>();
    }
}