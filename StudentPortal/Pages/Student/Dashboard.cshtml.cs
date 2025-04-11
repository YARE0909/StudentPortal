using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortal.Models;

namespace StudentPortal.Pages.Student
{
    [Authorize]
    public class DashboardModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DashboardModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public StudentPortal.Models.Student Student { get; set; }
        public List<Enrollment> Enrollments { get; set; } = new();
        public List<Invoice> Invoices { get; set; } = new();
        public List<Payment> Payments { get; set; } = new();

        public decimal TotalFees { get; set; }
        public decimal TotalPaid { get; set; }
        public decimal BalanceDue => TotalFees - TotalPaid;

        public async Task OnGetAsync()
        {
            // Retrieve the student id from the logged-in user's claims.
            // Make sure that during login you create a claim with key "StudentId"
            var studentIdClaim = User.FindFirst("StudentId")?.Value;
            if (string.IsNullOrEmpty(studentIdClaim))
            {
                // Handle the case where the claim is missing – perhaps redirect to login.
                // For example:
                Response.Redirect("/Account/Login");
                return;
            }

            int studentId = int.Parse(studentIdClaim);

            Student = await _context.Students.FindAsync(studentId);

            if (Student == null)
            {
                // Optionally handle if the student was not found (e.g., show an error message)
                // For now, redirect to a different page
                Response.Redirect("/Account/Login");
                return;
            }

            Enrollments = await _context.Enrollments
                .Where(e => e.StudentId == studentId)
                .Include(e => e.Course)
                .ToListAsync();

            Invoices = await _context.Invoices
                .Where(i => i.StudentId == studentId)
                .OrderByDescending(i => i.IssueDate)
                .ToListAsync();

            Payments = await _context.Payments
                .Where(p => p.StudentId == studentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();

            TotalFees = Invoices.Sum(i => i.FinalAmount);
            TotalPaid = Payments.Sum(p => p.Amount);
        }

    }
}
