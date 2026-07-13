using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GlassyStore.Pages.Admin
{
    [Authorize(Roles = "Administrator")]
    public class CheckUserRoleModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public CheckUserRoleModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Email { get; set; }

        public bool? IsAdmin { get; set; }

        public string Message { get; set; }

        public async Task OnGetAsync()
        {
            if (string.IsNullOrEmpty(Email))
            {
                Message = "Provide an email query parameter, e.g. ?email=admin@example.com";
                return;
            }

            var user = await _userManager.FindByEmailAsync(Email);
            if (user == null)
            {
                Message = $"User with email '{Email}' not found.";
                return;
            }

            IsAdmin = await _userManager.IsInRoleAsync(user, "Administrator");
            Message = IsAdmin == true ? $"{Email} IS an Administrator." : $"{Email} is NOT an Administrator.";
        }
    }
}
