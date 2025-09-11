using System.ComponentModel.DataAnnotations;

namespace StudentSystem.ViewModels
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100, ErrorMessage = "First Name cannot exceed 100 characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(100, ErrorMessage = "Last Name cannot exceed 100 characters.")]
        public string LastName { get; set; }
        [StringLength(50, ErrorMessage = "Gender cannot exceed 50 characters.")]
        public string? Gender { get; set; }
        [Required(ErrorMessage = "Role is required.")]
        public string Role { get; set; } // "Admin", "Teacher", "Student"
        [StringLength(100, ErrorMessage = "Teacher Subject cannot exceed 100 characters.")]
        public string? TeacherSubject { get; set; } // Required for Teacher
    }
}
