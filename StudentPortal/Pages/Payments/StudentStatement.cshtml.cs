using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;
using Microsoft.AspNetCore.Authorization;


namespace StudentPortal.Pages.Payments
{
    [Authorize]
    public class StudentStatementModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StudentStatementModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public StudentPortal.Models.Student Student { get; set; }
        public List<Enrollment> ActiveEnrollments { get; set; }
        public decimal TotalCost { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Get the currently logged-in user's email
            string userEmail = User.Identity.Name;

            // Find the student by email
            Student = await _context.Students
                .FirstOrDefaultAsync(s => s.Email == userEmail);

            if (Student == null)
            {
                return RedirectToPage("/Account/Login");
            }

            // Get enrolled courses only (Status = Enrolled)
            ActiveEnrollments = await _context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.StudentId == Student.StudentId && e.Status == EnrollmentStatus.Enrolled)
                .ToListAsync();

            TotalCost = ActiveEnrollments.Sum(e => e.Course.CourseCost);

            return Page();
        }
    }

}
