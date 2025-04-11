using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Pages.Account
{
    public class AdminRegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public AdminRegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required, EmailAddress]
            public string Email { get; set; }

            [Required, DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            public string Role { get; set; }
        }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // Check if email already exists
            if (_context.Admins.Any(a => a.Email == Input.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is already registered.");
                return Page();
            }

            var admin = new StudentPortal.Models.Admin
            {
                Email = Input.Email,
                PasswordHash = Input.Password, // Store plain-text password (NOT recommended for production)
                Role = Input.Role
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Account/AdminLogin"); // Or another page
        }
    }
}
