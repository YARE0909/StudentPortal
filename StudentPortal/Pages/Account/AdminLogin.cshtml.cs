using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace StudentPortal.Pages.Account
{
    public class AdminLoginModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminLoginModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; } = "/Dashboard/AdminDashboard"; // Default redirect URL after successful login

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet()
        {
            // Can be used to initialize any data or provide context for the login page, if needed.
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page(); // Return to the same page if model validation fails

            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == Input.Email);

            // Check if admin exists and compare the plain-text password directly
            if (admin == null || admin.PasswordHash != Input.Password)
            {
                // Add error to ModelState for Password
                ModelState.AddModelError("Input.Password", "Invalid email or password.");
                // Optionally set a generic error message in ViewData to show at the top
                ViewData["ErrorMessage"] = "Invalid login credentials.";
                return Page(); // Return to login page if invalid login
            }

            // Create claims for the admin
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, admin.Email),
                new Claim("AdminId", admin.AdminId.ToString()),
                new Claim(ClaimTypes.Role, admin.Role)
            };

            // Create the ClaimsIdentity and ClaimsPrincipal
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign in the user with the given claims
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            // Redirect to the admin dashboard (or any other page you'd like to use)
            return RedirectToPage(ReturnUrl); // Use the return URL for redirecting after login
        }
    }
}
