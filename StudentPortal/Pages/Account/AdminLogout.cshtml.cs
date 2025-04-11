using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentPortal.Pages.Account
{
    public class AdminLogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // Sign out from cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Redirect to Admin Login page
            return RedirectToPage("/Account/AdminLogin");
        }
    }
}
