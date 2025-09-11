using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentSystem.Models;
using StudentSystem.ViewModels;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using System.Linq;

namespace StudentSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<user> _userManager;

        public AdminController(UserManager<user> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new AdminIndexViewModel
            {
                Users = _userManager.Users,
                CreateUser = new CreateUserViewModel()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "CreateUser")] CreateUserViewModel model)//Prefix="CreateUser" tells ASP.NET to look for CreateUser.FirstName
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new AdminIndexViewModel
                {
                    Users = _userManager.Users,
                    CreateUser = model,
                    ShowCreateModal = true 
                };
                return View("Index", viewModel);
            }

            if (model.Role == Enums.Roles.Teacher.ToString() && string.IsNullOrEmpty(model.TeacherSubject))
            {
                ModelState.AddModelError("CreateUser.TeacherSubject", "Teaching Subject is required for Teachers.");
                var viewModel = new AdminIndexViewModel
                {
                    Users = _userManager.Users,
                    CreateUser = model
                };
                return View("Index", viewModel);
            }

            var user = new user
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                gender = model.Gender,
                roles = model.Role,
                TeacherSubject = model.Role == Enums.Roles.Teacher.ToString() ? model.TeacherSubject : null
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Role);
                if (user.roles == Enums.Roles.Teacher.ToString())
                {
                    TempData["Success"] = "Teacher created successfully.";
                }
                else if (user.roles == Enums.Roles.Student.ToString())
                {
                    TempData["Success"] = "Student created successfully.";
                }
                else
                {
                    TempData["Success"] = "Admin created successfully.";
                }
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("CreateUser", error.Description);
            }
            var failedModel = new AdminIndexViewModel
            {
                Users = _userManager.Users,
                CreateUser = model,
                ShowCreateModal = true
            };
            return View("Index", failedModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.gender,
                Role = user.roles,
                TeacherSubject = user.TeacherSubject
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Role == Enums.Roles.Teacher.ToString() && string.IsNullOrEmpty(model.TeacherSubject))
            {
                ModelState.AddModelError("TeacherSubject", "Teaching Subject is required for Teachers.");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.gender = model.Gender;
            user.TeacherSubject = model.Role == Enums.Roles.Teacher.ToString() ? model.TeacherSubject : null;

            // Sync roles
            var oldRole = user.roles;
            user.roles = model.Role;
            var currentRoles = await _userManager.GetRolesAsync(user);
            if (oldRole != model.Role)
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, model.Role);
            }

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {

                if (model.Role == Enums.Roles.Teacher.ToString())
                {
                    TempData["Success"] = "Teacher updated successfully.";
                }
                else if (model.Role == Enums.Roles.Student.ToString())
                {
                    TempData["Success"] = "Student updated successfully.";
                }
                else
                {
                    TempData["Success"] = "Admin updated successfully.";
                }
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpPost]
        [Route("Admin/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction("Index");
            }

            if (user.Email == "admin@pusula.com")
            {
                TempData["Error"] = "Cannot delete the default admin account.";
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "User deleted successfully.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to delete user.";
            return RedirectToAction("Index");
        }
    }
}

