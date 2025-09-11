using System.ComponentModel.DataAnnotations;

namespace StudentSystem.ViewModels
{
    public class UserViewModel
    {
        public string Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(100)]
        public string LastName { get; set; }
        [StringLength(50)]
        public string? Gender { get; set; }
        [Required]
        public string Role { get; set; } 
        [StringLength(100)]
        public string? TeacherSubject { get; set; } 
    }

}
