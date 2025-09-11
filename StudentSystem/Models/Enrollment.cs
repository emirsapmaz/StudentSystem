using System.ComponentModel.DataAnnotations;

namespace StudentSystem.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        public string StudentId { get; set; } 
        public int CourseId { get; set; }

        [Range(0, 100)]
        public decimal? Grade { get; set; } 
        [StringLength(500)]
        public string? Comment { get; set; } 

        public user Student { get; set; }
        public Course Course { get; set; }


    }
}
