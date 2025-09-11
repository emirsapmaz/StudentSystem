using System.ComponentModel.DataAnnotations;

namespace StudentSystem.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Enums.Status Status { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        public string TeacherId { get; set; }

        public user Teacher { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<ClassAttendance> Attendances { get; set; } = new List<ClassAttendance>();
    }
}
