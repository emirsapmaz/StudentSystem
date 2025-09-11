using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StudentSystem.Models;
using StudentSystem.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace StudentSystem.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeachersController : Controller
    {
        private readonly UserManager<user> _userManager;

        public TeachersController(UserManager<user> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            var model = new AdminIndexViewModel
            {
                Users = _userManager.Users.Where(u => u.roles == Enums.Roles.Student.ToString()),
                CreateUser = new CreateUserViewModel { Role = Enums.Roles.Student.ToString() }
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind(Prefix = "CreateUser")] CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var viewModel = new AdminIndexViewModel
                {
                    Users = _userManager.Users.Where(u => u.roles == Enums.Roles.Student.ToString()),
                    CreateUser = model,
                    ShowCreateModal = true
                };
                return View("Index", viewModel);
            }

            if (model.Role != Enums.Roles.Student.ToString())
            {
                ModelState.AddModelError("CreateUser.Role", "Teachers can only create Student users.");
                var viewModel = new AdminIndexViewModel
                {
                    Users = _userManager.Users.Where(u => u.roles == Enums.Roles.Student.ToString()),
                    CreateUser = model,
                    ShowCreateModal = true
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
                roles = Enums.Roles.Student.ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, Enums.Roles.Student.ToString());
                TempData["Success"] = "Student created successfully.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("CreateUser", error.Description);
            }
            var failedModel = new AdminIndexViewModel
            {
                Users = _userManager.Users.Where(u => u.roles == Enums.Roles.Student.ToString()),
                CreateUser = model,
                ShowCreateModal = true
            };
            return View("Index", failedModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.roles != Enums.Roles.Student.ToString())
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction("Index");
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Gender = user.gender,
                Role = user.roles
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

            if (model.Role != Enums.Roles.Student.ToString())
            {
                ModelState.AddModelError("Role", "Teachers can only edit Student users.");
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null || user.roles != Enums.Roles.Student.ToString())
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction("Index");
            }

            user.Email = model.Email;
            user.UserName = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.gender = model.Gender;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Student updated successfully.";
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(model);
        }

        [HttpPost]
        [Route("Teachers/Delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null || user.roles != Enums.Roles.Student.ToString())
            {
                TempData["Error"] = "Student not found.";
                return RedirectToAction("Index");
            }

            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
            {
                TempData["Success"] = "Student deleted successfully.";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "Failed to delete student.";
            return RedirectToAction("Index");
        }
    }
}