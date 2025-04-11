using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Pages.Student
{
    public class ProfileModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public ProfileModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public StudentInputModel StudentInput { get; set; }

        public StudentPortal.Models.Student Student { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            int studentId = GetCurrentStudentId();

            Student = await _context.Students.FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (Student == null)
            {
                return NotFound();
            }

            StudentInput = new StudentInputModel
            {
                StudentId = Student.StudentId,
                FirstName = Student.FirstName,
                LastName = Student.LastName,
                Email = Student.Email,
                PasswordHash = Student.PasswordHash,
                Phone = Student.Phone,
                DateOfBirth = Student.DateOfBirth,
                Address = Student.Address,
                BankDetails = Student.BankDetails
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            int studentId = GetCurrentStudentId();

            var studentToUpdate = await _context.Students.FindAsync(studentId);
            if (studentToUpdate == null)
            {
                return NotFound();
            }

            studentToUpdate.FirstName = StudentInput.FirstName;
            studentToUpdate.LastName = StudentInput.LastName;
            studentToUpdate.Email = StudentInput.Email;
            studentToUpdate.PasswordHash = StudentInput.PasswordHash;
            studentToUpdate.Phone = StudentInput.Phone;
            studentToUpdate.DateOfBirth = StudentInput.DateOfBirth;
            studentToUpdate.Address = StudentInput.Address;
            studentToUpdate.BankDetails = StudentInput.BankDetails;

            _context.Students.Update(studentToUpdate);
            await _context.SaveChangesAsync();

            TempData["Message"] = "Profile updated successfully!";
            return RedirectToPage("/Student/Profile");
        }

        private int GetCurrentStudentId()
        {
             return int.Parse(User.FindFirst("StudentId")?.Value);
        }

        public class StudentInputModel
        {
            public int StudentId { get; set; }

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
            public string PasswordHash { get; set; }

            [StringLength(15)]
            public string Phone { get; set; }

            [DataType(DataType.Date)]
            public DateTime DateOfBirth { get; set; }

            public string Address { get; set; }

            public string BankDetails { get; set; }
        }
    }
}
