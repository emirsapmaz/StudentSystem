using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace StudentSystem.Models
{
    public class user : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Required]
        public string roles { get; set; }

        [Required]
        public string gender { get; set; } = string.Empty;

        public string? TeacherSubject { get; set; } = string.Empty;

        public  ICollection<Course> TaughtCourses { get; set; } = new List<Course>(); 
        public  ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>(); 
        public virtual ICollection<ClassAttendance> Attendances { get; set; } = new List<ClassAttendance>();
    }

}