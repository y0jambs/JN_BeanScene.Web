using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BeanScene.Web.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BeanScene.Web.Controllers
{
    [Authorize(Roles = "Admin")] // Only Admins can access this controller
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<ApplicationUser> userManager,
                               RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: /Admin/Users
        // List all users with their current roles
        public async Task<IActionResult> Users()
        {
            var users = _userManager.Users.ToList();
            var model = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);

                model.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email ?? user.UserName ?? "",
                    Roles = string.Join(", ", roles)
                });
            }

            return View(model);
        }

        // POST: /Admin/AssignRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignRole(string userId, string role)
        {
            // role comes from dropdown: "Admin" | "Staff" | "Member"
            if (string.IsNullOrWhiteSpace(role))
            {
                TempData["Error"] = "Invalid role selection.";
                return RedirectToAction(nameof(Users));
            }

            if (!await _roleManager.RoleExistsAsync(role))
            {
                TempData["Error"] = $"Role '{role}' does not exist.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Users));
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // Safety: don't remove Admin from the last remaining admin
            if (role != "Admin")
            {
                if (currentRoles.Contains("Admin"))
                {
                    var adminCount = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
                    if (adminCount == 1)
                    {
                        TempData["Error"] = "You cannot remove the last Admin user.";
                        return RedirectToAction(nameof(Users));
                    }
                }
            }

            // Remove all current roles
            if (currentRoles.Any())
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
            }

            // Add the selected role
            var result = await _userManager.AddToRoleAsync(user, role);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
            }
            else
            {
                TempData["Message"] = $"Role '{role}' assigned to {user.Email}.";
            }

            return RedirectToAction(nameof(Users));
        }

        // POST: /Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                TempData["Error"] = "User not found.";
                return RedirectToAction(nameof(Users));
            }

            // Prevent deleting yourself from this screen
            if (user.Email == User.Identity?.Name)
            {
                TempData["Error"] = "You cannot delete your own account from this page.";
                return RedirectToAction(nameof(Users));
            }

            // Prevent deleting the last Admin
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                var adminCount = (await _userManager.GetUsersInRoleAsync("Admin")).Count;
                if (adminCount == 1)
                {
                    TempData["Error"] = "You cannot delete the last Admin user.";
                    return RedirectToAction(nameof(Users));
                }
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join("; ", result.Errors.Select(e => e.Description));
            }
            else
            {
                TempData["Message"] = $"User '{user.Email}' has been deleted.";
            }

            return RedirectToAction(nameof(Users));
        }
    }

    // Simple ViewModel for the Users page
    public class UserRoleViewModel
    {
        public string UserId { get; set; } = "";
        public string Email { get; set; } = "";
        public string Roles { get; set; } = "";
    }
}
