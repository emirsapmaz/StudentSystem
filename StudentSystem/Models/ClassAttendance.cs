using System.ComponentModel.DataAnnotations;

namespace StudentSystem.Models
{
    public class ClassAttendance
    {
        [Key]
        public int Id { get; set; }

        public string StudentId { get; set; } 

        public int CourseId { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool IsPresent { get; set; } 

        //navigation properties
        public Course Course { get; set; }
        public user Student { get; set; }
    }
}
