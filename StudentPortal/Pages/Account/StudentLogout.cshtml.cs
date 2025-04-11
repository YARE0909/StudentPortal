using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace StudentPortal.Pages.Account
{
    public class StudentLogoutModel : PageModel
    {
        public async Task<IActionResult> OnGetAsync()
        {
            // Sign out from cookie authentication
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Redirect to Student Login page (adjust the route if your page is named differently)
            return RedirectToPage("/Account/Login");
        }
    }
}
