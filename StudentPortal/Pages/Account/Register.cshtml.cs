using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public RegisterModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [DataType(DataType.Date)]
            [Display(Name = "Date of Birth")]
            public DateTime DateOfBirth { get; set; }

            [Required]
            [Display(Name = "Phone Number")]
            [StringLength(15, ErrorMessage = "Phone number cannot be longer than 15 characters.")]
            public string Phone { get; set; }

            [Required]
            [Display(Name = "Address")]
            public string Address { get; set; }

            [Display(Name = "Bank Details")]
            public string BankDetails { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Check if the email is already registered
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.Email == Input.Email);
            if (existingStudent != null)
            {
                ModelState.AddModelError("Input.Email", "This email is already registered.");
                return Page();
            }

            // Here you would hash the password. For demonstration, we store it directly.
            // In production, use a secure hashing library (e.g., BCrypt).
            var student = new StudentPortal.Models.Student
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                Email = Input.Email,
                PasswordHash = Input.Password, // Replace with a hash in production
                Address = Input.Address,
                BankDetails = Input.BankDetails,
                Phone = Input.Phone,
                DateOfBirth = Input.DateOfBirth
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            // Optionally, redirect to the login page after successful registration.
            return RedirectToPage("/Account/Login");
        }
    }
}
