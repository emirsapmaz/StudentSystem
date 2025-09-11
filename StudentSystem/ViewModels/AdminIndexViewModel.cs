using StudentSystem.Models;

namespace StudentSystem.ViewModels
{
    public class AdminIndexViewModel
    {
        public IQueryable<user> Users { get; set; }
        public CreateUserViewModel CreateUser { get; set; }
        public bool ShowCreateModal { get; set; }

    }
}
