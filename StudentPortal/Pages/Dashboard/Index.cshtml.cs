using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentPortal.Pages.Dashboard
{
    [Authorize] // Only authenticated users can access this page.
    public class IndexModel : PageModel
    {
        // Properties to hold user-specific information.
        public string UserEmail { get; set; }
        public string StudentId { get; set; }

        public void OnGet()
        {
            // Ensure the user is authenticated.
            if (User.Identity?.IsAuthenticated == true)
            {
                // Retrieve the email from the user's identity
                UserEmail = User.Identity.Name;

                // If you stored the student's ID as a claim, retrieve it as shown below.
                StudentId = User.FindFirst("StudentId")?.Value;
            }
        }

        public async Task<IActionResult> OnPostLogoutAsync()
        {
            // Sign the user out using cookie authentication.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // Redirect to the login page.
            return RedirectToPage("/Account/Login");
        }
    }
}
