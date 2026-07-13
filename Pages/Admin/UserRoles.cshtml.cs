using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GlassyStore.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class UserRolesModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRolesModel(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IList<UserRoleViewModel> Users { get; set; } = new List<UserRoleViewModel>();

        [TempData]
        public string StatusMessage { get; set; }

        public async Task OnGetAsync()
        {
            var users = _userManager.Users.ToList();
            var adminRoleExists = await _roleManager.RoleExistsAsync("Administrator");

            foreach (var u in users)
            {
                var isAdmin = false;
                if (adminRoleExists)
                {
                    isAdmin = await _userManager.IsInRoleAsync(u, "Administrator");
                }

                Users.Add(new UserRoleViewModel
                {
                    Id = u.Id,
                    Email = u.Email,
                    IsAdmin = isAdmin
                });
            }
        }

        public async Task<IActionResult> OnPostAddRoleAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return RedirectToPage();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            if (!await _roleManager.RoleExistsAsync("Administrator"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Administrator"));
            }

            if (!await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                await _userManager.AddToRoleAsync(user, "Administrator");
                StatusMessage = $"User {user.Email} promoted to Administrator.";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveRoleAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return RedirectToPage();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Prevent removing the last administrator
            var admins = await _userManager.GetUsersInRoleAsync("Administrator");
            var currentUserId = _userManager.GetUserId(User);
            if (admins.Count <= 1 && admins.Any(a => a.Id == user.Id))
            {
                StatusMessage = "Cannot remove the last Administrator.";
                return RedirectToPage();
            }

            // Prevent self-demotion
            if (user.Id == currentUserId)
            {
                StatusMessage = "You cannot remove your own Administrator role.";
                return RedirectToPage();
            }

            if (await _userManager.IsInRoleAsync(user, "Administrator"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Administrator");
                StatusMessage = $"User {user.Email} removed from Administrator role.";
            }

            return RedirectToPage();
        }
    }

    public class UserRoleViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public bool IsAdmin { get; set; }
    }
}
