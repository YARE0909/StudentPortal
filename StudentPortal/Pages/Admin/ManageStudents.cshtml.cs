using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Pages.Admin
{
    [Authorize(Roles = "Admin,Staff")]
    public class ManageStudentsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public List<StudentPortal.Models.Student> Students { get; set; } = new();

        [BindProperty]
        public NewStudentInputModel NewStudent { get; set; }

        public class NewStudentInputModel
        {
            [Required]
            [StringLength(50)]
            public string FirstName { get; set; }

            [Required]
            [StringLength(50)]
            public string LastName { get; set; }

            [Required]
            [StringLength(100)]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [StringLength(255)]
            public string Password { get; set; }

            [StringLength(15)]
            public string Phone { get; set; }

            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            public string Address { get; set; }

            public string BankDetails { get; set; }
        }

        public ManageStudentsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task OnGetAsync()
        {
            Students = await _context.Students.ToListAsync();
        }

        public async Task<IActionResult> OnPostAddStudentAsync()
        {
            if (!ModelState.IsValid)
            {
                Students = await _context.Students.ToListAsync();
                return Page();
            }

            var student = new StudentPortal.Models.Student
            {
                FirstName = NewStudent.FirstName,
                LastName = NewStudent.LastName,
                Email = NewStudent.Email,
                PasswordHash = NewStudent.Password,
                Phone = NewStudent.Phone,
                DateOfBirth = NewStudent.DateOfBirth,
                Address = NewStudent.Address,
                BankDetails = NewStudent.BankDetails,
                CreatedAt = DateTime.Now
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteStudentAsync(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
