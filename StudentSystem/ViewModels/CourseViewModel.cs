using StudentSystem.Models;

namespace StudentSystem.ViewModels
{
    public class CourseViewModel
    {
        public Course Course { get; set; }
        public string CurrentUserRole { get; set; }
        public Enrollment UserEnrollment { get; set; } 
        public List<Enrollment> Enrollments { get; set; } 
        public List<user> AvailableStudents { get; set; } = new List<user>(); //initialize empty list so we dont get error
        public List<ClassAttendance> Attendances { get; set; } = new List<ClassAttendance>(); 
    }
}
